namespace AssetManager
{
    public static class RunStatusFormatter
    {
        public static string Format(RunSessionState run)
        {
            var resources = run.Resources;
            var redemptionPressure = run.RedemptionPressure;
            var quarterGoal = FindQuarterGoal(run);
            var status = string.Format(
                "{0}회계년도 {1}Q | 남은 {2}영업일 | 분기 목표 {3}/{4} | 현금 {5} | 리서치 {6} | 신용 {7} | 원자재 {8} | 딜 {9}/{10} | 환매 압력 {11}/{12}",
                run.Calendar.FiscalYear,
                run.Calendar.Quarter,
                run.Calendar.RemainingBusinessDays,
                run.Performance.CurrentQuarterEarnedCash,
                quarterGoal,
                resources.Cash,
                resources.Research,
                resources.Credit,
                resources.Commodity,
                resources.Deal,
                run.StaticData.ResourceConfig.MaxDeal,
                redemptionPressure.CurrentPressure,
                redemptionPressure.MaxPressure);

            return run.BusinessDay.IsAwaitingExtraBuyChoice
                ? status + " | 추가 매수 가능"
                : status;
        }

        private static int FindQuarterGoal(RunSessionState run)
        {
            foreach (var quarter in run.StaticData.Quarters)
            {
                if (quarter.FiscalYear == run.Calendar.FiscalYear && quarter.Quarter == run.Calendar.Quarter)
                {
                    return quarter.EarnedCashGoal;
                }
            }

            return 0;
        }
    }
}
