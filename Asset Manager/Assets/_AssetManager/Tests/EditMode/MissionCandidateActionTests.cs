using System.Collections.Generic;
using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class MissionCandidateActionTests
    {
        [Test]
        public void CreateNewRunCreatesThreeVisibleMissionCandidateSlotsIndependentOfMarket()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();

            var run = RunBootstrapper.CreateNewRun(staticData);
            var refreshedMarket = MarketTape.Refresh(run);

            Assert.That(run.Missions.CandidateSlots, Has.Count.EqualTo(3));
            Assert.That(refreshedMarket.Missions.CandidateSlots, Has.Count.EqualTo(3));
            for (var i = 0; i < run.Missions.CandidateSlots.Count; i++)
            {
                Assert.That(run.Missions.CandidateSlots[i].Candidate, Is.Not.Null);
                Assert.That(refreshedMarket.Missions.CandidateSlots[i].Candidate.Id, Is.EqualTo(run.Missions.CandidateSlots[i].Candidate.Id));
            }
        }

        [Test]
        public void InitialMissionPoolContainsFiveMissionsPerTemplate()
        {
            var staticData = RunStaticDataSet.CreateMvpDefaults();

            Assert.That(staticData.Missions, Has.Count.EqualTo(25));
            Assert.That(CountTemplate(staticData.Missions, MissionTemplate.FastEntry), Is.EqualTo(5));
            Assert.That(CountTemplate(staticData.Missions, MissionTemplate.Concentration), Is.EqualTo(5));
            Assert.That(CountTemplate(staticData.Missions, MissionTemplate.Foil), Is.EqualTo(5));
            Assert.That(CountTemplate(staticData.Missions, MissionTemplate.HighValue), Is.EqualTo(5));
            Assert.That(CountTemplate(staticData.Missions, MissionTemplate.TwoTagStable), Is.EqualTo(5));
        }

        [Test]
        public void MissionDisplayDataIncludesNameTargetsConditionFormulaAndDisplayDifficulty()
        {
            var mission = RunStaticDataSet.CreateMvpDefaults().Missions[0];

            Assert.That(mission.DisplayName, Is.Not.Empty);
            Assert.That(mission.TargetTags, Has.Count.GreaterThanOrEqualTo(1));
            Assert.That(mission.ClearConditionDescription, Is.Not.Empty);
            Assert.That(mission.SettlementFormulaDescription, Is.Not.Empty);
            Assert.That(mission.DifficultyDisplay, Is.Not.Empty);
            Assert.That(mission.DifficultyDisplay, Is.EqualTo("Easy"));
            Assert.That(mission.ClearTargetCount, Is.EqualTo(3));
            Assert.That(mission.SettlementRewardPerCard, Is.EqualTo(1));
        }

        [Test]
        public void CandidateSlotCanMulliganOnceThenDiscardToEmptySlotWithoutRefill()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var originalCandidateId = run.Missions.CandidateSlots[0].Candidate.Id;

            var mulliganResult = MissionCandidateAction.Mulligan(run, 0);

            Assert.That(mulliganResult.Succeeded, Is.True);
            Assert.That(mulliganResult.Run.Missions.CandidateSlots[0].HasSpentMulligan, Is.True);
            Assert.That(mulliganResult.Run.Missions.CandidateSlots[0].Candidate.Id, Is.Not.EqualTo(originalCandidateId));

            var secondMulliganResult = MissionCandidateAction.Mulligan(mulliganResult.Run, 0);

            Assert.That(secondMulliganResult.Succeeded, Is.False);
            Assert.That(secondMulliganResult.Run.Missions.CandidateSlots[0].Candidate.Id, Is.EqualTo(mulliganResult.Run.Missions.CandidateSlots[0].Candidate.Id));

            var discardResult = MissionCandidateAction.Discard(secondMulliganResult.Run, 0);

            Assert.That(discardResult.Succeeded, Is.True);
            Assert.That(discardResult.Run.Missions.CandidateSlots[0].IsEmpty, Is.True);
            Assert.That(discardResult.Run.Missions.CandidateSlots[0].Candidate, Is.Null);
            Assert.That(discardResult.Run.Missions.CandidateSlots[0].HasSpentMulligan, Is.True);
            Assert.That(discardResult.Run.Missions.CandidateSlots, Has.Count.EqualTo(3));
        }

        [Test]
        public void SameTagCanAppearAcrossMultipleCandidatesInOneRun()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var seenTagIds = new HashSet<string>();

            foreach (var slot in run.Missions.CandidateSlots)
            {
                foreach (var tag in slot.Candidate.TargetTags)
                {
                    if (!seenTagIds.Add(tag.Id))
                    {
                        Assert.Pass();
                    }
                }
            }

            Assert.Fail("Expected at least one tag to appear in multiple initial mission candidates.");
        }

        private static int CountTemplate(IEnumerable<MissionDefinitionData> missions, MissionTemplate template)
        {
            var count = 0;
            foreach (var mission in missions)
            {
                if (mission.Template == template)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
