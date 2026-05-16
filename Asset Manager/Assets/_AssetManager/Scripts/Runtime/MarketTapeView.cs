using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class MarketTapeView : MonoBehaviour
    {
        [SerializeField]
        private GameObject marketPanel;

        private readonly List<Button> sellImminentButtons = new List<Button>();
        private readonly List<Button> currentMarketButtons = new List<Button>();
        private readonly List<Button> upcomingMarketButtons = new List<Button>();
        private Action<AssetCardRuntimeData, MarketTapeZone> onMarketCardSelected;

        public GameObject MarketPanel => marketPanel;
        public IReadOnlyList<Button> SellImminentButtons => sellImminentButtons;
        public IReadOnlyList<Button> CurrentMarketButtons => currentMarketButtons;
        public IReadOnlyList<Button> UpcomingMarketButtons => upcomingMarketButtons;

        public void Bind(
            GameObject market,
            IEnumerable<Button> sellImminentCardButtons,
            IEnumerable<Button> currentMarketCardButtons,
            IEnumerable<Button> upcomingMarketCardButtons)
        {
            marketPanel = market;

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

            ShowCards(sellImminentButtons, run.MarketTape.SellImminentCards, MarketTapeZone.SellImminent);
            ShowCards(currentMarketButtons, run.MarketTape.CurrentMarketCards, MarketTapeZone.CurrentMarket);
            ShowCards(upcomingMarketButtons, run.MarketTape.UpcomingMarketCards, MarketTapeZone.UpcomingMarket);
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
                SetButtonText(button, FormatCard(card));
                StyleCardButton(button, card, zone);
                button.onClick.AddListener(() => onMarketCardSelected?.Invoke(card, zone));
            }
        }

        private static string FormatCard(AssetCardRuntimeData card)
        {
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

        private static void StyleCardButton(Button button, AssetCardRuntimeData card, MarketTapeZone zone)
        {
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = GetCardColor(card.Card.Rarity, zone);
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
