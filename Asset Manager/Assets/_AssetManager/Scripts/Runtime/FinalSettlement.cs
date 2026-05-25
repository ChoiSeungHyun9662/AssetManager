using System;

namespace AssetManager
{
    public static class FinalSettlement
    {
        public static FinalSettlementResult Create(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var finalValue = run.OwnedAssets.CurrentValue + run.Performance.TotalMissionRevenue;
            var finalRating = SelectFinalRating(run.StaticData, finalValue);
            var pressureLevel = SelectPressureLevel(run.StaticData, run.RentArrears.CurrentArrears);
            var comment = SelectManagementComment(run.StaticData, finalRating, pressureLevel);

            return new FinalSettlementResult(
                finalValue,
                finalRating,
                run.Performance.TotalRevenue,
                run.Performance.TotalMissionRevenue,
                run.OwnedAssets.Count,
                run.RentArrears.CurrentArrears,
                run.RentArrears.MaxArrears,
                comment);
        }

        private static FinalRatingData SelectFinalRating(RunStaticDataSet staticData, int finalValue)
        {
            FinalRatingData selected = null;
            foreach (var rating in staticData.FinalRatings)
            {
                if (rating.MinimumFinalValue <= finalValue
                    && (selected == null || selected.MinimumFinalValue < rating.MinimumFinalValue))
                {
                    selected = rating;
                }
            }

            if (selected == null)
            {
                throw new InvalidOperationException("Final rating data is missing a reachable rating.");
            }

            return selected;
        }

        private static RedemptionPressureLevelData SelectPressureLevel(
            RunStaticDataSet staticData,
            int currentPressure)
        {
            foreach (var level in staticData.RedemptionPressureLevels)
            {
                if (level.MinimumPressure <= currentPressure && currentPressure <= level.MaximumPressure)
                {
                    return level;
                }
            }

            return null;
        }

        private static string SelectManagementComment(
            RunStaticDataSet staticData,
            FinalRatingData finalRating,
            RedemptionPressureLevelData pressureLevel)
        {
            foreach (var comment in staticData.FinalManagementComments)
            {
                if (comment.RatingId == finalRating.RatingId
                    && pressureLevel != null
                    && comment.PressureLevelId == pressureLevel.LevelId)
                {
                    return comment.Comment;
                }
            }

            foreach (var comment in staticData.FinalManagementComments)
            {
                if (comment.RatingId == finalRating.RatingId)
                {
                    return comment.Comment;
                }
            }

            return string.Empty;
        }
    }

    public sealed class FinalSettlementResult
    {
        public FinalSettlementResult(
            int finalValue,
            FinalRatingData finalRating,
            int totalEarnedCash,
            int totalMissionRevenue,
            int ownedAssetCount,
            int currentRedemptionPressure,
            int maxRedemptionPressure,
            string managementComment)
        {
            FinalValue = finalValue;
            FinalRating = finalRating ?? throw new ArgumentNullException(nameof(finalRating));
            TotalEarnedCash = totalEarnedCash;
            TotalMissionRevenue = totalMissionRevenue;
            OwnedAssetCount = ownedAssetCount;
            CurrentRedemptionPressure = currentRedemptionPressure;
            MaxRedemptionPressure = maxRedemptionPressure;
            ManagementComment = managementComment ?? string.Empty;
        }

        public int FinalValue { get; }
        public int FinalManagementValue => FinalValue;
        public FinalRatingData FinalRating { get; }
        public int TotalRevenue => TotalEarnedCash;
        public int TotalEarnedCash { get; }
        public int TotalMissionRevenue { get; }
        public int OwnedAssetCount { get; }
        public int OwnedStockCount => OwnedAssetCount;
        public int CurrentRedemptionPressure { get; }
        public int CurrentRentArrears => CurrentRedemptionPressure;
        public int MaxRedemptionPressure { get; }
        public int MaxRentArrears => MaxRedemptionPressure;
        public string ManagementComment { get; }
        public string FinalComment => ManagementComment;
    }
}
