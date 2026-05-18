using System.Text;
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

        public Text SummaryText => summaryText;
        public Text OwnedCardsText => ownedCardsText;

        public void Bind(GameObject summaryPanel, Text summary, Text ownedCards)
        {
            panel = summaryPanel;
            summaryText = summary;
            ownedCardsText = ownedCards;
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
                $"포트폴리오 | 보유 자산 {run.OwnedAssets.Count} | 현재 운용가치 {run.OwnedAssets.CurrentManagementValue} | 분기 운용 수익 {run.Performance.CurrentQuarterEarnedCash}");
            SetText(ownedCardsText, FormatOwnedCards(run.OwnedAssets));
        }

        private static string FormatOwnedCards(OwnedAssetState ownedAssets)
        {
            if (ownedAssets.Count == 0)
            {
                return "보유 자산 없음";
            }

            var builder = new StringBuilder();
            var shownCards = 0;
            foreach (var card in ownedAssets.OwnedCards)
            {
                if (card.State != AssetCardRuntimeState.Owned)
                {
                    continue;
                }

                if (shownCards == 3)
                {
                    builder.AppendLine();
                    builder.Append("+");
                    builder.Append(ownedAssets.Count - shownCards);
                    builder.Append(" 보유 자산");
                    break;
                }

                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                builder.Append("#");
                builder.Append(card.AcquiredOrder.HasValue ? card.AcquiredOrder.Value : 0);
                builder.Append(" ");
                builder.Append(card.Card.DisplayName);
                if (card.IsFoil)
                {
                    builder.Append(" FOIL");
                }

                builder.Append(" | 운용가치 ");
                builder.Append(card.ManagementValue);
                builder.Append(" | 영업일 시작 운용 수익 ");
                builder.Append(card.Income);
                shownCards++;
            }

            return builder.ToString();
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
    }
}
