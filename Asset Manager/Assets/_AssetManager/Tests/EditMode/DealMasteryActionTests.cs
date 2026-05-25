using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class DealMasteryActionTests
    {
        [Test]
        public void DroppingDealOnInvestmentPhilosophyConsumesOneDealAndAddsOneMastery()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddDeal(run, 1).Run;

            var result = DealMasteryAction.Apply(run, ResourceType.Reading);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Run.Resources.Deal, Is.EqualTo(0));
            Assert.That(result.Run.InvestmentPhilosophyMastery.Reading, Is.EqualTo(1));
        }

        [Test]
        public void DroppingDealWithNoDealOrMaxMasteryDoesNotConsumeDeal()
        {
            var noDealRun = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var noDeal = DealMasteryAction.Apply(noDealRun, ResourceType.Meditation);

            var maxRun = ResourceLedger.AddDeal(noDealRun, 1).Run;
            maxRun = ResourceLedger.AddInvestmentPhilosophyMastery(maxRun, ResourceType.Patience, 5).Run;
            var maxMastery = DealMasteryAction.Apply(maxRun, ResourceType.Patience);

            Assert.That(noDeal.Succeeded, Is.False);
            Assert.That(noDeal.Run.Resources.Deal, Is.EqualTo(0));
            Assert.That(noDeal.Run.InvestmentPhilosophyMastery.Meditation, Is.EqualTo(0));
            Assert.That(maxMastery.Succeeded, Is.False);
            Assert.That(maxMastery.Run.Resources.Deal, Is.EqualTo(1));
            Assert.That(maxMastery.Run.InvestmentPhilosophyMastery.Patience, Is.EqualTo(5));
        }
    }
}
