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
            run = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);

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
            var selectedCard = run.MarketTape.SellImminentCards[2];
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
        public void MarketCardPurchaseConsumesPaymentOwnsCardAdvancesOnlyPurchasedColumnAndConsumesBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            var selectedCard = run.MarketTape.CurrentMarketCards[0];
            var previousSellImminentIds = CollectCardIds(run.MarketTape.SellImminentCards);
            var previousCurrentMarketIds = CollectCardIds(run.MarketTape.CurrentMarketCards);
            var previousUpcomingMarketIds = CollectCardIds(run.MarketTape.UpcomingMarketCards);
            run = MarketAreaFlow.OpenMarketCardDetail(run, selectedCard);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(selectedCard.Card.Income));
            Assert.That(result.Run.Resources.Research, Is.EqualTo(0));
            Assert.That(result.Run.Resources.Credit, Is.EqualTo(0));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].State, Is.EqualTo(AssetCardRuntimeState.Owned));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].AcquiredOrder, Is.EqualTo(1));
            Assert.That(FindCard(result.Run.AssetCards, selectedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Owned));
            Assert.That(result.Run.MarketTape.SellImminentCards, Has.Count.EqualTo(previousSellImminentIds.Count));
            AssertZoneMatches(result.Run.MarketTape.SellImminentCards, previousSellImminentIds);
            Assert.That(result.Run.MarketTape.CurrentMarketCards, Has.Count.EqualTo(previousCurrentMarketIds.Count));
            Assert.That(result.Run.MarketTape.CurrentMarketCards[0].Card.Id, Is.EqualTo(previousUpcomingMarketIds[0]));
            Assert.That(result.Run.MarketTape.CurrentMarketCards[1].Card.Id, Is.EqualTo(previousCurrentMarketIds[1]));
            Assert.That(result.Run.MarketTape.CurrentMarketCards[2].Card.Id, Is.EqualTo(previousCurrentMarketIds[2]));
            Assert.That(result.Run.MarketTape.UpcomingMarketCards, Has.Count.EqualTo(previousUpcomingMarketIds.Count));
            Assert.That(result.Run.MarketTape.UpcomingMarketCards[0].Card.Id, Is.Not.EqualTo(previousUpcomingMarketIds[0]));
            Assert.That(result.Run.MarketTape.UpcomingMarketCards[1].Card.Id, Is.EqualTo(previousUpcomingMarketIds[1]));
            Assert.That(result.Run.MarketTape.UpcomingMarketCards[2].Card.Id, Is.EqualTo(previousUpcomingMarketIds[2]));
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
            run = WithExtraBuyGrantOnCurrentMarketCard(run, 0);
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            run = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);
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
            run = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);
            run = ReservationAction.ConfirmReservation(run).Run;
            var reservedCard = run.Reservation.ReservedCards[0];
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            run = WithExtraBuyGrantOnCurrentMarketCard(run, 0);
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            run = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;
            var extraBuyRun = PurchasePayment.ConfirmPurchase(run).Run;
            extraBuyRun = ResourceLedger.AddFundingCash(extraBuyRun, 3);
            extraBuyRun = ResourceLedger.AddProfessionalResource(extraBuyRun, ResourceType.Research, 1).Run;
            extraBuyRun = ResourceLedger.AddProfessionalResource(extraBuyRun, ResourceType.Credit, 1).Run;
            extraBuyRun = MarketAreaFlow.OpenReservedCardDetail(extraBuyRun, reservedCard);
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
        public void ReservedCardPurchaseOwnsCardClearsPurchasedReservationAndLeavesMarketTapeUnchanged()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            var marketCard = run.MarketTape.CurrentMarketCards[0];
            run = MarketAreaFlow.OpenMarketCardDetail(run, marketCard);
            run = ReservationAction.ConfirmReservation(run).Run;
            var reservedCard = run.Reservation.ReservedCards[0];
            run = MarketAreaFlow.OpenMarketCardDetail(run, run.MarketTape.CurrentMarketCards[0]);
            run = ReservationAction.ConfirmReservation(run).Run;
            var otherReservedCard = run.Reservation.ReservedCards[1];
            var previousSellImminentIds = CollectCardIds(run.MarketTape.SellImminentCards);
            var previousCurrentMarketIds = CollectCardIds(run.MarketTape.CurrentMarketCards);
            var previousUpcomingMarketIds = CollectCardIds(run.MarketTape.UpcomingMarketCards);
            run = MarketAreaFlow.OpenReservedCardDetail(run, reservedCard);
            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(reservedCard.Card.Id));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.Reserved));
            Assert.That(FindCard(result.Run.AssetCards, reservedCard.Card.Id).State, Is.EqualTo(AssetCardRuntimeState.Owned));
            Assert.That(result.Run.Reservation.ReservedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.Reservation.ReservedCards[0].Card.Id, Is.EqualTo(otherReservedCard.Card.Id));
            AssertZoneMatches(result.Run.MarketTape.SellImminentCards, previousSellImminentIds);
            AssertZoneMatches(result.Run.MarketTape.CurrentMarketCards, previousCurrentMarketIds);
            AssertZoneMatches(result.Run.MarketTape.UpcomingMarketCards, previousUpcomingMarketIds);
            Assert.That(result.Run.Calendar.RemainingBusinessDays, Is.EqualTo(run.Calendar.RemainingBusinessDays - 1));
        }

        [Test]
        public void ReservedCardPurchaseUsesCurrentInflationModifierAtPurchaseTime()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var marketCard = run.MarketTape.CurrentMarketCards[0];
            run = MarketAreaFlow.OpenMarketCardDetail(run, marketCard);
            run = ReservationAction.ConfirmReservation(run).Run;
            var reservedCard = run.Reservation.ReservedCards[0];
            run = WithCalendar(run, new RunCalendarState(1, 2, run.Calendar.RemainingBusinessDays));
            run = ResourceLedger.AddFundingCash(run, 1);
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 1).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 1).Run;
            var cashBeforePurchase = run.Resources.Cash;
            run = MarketAreaFlow.OpenReservedCardDetail(run, reservedCard);

            Assert.That(run.CardDetail.PendingPayment.InflationCostModifier, Is.EqualTo(1));
            Assert.That(run.CardDetail.PendingPayment.FinalCashCost, Is.EqualTo(reservedCard.Card.CashCost + 1));

            run = PurchasePayment.PlaceChip(run, ResourceType.Research).Run;
            run = PurchasePayment.PlaceChip(run, ResourceType.Credit).Run;

            var result = PurchasePayment.ConfirmPurchase(run);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(reservedCard.Card.Id));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.Reserved));
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
                if (card.Card.Id == replacement.Card.Id)
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
    }
}
