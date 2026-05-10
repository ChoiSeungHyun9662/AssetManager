using UnityEngine;

namespace AssetManager
{
    [DisallowMultipleComponent]
    public sealed class MainGameShellBootstrap : MonoBehaviour
    {
        [SerializeField]
        private RunStaticDataSet staticData;

        public RunStaticDataSet StaticData
        {
            get => staticData;
            set => staticData = value;
        }

        public RunSessionState CurrentRun { get; private set; }

        private RunStatusHud runStatusHud;
        private MarketTapeView marketTapeView;
        private CardDetailView cardDetailView;
        private MarketTapeDevControls marketTapeDevControls;
        private RunProgressControls runProgressControls;

        private void Awake()
        {
            var roots = ProjectShell.EnsureMainGameRoots();
            StartNewRun(roots.UiCanvas.transform);
        }

        public void StartNewRun()
        {
            var roots = ProjectShell.EnsureMainGameRoots();
            StartNewRun(roots.UiCanvas.transform);
        }

        private void StartNewRun(Transform uiRoot)
        {
            var data = staticData;
            if (data == null)
            {
                Debug.LogWarning("MainGameShellBootstrap has no RunStaticDataSet assigned; using temporary MVP defaults.");
                data = RunStaticDataSet.CreateMvpDefaults();
            }

            CurrentRun = RunBootstrapper.CreateNewRun(data);
            BindRunUi(uiRoot);
            RefreshRunUi();
        }

        public void AdvanceToNextBusinessDay()
        {
            if (CurrentRun == null)
            {
                return;
            }

            CurrentRun = BusinessDayFlow.AdvanceToNextBusinessDay(CurrentRun);
            RefreshRunUi();
        }

        public void OpenMarketCardDetail(AssetCardRuntimeData selectedCard)
        {
            if (CurrentRun == null)
            {
                return;
            }

            CurrentRun = MarketAreaFlow.OpenMarketCardDetail(CurrentRun, selectedCard);
            RefreshRunUi();
        }

        public void CloseCardDetail()
        {
            if (CurrentRun == null)
            {
                return;
            }

            CurrentRun = MarketAreaFlow.CloseCardDetail(CurrentRun);
            RefreshRunUi();
        }

        public void ContinueSchedule()
        {
            if (CurrentRun == null)
            {
                return;
            }

            if (CurrentRun.BusinessDay.Phase == BusinessDayPhase.QuarterSettlement)
            {
                CurrentRun = BusinessDayFlow.ContinueAfterQuarterSettlement(CurrentRun);
            }
            else if (CurrentRun.BusinessDay.Phase == BusinessDayPhase.Vacation)
            {
                CurrentRun = BusinessDayFlow.ContinueAfterVacation(CurrentRun);
            }

            RefreshRunUi();
        }

        public void AdvanceMarketTape()
        {
            if (CurrentRun == null)
            {
                return;
            }

            CurrentRun = MarketTape.Advance(CurrentRun);
            RefreshRunUi();
        }

        public void RefreshMarketTape()
        {
            if (CurrentRun == null)
            {
                return;
            }

            CurrentRun = MarketTape.Refresh(CurrentRun);
            RefreshRunUi();
        }

        private void BindRunUi(Transform uiRoot)
        {
            runStatusHud = ProjectShell.EnsureRunStatusHud(uiRoot);
            marketTapeView = ProjectShell.EnsureMarketTapeView(uiRoot);
            cardDetailView = ProjectShell.EnsureCardDetailView(uiRoot);
            marketTapeDevControls = ProjectShell.EnsureMarketTapeDevControls(uiRoot);
            runProgressControls = ProjectShell.EnsureRunProgressControls(uiRoot);

            marketTapeView.SetMarketCardSelectedHandler(OpenMarketCardDetail);

            cardDetailView.CloseButton.onClick.RemoveListener(CloseCardDetail);
            cardDetailView.CloseButton.onClick.AddListener(CloseCardDetail);

            runProgressControls.NextBusinessDayButton.onClick.RemoveListener(AdvanceToNextBusinessDay);
            runProgressControls.NextBusinessDayButton.onClick.AddListener(AdvanceToNextBusinessDay);

            runProgressControls.ContinueScheduleButton.onClick.RemoveListener(ContinueSchedule);
            runProgressControls.ContinueScheduleButton.onClick.AddListener(ContinueSchedule);

            marketTapeDevControls.AdvanceButton.onClick.RemoveListener(AdvanceMarketTape);
            marketTapeDevControls.AdvanceButton.onClick.AddListener(AdvanceMarketTape);

            marketTapeDevControls.RefreshButton.onClick.RemoveListener(RefreshMarketTape);
            marketTapeDevControls.RefreshButton.onClick.AddListener(RefreshMarketTape);
        }

        private void RefreshRunUi()
        {
            runStatusHud.Show(CurrentRun);
            marketTapeView.Show(CurrentRun);
            cardDetailView.Show(CurrentRun);
            marketTapeDevControls.Show(CurrentRun);
            runProgressControls.Show(CurrentRun);
        }
    }
}
