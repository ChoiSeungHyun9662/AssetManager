using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class ReservationView : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Text titleText;

        [SerializeField]
        private List<Button> reservedCardButtons = new List<Button>();

        private Action<AssetCardRuntimeData> onReservedCardSelected;

        public GameObject Panel => panel;
        public IReadOnlyList<Button> ReservedCardButtons => reservedCardButtons;

        public void Bind(GameObject reservationPanel, Text title, IEnumerable<Button> cardButtons)
        {
            panel = reservationPanel;
            titleText = title;
            reservedCardButtons = new List<Button>(cardButtons);
        }

        public void SetReservedCardSelectedHandler(Action<AssetCardRuntimeData> handler)
        {
            onReservedCardSelected = handler;
        }

        public void Show(RunSessionState run)
        {
            SetActive(panel, run.BusinessDay.MarketArea == MarketAreaState.Market);
            SetText(titleText, $"예약 구역 {run.Reservation.ReservedCards.Count}/{run.Reservation.Capacity}");

            for (var i = 0; i < reservedCardButtons.Count; i++)
            {
                var button = reservedCardButtons[i];
                if (button == null)
                {
                    continue;
                }

                var hasCard = i < run.Reservation.ReservedCards.Count;
                button.interactable = hasCard;
                button.onClick.RemoveAllListeners();
                SetButtonText(button, hasCard ? FormatCard(run.Reservation.ReservedCards[i]) : "비어 있음");

                if (hasCard)
                {
                    var card = run.Reservation.ReservedCards[i];
                    button.onClick.AddListener(() => onReservedCardSelected?.Invoke(card));
                }
            }
        }

        private static string FormatCard(AssetCardRuntimeData card)
        {
            return $"{card.Card.DisplayName} | 현금 {card.Card.CashCost} | 운용가치 {card.Card.ManagementValue}";
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
