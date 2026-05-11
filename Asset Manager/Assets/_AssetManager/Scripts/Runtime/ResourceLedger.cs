using System;

namespace AssetManager
{
    public static class ResourceLedger
    {
        public static RunSessionState AddFundingCash(RunSessionState run, int amount)
        {
            ValidateRun(run);
            ValidateAmount(amount);

            if (amount == 0)
            {
                return run;
            }

            return WithResourcesAndPerformance(
                run,
                new ResourceState(
                    run.Resources.Cash + amount,
                    run.Resources.Research,
                    run.Resources.Credit,
                    run.Resources.Commodity,
                    run.Resources.Deal),
                run.Performance);
        }

        public static RunSessionState AddEarnedCash(RunSessionState run, int amount)
        {
            ValidateRun(run);
            ValidateAmount(amount);

            if (amount == 0)
            {
                return run;
            }

            var performance = run.Performance;
            return WithResourcesAndPerformance(
                run,
                new ResourceState(
                    run.Resources.Cash + amount,
                    run.Resources.Research,
                    run.Resources.Credit,
                    run.Resources.Commodity,
                    run.Resources.Deal),
                new RunPerformanceState(
                    performance.CurrentQuarterEarnedCash + amount,
                    performance.CurrentFiscalYearEarnedCash + amount,
                    performance.TotalEarnedCash + amount,
                    performance.FundingCash));
        }

        public static ResourceLedgerResult AddProfessionalResource(
            RunSessionState run,
            ResourceType resourceType,
            int amount)
        {
            ValidateRun(run);
            ValidateAmount(amount);

            if (!IsProfessionalResource(resourceType))
            {
                throw new ArgumentException("Only professional resources can be added through this method.", nameof(resourceType));
            }

            if (amount == 0)
            {
                return new ResourceLedgerResult(run, 0, 0, string.Empty);
            }

            var remainingCapacity = Math.Max(
                0,
                run.StaticData.ResourceConfig.ProfessionalResourceCap - run.Resources.ProfessionalTotal);
            var gainedAmount = Math.Min(amount, remainingCapacity);
            var discardedAmount = amount - gainedAmount;
            var resources = AddProfessionalResourceAmount(run.Resources, resourceType, gainedAmount);
            var message = discardedAmount > 0
                ? $"자원칩 최대 보유: {GetResourceDisplayName(resourceType)} +{discardedAmount} 폐기"
                : string.Empty;

            return new ResourceLedgerResult(
                WithResourcesAndPerformance(run, resources, run.Performance),
                gainedAmount,
                discardedAmount,
                message);
        }

        public static ResourceLedgerResult AddDeal(RunSessionState run, int amount)
        {
            ValidateRun(run);
            ValidateAmount(amount);

            if (amount == 0)
            {
                return new ResourceLedgerResult(run, 0, 0, string.Empty);
            }

            var remainingCapacity = Math.Max(0, run.StaticData.ResourceConfig.MaxDeal - run.Resources.Deal);
            var gainedAmount = Math.Min(amount, remainingCapacity);
            var discardedAmount = amount - gainedAmount;
            var message = discardedAmount > 0
                ? "딜 최대 보유: 추가 딜 폐기"
                : string.Empty;

            return new ResourceLedgerResult(
                WithResourcesAndPerformance(
                    run,
                    new ResourceState(
                        run.Resources.Cash,
                        run.Resources.Research,
                        run.Resources.Credit,
                        run.Resources.Commodity,
                        run.Resources.Deal + gainedAmount),
                    run.Performance),
                gainedAmount,
                discardedAmount,
                message);
        }

        public static string GetResourceDisplayName(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Cash:
                    return "현금";
                case ResourceType.Research:
                    return "리서치";
                case ResourceType.Credit:
                    return "신용";
                case ResourceType.Commodity:
                    return "원자재";
                case ResourceType.Deal:
                    return "딜";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        private static bool IsProfessionalResource(ResourceType resourceType)
        {
            return resourceType == ResourceType.Research
                || resourceType == ResourceType.Credit
                || resourceType == ResourceType.Commodity;
        }

        private static ResourceState AddProfessionalResourceAmount(
            ResourceState resources,
            ResourceType resourceType,
            int amount)
        {
            switch (resourceType)
            {
                case ResourceType.Research:
                    return new ResourceState(
                        resources.Cash,
                        resources.Research + amount,
                        resources.Credit,
                        resources.Commodity,
                        resources.Deal);
                case ResourceType.Credit:
                    return new ResourceState(
                        resources.Cash,
                        resources.Research,
                        resources.Credit + amount,
                        resources.Commodity,
                        resources.Deal);
                case ResourceType.Commodity:
                    return new ResourceState(
                        resources.Cash,
                        resources.Research,
                        resources.Credit,
                        resources.Commodity + amount,
                        resources.Deal);
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        private static void ValidateRun(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }
        }

        private static void ValidateAmount(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Resource amount cannot be negative.");
            }
        }

        private static RunSessionState WithResourcesAndPerformance(
            RunSessionState run,
            ResourceState resources,
            RunPerformanceState performance)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                resources,
                performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail);
        }
    }

    public sealed class ResourceLedgerResult
    {
        public ResourceLedgerResult(RunSessionState run, int gainedAmount, int discardedAmount, string message)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            GainedAmount = gainedAmount;
            DiscardedAmount = discardedAmount;
            Message = message ?? string.Empty;
        }

        public RunSessionState Run { get; }
        public int GainedAmount { get; }
        public int DiscardedAmount { get; }
        public string Message { get; }
    }
}
