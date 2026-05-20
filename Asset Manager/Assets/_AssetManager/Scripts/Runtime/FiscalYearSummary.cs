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

            var quarterEarnedCash = new List<QuarterPerformanceRecord>();
            foreach (var record in run.Performance.CompletedQuarterEarnedCash)
            {
                if (record.FiscalYear == run.Calendar.FiscalYear)
                {
                    quarterEarnedCash.Add(record);
                }
            }

            return new FiscalYearSummaryResult(
                run.Calendar.FiscalYear,
                run.OwnedAssets.CurrentManagementValue,
                run.Performance.CurrentFiscalYearEarnedCash,
                quarterEarnedCash,
                run.OwnedAssets.Count,
                run.RedemptionPressure.CurrentPressure,
                run.RedemptionPressure.MaxPressure);
        }
    }

    public sealed class FiscalYearSummaryResult
    {
        public FiscalYearSummaryResult(
            int fiscalYear,
            int currentManagementValue,
            int fiscalYearEarnedCash,
            IEnumerable<QuarterPerformanceRecord> quarterEarnedCash,
            int ownedAssetCount,
            int currentRedemptionPressure,
            int maxRedemptionPressure)
        {
            FiscalYear = fiscalYear;
            CurrentManagementValue = currentManagementValue;
            FiscalYearEarnedCash = fiscalYearEarnedCash;
            QuarterEarnedCash = new List<QuarterPerformanceRecord>(quarterEarnedCash).AsReadOnly();
            OwnedAssetCount = ownedAssetCount;
            CurrentRedemptionPressure = currentRedemptionPressure;
            MaxRedemptionPressure = maxRedemptionPressure;
        }

        public int FiscalYear { get; }
        public int CurrentManagementValue { get; }
        public int CurrentValue => CurrentManagementValue;
        public int FiscalYearEarnedCash { get; }
        public int FiscalYearRevenue => FiscalYearEarnedCash;
        public IReadOnlyList<QuarterPerformanceRecord> QuarterEarnedCash { get; }
        public int OwnedAssetCount { get; }
        public int OwnedStockCount => OwnedAssetCount;
        public int CurrentRedemptionPressure { get; }
        public int CurrentRentArrears => CurrentRedemptionPressure;
        public int MaxRedemptionPressure { get; }
        public int MaxRentArrears => MaxRedemptionPressure;
    }
}
