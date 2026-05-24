using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class MarketTapeView : MonoBehaviour
    {
        private const float ClickDragThresholdPixels = 8f;
        private const float ReservedCardOffsetY = -37f;

        [SerializeField]
        private GameObject marketPanel;

        [SerializeField]
        private GameObject hoverCardPanel;

        [SerializeField]
        private Text hoverCardText;

        private readonly List<Button> sellImminentButtons = new List<Button>();
        private readonly List<Button> currentMarketButtons = new List<Button>();
        private readonly List<Button> upcomingMarketButtons = new List<Button>();
        private readonly Dictionary<string, Vector2> normalCurrentMarketButtonPositions = new Dictionary<string, Vector2>();
        private Action<AssetCardRuntimeData, MarketTapeZone> onMarketCardSelected;
        private Action<AssetCardRuntimeData, Vector2> onCurrentMarketCardReleased;
        private Action<AssetCardRuntimeData> onCurrentMarketCardReserved;
        private Action<AssetCardRuntimeData> onCurrentMarketCardUnreserved;
        private RectTransform draggedCard;
        private Vector2 draggedCardStartPosition;
        private Vector2 draggedPointerDelta;
        private bool hasDraggedCard;
        private string hoveredCardRuntimeId = string.Empty;
        private string hoveredActionRuntimeId = string.Empty;

        public GameObject MarketPanel => marketPanel;
        public GameObject HoverCardPanel => hoverCardPanel;
        public IReadOnlyList<Button> SellImminentButtons => sellImminentButtons;
        public IReadOnlyList<Button> CurrentMarketButtons => currentMarketButtons;
        public IReadOnlyList<Button> UpcomingMarketButtons => upcomingMarketButtons;

        public void Bind(
            GameObject market,
            GameObject hoverPanel,
            Text hoverText,
            IEnumerable<Button> sellImminentCardButtons,
            IEnumerable<Button> currentMarketCardButtons,
            IEnumerable<Button> upcomingMarketCardButtons)
        {
            marketPanel = market;
            hoverCardPanel = hoverPanel;
            hoverCardText = hoverText;

            ReplaceButtons(sellImminentButtons, sellImminentCardButtons);
            ReplaceButtons(currentMarketButtons, currentMarketCardButtons);
            ReplaceButtons(upcomingMarketButtons, upcomingMarketCardButtons);
        }

        public void SetMarketCardSelectedHandler(Action<AssetCardRuntimeData, MarketTapeZone> handler)
        {
            onMarketCardSelected = handler;
        }

        public void SetCurrentMarketCardReleasedHandler(Action<AssetCardRuntimeData, Vector2> handler)
        {
            onCurrentMarketCardReleased = handler;
        }

        public void SetCurrentMarketCardReservedHandler(Action<AssetCardRuntimeData> handler)
        {
            onCurrentMarketCardReserved = handler;
        }

        public void SetCurrentMarketCardUnreservedHandler(Action<AssetCardRuntimeData> handler)
        {
            onCurrentMarketCardUnreserved = handler;
        }

        public void Show(RunSessionState run, string purchaseFailureCardRuntimeId)
        {
            SetActive(marketPanel, run.BusinessDay.MarketArea == MarketAreaState.Market);
            HideHoverCard();

            ShowCards(sellImminentButtons, Array.Empty<AssetCardRuntimeData>(), MarketTapeZone.SellImminent);
            ShowSlots(currentMarketButtons, run.MarketTape.Slots, purchaseFailureCardRuntimeId);
            ShowCards(upcomingMarketButtons, Array.Empty<AssetCardRuntimeData>(), MarketTapeZone.UpcomingMarket);
        }

        private void ShowSlots(
            IReadOnlyList<Button> buttons,
            IReadOnlyList<MarketTapeSlotState> slots,
            string purchaseFailureCardRuntimeId)
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                if (button == null)
                {
                    continue;
                }

                var hasCard = i < slots.Count && !slots[i].IsEmpty;
                button.gameObject.SetActive(hasCard);
                button.onClick.RemoveAllListeners();

                if (!hasCard)
                {
                    SetButtonText(button, string.Empty);
                    continue;
                }

                var slot = slots[i];
                var card = slot.Card;
                ApplyReservedPosition(button.gameObject, slot.IsReserved);
                SetButtonText(button, MarketCardFormatter.Format(card, slot.IsReserved));
                StyleCardButton(button, card, MarketTapeZone.CurrentMarket, slot.IsReserved);
                ApplyFailureFeedback(button.gameObject, card.RuntimeId == purchaseFailureCardRuntimeId);
                ConfigureCurrentMarketTrigger(button.gameObject, i, card, slot.IsReserved);
                button.onClick.AddListener(() => onMarketCardSelected?.Invoke(card, MarketTapeZone.CurrentMarket));
            }
        }

        private void ShowCards(
            IReadOnlyList<Button> buttons,
            IReadOnlyList<AssetCardRuntimeData> cards,
            MarketTapeZone zone)
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                if (button == null)
                {
                    continue;
                }

                var hasCard = i < cards.Count;
                button.gameObject.SetActive(hasCard);
                button.onClick.RemoveAllListeners();

                if (!hasCard)
                {
                    SetButtonText(button, string.Empty);
                    continue;
                }

                var card = cards[i];
                SetButtonText(button, MarketCardFormatter.Format(card, false));
                StyleCardButton(button, card, zone, false);
                ConfigureHoverTrigger(button.gameObject, card, false);
                button.onClick.AddListener(() => onMarketCardSelected?.Invoke(card, zone));
            }
        }

        private void ConfigureCurrentMarketTrigger(GameObject target, int slotIndex, AssetCardRuntimeData card, bool isReserved)
        {
            var reserveButton = EnsureCardActionButton(
                target.transform,
                ProjectShell.MarketTapeCurrentMarketReserveButtonPrefix + (slotIndex + 1),
                "예약");
            var unreserveButton = EnsureCardActionButton(
                target.transform,
                ProjectShell.MarketTapeCurrentMarketUnreserveButtonPrefix + (slotIndex + 1),
                "해제");
            PositionActionButton(reserveButton, false);
            PositionActionButton(unreserveButton, true);
            ConfigureActionButton(reserveButton, card, false, isReserved, reserveButton, unreserveButton);
            ConfigureActionButton(unreserveButton, card, true, isReserved, reserveButton, unreserveButton);

            var trigger = EnsureEventTrigger(target);
            trigger.triggers.Clear();
            AddHoverTrigger(trigger, EventTriggerType.PointerEnter, () =>
            {
                hoveredCardRuntimeId = card.RuntimeId;
                ShowHoverCard(target, slotIndex, card, isReserved);
                UpdateActionButtons(card, isReserved, reserveButton, unreserveButton);
            });
            AddHoverTrigger(trigger, EventTriggerType.PointerExit, () =>
            {
                if (hoveredCardRuntimeId == card.RuntimeId)
                {
                    hoveredCardRuntimeId = string.Empty;
                }

                HideHoverCard();
                UpdateActionButtons(card, isReserved, reserveButton, unreserveButton);
            });
            AddPointerTrigger(trigger, EventTriggerType.PointerDown, eventData => BeginDrag(target, eventData));
            AddPointerTrigger(trigger, EventTriggerType.Drag, eventData => DragCard(eventData));
            AddPointerTrigger(trigger, EventTriggerType.PointerUp, eventData => EndDrag(card, eventData));
            UpdateActionButtons(card, isReserved, reserveButton, unreserveButton);
        }

        private void ConfigureHoverTrigger(GameObject target, AssetCardRuntimeData card, bool isReserved)
        {
            var trigger = EnsureEventTrigger(target);
            trigger.triggers.Clear();
            AddHoverTrigger(trigger, EventTriggerType.PointerEnter, () => ShowHoverCard(card, isReserved));
            AddHoverTrigger(trigger, EventTriggerType.PointerExit, HideHoverCard);
        }

        private static EventTrigger EnsureEventTrigger(GameObject target)
        {
            var trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.AddComponent<EventTrigger>();
            }

            return trigger;
        }

        private static void AddHoverTrigger(EventTrigger trigger, EventTriggerType eventType, Action action)
        {
            var entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener(_ => action());
            trigger.triggers.Add(entry);
        }

        private static void AddPointerTrigger(
            EventTrigger trigger,
            EventTriggerType eventType,
            Action<PointerEventData> action)
        {
            var entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener(eventData =>
            {
                if (eventData is PointerEventData pointerEventData)
                {
                    action(pointerEventData);
                }
            });
            trigger.triggers.Add(entry);
        }

        private void BeginDrag(GameObject target, PointerEventData eventData)
        {
            draggedCard = target.GetComponent<RectTransform>();
            if (draggedCard == null)
            {
                return;
            }

            draggedCardStartPosition = draggedCard.anchoredPosition;
            draggedPointerDelta = Vector2.zero;
            hasDraggedCard = false;
            target.transform.SetAsLastSibling();
        }

        private void DragCard(PointerEventData eventData)
        {
            if (draggedCard == null)
            {
                return;
            }

            HideHoverCard();
            hasDraggedCard = true;
            draggedPointerDelta += eventData.delta;
            draggedCard.anchoredPosition += eventData.delta;
        }

        private void EndDrag(AssetCardRuntimeData card, PointerEventData eventData)
        {
            var shouldRelease = draggedCard != null && hasDraggedCard;
            var isShortClick = shouldRelease && draggedPointerDelta.magnitude <= ClickDragThresholdPixels;
            if (draggedCard != null)
            {
                draggedCard.anchoredPosition = draggedCardStartPosition;
            }

            draggedCard = null;
            draggedPointerDelta = Vector2.zero;
            hasDraggedCard = false;

            if (isShortClick)
            {
                onMarketCardSelected?.Invoke(card, MarketTapeZone.CurrentMarket);
                return;
            }

            if (shouldRelease)
            {
                onCurrentMarketCardReleased?.Invoke(card, eventData.position);
            }
        }

        private void ShowHoverCard(AssetCardRuntimeData card, bool isReserved)
        {
            if (hoverCardText != null)
            {
                hoverCardText.text = MarketCardFormatter.Format(card, isReserved);
            }

            SetActive(hoverCardPanel, true);
        }

        private void ShowHoverCard(GameObject source, int slotIndex, AssetCardRuntimeData card, bool isReserved)
        {
            ShowHoverCard(card, isReserved);

            var sourceRect = source.GetComponent<RectTransform>();
            var hoverRect = hoverCardPanel != null ? hoverCardPanel.GetComponent<RectTransform>() : null;
            if (sourceRect == null || hoverRect == null)
            {
                return;
            }

            hoverRect.position = sourceRect.position + new Vector3(GetCurrentMarketHoverOffsetX(slotIndex), 0f, 0f);
        }

        private static float GetCurrentMarketHoverOffsetX(int slotIndex)
        {
            return slotIndex < 6 ? 300f : -300f;
        }

        private void HideHoverCard()
        {
            SetActive(hoverCardPanel, false);
        }

        private void ApplyReservedPosition(GameObject target, bool isReserved)
        {
            var rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                return;
            }

            if (!normalCurrentMarketButtonPositions.TryGetValue(target.name, out var normalPosition))
            {
                normalPosition = isReserved
                    ? rectTransform.anchoredPosition - new Vector2(0f, ReservedCardOffsetY)
                    : rectTransform.anchoredPosition;
                normalCurrentMarketButtonPositions[target.name] = normalPosition;
            }

            rectTransform.anchoredPosition = normalPosition + (isReserved ? new Vector2(0f, ReservedCardOffsetY) : Vector2.zero);
        }

        private void ConfigureActionButton(
            Button button,
            AssetCardRuntimeData card,
            bool isUnreserve,
            bool isReserved,
            Button reserveButton,
            Button unreserveButton)
        {
            var trigger = EnsureEventTrigger(button.gameObject);
            trigger.triggers.Clear();
            AddHoverTrigger(trigger, EventTriggerType.PointerEnter, () =>
            {
                hoveredActionRuntimeId = card.RuntimeId;
                UpdateActionButtons(card, isReserved, reserveButton, unreserveButton);
            });
            AddHoverTrigger(trigger, EventTriggerType.PointerExit, () =>
            {
                if (hoveredActionRuntimeId == card.RuntimeId)
                {
                    hoveredActionRuntimeId = string.Empty;
                }

                UpdateActionButtons(card, isReserved, reserveButton, unreserveButton);
            });

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                hoveredCardRuntimeId = string.Empty;
                hoveredActionRuntimeId = string.Empty;
                HideHoverCard();
                if (isUnreserve)
                {
                    onCurrentMarketCardUnreserved?.Invoke(card);
                }
                else
                {
                    onCurrentMarketCardReserved?.Invoke(card);
                }
            });
        }

        private void UpdateActionButtons(
            AssetCardRuntimeData card,
            bool isReserved,
            Button reserveButton,
            Button unreserveButton)
        {
            var shouldShow = card.Card.CardDomain == CardDomain.Stock
                && (hoveredCardRuntimeId == card.RuntimeId || hoveredActionRuntimeId == card.RuntimeId);
            reserveButton.gameObject.SetActive(shouldShow && !isReserved);
            unreserveButton.gameObject.SetActive(shouldShow && isReserved);
        }

        private static Button EnsureCardActionButton(Transform parent, string name, string label)
        {
            var existing = parent.Find(name);
            var buttonObject = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(parent, false);

            var image = buttonObject.GetComponent<Image>();
            if (image == null)
            {
                image = buttonObject.AddComponent<Image>();
            }

            image.color = new Color(0.03f, 0.05f, 0.06f, 0.96f);

            var button = buttonObject.GetComponent<Button>();
            if (button == null)
            {
                button = buttonObject.AddComponent<Button>();
            }

            var textTransform = buttonObject.transform.Find(name + " Text");
            var text = textTransform != null
                ? textTransform.GetComponent<Text>()
                : new GameObject(name + " Text", typeof(RectTransform)).AddComponent<Text>();
            text.transform.SetParent(buttonObject.transform, false);
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 14;
            text.color = Color.white;
            text.raycastTarget = false;

            var textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            buttonObject.SetActive(false);
            return button;
        }

        private static void PositionActionButton(Button button, bool isUnreserve)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(72f, 34f);
            rectTransform.anchoredPosition = isUnreserve
                ? new Vector2(0f, 90f)
                : new Vector2(0f, -90f);
        }

        private static void StyleCardButton(Button button, AssetCardRuntimeData card, MarketTapeZone zone, bool isReserved)
        {
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = isReserved
                    ? new Color(0.22f, 0.20f, 0.10f, 0.98f)
                    : GetCardColor(card.Card.Rarity, zone);
            }

            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.alignment = TextAnchor.UpperLeft;
                text.fontSize = zone == MarketTapeZone.UpcomingMarket ? 14 : 16;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = zone == MarketTapeZone.UpcomingMarket ? 10 : 12;
                text.resizeTextMaxSize = zone == MarketTapeZone.UpcomingMarket ? 14 : 16;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
                text.color = zone == MarketTapeZone.UpcomingMarket
                    ? new Color(0.78f, 0.82f, 0.86f, 0.86f)
                    : Color.white;
            }
        }

        private static Color GetCardColor(AssetRarity rarity, MarketTapeZone zone)
        {
            if (rarity == AssetRarity.Uncommon)
            {
                return zone == MarketTapeZone.CurrentMarket
                    ? new Color(0.18f, 0.19f, 0.12f, 0.98f)
                    : new Color(0.14f, 0.13f, 0.10f, 0.96f);
            }

            if (rarity == AssetRarity.Rare)
            {
                return zone == MarketTapeZone.CurrentMarket
                    ? new Color(0.15f, 0.12f, 0.20f, 0.98f)
                    : new Color(0.11f, 0.10f, 0.15f, 0.96f);
            }

            return zone == MarketTapeZone.CurrentMarket
                ? new Color(0.12f, 0.18f, 0.17f, 0.98f)
                : new Color(0.10f, 0.13f, 0.15f, 0.96f);
        }

        private static void ReplaceButtons(List<Button> target, IEnumerable<Button> source)
        {
            target.Clear();
            target.AddRange(source);
        }

        private static void SetActive(GameObject gameObject, bool isActive)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(isActive);
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

        private static void ApplyFailureFeedback(GameObject target, bool shouldShake)
        {
            var feedback = target.GetComponent<MarketCardFailureFeedback>();
            if (feedback == null)
            {
                feedback = target.AddComponent<MarketCardFailureFeedback>();
            }

            if (shouldShake)
            {
                feedback.RequestShake();
            }
        }

    }

    public sealed class MarketCardFailureFeedback : MonoBehaviour
    {
        public int ShakeRequestCount { get; private set; }

        public void RequestShake()
        {
            ShakeRequestCount++;
        }
    }
}
