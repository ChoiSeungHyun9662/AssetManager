using System.Collections.Generic;
using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class MissionConfirmationActionTests
    {
        [Test]
        public void FastEntryAndConcentrationClearFromOwnedTargetTagCounts()
        {
            var tag = StockTagCatalog.Technology;
            var fast = CreateMission("fast", MissionTemplate.FastEntry, new[] { tag }, 3);
            var concentration = CreateMission("concentration", MissionTemplate.Concentration, new[] { tag }, 5);
            var run = WithMissions(CreateRun(), fast, concentration);

            var belowFast = MissionConfirmationAction.Evaluate(WithOwnedStocks(run, tag, 2));
            Assert.That(belowFast.Confirmed, Is.False);

            var fastClear = MissionConfirmationAction.Evaluate(WithOwnedStocks(run, tag, 3));
            Assert.That(fastClear.Confirmed, Is.True);
            Assert.That(fastClear.Run.Missions.ConfirmedMission.Id, Is.EqualTo("fast"));

            var concentrationClear = MissionConfirmationAction.Evaluate(
                WithMissions(WithOwnedStocks(run, tag, 5), concentration));
            Assert.That(concentrationClear.Confirmed, Is.True);
            Assert.That(concentrationClear.Run.Missions.ConfirmedMission.Id, Is.EqualTo("concentration"));
        }

        [Test]
        public void FoilAndHighValueClearFromOwnedTargetTagCards()
        {
            var tag = StockTagCatalog.Consumer;
            var foil = CreateMission("foil", MissionTemplate.Foil, new[] { tag }, 1);
            var highValue = CreateMission("high-value", MissionTemplate.HighValue, new[] { tag }, 7);
            var nonFoilRun = WithMissions(CreateRun(), foil);
            nonFoilRun = WithOwnedStocks(nonFoilRun, tag, 1, value: 3, foil: false);

            Assert.That(MissionConfirmationAction.Evaluate(nonFoilRun).Confirmed, Is.False);

            var foilClear = MissionConfirmationAction.Evaluate(
                WithOwnedStocks(WithMissions(CreateRun(), foil), tag, 1, value: 3, foil: true));
            Assert.That(foilClear.Confirmed, Is.True);
            Assert.That(foilClear.Run.Missions.ConfirmedMission.Id, Is.EqualTo("foil"));

            var highValueClear = MissionConfirmationAction.Evaluate(
                WithOwnedStocks(WithMissions(CreateRun(), highValue), tag, 1, value: 7, foil: false));
            Assert.That(highValueClear.Confirmed, Is.True);
            Assert.That(highValueClear.Run.Missions.ConfirmedMission.Id, Is.EqualTo("high-value"));
        }

        [Test]
        public void TwoTagStableRequiresEachTargetTagCount()
        {
            var technology = StockTagCatalog.Technology;
            var consumer = StockTagCatalog.Consumer;
            var mission = CreateMission("two-tag", MissionTemplate.TwoTagStable, new[] { technology, consumer }, 2);
            var run = WithMissions(CreateRun(), mission);

            var onlyOneConsumer = WithOwnedStocks(run, technology, 2);
            onlyOneConsumer = WithOwnedStocks(onlyOneConsumer, consumer, 1, startingIndex: 2);
            Assert.That(MissionConfirmationAction.Evaluate(onlyOneConsumer).Confirmed, Is.False);

            var bothTags = WithOwnedStocks(run, technology, 2);
            bothTags = WithOwnedStocks(bothTags, consumer, 2, startingIndex: 2);
            var result = MissionConfirmationAction.Evaluate(bothTags);

            Assert.That(result.Confirmed, Is.True);
            Assert.That(result.Run.Missions.ConfirmedMission.Id, Is.EqualTo("two-tag"));
        }

        [Test]
        public void ClearChecksIgnoreMarketAndReservedStocks()
        {
            var tag = StockTagCatalog.Energy;
            var mission = CreateMission("owned-only", MissionTemplate.FastEntry, new[] { tag }, 3);
            var run = WithMissions(CreateRun(), mission);
            run = WithOwnedStocks(run, tag, 2);
            var marketCard = CreateRuntimeStock("market-target", tag, AssetCardRuntimeState.Available, value: 3, foil: false);
            var reservedCard = CreateRuntimeStock("reserved-target", tag, AssetCardRuntimeState.Reserved, value: 3, foil: false);
            run = WithMarketAndReservation(run, marketCard, reservedCard);

            var result = MissionConfirmationAction.Evaluate(run);

            Assert.That(result.Confirmed, Is.False);
            Assert.That(result.Run.Missions.ConfirmedMission, Is.Null);
        }

        [Test]
        public void SimultaneousClearsConfirmLeftmostCandidateAndDiscardTheRest()
        {
            var tag = StockTagCatalog.Financials;
            var left = CreateMission("left", MissionTemplate.FastEntry, new[] { tag }, 3);
            var right = CreateMission("right", MissionTemplate.FastEntry, new[] { tag }, 3);
            var run = WithMissions(CreateRun(), left, right);
            run = WithOwnedStocks(run, tag, 3);

            var result = MissionConfirmationAction.Evaluate(run);

            Assert.That(result.Confirmed, Is.True);
            Assert.That(result.Run.Missions.ConfirmedMission.Id, Is.EqualTo("left"));
            Assert.That(result.Run.Missions.CandidateSlots, Has.Count.EqualTo(3));
            Assert.That(result.Run.Missions.CandidateSlots[0].IsEmpty, Is.False);
            Assert.That(result.Run.Missions.CandidateSlots[1].IsEmpty, Is.True);
            Assert.That(result.Run.Missions.CandidateSlots[2].IsEmpty, Is.True);
        }

        [Test]
        public void ConfirmedMissionCannotBeReplacedByLaterClear()
        {
            var technology = StockTagCatalog.Technology;
            var consumer = StockTagCatalog.Consumer;
            var first = CreateMission("first", MissionTemplate.FastEntry, new[] { technology }, 3);
            var later = CreateMission("later", MissionTemplate.FastEntry, new[] { consumer }, 3);
            var run = WithMissions(CreateRun(), first, later);
            var confirmed = MissionConfirmationAction.Evaluate(WithOwnedStocks(run, technology, 3)).Run;
            var laterPortfolio = WithOwnedStocks(confirmed, consumer, 3, startingIndex: 3);

            var result = MissionConfirmationAction.Evaluate(laterPortfolio);

            Assert.That(result.Confirmed, Is.False);
            Assert.That(result.Run.Missions.ConfirmedMission.Id, Is.EqualTo("first"));
        }

        private static RunSessionState CreateRun()
        {
            return RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
        }

        private static MissionDefinitionData CreateMission(
            string id,
            MissionTemplate template,
            IEnumerable<TagData> tags,
            int clearTargetCount)
        {
            return new MissionDefinitionData(
                id,
                id,
                template,
                tags,
                "clear",
                "formula",
                "Test",
                clearTargetCount,
                1);
        }

        private static RunSessionState WithMissions(RunSessionState run, params MissionDefinitionData[] missions)
        {
            var slots = new List<MissionCandidateSlotState>();
            for (var i = 0; i < 3; i++)
            {
                slots.Add(new MissionCandidateSlotState(i < missions.Length ? missions[i] : null, false));
            }

            return WithMissions(run, new MissionRunState(slots, 3));
        }

        private static RunSessionState WithMissions(RunSessionState run, MissionRunState missions)
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
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                missions);
        }

        private static RunSessionState WithOwnedStocks(
            RunSessionState run,
            TagData tag,
            int count,
            int value = 3,
            bool foil = false,
            int startingIndex = 0)
        {
            var cards = new List<AssetCardRuntimeData>(run.OwnedAssets.StockSlots);
            for (var i = 0; i < count; i++)
            {
                cards.Add(CreateRuntimeStock(tag.Id + "-" + (startingIndex + i), tag, AssetCardRuntimeState.Owned, value, foil));
            }

            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                new OwnedAssetState(cards),
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
        }

        private static RunSessionState WithMarketAndReservation(
            RunSessionState run,
            AssetCardRuntimeData marketCard,
            AssetCardRuntimeData reservedCard)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                new[] { marketCard, reservedCard },
                new MarketTapeState(new[] { new MarketTapeSlotState(marketCard, false) }),
                new ReservationState(1, new[] { reservedCard }),
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
        }

        private static AssetCardRuntimeData CreateRuntimeStock(
            string id,
            TagData tag,
            AssetCardRuntimeState state,
            int value,
            bool foil)
        {
            var card = new AssetCardData(
                id,
                id,
                "test stock",
                AssetRarity.Common,
                0,
                new ProfessionalResourceCost[0],
                value,
                0,
                new[] { tag },
                foilValue: value);
            return new AssetCardRuntimeData(card, state, PurchaseSource.MarketTape, null, foil, id + "#runtime");
        }
    }
}
