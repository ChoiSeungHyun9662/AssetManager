using System;

namespace AssetManager
{
    public static class RentArrears
    {
        public const string FailureReason = "\uD30C\uC0B0";

        public static RentArrearsResult AddArrears(RunSessionState run, int amount)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Rent arrears amount cannot be negative.");
            }

            if (amount == 0 || run.State == RunState.Failed)
            {
                return new RentArrearsResult(run, amount, run.State == RunState.Failed);
            }

            var arrearsRun = WithArrears(
                run,
                new RedemptionPressureState(
                    run.RentArrears.CurrentArrears + amount,
                    run.RentArrears.MaxArrears));
            if (arrearsRun.RentArrears.CurrentArrears < arrearsRun.RentArrears.MaxArrears)
            {
                return new RentArrearsResult(arrearsRun, amount, false);
            }

            return new RentArrearsResult(FailRun(arrearsRun), amount, true);
        }

        private static RunSessionState WithArrears(RunSessionState run, RedemptionPressureState rentArrears)
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
                rentArrears,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery);
        }

        private static RunSessionState FailRun(RunSessionState run)
        {
            return new RunSessionState(
                RunState.Failed,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                new BusinessDayState(run.BusinessDay.Phase, MarketAreaState.Market),
                run.RedemptionPressure,
                CardDetailState.Empty,
                run.LiquidityAction,
                run.QuarterEndResult,
                FailureReason,
                run.InvestmentPhilosophyMastery);
        }
    }

    public sealed class RentArrearsResult
    {
        public RentArrearsResult(RunSessionState run, int arrearsIncrease, bool didFail)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            ArrearsIncrease = arrearsIncrease;
            DidFail = didFail;
        }

        public RunSessionState Run { get; }
        public int ArrearsIncrease { get; }
        public int PressureIncrease => ArrearsIncrease;
        public bool DidFail { get; }
    }

    public static class RedemptionPressure
    {
        public const string FailureReason = RentArrears.FailureReason;

        public static RedemptionPressureResult AddPressure(RunSessionState run, int amount)
        {
            var result = RentArrears.AddArrears(run, amount);
            return new RedemptionPressureResult(result.Run, result.ArrearsIncrease, result.DidFail);
        }
    }

    public sealed class RedemptionPressureResult
    {
        public RedemptionPressureResult(RunSessionState run, int pressureIncrease, bool didFail)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            PressureIncrease = pressureIncrease;
            DidFail = didFail;
        }

        public RunSessionState Run { get; }
        public int PressureIncrease { get; }
        public bool DidFail { get; }
    }
}
