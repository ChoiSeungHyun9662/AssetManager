using System;
using System.Collections.Generic;
using System.Text;
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

        public void Show(RunSessionState run)
        {
            SetActive(marketPanel, run.BusinessDay.MarketArea == MarketAreaState.Market);
            HideHoverCard();

            ShowCards(sellImminentButtons, Array.Empty<AssetCardRuntimeData>(), MarketTapeZone.SellImminent);
            ShowSlots(currentMarketButtons, run.MarketTape.Slots);
            ShowCards(upcomingMarketButtons, Array.Empty<AssetCardRuntimeData>(), MarketTapeZone.UpcomingMarket);
        }

        private void ShowSlots(
            IReadOnlyList<Button> buttons,
            IReadOnlyList<MarketTapeSlotState> slots)
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
                SetButtonText(button, FormatCard(card, slot.IsReserved));
                StyleCardButton(button, card, MarketTapeZone.CurrentMarket, slot.IsReserved);
                ConfigureHoverTrigger(button.gameObject, card, slot.IsReserved);
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
                SetButtonText(button, FormatCard(card, false));
                StyleCardButton(button, card, zone, false);
                ConfigureHoverTrigger(button.gameObject, card, false);
                button.onClick.AddListener(() => onMarketCardSelected?.Invoke(card, zone));
            }
        }

        private void ConfigureHoverTrigger(GameObject target, AssetCardRuntimeData card, bool isReserved)
        {
            var trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.AddComponent<EventTrigger>();
            }

            trigger.triggers.Clear();
            AddHoverTrigger(trigger, EventTriggerType.PointerEnter, () => ShowHoverCard(card, isReserved));
            AddHoverTrigger(trigger, EventTriggerType.PointerExit, HideHoverCard);
        }

        private static void AddHoverTrigger(EventTrigger trigger, EventTriggerType eventType, Action action)
        {
            var entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener(_ => action());
            trigger.triggers.Add(entry);
        }

        private void ShowHoverCard(AssetCardRuntimeData card, bool isReserved)
        {
            if (hoverCardText != null)
            {
                hoverCardText.text = FormatCard(card, isReserved);
            }

            SetActive(hoverCardPanel, true);
        }

        private void HideHoverCard()
        {
            SetActive(hoverCardPanel, false);
        }

        private static string FormatCard(AssetCardRuntimeData card, bool isReserved)
        {
            if (card.Card.CardDomain == CardDomain.ConsumableResource)
            {
                return FormatConsumableResourceCard(card.Card);
            }

            var builder = new StringBuilder();
            builder.Append("₩");
            builder.Append(card.Card.CashCost);
            builder.Append("  ");
            builder.Append(FormatProfessionalCosts(card.Card.ProfessionalCosts));
            builder.Append("        ↗");
            builder.Append(card.Card.Income);
            if (card.Card.GrantsExtraBuyAction)
            {
                builder.Append("  +↺");
            }

            builder.AppendLine();
            builder.Append("■ ");
            builder.AppendLine(card.Card.DisplayName);
            var tags = FormatTags(card.Card.Tags);
            if (tags != string.Empty)
            {
                builder.AppendLine(tags);
            }

            builder.Append("◆");
            builder.Append(card.Card.ManagementValue);
            if (isReserved)
            {
                builder.Append("  예약");
            }

            return builder.ToString();
        }

        private static string FormatConsumableResourceCard(AssetCardData card)
        {
            var builder = new StringBuilder();
            builder.Append("₩");
            builder.Append(card.CashCost);
            builder.AppendLine();
            builder.Append("■ ");
            builder.AppendLine(card.Rarity.ToString());
            builder.Append(ResourceLedger.GetResourceDisplayName(card.ProvidedResourceType));
            builder.Append(" +");
            builder.Append(card.ProvidedResourceAmount);

            return builder.ToString();
        }

        private static string FormatProfessionalCosts(IReadOnlyList<ProfessionalResourceCost> costs)
        {
            if (costs.Count == 0)
            {
                return "—";
            }

            var builder = new StringBuilder();
            for (var i = 0; i < costs.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(GetResourceToken(costs[i].ResourceType));
                builder.Append(costs[i].Amount);
            }

            return builder.ToString();
        }

        private static string GetResourceToken(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Research:
                    return "R";
                case ResourceType.Credit:
                    return "C";
                case ResourceType.Commodity:
                    return "M";
                case ResourceType.Deal:
                    return "D";
                case ResourceType.Cash:
                    return "₩";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        private static string FormatTags(IReadOnlyList<TagData> tags)
        {
            if (tags.Count == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            for (var i = 0; i < tags.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(" / ");
                }

                builder.Append(tags[i].DisplayName);
            }

            return builder.ToString();
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

    }
}
