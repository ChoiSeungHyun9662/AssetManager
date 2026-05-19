using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class PortfolioSummaryView : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Text summaryText;

        [SerializeField]
        private Text ownedCardsText;

        private readonly List<GameObject> ownedStockCards = new List<GameObject>();
        private readonly List<Button> cardButtons = new List<Button>();
        private readonly List<Text> cardTexts = new List<Text>();
        private readonly List<Button> sellButtons = new List<Button>();
        private readonly List<int> displayedStockSlotIndexes = new List<int>();
        private Action<int> onStockSaleSelected;
        private int? selectedSaleStockSlotIndex;

        public Text SummaryText => summaryText;
        public Text OwnedCardsText => ownedCardsText;
        public IReadOnlyList<Text> CardTexts => cardTexts;

        public void Bind(
            GameObject summaryPanel,
            Text summary,
            Text ownedCards,
            IEnumerable<GameObject> ownedStockCardContainers,
            IEnumerable<Button> ownedStockCardButtons,
            IEnumerable<Text> ownedStockCardTexts,
            IEnumerable<Button> ownedStockSellButtons)
        {
            panel = summaryPanel;
            summaryText = summary;
            ownedCardsText = ownedCards;
            ReplaceObjects(ownedStockCards, ownedStockCardContainers);
            ReplaceObjects(cardButtons, ownedStockCardButtons);
            ReplaceObjects(cardTexts, ownedStockCardTexts);
            ReplaceObjects(sellButtons, ownedStockSellButtons);
        }

        public void SetStockSaleSelectedHandler(Action<int> handler)
        {
            onStockSaleSelected = handler;
        }

        public void ClearSaleSelection()
        {
            selectedSaleStockSlotIndex = null;
            UpdateSellButtonVisibility();
        }

        public void Show(RunSessionState run)
        {
            if (run == null)
            {
                SetActive(panel, false);
                return;
            }

            SetActive(panel, true);
            SetText(
                summaryText,
                $"포트폴리오 | 보유 자산 {run.OwnedAssets.Count} | 현재 운용가치 {run.OwnedAssets.CurrentManagementValue} | 분기 수익 {run.Performance.CurrentQuarterEarnedCash}");
            SetText(ownedCardsText, string.Empty);
            ShowCards(run);
        }

        private void ShowCards(RunSessionState run)
        {
            var visibleStockSlotIndexes = CollectVisibleStockSlotIndexes(run.OwnedAssets);
            displayedStockSlotIndexes.Clear();
            if (!IsSelectedStockSlotVisible(visibleStockSlotIndexes))
            {
                selectedSaleStockSlotIndex = null;
            }

            for (var displayIndex = 0; displayIndex < ownedStockCards.Count; displayIndex++)
            {
                var hasCard = displayIndex < visibleStockSlotIndexes.Count;
                var container = ownedStockCards[displayIndex];
                SetActive(container, hasCard);

                if (!hasCard)
                {
                    displayedStockSlotIndexes.Add(-1);
                    SetCardText(displayIndex, string.Empty);
                    SetSellButtonActive(displayIndex, false);
                    continue;
                }

                var stockSlotIndex = visibleStockSlotIndexes[displayIndex];
                displayedStockSlotIndexes.Add(stockSlotIndex);
                var card = run.OwnedAssets.StockSlots[stockSlotIndex];
                SetCardText(displayIndex, FormatCard(card));
                StyleCardButton(displayIndex, card);
                WireCardButton(displayIndex, stockSlotIndex, run, card);
                WireSellButton(displayIndex, stockSlotIndex, run, card);
            }
        }

        private static string FormatCard(AssetCardRuntimeData card)
        {
            var foilLabel = card.IsFoil ? " FOIL" : string.Empty;
            return card.Card.DisplayName + foilLabel
                + "\n등급 " + card.Card.Rarity
                + "\n가치 " + card.ManagementValue
                + " | 배당금 " + card.Income;
        }

        private void StyleCardButton(int displayIndex, AssetCardRuntimeData card)
        {
            if (displayIndex >= cardButtons.Count || cardButtons[displayIndex] == null)
            {
                return;
            }

            var image = cardButtons[displayIndex].GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            image.color = card.IsFoil
                ? new Color(0.30f, 0.26f, 0.10f, 0.98f)
                : new Color(0.12f, 0.18f, 0.17f, 0.98f);
        }

        private void WireCardButton(int displayIndex, int stockSlotIndex, RunSessionState run, AssetCardRuntimeData card)
        {
            if (displayIndex >= cardButtons.Count || cardButtons[displayIndex] == null)
            {
                return;
            }

            var button = cardButtons[displayIndex];
            button.onClick.RemoveAllListeners();
            button.interactable = run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && card != null
                && card.State == AssetCardRuntimeState.Owned;
            if (button.interactable)
            {
                button.onClick.AddListener(() => ToggleSaleSelection(stockSlotIndex));
            }
        }

        private void WireSellButton(int displayIndex, int stockSlotIndex, RunSessionState run, AssetCardRuntimeData card)
        {
            if (displayIndex >= sellButtons.Count || sellButtons[displayIndex] == null)
            {
                return;
            }

            var button = sellButtons[displayIndex];
            button.onClick.RemoveAllListeners();
            SetButtonText(button, "매도 +" + GetSaleCash(run, card) + "$");
            SetSellButtonActive(displayIndex, selectedSaleStockSlotIndex == stockSlotIndex);
            button.interactable = run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction;
            if (button.interactable)
            {
                button.onClick.AddListener(() =>
                {
                    selectedSaleStockSlotIndex = null;
                    UpdateSellButtonVisibility();
                    onStockSaleSelected?.Invoke(stockSlotIndex);
                });
            }
        }

        private void ToggleSaleSelection(int stockSlotIndex)
        {
            selectedSaleStockSlotIndex = selectedSaleStockSlotIndex == stockSlotIndex
                ? (int?)null
                : stockSlotIndex;
            UpdateSellButtonVisibility();
        }

        private void UpdateSellButtonVisibility()
        {
            for (var i = 0; i < sellButtons.Count; i++)
            {
                if (i >= ownedStockCards.Count || ownedStockCards[i] == null || !ownedStockCards[i].activeSelf)
                {
                    SetSellButtonActive(i, false);
                    continue;
                }

                var button = i < sellButtons.Count ? sellButtons[i] : null;
                if (button == null)
                {
                    continue;
                }

                var stockSlotIndex = i < displayedStockSlotIndexes.Count
                    ? displayedStockSlotIndexes[i]
                    : -1;
                SetSellButtonActive(i, selectedSaleStockSlotIndex.HasValue
                    && selectedSaleStockSlotIndex.Value == stockSlotIndex);
            }
        }

        private void SetSellButtonActive(int displayIndex, bool isActive)
        {
            if (displayIndex < sellButtons.Count && sellButtons[displayIndex] != null)
            {
                sellButtons[displayIndex].gameObject.SetActive(isActive);
            }
        }

        private void SetCardText(int displayIndex, string value)
        {
            if (displayIndex < cardTexts.Count)
            {
                SetText(cardTexts[displayIndex], value);
            }
        }

        private static List<int> CollectVisibleStockSlotIndexes(OwnedAssetState ownedAssets)
        {
            var indexes = new List<int>();
            for (var i = 0; i < ownedAssets.StockSlots.Count; i++)
            {
                var card = ownedAssets.StockSlots[i];
                if (card != null && card.State == AssetCardRuntimeState.Owned)
                {
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        private bool IsSelectedStockSlotVisible(IReadOnlyList<int> visibleStockSlotIndexes)
        {
            if (!selectedSaleStockSlotIndex.HasValue)
            {
                return true;
            }

            for (var i = 0; i < visibleStockSlotIndexes.Count; i++)
            {
                if (visibleStockSlotIndexes[i] == selectedSaleStockSlotIndex.Value)
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetSaleCash(RunSessionState run, AssetCardRuntimeData card)
        {
            var baseSaleCash = card.IsFoil ? 3 : 1;
            return Math.Max(
                0,
                baseSaleCash + run.StaticData.GetInflationCostModifier(
                    run.Calendar.FiscalYear,
                    run.Calendar.Quarter));
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetButtonText(Button button, string value)
        {
            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void ReplaceObjects<T>(List<T> target, IEnumerable<T> source)
        {
            target.Clear();
            if (source != null)
            {
                target.AddRange(source);
            }
        }
    }
}
