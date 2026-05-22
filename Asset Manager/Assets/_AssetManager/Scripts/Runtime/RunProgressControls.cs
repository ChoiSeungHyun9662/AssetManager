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
            SetButtonText(
                nextBusinessDayButton,
                run.BusinessDay.IsAwaitingExtraBuyChoice ? "추가 매수 포기" : "다음 영업일");
            SetActive(
                continueScheduleButton,
                run.State == RunState.Playing && (phase == BusinessDayPhase.QuarterSettlement || phase == BusinessDayPhase.Vacation));

            SetActive(quarterSettlementPanel, run.State == RunState.Playing && phase == BusinessDayPhase.QuarterSettlement);
            SetActive(vacationPanel, run.State == RunState.Playing && phase == BusinessDayPhase.Vacation);
            SetActive(finalSettlementPanel, run.State == RunState.Completed && phase == BusinessDayPhase.FinalSettlement);
            SetActive(runFailurePanel, run.State == RunState.Failed);

            SetText(quarterSettlementText, FormatQuarterSettlement(run));
            SetText(vacationText, FormatVacation(run));
            SetText(finalSettlementText, FormatFinalSettlement(run));
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
                "분기 마감: {0}회계년도 {1}Q\n현재 분기 수익 {2} / 분기 목표 {3}\n목표 달성률 {4}% | 월세 밀림 +{5} | 현재 월세 밀림 {6}/{7}",
                run.Calendar.FiscalYear,
                run.Calendar.Quarter,
                result.QuarterRevenue,
                result.QuarterRevenueTarget,
                (int)(result.AchievementRate * 100d),
                result.RentArrearsIncrease,
                result.CurrentRentArrears,
                run.RentArrears.MaxArrears);
        }

        private static string FormatVacation(RunSessionState run)
        {
            var summary = FiscalYearSummary.Create(run);
            return string.Format(
                "4Q 휴가: {0}회계년도 요약\n현재 가치 {1} | 올해 수익 {2}\n분기별 수익 {3}\n보유 주식 {4} | 월세 밀림 {5}/{6}",
                summary.FiscalYear,
                summary.CurrentValue,
                summary.FiscalYearRevenue,
                FormatQuarterRevenue(summary),
                summary.OwnedStockCount,
                summary.CurrentRentArrears,
                summary.MaxRentArrears);
        }

        private static string FormatQuarterRevenue(FiscalYearSummaryResult summary)
        {
            if (summary.QuarterRevenue.Count == 0)
            {
                return "없음";
            }

            var lines = new string[summary.QuarterRevenue.Count];
            for (var i = 0; i < summary.QuarterRevenue.Count; i++)
            {
                var record = summary.QuarterRevenue[i];
                lines[i] = record.Quarter + "Q " + record.Revenue;
            }

            return string.Join(" / ", lines);
        }

        private static string FormatFinalSettlement(RunSessionState run)
        {
            var settlement = FinalSettlement.Create(run);
            return string.Format(
                "최종 정산\n최종 가치 {0} | 최종 평가 {1}\n총 수익 {2} | 보유 주식 {3}\n월세 밀림 {4}/{5}\n최종 코멘트: {6}",
                settlement.FinalValue,
                settlement.FinalRating.DisplayName,
                settlement.TotalRevenue,
                settlement.OwnedStockCount,
                settlement.CurrentRentArrears,
                settlement.MaxRentArrears,
                settlement.FinalComment);
        }

        private static string FormatRunFailure(RunSessionState run)
        {
            var failureReason = run.FailureReason == string.Empty
                ? RentArrears.FailureReason
                : run.FailureReason;

            return string.Format(
                "{0}\n도달 지점 {1}회계년도 {2}Q\n현재 가치 {3} | 총 수익 {4} | 보유 주식 {5}\n월세 밀림 {6}/{7}",
                failureReason,
                run.Calendar.FiscalYear,
                run.Calendar.Quarter,
                run.OwnedAssets.CurrentValue,
                run.Performance.TotalRevenue,
                run.OwnedAssets.Count,
                run.RentArrears.CurrentArrears,
                run.RentArrears.MaxArrears);
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

        private static void SetButtonText(Button button, string value)
        {
            if (button == null)
            {
                return;
            }

            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
