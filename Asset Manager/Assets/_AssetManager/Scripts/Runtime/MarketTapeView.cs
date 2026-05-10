using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class MarketTapeView : MonoBehaviour
    {
        [SerializeField]
        private Text sellImminentText;

        [SerializeField]
        private Text currentMarketText;

        [SerializeField]
        private Text upcomingMarketText;

        public Text SellImminentText => sellImminentText;
        public Text CurrentMarketText => currentMarketText;
        public Text UpcomingMarketText => upcomingMarketText;

        public void Bind(Text sellImminent, Text currentMarket, Text upcomingMarket)
        {
            sellImminentText = sellImminent;
            currentMarketText = currentMarket;
            upcomingMarketText = upcomingMarket;
        }

        public void Show(RunSessionState run)
        {
            SetText(sellImminentText, FormatZone("매도 임박", run.MarketTape.SellImminentCards));
            SetText(currentMarketText, FormatZone("현재 시장", run.MarketTape.CurrentMarketCards));
            SetText(upcomingMarketText, FormatZone("예비 시장", run.MarketTape.UpcomingMarketCards));
        }

        private static string FormatZone(string title, IReadOnlyList<AssetCardRuntimeData> cards)
        {
            var builder = new StringBuilder();
            builder.AppendLine(title);

            foreach (var card in cards)
            {
                builder.Append(card.Card.DisplayName);
                builder.Append(" | 현금 ");
                builder.Append(card.Card.CashCost);
                builder.Append(" | 운용가치 ");
                builder.Append(card.Card.ManagementValue);
                builder.Append(" | 인컴 ");
                builder.AppendLine(card.Card.Income.ToString());
            }

            return builder.ToString().TrimEnd();
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
