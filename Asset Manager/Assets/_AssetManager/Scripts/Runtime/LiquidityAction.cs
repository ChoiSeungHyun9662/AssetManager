using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class LiquidityAction
    {
        public static RunSessionState Enter(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (run.State != RunState.Playing
                || run.BusinessDay.Phase != BusinessDayPhase.AwaitingAction
                || run.BusinessDay.MarketArea != MarketAreaState.Market
                || run.BusinessDay.IsAwaitingExtraBuyChoice)
            {
                return run;
            }

            return WithMarketArea(run, MarketAreaState.GainLiquidity, LiquidityActionState.Empty);
        }

        public static RunSessionState Close(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (!CanClose(run))
            {
                return run;
            }

            return WithMarketArea(run, MarketAreaState.Market, LiquidityActionState.Empty);
        }

        public static bool CanClose(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            return run.BusinessDay.MarketArea == MarketAreaState.GainLiquidity
                && !run.LiquidityAction.HasGainedAnyResource;
        }

        public static bool CanSelect(RunSessionState run, ResourceType resourceType)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (run.BusinessDay.MarketArea != MarketAreaState.GainLiquidity
                || !IsBasicResource(resourceType)
                || !HasCapacityFor(run, resourceType)
                || run.LiquidityAction.SelectedResources.Count >= 3)
            {
                return false;
            }

            var selectedResources = AddSelection(run.LiquidityAction.SelectedResources, resourceType);
            return IsValidSelectionPrefix(selectedResources);
        }

        public static LiquidityActionResult Select(RunSessionState run, ResourceType resourceType)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (!CanSelect(run, resourceType))
            {
                return new LiquidityActionResult(run, string.Empty);
            }

            string message;
            var resourceRun = AddSelectedResource(run, resourceType, out message);
            var selectedResources = AddSelection(run.LiquidityAction.SelectedResources, resourceType);
            var selectedRun = WithMarketArea(
                resourceRun,
                MarketAreaState.GainLiquidity,
                new LiquidityActionState(selectedResources));
            var shouldAutoEnd = !IsComplete(selectedResources)
                && !HasAnyValidNextChoice(selectedRun);

            if (shouldAutoEnd && string.IsNullOrEmpty(message))
            {
                message = "투자 철학 한도";
            }

            return IsComplete(selectedResources) || shouldAutoEnd
                ? new LiquidityActionResult(BusinessDayFlow.ConsumeBusinessDay(selectedRun), message)
                : new LiquidityActionResult(selectedRun, message);
        }

        private static bool IsComplete(IReadOnlyList<ResourceType> selectedResources)
        {
            return selectedResources.Count == 2
                && selectedResources[0] == selectedResources[1]
                || selectedResources.Count == 3
                && AllDifferent(selectedResources);
        }

        private static bool IsValidSelectionPrefix(IReadOnlyList<ResourceType> selectedResources)
        {
            if (selectedResources.Count <= 1)
            {
                return true;
            }

            if (selectedResources.Count == 2)
            {
                return selectedResources[0] == selectedResources[1]
                    || AllDifferent(selectedResources);
            }

            return selectedResources.Count == 3
                && AllDifferent(selectedResources);
        }

        private static RunSessionState AddSelectedResource(
            RunSessionState run,
            ResourceType resourceType,
            out string message)
        {
            message = string.Empty;
            if (resourceType == ResourceType.Cash)
            {
                return ResourceLedger.AddFundingCash(run, 1);
            }

            var result = ResourceLedger.AddInvestmentPhilosophy(run, resourceType, 1);
            message = result.Message;
            return result.Run;
        }

        private static bool AllDifferent(IReadOnlyList<ResourceType> selectedResources)
        {
            for (var i = 0; i < selectedResources.Count; i++)
            {
                for (var j = i + 1; j < selectedResources.Count; j++)
                {
                    if (selectedResources[i] == selectedResources[j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsBasicResource(ResourceType resourceType)
        {
            return resourceType == ResourceType.Cash
                || resourceType == ResourceType.Reading
                || resourceType == ResourceType.Meditation
                || resourceType == ResourceType.Patience;
        }

        private static bool HasCapacityFor(RunSessionState run, ResourceType resourceType)
        {
            if (resourceType == ResourceType.Cash)
            {
                return true;
            }

            return run.Resources.Get(resourceType) < run.StaticData.ResourceConfig.InvestmentPhilosophyTypeCap;
        }

        private static bool HasAnyValidNextChoice(RunSessionState run)
        {
            return CanSelect(run, ResourceType.Cash)
                || CanSelect(run, ResourceType.Reading)
                || CanSelect(run, ResourceType.Meditation)
                || CanSelect(run, ResourceType.Patience);
        }

        private static IReadOnlyList<ResourceType> AddSelection(
            IReadOnlyList<ResourceType> selectedResources,
            ResourceType resourceType)
        {
            var nextSelectedResources = new List<ResourceType>(selectedResources)
            {
                resourceType
            };
            return nextSelectedResources;
        }

        private static RunSessionState WithMarketArea(
            RunSessionState run,
            MarketAreaState marketArea,
            LiquidityActionState liquidityAction)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                new BusinessDayState(run.BusinessDay.Phase, marketArea),
                run.RedemptionPressure,
                CardDetailState.Empty,
                liquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                run.Missions);
        }
    }

    public sealed class LiquidityActionResult
    {
        public LiquidityActionResult(RunSessionState run, string message)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            Message = message ?? string.Empty;
        }

        public RunSessionState Run { get; }
        public string Message { get; }
    }
}
