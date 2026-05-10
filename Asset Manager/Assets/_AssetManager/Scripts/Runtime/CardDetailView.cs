using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class CardDetailView : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Text nameText;

        [SerializeField]
        private Text descriptionText;

        [SerializeField]
        private Text costText;

        [SerializeField]
        private Text managementValueText;

        [SerializeField]
        private Text incomeText;

        [SerializeField]
        private Text tagsText;

        [SerializeField]
        private Text rarityText;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button buyButton;

        [SerializeField]
        private Button reserveButton;

        public GameObject Panel => panel;
        public Text NameText => nameText;
        public Button CloseButton => closeButton;
        public Button BuyButton => buyButton;
        public Button ReserveButton => reserveButton;

        public void Bind(
            GameObject cardDetailPanel,
            Text cardName,
            Text description,
            Text cost,
            Text managementValue,
            Text income,
            Text tags,
            Text rarity,
            Button close,
            Button buy,
            Button reserve)
        {
            panel = cardDetailPanel;
            nameText = cardName;
            descriptionText = description;
            costText = cost;
            managementValueText = managementValue;
            incomeText = income;
            tagsText = tags;
            rarityText = rarity;
            closeButton = close;
            buyButton = buy;
            reserveButton = reserve;
        }

        public void Show(RunSessionState run)
        {
            var isCardDetail = run.BusinessDay.MarketArea == MarketAreaState.CardDetail;
            SetActive(panel, isCardDetail);

            if (!isCardDetail || run.CardDetail.DisplayData == null)
            {
                return;
            }

            var display = run.CardDetail.DisplayData;
            SetText(nameText, display.DisplayName);
            SetText(descriptionText, display.Description);
            SetText(costText, FormatCosts(display));
            SetText(managementValueText, "운용가치 " + display.ManagementValue);
            SetText(incomeText, "운용 수익 " + display.Income);
            SetText(tagsText, FormatTags(display));
            SetText(rarityText, "희귀도 " + display.Rarity);
            SetActive(buyButton != null ? buyButton.gameObject : null, true);
            SetActive(reserveButton != null ? reserveButton.gameObject : null, run.CardDetail.ShouldShowReserveButton);
        }

        private static string FormatCosts(CardDetailDisplayData display)
        {
            var builder = new StringBuilder();
            builder.Append("현금 ");
            builder.Append(display.CashCost);

            foreach (var cost in display.ProfessionalCosts)
            {
                builder.Append(" | ");
                builder.Append(cost.ResourceType);
                builder.Append(" ");
                builder.Append(cost.Amount);
            }

            return builder.ToString();
        }

        private static string FormatTags(CardDetailDisplayData display)
        {
            if (display.Tags.Count == 0)
            {
                return "태그 없음";
            }

            var builder = new StringBuilder();
            builder.Append("태그 ");

            for (var i = 0; i < display.Tags.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(display.Tags[i].DisplayName);
            }

            return builder.ToString();
        }

        private static void SetActive(GameObject gameObject, bool isActive)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(isActive);
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
