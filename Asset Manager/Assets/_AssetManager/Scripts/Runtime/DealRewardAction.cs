using System;

namespace AssetManager
{
    public static class DealRewardAction
    {
        public static DealRewardActionResult ApplyPurchaseRewards(RunSessionState run, bool createdFoil)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            var rewards = run.DealRewards ?? DealRewardState.Empty;
            var occupiedStockSlots = run.OwnedAssets.Count;
            var granted = 0;

            var grantedThree = rewards.GrantedThreeStockSlots;
            var grantedFive = rewards.GrantedFiveStockSlots;
            var grantedEight = rewards.GrantedEightStockSlots;
            var grantedFirstFoil = rewards.GrantedFirstFoil;

            GrantStockSlotThreshold(occupiedStockSlots, 3, ref grantedThree, ref granted);
            GrantStockSlotThreshold(occupiedStockSlots, 5, ref grantedFive, ref granted);
            GrantStockSlotThreshold(occupiedStockSlots, 8, ref grantedEight, ref granted);

            if (createdFoil && !grantedFirstFoil)
            {
                grantedFirstFoil = true;
                granted++;
            }

            if (granted == 0)
            {
                return new DealRewardActionResult(run, 0);
            }

            return new DealRewardActionResult(
                WithDealRewards(
                    run,
                    new ResourceState(
                        run.Resources.Cash,
                        run.Resources.Reading,
                        run.Resources.Meditation,
                        run.Resources.Patience,
                        run.Resources.Deal + granted),
                    new DealRewardState(grantedThree, grantedFive, grantedEight, grantedFirstFoil)),
                granted);
        }

        private static void GrantStockSlotThreshold(
            int occupiedStockSlots,
            int threshold,
            ref bool alreadyGranted,
            ref int granted)
        {
            if (alreadyGranted || occupiedStockSlots < threshold)
            {
                return;
            }

            alreadyGranted = true;
            granted++;
        }

        private static RunSessionState WithDealRewards(
            RunSessionState run,
            ResourceState resources,
            DealRewardState dealRewards)
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
                run.InvestmentPhilosophyMastery,
                dealRewards,
                run.Missions);
        }
    }

    public sealed class DealRewardActionResult
    {
        public DealRewardActionResult(RunSessionState run, int dealsGranted)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            DealsGranted = dealsGranted;
        }

        public RunSessionState Run { get; }
        public int DealsGranted { get; }
    }
}
