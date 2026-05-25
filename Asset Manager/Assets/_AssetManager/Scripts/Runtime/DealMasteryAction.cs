using System;

namespace AssetManager
{
    public static class DealMasteryAction
    {
        public const string MaxMasteryMessage = "투자 철학 마스터리가 이미 최대입니다.";

        public static DealMasteryActionResult Apply(RunSessionState run, ResourceType resourceType)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (!IsInvestmentPhilosophy(resourceType))
            {
                return new DealMasteryActionResult(run, false, string.Empty);
            }

            if (run.Resources.Deal <= 0)
            {
                return new DealMasteryActionResult(run, false, string.Empty);
            }

            if (run.InvestmentPhilosophyMastery.Get(resourceType) >= 5)
            {
                return new DealMasteryActionResult(run, false, MaxMasteryMessage);
            }

            return new DealMasteryActionResult(
                WithResourcesAndMastery(
                    run,
                    new ResourceState(
                        run.Resources.Cash,
                        run.Resources.Reading,
                        run.Resources.Meditation,
                        run.Resources.Patience,
                        run.Resources.Deal - 1),
                    AddMastery(run.InvestmentPhilosophyMastery, resourceType)),
                true,
                string.Empty);
        }

        private static bool IsInvestmentPhilosophy(ResourceType resourceType)
        {
            return resourceType == ResourceType.Reading
                || resourceType == ResourceType.Meditation
                || resourceType == ResourceType.Patience;
        }

        private static InvestmentPhilosophyMasteryState AddMastery(
            InvestmentPhilosophyMasteryState mastery,
            ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Reading:
                    return new InvestmentPhilosophyMasteryState(
                        mastery.Reading + 1,
                        mastery.Meditation,
                        mastery.Patience);
                case ResourceType.Meditation:
                    return new InvestmentPhilosophyMasteryState(
                        mastery.Reading,
                        mastery.Meditation + 1,
                        mastery.Patience);
                case ResourceType.Patience:
                    return new InvestmentPhilosophyMasteryState(
                        mastery.Reading,
                        mastery.Meditation,
                        mastery.Patience + 1);
                default:
                    return mastery;
            }
        }

        private static RunSessionState WithResourcesAndMastery(
            RunSessionState run,
            ResourceState resources,
            InvestmentPhilosophyMasteryState mastery)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                resources,
                run.Performance,
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
                mastery,
                run.DealRewards,
                run.Missions);
        }
    }

    public sealed class DealMasteryActionResult
    {
        public DealMasteryActionResult(RunSessionState run, bool succeeded, string message)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            Succeeded = succeeded;
            Message = message ?? string.Empty;
        }

        public RunSessionState Run { get; }
        public bool Succeeded { get; }
        public string Message { get; }
    }
}
