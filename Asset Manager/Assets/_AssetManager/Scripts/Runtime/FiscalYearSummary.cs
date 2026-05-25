using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class FiscalYearSummary
    {
        public static FiscalYearSummaryResult Create(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var quarterRevenue = new List<QuarterPerformanceRecord>();
            foreach (var record in run.Performance.CompletedQuarterRevenue)
            {
                if (record.FiscalYear == run.Calendar.FiscalYear)
                {
                    quarterRevenue.Add(record);
                }
            }

            return new FiscalYearSummaryResult(
                run.Calendar.FiscalYear,
                run.OwnedAssets.CurrentValue + run.Performance.TotalMissionRevenue,
                run.Performance.CurrentFiscalYearRevenue,
                run.Performance.CurrentFiscalYearMissionRevenue,
                quarterRevenue,
                run.OwnedAssets.Count,
                run.RentArrears.CurrentArrears,
                run.RentArrears.MaxArrears);
        }
    }

    public sealed class FiscalYearSummaryResult
    {
        public FiscalYearSummaryResult(
            int fiscalYear,
            int currentValue,
            int fiscalYearEarnedCash,
            int fiscalYearMissionRevenue,
            IEnumerable<QuarterPerformanceRecord> quarterEarnedCash,
            int ownedAssetCount,
            int currentRedemptionPressure,
            int maxRedemptionPressure)
        {
            FiscalYear = fiscalYear;
            CurrentValue = currentValue;
            FiscalYearEarnedCash = fiscalYearEarnedCash;
            FiscalYearMissionRevenue = fiscalYearMissionRevenue;
            QuarterEarnedCash = new List<QuarterPerformanceRecord>(quarterEarnedCash).AsReadOnly();
            OwnedAssetCount = ownedAssetCount;
            CurrentRedemptionPressure = currentRedemptionPressure;
            MaxRedemptionPressure = maxRedemptionPressure;
        }

        public int FiscalYear { get; }
        public int CurrentValue { get; }
        public int CurrentManagementValue => CurrentValue;
        public int FiscalYearRevenue => FiscalYearEarnedCash;
        public int FiscalYearEarnedCash { get; }
        public int FiscalYearMissionRevenue { get; }
        public IReadOnlyList<QuarterPerformanceRecord> QuarterRevenue => QuarterEarnedCash;
        public IReadOnlyList<QuarterPerformanceRecord> QuarterEarnedCash { get; }
        public int OwnedAssetCount { get; }
        public int OwnedStockCount => OwnedAssetCount;
        public int CurrentRedemptionPressure { get; }
        public int CurrentRentArrears => CurrentRedemptionPressure;
        public int MaxRedemptionPressure { get; }
        public int MaxRentArrears => MaxRedemptionPressure;
    }
}
