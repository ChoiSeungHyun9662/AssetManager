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

        [SerializeField]
        private GameObject saleDropZone;

        [SerializeField]
        private Text saleDropZoneText;

        [SerializeField]
        private GameObject ownedStockDragDetailPanel;

        [SerializeField]
        private Text ownedStockDragDetailText;

        private readonly List<GameObject> ownedStockCards = new List<GameObject>();
        private readonly List<Button> cardButtons = new List<Button>();
        private readonly List<Text> cardTexts = new List<Text>();
        private readonly List<Button> sellButtons = new List<Button>();
        private readonly List<int> displayedStockSlotIndexes = new List<int>();
        private Action<int> onStockSaleSelected;
        private int draggedDisplayIndex = -1;
        private int draggedStockSlotIndex = -1;
        private AssetCardRuntimeData draggedCard;
        private bool isDraggingOwnedStock;

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
            IEnumerable<Button> ownedStockSellButtons,
            GameObject stockSaleDropZone,
            Text stockSaleDropZoneText,
            GameObject stockDragDetailPanel,
            Text stockDragDetailText)
        {
            panel = summaryPanel;
            summaryText = summary;
            ownedCardsText = ownedCards;
            saleDropZone = stockSaleDropZone;
            saleDropZoneText = stockSaleDropZoneText;
            ownedStockDragDetailPanel = stockDragDetailPanel;
            ownedStockDragDetailText = stockDragDetailText;
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
            CancelOwnedStockDrag();
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
            SetActive(saleDropZone, true);
            if (!isDraggingOwnedStock)
            {
                SetText(saleDropZoneText, "$");
                SetActive(ownedStockDragDetailPanel, false);
            }

            ShowCards(run);
        }

        private void ShowCards(RunSessionState run)
        {
            var visibleStockSlotIndexes = CollectVisibleStockSlotIndexes(run.OwnedAssets);
            displayedStockSlotIndexes.Clear();

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
                SetOwnedStockCardVisible(displayIndex, !(isDraggingOwnedStock && displayIndex == draggedDisplayIndex));
                StyleCardButton(displayIndex, card);
                WireCardButton(displayIndex, stockSlotIndex, run, card);
                WireSellButton(displayIndex);
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

        private void WireCardButton(int displayIndex, int stockSlotIndex, RunSessionState run, AssetCardRuntimeData card)
        {
            if (displayIndex >= cardButtons.Count || cardButtons[displayIndex] == null)
            {
                return;
            }

            var button = cardButtons[displayIndex];
            button.onClick.RemoveAllListeners();
            button.interactable = CanShowSellButton(run, card);
            ConfigureOwnedStockDragTrigger(button.gameObject, displayIndex, stockSlotIndex, run, card);
        }

        private void WireSellButton(int displayIndex)
        {
            if (displayIndex >= sellButtons.Count || sellButtons[displayIndex] == null)
            {
                return;
            }

            var button = sellButtons[displayIndex];
            button.onClick.RemoveAllListeners();
            SetButtonText(button, string.Empty);
            SetSellButtonActive(displayIndex, false);
            button.interactable = false;
        }

        private void ConfigureOwnedStockDragTrigger(
            GameObject target,
            int displayIndex,
            int stockSlotIndex,
            RunSessionState run,
            AssetCardRuntimeData card)
        {
            var trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.AddComponent<EventTrigger>();
            }

            trigger.triggers.Clear();
            AddPointerTrigger(trigger, EventTriggerType.PointerDown, _ =>
            {
                draggedDisplayIndex = displayIndex;
                draggedStockSlotIndex = stockSlotIndex;
                draggedCard = card;
                isDraggingOwnedStock = false;
            });
            AddPointerTrigger(trigger, EventTriggerType.Drag, eventData =>
            {
                if (draggedStockSlotIndex != stockSlotIndex || draggedCard == null)
                {
                    return;
                }

                isDraggingOwnedStock = true;
                SetOwnedStockCardVisible(displayIndex, false);
                ShowOwnedStockDragDetail(eventData.position, card);
                SetText(saleDropZoneText, "+" + GetSaleCash(run, card));
            });
            AddPointerTrigger(trigger, EventTriggerType.PointerUp, eventData =>
            {
                if (draggedStockSlotIndex != stockSlotIndex || draggedCard == null)
                {
                    CancelOwnedStockDrag();
                    return;
                }

                var shouldSell = isDraggingOwnedStock && IsInsideSaleDropZone(eventData.position);
                CancelOwnedStockDrag();
                if (shouldSell)
                {
                    onStockSaleSelected?.Invoke(stockSlotIndex);
                }
            });
        }

        private void ShowOwnedStockDragDetail(Vector2 screenPosition, AssetCardRuntimeData card)
        {
            SetText(ownedStockDragDetailText, FormatOwnedStockDragCard(card));
            SetActive(ownedStockDragDetailPanel, true);

            var rectTransform = ownedStockDragDetailPanel != null
                ? ownedStockDragDetailPanel.GetComponent<RectTransform>()
                : null;
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.pivot = new Vector2(0f, 0f);
            rectTransform.position = screenPosition;
        }

        private static string FormatOwnedStockDragCard(AssetCardRuntimeData card)
        {
            var foilLabel = card.IsFoil ? " FOIL" : string.Empty;
            return card.Card.DisplayName + foilLabel
                + "\n" + card.Card.Rarity
                + "\nValue " + card.Value
                + "\nDividend " + card.Income;
        }

        private bool IsInsideSaleDropZone(Vector2 screenPosition)
        {
            var rectTransform = saleDropZone != null ? saleDropZone.GetComponent<RectTransform>() : null;
            return rectTransform != null
                && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition);
        }

        private void CancelOwnedStockDrag()
        {
            if (draggedDisplayIndex >= 0)
            {
                SetOwnedStockCardVisible(draggedDisplayIndex, true);
            }

            draggedDisplayIndex = -1;
            draggedStockSlotIndex = -1;
            draggedCard = null;
            isDraggingOwnedStock = false;
            SetActive(ownedStockDragDetailPanel, false);
            SetText(saleDropZoneText, "$");
        }

        private void SetOwnedStockCardVisible(int displayIndex, bool isVisible)
        {
            if (displayIndex >= ownedStockCards.Count || ownedStockCards[displayIndex] == null)
            {
                return;
            }

            var canvasGroup = ownedStockCards[displayIndex].GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = ownedStockCards[displayIndex].AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.blocksRaycasts = isVisible;
        }

        private static bool CanShowSellButton(RunSessionState run, AssetCardRuntimeData card)
        {
            return run.State == RunState.Playing
                && run.BusinessDay.Phase == BusinessDayPhase.AwaitingAction
                && card != null
                && card.State == AssetCardRuntimeState.Owned;
        }

        private static void AddPointerTrigger(
            EventTrigger trigger,
            EventTriggerType eventType,
            Action<PointerEventData> action)
        {
            var entry = new EventTrigger.Entry
            {
                eventID = eventType
            };
            entry.callback.AddListener(eventData =>
            {
                if (eventData is PointerEventData pointerEventData)
                {
                    action(pointerEventData);
                }
            });
            trigger.triggers.Add(entry);
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
