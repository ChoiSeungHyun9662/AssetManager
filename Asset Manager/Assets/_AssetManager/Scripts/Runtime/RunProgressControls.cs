using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class RunProgressControls : MonoBehaviour
    {
        [SerializeField]
        private Button nextBusinessDayButton;

        [SerializeField]
        private Button continueScheduleButton;

        [SerializeField]
        private GameObject quarterSettlementPanel;

        [SerializeField]
        private Text quarterSettlementText;

        [SerializeField]
        private GameObject vacationPanel;

        [SerializeField]
        private Text vacationText;

        [SerializeField]
        private GameObject finalSettlementPanel;

        [SerializeField]
        private Text finalSettlementText;

        public Button NextBusinessDayButton => nextBusinessDayButton;
        public Button ContinueScheduleButton => continueScheduleButton;

        public void Bind(
            Button nextBusinessDay,
            Button continueSchedule,
            GameObject quarterSettlement,
            Text quarterSettlementLabel,
            GameObject vacation,
            Text vacationLabel,
            GameObject finalSettlement,
            Text finalSettlementLabel)
        {
            nextBusinessDayButton = nextBusinessDay;
            continueScheduleButton = continueSchedule;
            quarterSettlementPanel = quarterSettlement;
            quarterSettlementText = quarterSettlementLabel;
            vacationPanel = vacation;
            vacationText = vacationLabel;
            finalSettlementPanel = finalSettlement;
            finalSettlementText = finalSettlementLabel;
        }

        public void Show(RunSessionState run)
        {
            var phase = run.BusinessDay.Phase;

            SetActive(nextBusinessDayButton, run.State == RunState.Playing && phase == BusinessDayPhase.AwaitingAction);
            SetActive(continueScheduleButton, phase == BusinessDayPhase.QuarterSettlement || phase == BusinessDayPhase.Vacation);

            SetActive(quarterSettlementPanel, phase == BusinessDayPhase.QuarterSettlement);
            SetActive(vacationPanel, phase == BusinessDayPhase.Vacation);
            SetActive(finalSettlementPanel, phase == BusinessDayPhase.FinalSettlement);

            SetText(quarterSettlementText, $"분기 마감: {run.Calendar.FiscalYear}회계년도 {run.Calendar.Quarter}Q");
            SetText(vacationText, $"4Q 휴가: {run.Calendar.FiscalYear}회계년도 {run.Calendar.Quarter}Q");
            SetText(finalSettlementText, $"최종 정산: {run.Calendar.FiscalYear}회계년도 {run.Calendar.Quarter}Q");
        }

        private static void SetActive(Button button, bool isActive)
        {
            if (button != null)
            {
                button.gameObject.SetActive(isActive);
            }
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
