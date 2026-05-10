namespace AssetManager
{
    public static class RunStatusFormatter
    {
        public static string Format(RunSessionState run)
        {
            var resources = run.Resources;
            var redemptionPressure = run.RedemptionPressure;

            return string.Format(
                "{0}회계년도 {1}Q | 남은 {2}영업일 | 현금 {3} | 리서치 {4} | 신용 {5} | 원자재 {6} | 딜 {7}/{8} | 환매 압력 {9}/{10}",
                run.Calendar.FiscalYear,
                run.Calendar.Quarter,
                run.Calendar.RemainingBusinessDays,
                resources.Cash,
                resources.Research,
                resources.Credit,
                resources.Commodity,
                resources.Deal,
                run.StaticData.ResourceConfig.MaxDeal,
                redemptionPressure.CurrentPressure,
                redemptionPressure.MaxPressure);
        }
    }
}
