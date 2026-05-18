using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class PurchasePaymentTests
    {
        [Test]
        public void PaymentSlotsAreCreatedFromCardProfessionalCosts()
        {
            var card = new AssetCardData(
                "payment-test-card",
                "결제 테스트 카드",
                "비용 슬롯 생성 확인용 카드",
                AssetRarity.Common,
                3,
                new[]
                {
                    new ProfessionalResourceCost(ResourceType.Research, 2),
                    new ProfessionalResourceCost(ResourceType.Credit, 1)
                },
                5,
                1,
                new TagData[0]);

            var payment = PurchasePayment.CreateForCard(card);

            Assert.That(payment.CardId, Is.EqualTo(card.Id));
            Assert.That(payment.CashCost, Is.EqualTo(3));
            Assert.That(payment.FinalCashCost, Is.EqualTo(3));
            Assert.That(payment.Slots, Has.Count.EqualTo(3));
            Assert.That(payment.Slots[0].RequiredResourceType, Is.EqualTo(ResourceType.Research));
            Assert.That(payment.Slots[1].RequiredResourceType, Is.EqualTo(ResourceType.Research));
            Assert.That(payment.Slots[2].RequiredResourceType, Is.EqualTo(ResourceType.Credit));
            Assert.That(payment.Slots[0].PlacedResourceType, Is.Null);
        }

        [Test]
        public void ProfessionalResourceChipPlacementCanBeRecoveredWithoutSpendingResource()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstAvailableMarketSlotCard(run.MarketTape));

            var placed = PurchasePayment.PlaceChip(run, ResourceType.Research);

            Assert.That(placed.Run.Resources.Research, Is.EqualTo(1));
            Assert.That(placed.Run.CardDetail.PendingPayment.Slots[0].PlacedResourceType, Is.EqualTo(ResourceType.Research));

            var removed = PurchasePayment.RemoveChip(placed.Run, 0);

            Assert.That(removed.Run.Resources.Research, Is.EqualTo(1));
            Assert.That(removed.Run.CardDetail.PendingPayment.Slots[0].PlacedResourceType, Is.Null);
        }

        [Test]
        public void IncompatibleProfessionalResourceChipPlacementIsRejectedAndLeavesSlotEmpty()
        {
            var card = new AssetCardData(
                "research-only-payment-test-card",
                "리서치 전용 테스트 카드",
                "호환되지 않는 칩 거부를 확인하는 카드",
                AssetRarity.Common,
                1,
                new[] { new ProfessionalResourceCost(ResourceType.Research, 1) },
                1,
                0,
                new TagData[0]);
            var runtimeCard = new AssetCardRuntimeData(card, AssetCardRuntimeState.Available, PurchaseSource.MarketTape);
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            run = WithCurrentMarketCard(run, runtimeCard, 0);
            run = MarketAreaFlow.OpenMarketCardDetail(run, runtimeCard);

            var result = PurchasePayment.PlaceChip(run, ResourceType.Credit);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Run.CardDetail.PendingPayment.Slots[0].PlacedResourceType, Is.Null);
            Assert.That(result.Run.Resources.Credit, Is.EqualTo(1));
        }

        [Test]
        public void DealChipPlacementCanFillAnyProfessionalSlotAndDiscountsCashCost()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddDeal(run, 1).Run;
            var selectedCard = run.MarketTape.CurrentMarketCards[2];
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);

            var placed = PurchasePayment.PlaceChip(run, ResourceType.Deal);

            Assert.That(placed.Run.Resources.Deal, Is.EqualTo(1));
            Assert.That(placed.Run.CardDetail.PendingPayment.Slots[0].RequiredResourceType, Is.EqualTo(ResourceType.Research));
            Assert.That(placed.Run.CardDetail.PendingPayment.Slots[0].PlacedResourceType, Is.EqualTo(ResourceType.Deal));
            Assert.That(placed.Run.CardDetail.PendingPayment.FinalCashCost, Is.EqualTo(0));
        }

        [Test]
        public void InflationCostModifierAppliesAfterDealDiscount()
        {
            var payment = new PurchasePaymentState(
                "inflation-test-card",
                5,
                new[]
                {
                    new PaymentSlotState(ResourceType.Research, ResourceType.Deal),
                    new PaymentSlotState(ResourceType.Credit, ResourceType.Deal)
                },
                1);

            Assert.That(payment.FinalCashCost, Is.EqualTo(4));
        }

        [Test]
        public void CurrentQuarterInflationModifierAppliesWhenOpeningMarketCardPayment()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithCalendar(run, new RunCalendarState(1, 2, run.Calendar.RemainingBusinessDays));
            var selectedCard = run.MarketTape.CurrentMarketCards[0];

            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);

            Assert.That(run.CardDetail.PendingPayment.InflationCostModifier, Is.EqualTo(1));
            Assert.That(run.CardDetail.PendingPayment.FinalCashCost, Is.EqualTo(selectedCard.Card.CashCost + 1));
        }

        [Test]
        public void InflationCashShortageBlocksPurchaseAndLeavesResourcesCardMarketAndBusinessDayUnchanged()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithCalendar(run, new RunCalendarState(1, 2, run.Calendar.RemainingBusinessDays));
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            var selectedCard = run.MarketTape.CurrentMarketCards[0];
            run = WithResources(
                run,
                new ResourceState(
                    selectedCard.Card.CashCost,
                    run.Resources.Reading,
                    run.Resources.Meditation,
                    run.Resources.Patience,
                    run.Resources.Deal));
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash));
            Assert.That(result.Run.Resources.Research, Is.EqualTo(run.Resources.Research));
            Assert.That(result.Run.Resources.Credit, Is.EqualTo(run.Resources.Credit));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(run.Resources.Deal));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(FindCard(result.Run.AssetCards, selectedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Available));
            Assert.That(result.Run.MarketTape.CurrentMarketCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
        }

        [Test]
        public void IncompletePurchaseConfirmationLeavesResourcesCardMarketAndBusinessDayUnchanged()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var selectedCard = run.MarketTape.CurrentMarketCards[0];
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash));
            Assert.That(result.Run.Resources.Research, Is.EqualTo(run.Resources.Research));
            Assert.That(result.Run.Resources.Credit, Is.EqualTo(run.Resources.Credit));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(run.Resources.Deal));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(FindCard(result.Run.AssetCards, selectedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Available));
            Assert.That(result.Run.MarketTape.CurrentMarketCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
        }

        [Test]
        public void MarketCardPurchaseConsumesPaymentOwnsCardPullsMarketTapeAndConsumesBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var selectedCard = FindFirstAvailableMarketSlotCard(run.MarketTape);
            run = AddPaymentResources(run, selectedCard.Card);
            var previousSlotIds = CollectSlotCardIds(run.MarketTape);
            var resourcesBeforePurchase = run.Resources;
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PlaceRequiredPaymentChips(run, selectedCard.Card);

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(resourcesBeforePurchase.Cash - selectedCard.Card.CashCost + selectedCard.Card.Income));
            Assert.That(result.Run.Resources.Research, Is.EqualTo(resourcesBeforePurchase.Research - CountProfessionalCost(selectedCard.Card, ResourceType.Research)));
            Assert.That(result.Run.Resources.Credit, Is.EqualTo(resourcesBeforePurchase.Credit - CountProfessionalCost(selectedCard.Card, ResourceType.Credit)));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].State, Is.EqualTo(AssetCardRuntimeState.Owned));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].AcquiredOrder, Is.EqualTo(1));
            Assert.That(FindCard(result.Run.AssetCards, selectedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Owned));
            Assert.That(result.Run.MarketTape.Slots, Has.Count.EqualTo(run.StaticData.MarketConfig.MarketTapeSlots));
            Assert.That(CollectSlotCardIds(result.Run.MarketTape), Is.Unique);
            Assert.That(CollectSlotCardIds(result.Run.MarketTape), Does.Not.Contain(selectedCard.Card.Id));
            Assert.That(CollectSlotCardIds(result.Run.MarketTape), Is.Not.EqualTo(previousSlotIds));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays - 1));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(result.Run.CardDetail.SelectedCard, Is.Null);
        }

        [Test]
        public void MarketCardPurchaseAppliesOwnedAssetIncomeAtNextBusinessDayStart()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            var selectedCard = run.MarketTape.CurrentMarketCards[0];
            var cashBeforePurchase = run.Resources.Cash;
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(
                result.Run.Resources.Cash,
                Is.EqualTo(cashBeforePurchase - selectedCard.Card.CashCost + selectedCard.Card.Income));
            Assert.That(result.Run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(selectedCard.Card.Income));
            Assert.That(result.Run.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(selectedCard.Card.Income));
            Assert.That(result.Run.Performance.TotalEarnedCash, Is.EqualTo(selectedCard.Card.Income));
        }

        [Test]
        public void FullPortfolioBlocksNewStockPurchaseBeforeSpendingResourcesOrBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            run = WithOwnedAssets(run, CreateOwnedCards(8));
            var selectedCard = run.MarketTape.CurrentMarketCards[0];
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("주식 매도가 필요합니다"));
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(run.Resources.Cash));
            Assert.That(result.Run.Resources.Research, Is.EqualTo(run.Resources.Research));
            Assert.That(result.Run.Resources.Credit, Is.EqualTo(run.Resources.Credit));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(run.Resources.Deal));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(8));
            Assert.That(FindCard(result.Run.AssetCards, selectedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Available));
            Assert.That(result.Run.MarketTape.CurrentMarketCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.CardDetail));
        }

        [Test]
        public void ThirdSameStockPurchaseMergesOwnedCopiesIntoOneFoilInEarliestSlot()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var stock = run.StaticData.AssetCards[0];
            var copies = FindRuntimeStockCopies(run.AssetCards, stock.Id, 3);
            var firstOwned = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 1, false, copies[0].RuntimeId);
            var otherOwned = new AssetCardRuntimeData(
                new AssetCardData("other-owned-stock", "Other", "Other", AssetRarity.Common, 1, new ProfessionalResourceCost[0], 2, 0, new TagData[0]),
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape,
                2);
            var secondOwned = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 3, false, copies[1].RuntimeId);
            var selectedCard = copies[2];
            run = WithAssetCards(run, ReplaceRuntimeCards(run.AssetCards, firstOwned, secondOwned));
            run = WithOwnedAssets(run, new[] { firstOwned, otherOwned, secondOwned });
            run = WithCurrentMarketCard(run, selectedCard, 0);
            run = AddPaymentResources(run, selectedCard.Card);
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PlaceRequiredPaymentChips(run, selectedCard.Card);

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(2));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(stock.Id));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].IsFoil, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].AcquiredOrder, Is.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards[1].Card.Id, Is.EqualTo(otherOwned.Card.Id));
            Assert.That(result.Run.OwnedAssets.CurrentManagementValue, Is.EqualTo(stock.FoilValue + otherOwned.Card.ManagementValue));
            Assert.That(result.Run.OwnedAssets.BusinessDayStartIncome, Is.EqualTo(stock.FoilDividend + otherOwned.Card.Income));
        }

        [Test]
        public void FoilMergeLeavesConsumedPortfolioSlotsEmptyAndNextStockUsesLeftmostEmptySlot()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var foilStock = run.StaticData.AssetCards[0];
            var nextStock = run.StaticData.AssetCards[1];
            var foilCopies = FindRuntimeStockCopies(run.AssetCards, foilStock.Id, 3);
            var nextCopies = FindRuntimeStockCopies(run.AssetCards, nextStock.Id, 1);
            var firstOwned = new AssetCardRuntimeData(foilStock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 1, false, foilCopies[0].RuntimeId);
            var secondOwned = new AssetCardRuntimeData(foilStock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 2, false, foilCopies[1].RuntimeId);
            var otherOwned = new AssetCardRuntimeData(
                new AssetCardData("other-owned-stock", "Other", "Other", AssetRarity.Common, 1, new ProfessionalResourceCost[0], 2, 0, new TagData[0]),
                AssetCardRuntimeState.Owned,
                PurchaseSource.MarketTape,
                3);
            var thirdFoilCopy = foilCopies[2];
            run = WithAssetCards(run, ReplaceRuntimeCards(run.AssetCards, firstOwned, secondOwned));
            run = WithOwnedAssets(run, new[] { firstOwned, secondOwned, otherOwned });
            run = WithCurrentMarketCard(run, thirdFoilCopy, 0);
            run = AddPaymentResources(run, thirdFoilCopy.Card);
            run = MarketAreaFlow.OpenMarketCardDetail(run, thirdFoilCopy);
            run = PlaceRequiredPaymentChips(run, thirdFoilCopy.Card);

            var mergeResult = PurchasePayment.ConfirmPurchase(run);

            Assert.That(mergeResult.Succeeded, Is.True);
            Assert.That(mergeResult.Run.OwnedAssets.StockSlots, Has.Count.EqualTo(3));
            Assert.That(mergeResult.Run.OwnedAssets.StockSlots[0].Card.Id, Is.EqualTo(foilStock.Id));
            Assert.That(mergeResult.Run.OwnedAssets.StockSlots[0].IsFoil, Is.True);
            Assert.That(mergeResult.Run.OwnedAssets.StockSlots[1], Is.Null);
            Assert.That(mergeResult.Run.OwnedAssets.StockSlots[2].Card.Id, Is.EqualTo(otherOwned.Card.Id));

            var nextRun = WithCurrentMarketCard(mergeResult.Run, nextCopies[0], 0);
            nextRun = AddPaymentResources(nextRun, nextCopies[0].Card);
            nextRun = MarketAreaFlow.OpenMarketCardDetail(nextRun, nextCopies[0]);
            nextRun = PlaceRequiredPaymentChips(nextRun, nextCopies[0].Card);

            var nextResult = PurchasePayment.ConfirmPurchase(nextRun);

            Assert.That(nextResult.Succeeded, Is.True);
            Assert.That(nextResult.Run.OwnedAssets.StockSlots[0].Card.Id, Is.EqualTo(foilStock.Id));
            Assert.That(nextResult.Run.OwnedAssets.StockSlots[1].Card.Id, Is.EqualTo(nextStock.Id));
            Assert.That(nextResult.Run.OwnedAssets.StockSlots[2].Card.Id, Is.EqualTo(otherOwned.Card.Id));
        }

        [Test]
        public void FoilMergeRemovesSameStockFromRemainingDeckAndMarket()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var stock = run.StaticData.AssetCards[0];
            var copies = FindRuntimeStockCopies(run.AssetCards, stock.Id, 3);
            var firstOwned = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 1, false, copies[0].RuntimeId);
            var secondOwned = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 2, false, copies[1].RuntimeId);
            var selectedCard = copies[2];
            run = WithAssetCards(run, ReplaceRuntimeCards(run.AssetCards, firstOwned, secondOwned));
            run = WithOwnedAssets(run, new[] { firstOwned, secondOwned });
            run = WithCurrentMarketCard(run, selectedCard, 0);
            run = AddPaymentResources(run, selectedCard.Card);
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PlaceRequiredPaymentChips(run, selectedCard.Card);

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(CountAvailableOrReservedStockCopies(result.Run.AssetCards, stock.Id), Is.EqualTo(0));
            Assert.That(MarketTapeContainsStock(result.Run.MarketTape, stock.Id), Is.False);
        }

        [Test]
        public void FoilMergeRemovesSameStockReservedSlotsAndReservationCount()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var stock = run.StaticData.AssetCards[0];
            var copies = FindRuntimeStockCopies(run.AssetCards, stock.Id, 4);
            var firstOwned = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 1, false, copies[0].RuntimeId);
            var secondOwned = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 2, false, copies[1].RuntimeId);
            var selectedCard = copies[2];
            var reservedCard = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Reserved, null, null, false, copies[3].RuntimeId);
            run = WithAssetCards(run, ReplaceRuntimeCards(run.AssetCards, firstOwned, secondOwned, reservedCard));
            run = WithOwnedAssets(run, new[] { firstOwned, secondOwned });
            run = WithReservation(run, new ReservationState(run.Reservation.Capacity, new[] { reservedCard }));
            run = WithMarketTapeSlots(
                run,
                new[]
                {
                    new MarketTapeSlotState(selectedCard, false),
                    new MarketTapeSlotState(reservedCard, true)
                });
            run = AddPaymentResources(run, selectedCard.Card);
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PlaceRequiredPaymentChips(run, selectedCard.Card);

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Reservation.ReservedCards, Is.Empty);
            Assert.That(CountReservedSlots(result.Run.MarketTape), Is.EqualTo(0));
            Assert.That(MarketTapeContainsStock(result.Run.MarketTape, stock.Id), Is.False);
        }

        [Test]
        public void FullPortfolioAllowsThirdSameStockPurchaseWhenItImmediatelyMergesToFoil()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var stock = run.StaticData.AssetCards[0];
            var copies = FindRuntimeStockCopies(run.AssetCards, stock.Id, 3);
            var firstOwned = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 1, false, copies[0].RuntimeId);
            var secondOwned = new AssetCardRuntimeData(stock, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, 2, false, copies[1].RuntimeId);
            var ownedCards = new System.Collections.Generic.List<AssetCardRuntimeData> { firstOwned, secondOwned };
            ownedCards.AddRange(CreateOwnedCards(6));
            var selectedCard = copies[2];
            run = WithAssetCards(run, ReplaceRuntimeCards(run.AssetCards, firstOwned, secondOwned));
            run = WithOwnedAssets(run, ownedCards);
            run = WithCurrentMarketCard(run, selectedCard, 0);
            run = AddPaymentResources(run, selectedCard.Card);
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PlaceRequiredPaymentChips(run, selectedCard.Card);

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(7));
            Assert.That(CountOwnedFoils(result.Run.OwnedAssets, stock.Id), Is.EqualTo(1));
        }

        [Test]
        public void ConsumableCashResourceCardPurchaseGrantsFundingCashWithoutOwningCardOrRevenue()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var resourceCard = new AssetCardData(
                "cash-resource-card",
                string.Empty,
                "조달 현금 테스트 카드",
                AssetRarity.Common,
                1,
                new ProfessionalResourceCost[0],
                0,
                0,
                new TagData[0],
                cardDomain: CardDomain.ConsumableResource,
                providedResourceType: ResourceType.Cash,
                providedResourceAmount: 3);
            var runtimeCard = new AssetCardRuntimeData(resourceCard, AssetCardRuntimeState.Available, PurchaseSource.MarketTape);
            run = WithCurrentMarketCard(run, runtimeCard, 0);
            var cashBeforePurchase = run.Resources.Cash;
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            run = MarketAreaFlow.OpenMarketCardDetail(run, runtimeCard);

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(cashBeforePurchase - resourceCard.CashCost + 3));
            Assert.That(result.Run.Performance.FundingCash, Is.EqualTo(3));
            Assert.That(result.Run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
            Assert.That(result.Run.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(0));
            Assert.That(result.Run.Performance.TotalEarnedCash, Is.EqualTo(0));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(FindCard(result.Run.AssetCards, resourceCard.Id).State, Is.EqualTo(AssetCardRuntimeState.Removed));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(result.Run.CardDetail.SelectedCard, Is.Null);
        }

        [Test]
        public void ConsumableInvestmentPhilosophyResourceCardPurchaseUsesCashOnlyAndDoesNotSpendDeal()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddDeal(run, 1).Run;
            var resourceCard = new AssetCardData(
                "patience-resource-card",
                string.Empty,
                "투자 철학 테스트 카드",
                AssetRarity.Uncommon,
                1,
                new[] { new ProfessionalResourceCost(ResourceType.Reading, 1) },
                0,
                0,
                new TagData[0],
                cardDomain: CardDomain.ConsumableResource,
                providedResourceType: ResourceType.Patience,
                providedResourceAmount: 2);
            var runtimeCard = new AssetCardRuntimeData(resourceCard, AssetCardRuntimeState.Available, PurchaseSource.MarketTape);
            run = WithCurrentMarketCard(run, runtimeCard, 0);
            var cashBeforePurchase = run.Resources.Cash;
            run = MarketAreaFlow.OpenMarketCardDetail(run, runtimeCard);

            Assert.That(run.CardDetail.PendingPayment.Slots, Is.Empty);

            var dealPlacement = PurchasePayment.PlaceChip(run, ResourceType.Deal);
            Assert.That(dealPlacement.Succeeded, Is.False);
            Assert.That(dealPlacement.Run.Resources.Deal, Is.EqualTo(1));

            var result = PurchasePayment.ConfirmPurchase(dealPlacement.Run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(cashBeforePurchase - resourceCard.CashCost));
            Assert.That(result.Run.Resources.Patience, Is.EqualTo(2));
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Is.Empty);
            Assert.That(FindCard(result.Run.AssetCards, resourceCard.Id).State, Is.EqualTo(AssetCardRuntimeState.Removed));
        }

        [Test]
        public void FullPortfolioDoesNotBlockConsumableResourceCardPurchase()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithOwnedAssets(run, CreateOwnedCards(8));
            var resourceCard = new AssetCardData(
                "cash-resource-card-full-portfolio",
                string.Empty,
                "가득 찬 포트폴리오의 자원 카드 테스트",
                AssetRarity.Common,
                1,
                new ProfessionalResourceCost[0],
                0,
                0,
                new TagData[0],
                cardDomain: CardDomain.ConsumableResource,
                providedResourceType: ResourceType.Cash,
                providedResourceAmount: 3);
            var runtimeCard = new AssetCardRuntimeData(resourceCard, AssetCardRuntimeState.Available, PurchaseSource.MarketTape);
            run = WithCurrentMarketCard(run, runtimeCard, 0);
            var cashBeforePurchase = run.Resources.Cash;
            run = MarketAreaFlow.OpenMarketCardDetail(run, runtimeCard);

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(cashBeforePurchase - resourceCard.CashCost + 3));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(8));
            Assert.That(FindCard(result.Run.AssetCards, resourceCard.Id).State, Is.EqualTo(AssetCardRuntimeState.Removed));
        }

        [Test]
        public void ExtraBuyGrantingMarketPurchaseWaitsForExtraBuyInsteadOfConsumingBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            run = WithExtraBuyGrantOnCurrentMarketCard(run, 0);
            var selectedCard = run.MarketTape.CurrentMarketCards[0];
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            var cashBeforePurchase = run.Resources.Cash;
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(cashBeforePurchase - selectedCard.Card.CashCost));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(result.Run.BusinessDay.HasExtraBuyAction, Is.True);
            Assert.That(result.Run.BusinessDay.IsAwaitingExtraBuyChoice, Is.True);
            Assert.That(result.Run.BusinessDay.IsBuyingWithExtraBuy, Is.False);
            Assert.That(result.Run.CardDetail.SelectedCard, Is.Null);
        }

        [Test]
        public void ExtraBuyPurchaseConsumesExtraBuyAndIgnoresNestedExtraBuyGrant()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            run = WithExtraBuyGrantOnCurrentMarketCard(run, FindFirstAvailableSlotIndex(run.MarketTape));
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            run = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstAvailableMarketSlotCard(run.MarketTape));
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;
            var extraBuyRun = PurchasePayment.ConfirmPurchase(run).Run;
            extraBuyRun = ResourceLedger.AddFundingCash(extraBuyRun, 3);
            extraBuyRun = ResourceLedger.AddProfessionalResource(extraBuyRun, ResourceType.Research, 1).Run;
            extraBuyRun = ResourceLedger.AddProfessionalResource(extraBuyRun, ResourceType.Credit, 1).Run;
            extraBuyRun = WithExtraBuyGrantOnCurrentMarketCard(extraBuyRun, 0);
            var secondCard = extraBuyRun.MarketTape.CurrentMarketCards[0];
            extraBuyRun = MarketAreaFlow.OpenMarketCardDetail(extraBuyRun, secondCard);
            extraBuyRun = PurchasePayment.PlaceChip(extraBuyRun, ResourceType.Research).Run;
            extraBuyRun = PurchasePayment.PlaceChip(extraBuyRun, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(extraBuyRun);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(2));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(result.Run.BusinessDay.HasExtraBuyAction, Is.False);
            Assert.That(result.Run.BusinessDay.IsAwaitingExtraBuyChoice, Is.False);
            Assert.That(result.Run.BusinessDay.IsBuyingWithExtraBuy, Is.False);
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
        }

        [Test]
        public void ExtraBuyCanPurchaseReservedCardAndThenConsumesBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstAvailableMarketSlotCard(run.MarketTape));
            run = ReservationAction.ConfirmReservation(run).Run;
            var reservedCard = FindReservedSlotCard(run.MarketTape);
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            run = WithExtraBuyGrantOnCurrentMarketCard(run, FindFirstAvailableSlotIndex(run.MarketTape));
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            run = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstAvailableMarketSlotCard(run.MarketTape));
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;
            var extraBuyRun = PurchasePayment.ConfirmPurchase(run).Run;
            extraBuyRun = ResourceLedger.AddFundingCash(extraBuyRun, 3);
            extraBuyRun = ResourceLedger.AddProfessionalResource(extraBuyRun, ResourceType.Research, 1).Run;
            extraBuyRun = ResourceLedger.AddProfessionalResource(extraBuyRun, ResourceType.Credit, 1).Run;
            extraBuyRun = MarketAreaFlow.OpenMarketCardDetail(extraBuyRun, reservedCard);
            extraBuyRun = PurchasePayment.PlaceChip(extraBuyRun, ResourceType.Research).Run;
            extraBuyRun = PurchasePayment.PlaceChip(extraBuyRun, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(extraBuyRun);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(2));
            Assert.That(result.Run.Reservation.ReservedCards, Is.Empty);
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(result.Run.BusinessDay.HasExtraBuyAction, Is.False);
        }

        [Test]
        public void ReservedMarketSlotPurchaseOwnsCardUnlocksSlotAndPullsMarketTape()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            var marketCard = FindFirstAvailableMarketSlotCard(run.MarketTape);
            run = MarketAreaFlow.OpenMarketCardDetail(run, marketCard);
            run = ReservationAction.ConfirmReservation(run).Run;
            var reservedCard = FindReservedSlotCard(run.MarketTape);
            run = MarketAreaFlow.OpenMarketCardDetail(run, FindFirstAvailableMarketSlotCard(run.MarketTape));
            run = ReservationAction.ConfirmReservation(run).Run;
            var otherReservedCard = FindOtherReservedSlotCard(run.MarketTape, reservedCard.Card.Id);
            run = MarketAreaFlow.OpenMarketCardDetail(run, reservedCard);
            Assert.That(run.CardDetail.PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(run.CardDetail.ShouldShowReserveButton, Is.False);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(reservedCard.Card.Id));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(FindCard(result.Run.AssetCards, reservedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Owned));
            Assert.That(result.Run.Reservation.ReservedCards, Is.Empty);
            Assert.That(CollectSlotCardIds(result.Run.MarketTape), Does.Not.Contain(reservedCard.Card.Id));
            Assert.That(CollectSlotCardIds(result.Run.MarketTape), Does.Contain(otherReservedCard.Card.Id));
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays - 1));
        }

        [Test]
        public void ReservedCardPurchaseUsesCurrentInflationModifierAtPurchaseTime()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var marketCard = FindFirstAvailableMarketSlotCard(run.MarketTape);
            run = MarketAreaFlow.OpenMarketCardDetail(run, marketCard);
            run = ReservationAction.ConfirmReservation(run).Run;
            var reservedCard = FindReservedSlotCard(run.MarketTape);
            run = WithCalendar(run, new RunCalendarState(1, 2, run.Calendar.RemainingBusinessDays));
            run = ResourceLedger.AddFundingCash(run, 1);
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            var cashBeforePurchase = run.Resources.Cash;
            run = MarketAreaFlow.OpenMarketCardDetail(run, reservedCard);

            Assert.That(run.CardDetail.PendingPayment.InflationCostModifier, Is.EqualTo(1));
            Assert.That(run.CardDetail.PendingPayment.FinalCashCost, Is.EqualTo(reservedCard.Card.CashCost + 1));

            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(reservedCard.Card.Id));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(cashBeforePurchase - reservedCard.Card.CashCost - 1 + reservedCard.Card.Income));
            Assert.That(result.Run.Reservation.ReservedCards, Is.Empty);
        }

        private static AssetCardRuntimeData FindCard(System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards, string cardId)
        {
            foreach (var card in cards)
            {
                if (card.Card.Id == cardId)
                {
                    return card;
                }
            }

            Assert.Fail("Expected to find card " + cardId + ".");
            return null;
        }

        private static System.Collections.Generic.List<AssetCardRuntimeData> FindRuntimeStockCopies(
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards,
            string stockId,
            int count)
        {
            var copies = new System.Collections.Generic.List<AssetCardRuntimeData>();
            foreach (var card in cards)
            {
                if (card.Card.Id == stockId)
                {
                    copies.Add(card);
                    if (copies.Count == count)
                    {
                        return copies;
                    }
                }
            }

            Assert.Fail("Expected to find " + count + " copies for stock " + stockId + ".");
            return copies;
        }

        private static RunSessionState AddPaymentResources(RunSessionState run, AssetCardData card)
        {
            foreach (var cost in card.ProfessionalCosts)
            {
                for (var i = 0; i < cost.Amount; i++)
                {
                    run = ResourceLedger.AddProfessionalResource(run, cost.ResourceType, 1).Run;
                }
            }

            return run;
        }

        private static RunSessionState PlaceRequiredPaymentChips(RunSessionState run, AssetCardData card)
        {
            foreach (var cost in card.ProfessionalCosts)
            {
                for (var i = 0; i < cost.Amount; i++)
                {
                    run = PurchasePayment.PlaceChip(run, cost.ResourceType).Run;
                }
            }

            return run;
        }

        private static int CountAvailableOrReservedStockCopies(
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards,
            string stockId)
        {
            var count = 0;
            foreach (var card in cards)
            {
                if (card.Card.Id == stockId
                    && (card.State == AssetCardRuntimeState.Available
                        || card.State == AssetCardRuntimeState.Reserved))
                {
                    count++;
                }
            }

            return count;
        }

        private static bool MarketTapeContainsStock(MarketTapeState tape, string stockId)
        {
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty && slot.Card.Card.Id == stockId)
                {
                    return true;
                }
            }

            return false;
        }

        private static int CountReservedSlots(MarketTapeState tape)
        {
            var count = 0;
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountOwnedFoils(OwnedAssetState ownedAssets, string stockId)
        {
            var count = 0;
            foreach (var card in ownedAssets.OwnedCards)
            {
                if (card.State == AssetCardRuntimeState.Owned
                    && card.IsFoil
                    && card.Card.Id == stockId)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountProfessionalCost(AssetCardData card, ResourceType resourceType)
        {
            var count = 0;
            foreach (var cost in card.ProfessionalCosts)
            {
                if (cost.ResourceType == resourceType)
                {
                    count += cost.Amount;
                }
            }

            return count;
        }

        private static System.Collections.Generic.List<string> CollectCardIds(
            System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> cards)
        {
            var cardIds = new System.Collections.Generic.List<string>();
            foreach (var card in cards)
            {
                cardIds.Add(card.Card.Id);
            }

            return cardIds;
        }

        private static System.Collections.Generic.List<string> CollectSlotCardIds(MarketTapeState tape)
        {
            var cardIds = new System.Collections.Generic.List<string>();
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsEmpty)
                {
                    cardIds.Add(slot.Card.Card.Id);
                }
            }

            return cardIds;
        }

        private static AssetCardRuntimeData FindReservedSlotCard(MarketTapeState tape)
        {
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved && !slot.IsEmpty)
                {
                    return slot.Card;
                }
            }

            Assert.Fail("Expected to find a reserved market slot.");
            return null;
        }

        private static AssetCardRuntimeData FindOtherReservedSlotCard(MarketTapeState tape, string excludedCardId)
        {
            foreach (var slot in tape.Slots)
            {
                if (slot.IsReserved && !slot.IsEmpty && slot.Card.Card.Id != excludedCardId)
                {
                    return slot.Card;
                }
            }

            Assert.Fail("Expected to find another reserved market slot.");
            return null;
        }

        private static AssetCardRuntimeData FindFirstAvailableMarketSlotCard(MarketTapeState tape)
        {
            foreach (var slot in tape.Slots)
            {
                if (!slot.IsReserved
                    && !slot.IsEmpty
                    && slot.Card.State == AssetCardRuntimeState.Available
                    && slot.Card.Card.CardDomain == CardDomain.Stock)
                {
                    return slot.Card;
                }
            }

            Assert.Fail("Expected to find an available stock market slot.");
            return null;
        }

        private static int FindFirstAvailableSlotIndex(MarketTapeState tape)
        {
            for (var i = 0; i < tape.Slots.Count; i++)
            {
                var slot = tape.Slots[i];
                if (!slot.IsReserved
                    && !slot.IsEmpty
                    && slot.Card.State == AssetCardRuntimeState.Available
                    && slot.Card.Card.CardDomain == CardDomain.Stock)
                {
                    return i;
                }
            }

            Assert.Fail("Expected to find an available stock market slot.");
            return -1;
        }

        private static void AssertZoneMatches(
            System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> actualCards,
            System.Collections.Generic.IReadOnlyList<string> expectedCardIds)
        {
            Assert.That(actualCards, Has.Count.EqualTo(expectedCardIds.Count));
            for (var i = 0; i < expectedCardIds.Count; i++)
            {
                Assert.That(actualCards[i].Card.Id, Is.EqualTo(expectedCardIds[i]));
            }
        }

        private static RunSessionState WithExtraBuyGrantOnCurrentMarketCard(RunSessionState run, int slotIndex)
        {
            var card = run.MarketTape.CurrentMarketCards[slotIndex].Card;
            var grantCard = new AssetCardData(
                card.Id,
                card.DisplayName,
                card.Description,
                card.Rarity,
                card.CashCost,
                card.ProfessionalCosts,
                card.ManagementValue,
                card.Income,
                card.Tags,
                true);
            var grantRuntimeCard = new AssetCardRuntimeData(grantCard, AssetCardRuntimeState.Available, null);
            var currentMarketCards = new System.Collections.Generic.List<AssetCardRuntimeData>(run.MarketTape.CurrentMarketCards);
            currentMarketCards[slotIndex] = grantRuntimeCard;

            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                ReplaceCard(run.AssetCards, grantRuntimeCard),
                new MarketTapeState(
                    run.MarketTape.SellImminentCards,
                    currentMarketCards,
                    run.MarketTape.UpcomingMarketCards),
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithCurrentMarketCard(
            RunSessionState run,
            AssetCardRuntimeData runtimeCard,
            int slotIndex)
        {
            var currentMarketCards = new System.Collections.Generic.List<AssetCardRuntimeData>(run.MarketTape.CurrentMarketCards);
            currentMarketCards[slotIndex] = runtimeCard;

            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                ReplaceCard(run.AssetCards, runtimeCard),
                new MarketTapeState(
                    run.MarketTape.SellImminentCards,
                    currentMarketCards,
                    run.MarketTape.UpcomingMarketCards),
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> ReplaceCard(
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards,
            AssetCardRuntimeData replacement)
        {
            var updatedCards = new System.Collections.Generic.List<AssetCardRuntimeData>();
            var replaced = false;
            foreach (var card in cards)
            {
                if (card.RuntimeId == replacement.RuntimeId)
                {
                    updatedCards.Add(replacement);
                    replaced = true;
                }
                else
                {
                    updatedCards.Add(card);
                }
            }

            if (!replaced)
            {
                updatedCards.Add(replacement);
            }

            return updatedCards;
        }

        private static RunSessionState WithCalendar(RunSessionState run, RunCalendarState calendar)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithOwnedAssets(
            RunSessionState run,
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> ownedAssets)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                new OwnedAssetState(ownedAssets),
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithReservation(RunSessionState run, ReservationState reservation)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithMarketTapeSlots(
            RunSessionState run,
            System.Collections.Generic.IEnumerable<MarketTapeSlotState> slots)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                new MarketTapeState(slots),
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static RunSessionState WithAssetCards(
            RunSessionState run,
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> assetCards)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                assetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> ReplaceRuntimeCards(
            System.Collections.Generic.IEnumerable<AssetCardRuntimeData> cards,
            params AssetCardRuntimeData[] replacements)
        {
            var replacementByRuntimeId = new System.Collections.Generic.Dictionary<string, AssetCardRuntimeData>();
            foreach (var replacement in replacements)
            {
                replacementByRuntimeId[replacement.RuntimeId] = replacement;
            }

            var updatedCards = new System.Collections.Generic.List<AssetCardRuntimeData>();
            foreach (var card in cards)
            {
                if (replacementByRuntimeId.TryGetValue(card.RuntimeId, out var replacement))
                {
                    updatedCards.Add(replacement);
                }
                else
                {
                    updatedCards.Add(card);
                }
            }

            return updatedCards;
        }

        private static RunSessionState WithResources(RunSessionState run, ResourceState resources)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction);
        }

        private static System.Collections.Generic.IReadOnlyList<AssetCardRuntimeData> CreateOwnedCards(int count)
        {
            var cards = new System.Collections.Generic.List<AssetCardRuntimeData>();
            for (var i = 0; i < count; i++)
            {
                var card = new AssetCardData(
                    "owned-stock-" + i,
                    "보유 주식 " + i,
                    "포트폴리오 제한 테스트 카드",
                    AssetRarity.Common,
                    1,
                    new ProfessionalResourceCost[0],
                    1,
                    0,
                    new TagData[0]);
                cards.Add(new AssetCardRuntimeData(card, AssetCardRuntimeState.Owned, PurchaseSource.MarketTape, i + 1));
            }

            return cards;
        }
    }
}
