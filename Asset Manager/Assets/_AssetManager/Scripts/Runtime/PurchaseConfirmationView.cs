using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class PurchaseConfirmationView : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Text cardText;

        [SerializeField]
        private Button confirmButton;

        [SerializeField]
        private Button backButton;

        public GameObject Panel => panel;
        public Text CardText => cardText;
        public Button ConfirmButton => confirmButton;
        public Button BackButton => backButton;

        public void Bind(GameObject modalPanel, Text modalCardText, Button confirm, Button back)
        {
            panel = modalPanel;
            cardText = modalCardText;
            confirmButton = confirm;
            backButton = back;
        }

        public void Show(RunSessionState run, bool isOpen)
        {
            SetActive(panel, isOpen);
            if (!isOpen || run.CardDetail.SelectedCard == null)
            {
                return;
            }

            SetText(cardText, FormatModalCard(run));
        }

        private static string FormatModalCard(RunSessionState run)
        {
            var selectedCard = run.CardDetail.SelectedCard;
            var builder = new StringBuilder();
            builder.Append(MarketCardFormatter.Format(selectedCard, IsSelectedCardReserved(run)));
            builder.AppendLine();
            builder.AppendLine();
            builder.Append("비용 ");
            builder.Append(FormatCosts(
                PurchaseCostCalculator.Calculate(
                    selectedCard.Card,
                    run.Resources,
                    run.InvestmentPhilosophyMastery,
                    run.StaticData.GetInflationCostModifier(run.Calendar.FiscalYear, run.Calendar.Quarter))));
            return builder.ToString();
        }

        private static string FormatCosts(PurchaseCostBreakdown breakdown)
        {
            var parts = new List<string> { FormatToken(breakdown.CashToken) };
            AppendCostTokens(parts, breakdown.OriginalPhilosophyCosts, !breakdown.HasDiscount);

            if (!breakdown.HasDiscount)
            {
                return string.Join(", ", parts);
            }

            var discountedParts = new List<string>();
            AppendCostTokens(discountedParts, breakdown.DiscountedPhilosophyCosts, true);
            return string.Join(", ", parts) + " -> " + string.Join(", ", discountedParts);
        }

        private static void AppendCostTokens(
            List<string> parts,
            IReadOnlyList<PurchaseCostToken> tokens,
            bool markInsufficient)
        {
            foreach (var token in tokens)
            {
                parts.Add(markInsufficient ? FormatToken(token) : FormatPlainToken(token));
            }
        }

        private static string FormatToken(PurchaseCostToken token)
        {
            var text = FormatPlainToken(token);
            return token.IsInsufficient ? "<color=red>" + text + "</color>" : text;
        }

        private static string FormatPlainToken(PurchaseCostToken token)
        {
            return GetResourceToken(token.ResourceType) + token.Amount;
        }

        private static string GetResourceToken(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Cash:
                    return "$";
                case ResourceType.Reading:
                    return "R";
                case ResourceType.Meditation:
                    return "M";
                case ResourceType.Patience:
                    return "P";
                default:
                    return resourceType.ToString();
            }
        }

        private static bool IsSelectedCardReserved(RunSessionState run)
        {
            var selectedCard = run.CardDetail.SelectedCard;
            if (selectedCard == null)
            {
                return false;
            }

            foreach (var slot in run.MarketTape.Slots)
            {
                if (!slot.IsEmpty && slot.Card.RuntimeId == selectedCard.RuntimeId)
                {
                    return slot.IsReserved;
                }
            }

            return false;
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
