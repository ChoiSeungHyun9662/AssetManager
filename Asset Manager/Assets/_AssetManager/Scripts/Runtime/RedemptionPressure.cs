using System;

namespace AssetManager
{
    public static class RedemptionPressure
    {
        public const string FailureReason = "대규모 환매 발생";

        public static RedemptionPressureResult AddPressure(RunSessionState run, int amount)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Pressure amount cannot be negative.");
            }

            if (amount == 0 || run.State == RunState.Failed)
            {
                return new RedemptionPressureResult(run, amount, run.State == RunState.Failed);
            }

            var pressuredRun = WithPressure(
                run,
                new RedemptionPressureState(
                    run.RedemptionPressure.CurrentPressure + amount,
                    run.RedemptionPressure.MaxPressure));
            if (pressuredRun.RedemptionPressure.CurrentPressure < pressuredRun.RedemptionPressure.MaxPressure)
            {
                return new RedemptionPressureResult(pressuredRun, amount, false);
            }

            return new RedemptionPressureResult(FailRun(pressuredRun), amount, true);
        }

        private static RunSessionState WithPressure(RunSessionState run, RedemptionPressureState pressure)
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
                pressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
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
                FailureReason);
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
