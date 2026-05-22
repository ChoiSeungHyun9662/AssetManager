using NUnit.Framework;

namespace AssetManager.Tests
{
    public sealed class RentArrearsTests
    {
        [Test]
        public void AddArrearsIncreasesRentArrearsWithoutBankruptcyBelowLimit()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());

            var result = RentArrears.AddArrears(run, 2);

            Assert.That(result.DidFail, Is.False);
            Assert.That(result.ArrearsIncrease, Is.EqualTo(2));
            Assert.That(result.Run.RentArrears.CurrentArrears, Is.EqualTo(2));
            Assert.That(result.Run.State, Is.EqualTo(RunState.Playing));
        }

        [Test]
        public void AddArrearsAtLimitBankruptsRunImmediately()
        {
            var run = RunBootstrapper.CreateNewRun(RunStaticDataSet.CreateMvpDefaults());
            run = WithRentArrears(run, 9);

            var result = RentArrears.AddArrears(run, 1);

            Assert.That(result.DidFail, Is.True);
            Assert.That(result.Run.RentArrears.CurrentArrears, Is.EqualTo(10));
            Assert.That(result.Run.State, Is.EqualTo(RunState.Failed));
            Assert.That(result.Run.FailureReason, Is.EqualTo(RentArrears.FailureReason));
            Assert.That(result.Run.BusinessDay.MarketArea, Is.EqualTo(MarketAreaState.Market));
            Assert.That(result.Run.CardDetail.SelectedCard, Is.Null);
        }

        private static RunSessionState WithRentArrears(RunSessionState run, int currentArrears)
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
                new RedemptionPressureState(currentArrears, run.RentArrears.MaxArrears),
                run.CardDetail,
                run.LiquidityAction);
        }
    }
}
