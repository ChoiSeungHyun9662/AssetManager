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

        [SerializeField]
        private Text sellImminentText;

        [SerializeField]
        private Text currentMarketText;

        [SerializeField]
        private Text upcomingMarketText;

        private readonly List<Button> sellImminentButtons = new List<Button>();
        private readonly List<Button> currentMarketButtons = new List<Button>();
        private readonly List<Button> upcomingMarketButtons = new List<Button>();
        private Action<AssetCardRuntimeData> onMarketCardSelected;

        public GameObject MarketPanel => marketPanel;
        public Text SellImminentText => sellImminentText;
        public Text CurrentMarketText => currentMarketText;
        public Text UpcomingMarketText => upcomingMarketText;
        public IReadOnlyList<Button> SellImminentButtons => sellImminentButtons;
        public IReadOnlyList<Button> CurrentMarketButtons => currentMarketButtons;
        public IReadOnlyList<Button> UpcomingMarketButtons => upcomingMarketButtons;

        public void Bind(
            GameObject market,
            Text sellImminent,
            Text currentMarket,
            Text upcomingMarket,
            IEnumerable<Button> sellImminentCardButtons,
            IEnumerable<Button> currentMarketCardButtons,
            IEnumerable<Button> upcomingMarketCardButtons)
        {
            marketPanel = market;
            sellImminentText = sellImminent;
            currentMarketText = currentMarket;
            upcomingMarketText = upcomingMarket;

            ReplaceButtons(sellImminentButtons, sellImminentCardButtons);
            ReplaceButtons(currentMarketButtons, currentMarketCardButtons);
            ReplaceButtons(upcomingMarketButtons, upcomingMarketCardButtons);
        }

        public void SetMarketCardSelectedHandler(Action<AssetCardRuntimeData> handler)
        {
            onMarketCardSelected = handler;
        }

        public void Show(RunSessionState run)
        {
            SetActive(marketPanel, run.BusinessDay.MarketArea == MarketAreaState.Market);
            SetZoneTitle(sellImminentText, "매도 임박", "다음 진행 시 사라짐");
            SetZoneTitle(currentMarketText, "현재 시장", "매수 / 예약 가능");
            SetZoneTitle(upcomingMarketText, "예비 시장", "다음 진행 시 이동");
            SetZoneColor(sellImminentText, new Color(0.17f, 0.09f, 0.08f, 0.94f));
            SetZoneColor(currentMarketText, new Color(0.07f, 0.15f, 0.13f, 0.96f));
            SetZoneColor(upcomingMarketText, new Color(0.09f, 0.11f, 0.18f, 0.94f));

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
                button.onClick.AddListener(() => onMarketCardSelected?.Invoke(card));
            }
        }

        private static string FormatCard(AssetCardRuntimeData card)
        {
            var builder = new StringBuilder();
            builder.AppendLine(card.Card.DisplayName);
            builder.Append("현금 ");
            builder.Append(card.Card.CashCost);
            builder.Append("  |  ");
            builder.Append(FormatProfessionalCosts(card.Card.ProfessionalCosts));
            builder.AppendLine();
            builder.Append("운용가치 ");
            builder.Append(card.Card.ManagementValue);
            builder.Append("  |  운용 수익 ");
            builder.Append(card.Card.Income);

            if (card.Card.GrantsExtraBuyAction)
            {
                builder.Append("  |  추가 매수권");
            }

            var tags = FormatTags(card.Card.Tags);
            if (tags != string.Empty)
            {
                builder.AppendLine();
                builder.Append(tags);
            }

            return builder.ToString();
        }

        private static string FormatProfessionalCosts(IReadOnlyList<ProfessionalResourceCost> costs)
        {
            if (costs.Count == 0)
            {
                return "전문자원 없음";
            }

            var builder = new StringBuilder();
            for (var i = 0; i < costs.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(ResourceLedger.GetResourceDisplayName(costs[i].ResourceType));
                builder.Append(" ");
                builder.Append(costs[i].Amount);
            }

            return builder.ToString();
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
                text.fontSize = 16;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 12;
                text.resizeTextMaxSize = 16;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
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

        private static void SetZoneTitle(Text text, string title, string subtitle)
        {
            SetText(text, title + "\n" + subtitle);
            if (text != null)
            {
                text.fontSize = 19;
                text.lineSpacing = 0.9f;
            }
        }

        private static void SetZoneColor(Text text, Color color)
        {
            if (text == null || text.transform.parent == null)
            {
                return;
            }

            var image = text.transform.parent.GetComponent<Image>();
            if (image != null)
            {
                image.color = color;
            }
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

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
