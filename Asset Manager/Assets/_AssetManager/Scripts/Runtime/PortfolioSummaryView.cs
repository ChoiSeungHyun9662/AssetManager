using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private readonly List<bool> cardButtonHoverStates = new List<bool>();
        private readonly List<bool> sellButtonHoverStates = new List<bool>();
        private readonly List<bool> sellButtonCanShowStates = new List<bool>();
        private Action<int> onStockSaleSelected;
        private Coroutine deferredVisibilityUpdate;

        public GameObject Panel => panel;
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
            StopDeferredVisibilityUpdate();
            ClearHoverStates();
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
                $"포트폴리오 | 보유 자산 {run.OwnedAssets.Count} | 현재 운용가치 {run.OwnedAssets.CurrentValue} | 분기 수익 {run.Performance.CurrentQuarterRevenue}");
            SetText(ownedCardsText, string.Empty);
            ShowCards(run);
        }

        private void ShowCards(RunSessionState run)
        {
            var visibleStockSlotIndexes = CollectVisibleStockSlotIndexes(run.OwnedAssets);
            displayedStockSlotIndexes.Clear();
            EnsureHoverStateCount(ownedStockCards.Count);

            for (var displayIndex = 0; displayIndex < ownedStockCards.Count; displayIndex++)
            {
                var hasCard = displayIndex < visibleStockSlotIndexes.Count;
                var container = ownedStockCards[displayIndex];
                SetActive(container, hasCard);

                if (!hasCard)
                {
                    displayedStockSlotIndexes.Add(-1);
                    SetCardText(displayIndex, string.Empty);
                    SetSellButtonCanShow(displayIndex, false);
                    SetHoverState(displayIndex, true, false, false);
                    SetHoverState(displayIndex, false, false, false);
                    SetSellButtonActive(displayIndex, false);
                    continue;
                }

                var stockSlotIndex = visibleStockSlotIndexes[displayIndex];
                displayedStockSlotIndexes.Add(stockSlotIndex);
                var card = run.OwnedAssets.StockSlots[stockSlotIndex];
                SetCardText(displayIndex, FormatCard(card));
                StyleCardButton(displayIndex, card);
                WireCardButton(displayIndex, run, card);
                WireSellButton(displayIndex, stockSlotIndex, run, card);
            }
        }

        private static string FormatCard(AssetCardRuntimeData card)
        {
            var foilLabel = card.IsFoil ? " FOIL" : string.Empty;
            return card.Card.DisplayName + foilLabel
                + "\n등급 " + card.Card.Rarity
                + "\n가치 " + card.Value
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

        private void WireCardButton(int displayIndex, RunSessionState run, AssetCardRuntimeData card)
        {
            if (displayIndex >= cardButtons.Count || cardButtons[displayIndex] == null)
            {
                return;
            }

            var button = cardButtons[displayIndex];
            button.onClick.RemoveAllListeners();
            button.interactable = CanShowSellButton(run, card);
            ConfigureHoverTrigger(button.gameObject, displayIndex, true);
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
            SetSellButtonCanShow(displayIndex, CanShowSellButton(run, card));
            SetSellButtonActive(displayIndex, ShouldShowSellButton(displayIndex));
            button.interactable = sellButtonCanShowStates[displayIndex];
            ConfigureHoverTrigger(button.gameObject, displayIndex, false);
            if (button.interactable)
            {
                button.onClick.AddListener(() =>
                {
                    StopDeferredVisibilityUpdate();
                    SetHoverState(displayIndex, true, false, false);
                    SetHoverState(displayIndex, false, false, false);
                    UpdateSellButtonVisibility();
                    onStockSaleSelected?.Invoke(stockSlotIndex);
                });
            }
        }

        private void SetHoverState(int displayIndex, bool isCardButton, bool isHovered, bool deferVisibilityUpdate)
        {
            EnsureHoverStateCount(displayIndex + 1);
            if (isCardButton)
            {
                cardButtonHoverStates[displayIndex] = isHovered;
            }
            else
            {
                sellButtonHoverStates[displayIndex] = isHovered;
            }

            if (deferVisibilityUpdate)
            {
                ScheduleDeferredVisibilityUpdate();
                return;
            }

            StopDeferredVisibilityUpdate();
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
                SetSellButtonActive(i, stockSlotIndex >= 0 && ShouldShowSellButton(i));
            }
        }

        private bool ShouldShowSellButton(int displayIndex)
        {
            return GetHoverState(sellButtonCanShowStates, displayIndex)
                && (GetHoverState(cardButtonHoverStates, displayIndex)
                    || GetHoverState(sellButtonHoverStates, displayIndex));
        }

        private static bool CanShowSellButton(RunSessionState run, AssetCardRuntimeData card)
        {
            return run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && card != null
                && card.State == AssetCardRuntimeState.Owned;
        }

        private void SetSellButtonCanShow(int displayIndex, bool canShow)
        {
            EnsureHoverStateCount(displayIndex + 1);
            sellButtonCanShowStates[displayIndex] = canShow;
        }

        private void ClearHoverStates()
        {
            for (var i = 0; i < cardButtonHoverStates.Count; i++)
            {
                cardButtonHoverStates[i] = false;
            }

            for (var i = 0; i < sellButtonHoverStates.Count; i++)
            {
                sellButtonHoverStates[i] = false;
            }
        }

        private void EnsureHoverStateCount(int count)
        {
            EnsureBoolListCount(cardButtonHoverStates, count);
            EnsureBoolListCount(sellButtonHoverStates, count);
            EnsureBoolListCount(sellButtonCanShowStates, count);
        }

        private static void EnsureBoolListCount(List<bool> values, int count)
        {
            while (values.Count < count)
            {
                values.Add(false);
            }
        }

        private static bool GetHoverState(IReadOnlyList<bool> values, int index)
        {
            return index >= 0 && index < values.Count && values[index];
        }

        private void ConfigureHoverTrigger(GameObject target, int displayIndex, bool isCardButton)
        {
            var trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.AddComponent<EventTrigger>();
            }

            trigger.triggers.Clear();
            AddHoverTrigger(trigger, EventTriggerType.PointerEnter, () => SetHoverState(displayIndex, isCardButton, true, false));
            AddHoverTrigger(trigger, EventTriggerType.PointerExit, () => SetHoverState(displayIndex, isCardButton, false, true));
        }

        private static void AddHoverTrigger(EventTrigger trigger, EventTriggerType eventType, Action action)
        {
            var entry = new EventTrigger.Entry
            {
                eventID = eventType
            };
            entry.callback.AddListener(_ => action());
            trigger.triggers.Add(entry);
        }

        private void ScheduleDeferredVisibilityUpdate()
        {
            if (deferredVisibilityUpdate == null && gameObject.activeInHierarchy)
            {
                deferredVisibilityUpdate = StartCoroutine(UpdateSellButtonVisibilityNextFrame());
            }
        }

        private System.Collections.IEnumerator UpdateSellButtonVisibilityNextFrame()
        {
            yield return null;
            deferredVisibilityUpdate = null;
            UpdateSellButtonVisibility();
        }

        private void StopDeferredVisibilityUpdate()
        {
            if (deferredVisibilityUpdate != null)
            {
                StopCoroutine(deferredVisibilityUpdate);
                deferredVisibilityUpdate = null;
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
