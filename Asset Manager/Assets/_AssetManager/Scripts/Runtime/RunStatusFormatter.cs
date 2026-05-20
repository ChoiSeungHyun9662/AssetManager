namespace AssetManager
{
    public static class RunStatusFormatter
    {
        public static string Format(RunSessionState run)
        {
            var redemptionPressure = run.RedemptionPressure;
            var quarterGoal = FindQuarterGoal(run);
            var status = string.Format(
                "{0}회계년도 {1}Q | 남은 {2}영업일 | 분기 목표 {3}/{4} | 월세 밀림 {5}/{6}",
                run.Calendar.FiscalYear,
                run.Calendar.Quarter,
                run.Calendar.RemainingBusinessDays,
                run.Performance.CurrentQuarterEarnedCash,
                quarterGoal,
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
