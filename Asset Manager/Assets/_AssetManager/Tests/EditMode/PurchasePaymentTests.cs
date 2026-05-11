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
            Assert.That(result.Run.Resources.Cash, Is.EqualTo(0));
            Assert.That(result.Run.Resources.Research, Is.EqualTo(0));
            Assert.That(result.Run.Resources.Credit, Is.EqualTo(0));
            Assert.That(result.Run.OwnedAssets.OwnedCards, Has.Count.EqualTo(1));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].Card.Id, Is.EqualTo(selectedCard.Card.Id));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].State, Is.EqualTo(AssetCardRuntimeState.Owned));
            Assert.That(result.Run.OwnedAssets.OwnedCards[0].PurchaseSource, Is.EqualTo(PurchaseSource.MarketTape));
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
    }
}
