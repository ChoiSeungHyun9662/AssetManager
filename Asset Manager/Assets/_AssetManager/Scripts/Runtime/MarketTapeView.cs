using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class MarketTapeView : MonoBehaviour
    {
        [SerializeField]
        private GameObject marketPanel;

        [SerializeField]
        private GameObject hoverCardPanel;

        [SerializeField]
        private Text hoverCardText;

        private readonly List<Button> sellImminentButtons = new List<Button>();
        private readonly List<Button> currentMarketButtons = new List<Button>();
        private readonly List<Button> upcomingMarketButtons = new List<Button>();
        private Action<AssetCardRuntimeData, MarketTapeZone> onMarketCardSelected;
        private Action<AssetCardRuntimeData, Vector2> onCurrentMarketCardReleased;
        private RectTransform draggedCard;
        private Vector2 draggedCardStartPosition;
        private bool hasDraggedCard;

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
            var trigger = EnsureEventTrigger(target);
            trigger.triggers.Clear();
            AddHoverTrigger(trigger, EventTriggerType.PointerEnter, () => ShowHoverCard(target, slotIndex, card, isReserved));
            AddHoverTrigger(trigger, EventTriggerType.PointerExit, HideHoverCard);
            AddPointerTrigger(trigger, EventTriggerType.PointerDown, eventData => BeginDrag(target, eventData));
            AddPointerTrigger(trigger, EventTriggerType.Drag, eventData => DragCard(eventData));
            AddPointerTrigger(trigger, EventTriggerType.PointerUp, eventData => EndDrag(card, eventData));
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
            draggedCard.anchoredPosition += eventData.delta;
        }

        private void EndDrag(AssetCardRuntimeData card, PointerEventData eventData)
        {
            var shouldRelease = draggedCard != null && hasDraggedCard;
            if (draggedCard != null)
            {
                draggedCard.anchoredPosition = draggedCardStartPosition;
            }

            draggedCard = null;
            hasDraggedCard = false;

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
