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

        [SerializeField]
        private GameObject runFailurePanel;

        [SerializeField]
        private Text runFailureText;

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
            Text finalSettlementLabel,
            GameObject failure,
            Text failureLabel)
        {
            nextBusinessDayButton = nextBusinessDay;
            continueScheduleButton = continueSchedule;
            quarterSettlementPanel = quarterSettlement;
            quarterSettlementText = quarterSettlementLabel;
            vacationPanel = vacation;
            vacationText = vacationLabel;
            finalSettlementPanel = finalSettlement;
            finalSettlementText = finalSettlementLabel;
            runFailurePanel = failure;
            runFailureText = failureLabel;
        }

        public void Show(RunSessionState run)
        {
            var phase = run.BusinessDay.Phase;

            SetActive(nextBusinessDayButton, run.State == RunState.Playing && MarketAreaFlow.CanAdvanceToNextBusinessDay(run));
            SetActive(
                continueScheduleButton,
                run.State == RunState.Playing && (phase == BusinessDayPhase.QuarterSettlement || phase == BusinessDayPhase.Vacation));

            SetActive(quarterSettlementPanel, run.State == RunState.Playing && phase == BusinessDayPhase.QuarterSettlement);
            SetActive(vacationPanel, run.State == RunState.Playing && phase == BusinessDayPhase.Vacation);
            SetActive(finalSettlementPanel, run.State == RunState.Completed && phase == BusinessDayPhase.FinalSettlement);
            SetActive(runFailurePanel, run.State == RunState.Failed);

            SetText(quarterSettlementText, FormatQuarterSettlement(run));
            SetText(vacationText, $"4Q 휴가: {run.Calendar.FiscalYear}회계년도 {run.Calendar.Quarter}Q");
            SetText(finalSettlementText, $"최종 정산: {run.Calendar.FiscalYear}회계년도 {run.Calendar.Quarter}Q");
            SetText(runFailureText, FormatRunFailure(run));
        }

        private static string FormatQuarterSettlement(RunSessionState run)
        {
            var result = run.QuarterEndResult;
            if (result == null)
            {
                return $"분기 마감: {run.Calendar.FiscalYear}회계년도 {run.Calendar.Quarter}Q";
            }

            return string.Format(
                "분기 마감: {0}회계년도 {1}Q\n분기 운용 수익 {2} / 분기 목표 {3}\n목표 달성률 {4}% | 환매 압력 +{5} | 현재 환매 압력 {6}/{7}",
                run.Calendar.FiscalYear,
                run.Calendar.Quarter,
                result.QuarterEarnedCash,
                result.TargetEarnedCash,
                (int)(result.AchievementRate * 100d),
                result.RedemptionPressureIncrease,
                result.CurrentRedemptionPressure,
                run.RedemptionPressure.MaxPressure);
        }

        private static string FormatRunFailure(RunSessionState run)
        {
            var failureReason = run.FailureReason == string.Empty
                ? RedemptionPressure.FailureReason
                : run.FailureReason;

            return string.Format(
                "런 실패: {0}\n도달 지점 {1}회계년도 {2}Q\n현재 운용가치 {3} | 총 운용 수익 {4} | 보유 자산 {5}\n환매 압력 {6}/{7}",
                failureReason,
                run.Calendar.FiscalYear,
                run.Calendar.Quarter,
                run.OwnedAssets.CurrentManagementValue,
                run.Performance.TotalEarnedCash,
                run.OwnedAssets.Count,
                run.RedemptionPressure.CurrentPressure,
                run.RedemptionPressure.MaxPressure);
        }

        private static void SetActive(Button button, bool isActive)
        {
            if (button != null)
            {
                button.gameObject.SetActive(isActive);
                button.interactable = isActive;
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
