using System;

namespace AssetManager
{
    public static class QuarterSettlement
    {
        public static QuarterSettlementResult Settle(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var settlementIncome = run.OwnedAssets.CurrentManagementValue;
            var settledRun = ResourceLedger.AddEarnedCash(run, settlementIncome);
            var quarterEarnedCash = settledRun.Performance.CurrentQuarterEarnedCash;
            var targetEarnedCash = GetTargetEarnedCash(settledRun);
            var achievementRate = targetEarnedCash <= 0
                ? 1d
                : Math.Min(1d, quarterEarnedCash / (double)targetEarnedCash);
            var pressureIncrease = CalculatePressureIncrease(achievementRate);
            settledRun = RecordCompletedQuarter(settledRun, quarterEarnedCash);
            var pressureResult = RedemptionPressure.AddPressure(settledRun, pressureIncrease);
            var result = new QuarterEndResult(
                settlementIncome,
                quarterEarnedCash,
                targetEarnedCash,
                achievementRate,
                pressureIncrease,
                pressureResult.Run.RedemptionPressure.CurrentPressure);

            return new QuarterSettlementResult(WithQuarterEndResult(pressureResult.Run, result), result);
        }

        private static int CalculatePressureIncrease(double achievementRate)
        {
            if (achievementRate >= 1d)
            {
                return 0;
            }

            if (achievementRate >= 0.75d)
            {
                return 1;
            }

            if (achievementRate >= 0.5d)
            {
                return 2;
            }

            return 3;
        }

        private static int GetTargetEarnedCash(RunSessionState run)
        {
            foreach (var quarter in run.StaticData.Quarters)
            {
                if (quarter.FiscalYear == run.Calendar.FiscalYear && quarter.Quarter == run.Calendar.Quarter)
                {
                    return quarter.EarnedCashGoal;
                }
            }

            return run.StaticData.Quarters[0].EarnedCashGoal;
        }

        private static RunSessionState RecordCompletedQuarter(RunSessionState run, int quarterEarnedCash)
        {
            var records = new System.Collections.Generic.List<QuarterPerformanceRecord>(
                run.Performance.CompletedQuarterEarnedCash)
            {
                new QuarterPerformanceRecord(run.Calendar.FiscalYear, run.Calendar.Quarter, quarterEarnedCash)
            };

            var performance = new RunPerformanceState(
                run.Performance.CurrentQuarterEarnedCash,
                run.Performance.CurrentFiscalYearEarnedCash,
                run.Performance.TotalEarnedCash,
                run.Performance.FundingCash,
                records);

            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
        }

        private static RunSessionState WithQuarterEndResult(RunSessionState run, QuarterEndResult result)
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
                result,
                run.FailureReason);
        }
    }

    public sealed class QuarterSettlementResult
    {
        public QuarterSettlementResult(RunSessionState run, QuarterEndResult quarterEnd)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            QuarterEnd = quarterEnd ?? throw new ArgumentNullException(nameof(quarterEnd));
        }

        public RunSessionState Run { get; }
        public QuarterEndResult QuarterEnd { get; }
        public int SettlementIncome => QuarterEnd.SettlementIncome;
        public int QuarterEarnedCash => QuarterEnd.QuarterEarnedCash;
        public int TargetEarnedCash => QuarterEnd.TargetEarnedCash;
        public double AchievementRate => QuarterEnd.AchievementRate;
        public int RedemptionPressureIncrease => QuarterEnd.RedemptionPressureIncrease;
        public int CurrentRedemptionPressure => QuarterEnd.CurrentRedemptionPressure;
    }
}
