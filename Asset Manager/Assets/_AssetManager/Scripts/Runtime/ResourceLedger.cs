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
                    run.Resources.Reading,
                    run.Resources.Meditation,
                    run.Resources.Patience,
                    run.Resources.Deal),
                new RunPerformanceState(
                    run.Performance.CurrentQuarterRevenue,
                    run.Performance.CurrentFiscalYearRevenue,
                    run.Performance.TotalRevenue,
                    run.Performance.FundingCash + amount,
                    run.Performance.CompletedQuarterRevenue));
        }

        public static RunSessionState AddRevenue(RunSessionState run, int amount)
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
                    run.Resources.Reading,
                    run.Resources.Meditation,
                    run.Resources.Patience,
                    run.Resources.Deal),
                new RunPerformanceState(
                    performance.CurrentQuarterRevenue + amount,
                    performance.CurrentFiscalYearRevenue + amount,
                    performance.TotalRevenue + amount,
                    performance.FundingCash,
                    performance.CompletedQuarterRevenue));
        }

        public static RunSessionState AddEarnedCash(RunSessionState run, int amount)
        {
            return AddRevenue(run, amount);
        }

        public static ResourceLedgerResult AddInvestmentPhilosophy(
            RunSessionState run,
            ResourceType resourceType,
            int amount)
        {
            ValidateRun(run);
            ValidateAmount(amount);

            if (!IsInvestmentPhilosophy(resourceType))
            {
                throw new ArgumentException("Only investment philosophy resources can be added through this method.", nameof(resourceType));
            }

            if (amount == 0)
            {
                return new ResourceLedgerResult(run, 0, 0, string.Empty);
            }

            var totalCapacity = Math.Max(
                0,
                run.StaticData.ResourceConfig.InvestmentPhilosophyCap - run.Resources.InvestmentPhilosophyTotal);
            var typeCapacity = Math.Max(
                0,
                run.StaticData.ResourceConfig.InvestmentPhilosophyTypeCap - run.Resources.Get(resourceType));
            var gainedAmount = Math.Min(amount, Math.Min(totalCapacity, typeCapacity));
            var discardedAmount = amount - gainedAmount;
            var resources = AddInvestmentPhilosophyAmount(run.Resources, resourceType, gainedAmount);
            var message = discardedAmount > 0
                ? $"투자 철학 한도: {GetResourceDisplayName(resourceType)} +{discardedAmount} 버림"
                : string.Empty;

            return new ResourceLedgerResult(
                WithResourcesAndPerformance(run, resources, run.Performance),
                gainedAmount,
                discardedAmount,
                message);
        }

        public static ResourceLedgerResult AddProfessionalResource(
            RunSessionState run,
            ResourceType resourceType,
            int amount)
        {
            return AddInvestmentPhilosophy(run, resourceType, amount);
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
                ? "딜 한도: 추가 딜 버림"
                : string.Empty;

            return new ResourceLedgerResult(
                WithResourcesAndPerformance(
                    run,
                    new ResourceState(
                        run.Resources.Cash,
                        run.Resources.Reading,
                        run.Resources.Meditation,
                        run.Resources.Patience,
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
                case ResourceType.Reading:
                    return "독서";
                case ResourceType.Meditation:
                    return "명상";
                case ResourceType.Patience:
                    return "인내";
                case ResourceType.Deal:
                    return "딜";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        private static bool IsInvestmentPhilosophy(ResourceType resourceType)
        {
            return resourceType == ResourceType.Reading
                || resourceType == ResourceType.Meditation
                || resourceType == ResourceType.Patience;
        }

        private static ResourceState AddInvestmentPhilosophyAmount(
            ResourceState resources,
            ResourceType resourceType,
            int amount)
        {
            switch (resourceType)
            {
                case ResourceType.Reading:
                    return new ResourceState(
                        resources.Cash,
                        resources.Reading + amount,
                        resources.Meditation,
                        resources.Patience,
                        resources.Deal);
                case ResourceType.Meditation:
                    return new ResourceState(
                        resources.Cash,
                        resources.Reading,
                        resources.Meditation + amount,
                        resources.Patience,
                        resources.Deal);
                case ResourceType.Patience:
                    return new ResourceState(
                        resources.Cash,
                        resources.Reading,
                        resources.Meditation,
                        resources.Patience + amount,
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
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);
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
