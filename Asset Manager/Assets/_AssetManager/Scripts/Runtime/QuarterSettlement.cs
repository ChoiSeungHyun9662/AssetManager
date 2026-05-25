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

            var missionRevenue = MissionSettlement.Calculate(run.Missions.ConfirmedMission, run.OwnedAssets);
            var settledRun = AddMissionRevenue(run, missionRevenue);
            var quarterRevenue = settledRun.Performance.CurrentQuarterRevenue;
            var quarterEvaluationValue = quarterRevenue + missionRevenue;
            var targetRevenue = GetTargetRevenue(settledRun);
            var achievementRate = targetRevenue <= 0
                ? 1d
                : Math.Min(1d, quarterEvaluationValue / (double)targetRevenue);
            var rentArrearsIncrease = CalculateRentArrearsIncrease(achievementRate);
            settledRun = RecordCompletedQuarter(settledRun, quarterRevenue);
            var arrearsResult = RentArrears.AddArrears(settledRun, rentArrearsIncrease);
            var result = new QuarterEndResult(
                missionRevenue,
                quarterRevenue,
                targetRevenue,
                achievementRate,
                rentArrearsIncrease,
                arrearsResult.Run.RentArrears.CurrentArrears,
                missionRevenue,
                quarterEvaluationValue);

            return new QuarterSettlementResult(WithQuarterEndResult(arrearsResult.Run, result), result);
        }

        private static RunSessionState AddMissionRevenue(RunSessionState run, int missionRevenue)
        {
            var performance = run.Performance;
            return WithPerformance(
                run,
                new RunPerformanceState(
                    performance.CurrentQuarterRevenue,
                    performance.CurrentFiscalYearRevenue,
                    performance.TotalRevenue,
                    performance.FundingCash,
                    performance.CompletedQuarterRevenue,
                    performance.CurrentQuarterMissionRevenue + missionRevenue,
                    performance.CurrentFiscalYearMissionRevenue + missionRevenue,
                    performance.TotalMissionRevenue + missionRevenue));
        }

        private static int CalculateRentArrearsIncrease(double achievementRate)
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

        private static int GetTargetRevenue(RunSessionState run)
        {
            foreach (var quarter in run.StaticData.Quarters)
            {
                if (quarter.FiscalYear == run.Calendar.FiscalYear && quarter.Quarter == run.Calendar.Quarter)
                {
                    return quarter.RevenueGoal;
                }
            }

            return run.StaticData.Quarters[0].RevenueGoal;
        }

        private static RunSessionState RecordCompletedQuarter(RunSessionState run, int quarterRevenue)
        {
            var records = new System.Collections.Generic.List<QuarterPerformanceRecord>(
                run.Performance.CompletedQuarterRevenue)
            {
                new QuarterPerformanceRecord(run.Calendar.FiscalYear, run.Calendar.Quarter, quarterRevenue)
            };

            var performance = new RunPerformanceState(
                run.Performance.CurrentQuarterRevenue,
                run.Performance.CurrentFiscalYearRevenue,
                run.Performance.TotalRevenue,
                run.Performance.FundingCash,
                records,
                run.Performance.CurrentQuarterMissionRevenue,
                run.Performance.CurrentFiscalYearMissionRevenue,
                run.Performance.TotalMissionRevenue);

            return WithPerformance(run, performance);
        }

        private static RunSessionState WithPerformance(RunSessionState run, RunPerformanceState performance)
        {
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
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
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
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
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
        public int SettlementRevenue => SettlementIncome;
        public int MissionRevenue => QuarterEnd.MissionRevenue;
        public int QuarterEarnedCash => QuarterEnd.QuarterEarnedCash;
        public int QuarterRevenue => QuarterEarnedCash;
        public int CashFlow => QuarterEnd.CashFlow;
        public int QuarterEvaluationValue => QuarterEnd.QuarterEvaluationValue;
        public int TargetEarnedCash => QuarterEnd.TargetEarnedCash;
        public int QuarterRevenueTarget => TargetEarnedCash;
        public double AchievementRate => QuarterEnd.AchievementRate;
        public int RedemptionPressureIncrease => QuarterEnd.RedemptionPressureIncrease;
        public int RentArrearsIncrease => RedemptionPressureIncrease;
        public int CurrentRedemptionPressure => QuarterEnd.CurrentRedemptionPressure;
        public int CurrentRentArrears => CurrentRedemptionPressure;
    }
}
