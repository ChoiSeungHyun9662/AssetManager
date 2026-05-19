using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class StockSaleAction
    {
        public static StockSaleActionResult ConfirmSale(RunSessionState run, int stockSlotIndex)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (run.State != RunState.Playing
                || run.BusinessDay.Phase != BusinessDayPhase.AwaitingAction)
            {
                return new StockSaleActionResult(run, false, "주식을 매도할 수 없습니다.");
            }

            if (stockSlotIndex < 0 || stockSlotIndex >= run.OwnedAssets.StockSlots.Count)
            {
                return new StockSaleActionResult(run, false, "주식 슬롯을 찾을 수 없습니다.");
            }

            var soldCard = run.OwnedAssets.StockSlots[stockSlotIndex];
            if (soldCard == null || soldCard.State != AssetCardRuntimeState.Owned)
            {
                return new StockSaleActionResult(run, false, "매도할 주식이 없습니다.");
            }

            var saleCash = GetSaleCash(run, soldCard);
            var soldRun = new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                MarkSoldCardRemoved(run.AssetCards, soldCard),
                run.MarketTape,
                run.Reservation,
                RemoveOwnedSlot(run.OwnedAssets, stockSlotIndex),
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason);

            return new StockSaleActionResult(
                ResourceLedger.AddEarnedCash(soldRun, saleCash),
                true,
                $"주식 매도: 현금 +{saleCash}");
        }

        private static int GetSaleCash(RunSessionState run, AssetCardRuntimeData soldCard)
        {
            var baseSaleCash = soldCard.IsFoil ? 3 : 1;
            return Math.Max(
                0,
                baseSaleCash + run.StaticData.GetInflationCostModifier(
                    run.Calendar.FiscalYear,
                    run.Calendar.Quarter));
        }

        private static IReadOnlyList<AssetCardRuntimeData> MarkSoldCardRemoved(
            IEnumerable<AssetCardRuntimeData> assetCards,
            AssetCardRuntimeData soldCard)
        {
            var updatedCards = new List<AssetCardRuntimeData>();
            foreach (var card in assetCards)
            {
                updatedCards.Add(card.RuntimeId == soldCard.RuntimeId
                    ? new AssetCardRuntimeData(
                        card.Card,
                        AssetCardRuntimeState.Removed,
                        null,
                        card.AcquiredOrder,
                        card.IsFoil,
                        card.RuntimeId)
                    : card);
            }

            return updatedCards;
        }

        private static OwnedAssetState RemoveOwnedSlot(OwnedAssetState ownedAssets, int stockSlotIndex)
        {
            var slots = new List<AssetCardRuntimeData>(ownedAssets.StockSlots);
            slots[stockSlotIndex] = null;
            return new OwnedAssetState(slots);
        }
    }

    public sealed class StockSaleActionResult
    {
        public StockSaleActionResult(RunSessionState run, bool succeeded, string message)
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
