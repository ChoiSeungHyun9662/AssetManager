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
        private ResourceHud resourceHud;
        private PortfolioSummaryView portfolioSummaryView;
        private MarketTapeView marketTapeView;
        private ReservationView reservationView;
        private CardDetailView cardDetailView;
        private MarketTapeDevControls marketTapeDevControls;
        private ResourceDevControls resourceDevControls;
        private RunProgressControls runProgressControls;
        private string resourceFeedbackMessage = string.Empty;

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
            resourceFeedbackMessage = string.Empty;
            BindRunUi(uiRoot);
            RefreshRunUi();
        }

        public void AdvanceToNextBusinessDay()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = BusinessDayFlow.AdvanceToNextBusinessDay(CurrentRun);
            RefreshRunUi();
        }

        public void OpenMarketCardDetail(AssetCardRuntimeData selectedCard)
        {
            OpenMarketCardDetail(selectedCard, MarketTapeZone.CurrentMarket);
        }

        public void OpenMarketCardDetail(AssetCardRuntimeData selectedCard, MarketTapeZone zone)
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = zone == MarketTapeZone.UpcomingMarket
                ? MarketAreaFlow.OpenMarketPreviewCardDetail(CurrentRun, selectedCard)
                : MarketAreaFlow.OpenMarketCardDetail(CurrentRun, selectedCard);
            resourceFeedbackMessage = CurrentRun.CardDetail.ShouldShowReserveButton
                && !ReservationAction.CanReserve(CurrentRun)
                ? "예약 구역이 가득 찼습니다."
                : string.Empty;
            RefreshRunUi();
        }

        public void OpenReservedCardDetail(AssetCardRuntimeData selectedCard)
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = MarketAreaFlow.OpenReservedCardDetail(CurrentRun, selectedCard);
            resourceFeedbackMessage = string.Empty;
            RefreshRunUi();
        }

        public void CloseCardDetail()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = MarketAreaFlow.CloseCardDetail(CurrentRun);
            RefreshRunUi();
        }

        public void EnterLiquidityAction()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            resourceFeedbackMessage = string.Empty;
            RefreshRunUi();
        }

        public void CloseLiquidityAction()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = LiquidityAction.Close(CurrentRun);
            resourceFeedbackMessage = string.Empty;
            RefreshRunUi();
        }

        public void SelectLiquidityCash()
        {
            SelectLiquidityResource(ResourceType.Cash);
        }

        public void SelectLiquidityResearch()
        {
            SelectLiquidityResource(ResourceType.Research);
        }

        public void SelectLiquidityCredit()
        {
            SelectLiquidityResource(ResourceType.Credit);
        }

        public void SelectLiquidityCommodity()
        {
            SelectLiquidityResource(ResourceType.Commodity);
        }

        public void ConfirmPurchase()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            ApplyPaymentResult(PurchasePayment.ConfirmPurchase(CurrentRun));
        }

        public void ConfirmReservation()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            ApplyReservationResult(ReservationAction.ConfirmReservation(CurrentRun));
        }

        public void SellStockSlot(int stockSlotIndex)
        {
            if (CurrentRun == null)
            {
                return;
            }

            ApplyStockSaleResult(StockSaleAction.ConfirmSale(CurrentRun, stockSlotIndex));
        }


        public void PlaceResearchPaymentChip()
        {
            PlacePaymentChip(ResourceType.Research);
        }

        public void PlaceCreditPaymentChip()
        {
            PlacePaymentChip(ResourceType.Credit);
        }

        public void PlaceCommodityPaymentChip()
        {
            PlacePaymentChip(ResourceType.Commodity);
        }

        public void PlaceDealPaymentChip()
        {
            PlacePaymentChip(ResourceType.Deal);
        }

        public void RemovePaymentSlot1()
        {
            RemovePaymentSlot(0);
        }

        public void RemovePaymentSlot2()
        {
            RemovePaymentSlot(1);
        }

        public void RemovePaymentSlot3()
        {
            RemovePaymentSlot(2);
        }

        public void RemovePaymentSlot4()
        {
            RemovePaymentSlot(3);
        }

        public void AddFundingCashForDevelopment()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = ResourceLedger.AddFundingCash(CurrentRun, 1);
            resourceFeedbackMessage = string.Empty;
            RefreshRunUi();
        }

        public void AddEarnedCashForDevelopment()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = ResourceLedger.AddEarnedCash(CurrentRun, 1);
            resourceFeedbackMessage = string.Empty;
            RefreshRunUi();
        }

        public void AddResearchForDevelopment()
        {
            AddProfessionalResourceForDevelopment(ResourceType.Research);
        }

        public void AddCreditForDevelopment()
        {
            AddProfessionalResourceForDevelopment(ResourceType.Credit);
        }

        public void AddCommodityForDevelopment()
        {
            AddProfessionalResourceForDevelopment(ResourceType.Commodity);
        }

        public void AddDealForDevelopment()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            ApplyResourceResult(ResourceLedger.AddDeal(CurrentRun, 1));
        }

        public void ContinueSchedule()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
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

            ClearPortfolioSaleSelection();
            CurrentRun = MarketTape.Advance(CurrentRun);
            RefreshRunUi();
        }

        public void RefreshMarketTape()
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = MarketTape.Refresh(CurrentRun);
            RefreshRunUi();
        }

        private void BindRunUi(Transform uiRoot)
        {
            runStatusHud = ProjectShell.EnsureRunStatusHud(uiRoot);
            resourceHud = ProjectShell.EnsureResourceHud(uiRoot);
            portfolioSummaryView = ProjectShell.EnsurePortfolioSummaryView(uiRoot);
            marketTapeView = ProjectShell.EnsureMarketTapeView(uiRoot);
            reservationView = ProjectShell.EnsureReservationView(uiRoot);
            cardDetailView = ProjectShell.EnsureCardDetailView(uiRoot);
            marketTapeDevControls = ProjectShell.EnsureMarketTapeDevControls(uiRoot);
            resourceDevControls = ProjectShell.EnsureResourceDevControls(uiRoot);
            runProgressControls = ProjectShell.EnsureRunProgressControls(uiRoot);

            marketTapeView.SetMarketCardSelectedHandler(OpenMarketCardDetail);
            portfolioSummaryView.SetStockSaleSelectedHandler(SellStockSlot);
            reservationView.SetReservedCardSelectedHandler(OpenReservedCardDetail);

            cardDetailView.CloseButton.onClick.RemoveListener(CloseCardDetail);
            cardDetailView.CloseButton.onClick.AddListener(CloseCardDetail);

            cardDetailView.BuyButton.onClick.RemoveListener(ConfirmPurchase);
            cardDetailView.BuyButton.onClick.AddListener(ConfirmPurchase);

            cardDetailView.ReserveButton.onClick.RemoveListener(ConfirmReservation);
            cardDetailView.ReserveButton.onClick.AddListener(ConfirmReservation);

            cardDetailView.PlaceResearchButton.onClick.RemoveListener(PlaceResearchPaymentChip);
            cardDetailView.PlaceResearchButton.onClick.AddListener(PlaceResearchPaymentChip);

            cardDetailView.PlaceCreditButton.onClick.RemoveListener(PlaceCreditPaymentChip);
            cardDetailView.PlaceCreditButton.onClick.AddListener(PlaceCreditPaymentChip);

            cardDetailView.PlaceCommodityButton.onClick.RemoveListener(PlaceCommodityPaymentChip);
            cardDetailView.PlaceCommodityButton.onClick.AddListener(PlaceCommodityPaymentChip);

            cardDetailView.PlaceDealButton.onClick.RemoveListener(PlaceDealPaymentChip);
            cardDetailView.PlaceDealButton.onClick.AddListener(PlaceDealPaymentChip);

            BindPaymentSlotButtons();

            runProgressControls.NextBusinessDayButton.onClick.RemoveListener(AdvanceToNextBusinessDay);
            runProgressControls.NextBusinessDayButton.onClick.AddListener(AdvanceToNextBusinessDay);

            runProgressControls.ContinueScheduleButton.onClick.RemoveListener(ContinueSchedule);
            runProgressControls.ContinueScheduleButton.onClick.AddListener(ContinueSchedule);

            marketTapeDevControls.AdvanceButton.onClick.RemoveListener(AdvanceMarketTape);
            marketTapeDevControls.AdvanceButton.onClick.AddListener(AdvanceMarketTape);

            marketTapeDevControls.RefreshButton.onClick.RemoveListener(RefreshMarketTape);
            marketTapeDevControls.RefreshButton.onClick.AddListener(RefreshMarketTape);

            resourceDevControls.FundingCashButton.onClick.RemoveListener(AddFundingCashForDevelopment);
            resourceDevControls.FundingCashButton.onClick.AddListener(AddFundingCashForDevelopment);

            resourceDevControls.EarnedCashButton.onClick.RemoveListener(AddEarnedCashForDevelopment);
            resourceDevControls.EarnedCashButton.onClick.AddListener(AddEarnedCashForDevelopment);

            resourceDevControls.ResearchButton.onClick.RemoveListener(AddResearchForDevelopment);
            resourceDevControls.ResearchButton.onClick.AddListener(AddResearchForDevelopment);

            resourceDevControls.CreditButton.onClick.RemoveListener(AddCreditForDevelopment);
            resourceDevControls.CreditButton.onClick.AddListener(AddCreditForDevelopment);

            resourceDevControls.CommodityButton.onClick.RemoveListener(AddCommodityForDevelopment);
            resourceDevControls.CommodityButton.onClick.AddListener(AddCommodityForDevelopment);

            resourceDevControls.DealButton.onClick.RemoveListener(AddDealForDevelopment);
            resourceDevControls.DealButton.onClick.AddListener(AddDealForDevelopment);
        }

        private void RefreshRunUi()
        {
            runStatusHud.Show(CurrentRun);
            resourceHud.Show(CurrentRun, resourceFeedbackMessage);
            portfolioSummaryView.Show(CurrentRun);
            marketTapeView.Show(CurrentRun);
            reservationView.Show(CurrentRun);
            cardDetailView.Show(CurrentRun);
            marketTapeDevControls.Show(CurrentRun);
            resourceDevControls.Show(CurrentRun);
            runProgressControls.Show(CurrentRun);
        }

        private void AddProfessionalResourceForDevelopment(ResourceType resourceType)
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            ApplyResourceResult(ResourceLedger.AddProfessionalResource(CurrentRun, resourceType, 1));
        }

        private void ApplyResourceResult(ResourceLedgerResult result)
        {
            CurrentRun = result.Run;
            resourceFeedbackMessage = result.Message;
            RefreshRunUi();
        }

        private void SelectLiquidityResource(ResourceType resourceType)
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            ApplyLiquidityActionResult(LiquidityAction.Select(CurrentRun, resourceType));
        }

        private void ApplyLiquidityActionResult(LiquidityActionResult result)
        {
            CurrentRun = result.Run;
            resourceFeedbackMessage = result.Message;
            RefreshRunUi();
        }

        private void PlacePaymentChip(ResourceType resourceType)
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            ApplyPaymentResult(PurchasePayment.PlaceChip(CurrentRun, resourceType));
        }

        private void RemovePaymentSlot(int slotIndex)
        {
            if (CurrentRun == null)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            ApplyPaymentResult(PurchasePayment.RemoveChip(CurrentRun, slotIndex));
        }

        private void ApplyPaymentResult(PurchasePaymentResult result)
        {
            CurrentRun = result.Run;
            resourceFeedbackMessage = result.Message;
            RefreshRunUi();
        }

        private void ApplyReservationResult(ReservationActionResult result)
        {
            CurrentRun = result.Run;
            resourceFeedbackMessage = result.Message;
            RefreshRunUi();
        }

        private void ApplyStockSaleResult(StockSaleActionResult result)
        {
            CurrentRun = result.Run;
            resourceFeedbackMessage = result.Message;
            RefreshRunUi();
        }

        private void ClearPortfolioSaleSelection()
        {
            if (portfolioSummaryView != null)
            {
                portfolioSummaryView.ClearSaleSelection();
            }
        }

        private void BindPaymentSlotButtons()
        {
            BindPaymentSlotButton(0, RemovePaymentSlot1);
            BindPaymentSlotButton(1, RemovePaymentSlot2);
            BindPaymentSlotButton(2, RemovePaymentSlot3);
            BindPaymentSlotButton(3, RemovePaymentSlot4);
        }

        private void BindPaymentSlotButton(int index, UnityEngine.Events.UnityAction action)
        {
            if (index >= cardDetailView.PaymentSlotButtons.Count)
            {
                return;
            }

            var button = cardDetailView.PaymentSlotButtons[index];
            button.onClick.RemoveListener(action);
            button.onClick.AddListener(action);
        }
    }
}
