using System;
using System.Collections.Generic;
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
            SetText(sellImminentText, "매도 임박");
            SetText(currentMarketText, "현재 시장");
            SetText(upcomingMarketText, "예비 시장");

            ShowCards(sellImminentButtons, run.MarketTape.SellImminentCards);
            ShowCards(currentMarketButtons, run.MarketTape.CurrentMarketCards);
            ShowCards(upcomingMarketButtons, run.MarketTape.UpcomingMarketCards);
        }

        private void ShowCards(IReadOnlyList<Button> buttons, IReadOnlyList<AssetCardRuntimeData> cards)
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
                button.onClick.AddListener(() => onMarketCardSelected?.Invoke(card));
            }
        }

        private static string FormatCard(AssetCardRuntimeData card)
        {
            return $"{card.Card.DisplayName} | 현금 {card.Card.CashCost} | 운용가치 {card.Card.ManagementValue} | 운용 수익 {card.Card.Income}";
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
