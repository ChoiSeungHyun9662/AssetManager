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

            var finalManagementValue = run.OwnedAssets.CurrentManagementValue;
            var finalRating = SelectFinalRating(run.StaticData, finalManagementValue);
            var pressureLevel = SelectPressureLevel(run.StaticData, run.RedemptionPressure.CurrentPressure);
            var comment = SelectManagementComment(run.StaticData, finalRating, pressureLevel);

            return new FinalSettlementResult(
                finalManagementValue,
                finalRating,
                run.Performance.TotalEarnedCash,
                run.OwnedAssets.Count,
                run.RedemptionPressure.CurrentPressure,
                run.RedemptionPressure.MaxPressure,
                comment);
        }

        private static FinalRatingData SelectFinalRating(RunStaticDataSet staticData, int finalManagementValue)
        {
            FinalRatingData selected = null;
            foreach (var rating in staticData.FinalRatings)
            {
                if (rating.MinimumManagementValue <= finalManagementValue
                    && (selected == null || selected.MinimumManagementValue < rating.MinimumManagementValue))
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
            int finalManagementValue,
            FinalRatingData finalRating,
            int totalEarnedCash,
            int ownedAssetCount,
            int currentRedemptionPressure,
            int maxRedemptionPressure,
            string managementComment)
        {
            FinalManagementValue = finalManagementValue;
            FinalRating = finalRating ?? throw new ArgumentNullException(nameof(finalRating));
            TotalEarnedCash = totalEarnedCash;
            OwnedAssetCount = ownedAssetCount;
            CurrentRedemptionPressure = currentRedemptionPressure;
            MaxRedemptionPressure = maxRedemptionPressure;
            ManagementComment = managementComment ?? string.Empty;
        }

        public int FinalManagementValue { get; }
        public int FinalValue => FinalManagementValue;
        public FinalRatingData FinalRating { get; }
        public int TotalEarnedCash { get; }
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
