using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class LiquidityActionTests
    {
        [Test]
        public void ClosingBeforeFirstResourceReturnsToMarketWithoutConsumingBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;

            var liquidityRun = LiquidityAction.Enter(run);

            Assert.That(liquidityRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.GainLiquidity));
            Assert.That(MarketAreaFlow.CanAdvanceToNextBusinessDay(liquidityRun), Is.False);

            var closedRun = LiquidityAction.Close(liquidityRun);

            Assert.That(closedRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(closedRun.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(closedRun.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
            Assert.That(MarketAreaFlow.CanAdvanceToNextBusinessDay(closedRun), Is.True);
        }

        [Test]
        public void SelectingCashTwiceAddsFundingCashAndConsumesOneBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var startingCash = run.Resources.Cash;
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;

            var firstSelection = LiquidityAction.Select(
                LiquidityAction.Enter(run),
                ResourceType.Cash);

            Assert.That(firstSelection.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.GainLiquidity));
            Assert.That(firstSelection.Run.Resources.Cash, Is.EqualTo(startingCash + 1));
            Assert.That(firstSelection.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays));
            Assert.That(LiquidityAction.CanClose(firstSelection.Run), Is.False);

            var secondSelection = LiquidityAction.Select(firstSelection.Run, ResourceType.Cash);

            Assert.That(secondSelection.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(secondSelection.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(secondSelection.Run.Resources.Cash, Is.EqualTo(startingCash + 2));
            Assert.That(secondSelection.Run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
            Assert.That(secondSelection.Run.Performance.CurrentFiscalYearEarnedCash, Is.EqualTo(0));
            Assert.That(secondSelection.Run.Performance.TotalEarnedCash, Is.EqualTo(0));
        }

        [Test]
        public void SelectingThreeDifferentBasicResourcesAppliesResourcesAndConsumesOneBusinessDay()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            var startingCash = run.Resources.Cash;
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;

            var cashSelection = LiquidityAction.Select(
                LiquidityAction.Enter(run),
                ResourceType.Cash);
            var researchSelection = LiquidityAction.Select(cashSelection.Run, ResourceType.Research);
            var creditSelection = LiquidityAction.Select(researchSelection.Run, ResourceType.Credit);

            Assert.That(creditSelection.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(creditSelection.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(creditSelection.Run.Resources.Cash, Is.EqualTo(startingCash + 1));
            Assert.That(creditSelection.Run.Resources.Research, Is.EqualTo(1));
            Assert.That(creditSelection.Run.Resources.Credit, Is.EqualTo(1));
            Assert.That(creditSelection.Run.Resources.Commodity, Is.EqualTo(0));
            Assert.That(creditSelection.Run.Performance.CurrentQuarterEarnedCash, Is.EqualTo(0));
        }

        [Test]
        public void InvalidNextResourceChoicesAreNotSelectable()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());

            var cashSelection = LiquidityAction.Select(
                LiquidityAction.Enter(run),
                ResourceType.Cash);
            var researchSelection = LiquidityAction.Select(cashSelection.Run, ResourceType.Research);

            Assert.That(LiquidityAction.CanSelect(researchSelection.Run, ResourceType.Cash), Is.False);
            Assert.That(LiquidityAction.CanSelect(researchSelection.Run, ResourceType.Research), Is.False);
            Assert.That(LiquidityAction.CanSelect(researchSelection.Run, ResourceType.Credit), Is.True);
            Assert.That(LiquidityAction.CanSelect(researchSelection.Run, ResourceType.Commodity), Is.True);
            Assert.That(LiquidityAction.CanSelect(researchSelection.Run, ResourceType.Deal), Is.False);

            var blockedSelection = LiquidityAction.Select(researchSelection.Run, ResourceType.Cash);

            Assert.That(blockedSelection.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.GainLiquidity));
            Assert.That(blockedSelection.Run.LiquidityAction.SelectedResources, Has.Count.EqualTo(2));
            Assert.That(blockedSelection.Run.Resources.Cash, Is.EqualTo(researchSelection.Run.Resources.Cash));
            Assert.That(blockedSelection.Run.Resources.Research, Is.EqualTo(researchSelection.Run.Resources.Research));
        }

        [Test]
        public void ProfessionalResourcesAreNotSelectableAtProfessionalResourceCap()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 4).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 3).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Commodity, 3).Run;

            var liquidityRun = LiquidityAction.Enter(run);

            Assert.That(liquidityRun.Resources.ProfessionalTotal, Is.EqualTo(10));
            Assert.That(LiquidityAction.CanSelect(liquidityRun, ResourceType.Cash), Is.True);
            Assert.That(LiquidityAction.CanSelect(liquidityRun, ResourceType.Research), Is.False);
            Assert.That(LiquidityAction.CanSelect(liquidityRun, ResourceType.Credit), Is.False);
            Assert.That(LiquidityAction.CanSelect(liquidityRun, ResourceType.Commodity), Is.False);
            Assert.That(LiquidityAction.CanSelect(liquidityRun, ResourceType.Deal), Is.False);

            var blockedSelection = LiquidityAction.Select(liquidityRun, ResourceType.Research);

            Assert.That(blockedSelection.Run.Resources.ProfessionalTotal, Is.EqualTo(10));
            Assert.That(blockedSelection.Run.LiquidityAction.SelectedResources, Is.Empty);
            Assert.That(blockedSelection.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.GainLiquidity));
        }

        [Test]
        public void ReachingProfessionalResourceCapAutoEndsWhenNoValidNextChoiceRemains()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Research, 3).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Credit, 3).Run;
            run = ResourceLedger.AddProfessionalResource(run, ResourceType.Commodity, 3).Run;
            var remainingBusinessDays = run.Calendar.RemainingBusinessDays;
            var startingCash = run.Resources.Cash;

            var cashSelection = LiquidityAction.Select(
                LiquidityAction.Enter(run),
                ResourceType.Cash);
            var researchSelection = LiquidityAction.Select(cashSelection.Run, ResourceType.Research);

            Assert.That(researchSelection.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(researchSelection.Run.Calendar.RemainingBusinessDays, Is.EqualTo(remainingBusinessDays - 1));
            Assert.That(researchSelection.Run.Resources.Cash, Is.EqualTo(startingCash + 1));
            Assert.That(researchSelection.Run.Resources.ProfessionalTotal, Is.EqualTo(10));
            Assert.That(researchSelection.Message, Is.Not.Empty);
        }

        [Test]
        public void ExtraBuyChoiceCannotEnterLiquidityAction()
        {
            var run = ExtraBuyAction.BeginChoice(RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults()));

            var liquidityRun = LiquidityAction.Enter(run);

            Assert.That(liquidityRun.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(liquidityRun.BusinessDay.HasExtraBuyAction, Is.True);
            Assert.That(liquidityRun.BusinessDay.IsAwaitingExtraBuyChoice, Is.True);
        }
    }
}
