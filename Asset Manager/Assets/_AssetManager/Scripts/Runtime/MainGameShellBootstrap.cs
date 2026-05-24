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
        private PurchaseConfirmationView purchaseConfirmationView;
        private CardDetailView cardDetailView;
        private MarketTapeDevControls marketTapeDevControls;
        private ResourceDevControls resourceDevControls;
        private RunProgressControls runProgressControls;
        private string resourceFeedbackMessage = string.Empty;
        private string pendingPurchaseFailureCardRuntimeId = string.Empty;
        private bool isPurchaseConfirmationOpen;

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
            isPurchaseConfirmationOpen = false;
            BindRunUi(uiRoot);
            RefreshRunUi();
        }

        public void AdvanceToNextBusinessDay()
        {
            if (CurrentRun == null)
            {
                return;
            }

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = zone == MarketTapeZone.UpcomingMarket
                ? MarketAreaFlow.OpenMarketPreviewCardDetail(CurrentRun, selectedCard)
                : MarketAreaFlow.OpenMarketCardDetail(CurrentRun, selectedCard);
            if (zone != MarketTapeZone.UpcomingMarket && CurrentRun.CardDetail.SelectedCard != null)
            {
                if (PurchasePayment.CanConfirmPurchase(CurrentRun))
                {
                    if (CurrentRun.CardDetail.SelectedCard.Card.CardDomain == CardDomain.ConsumableResource)
                    {
                        ApplyPaymentResult(PurchasePayment.ConfirmPurchase(CurrentRun));
                        return;
                    }

                    isPurchaseConfirmationOpen = true;
                    resourceFeedbackMessage = string.Empty;
                    RefreshRunUi();
                    return;
                }

                ApplyFailedPurchaseAndCloseWorkingState(PurchasePayment.ConfirmPurchase(CurrentRun));
                return;
            }

            resourceFeedbackMessage = CurrentRun.CardDetail.ShouldShowReserveButton
                && !ReservationAction.CanReserve(CurrentRun)
                ? "예약 구역이 가득 찼습니다."
                : string.Empty;
            RefreshRunUi();
        }

        public void CloseCardDetail()
        {
            if (CurrentRun == null)
            {
                return;
            }

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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
            isPurchaseConfirmationOpen = false;
            ApplyPaymentResult(PurchasePayment.ConfirmPurchase(CurrentRun));
        }

        public void ClosePurchaseConfirmation()
        {
            if (CurrentRun == null || !isPurchaseConfirmationOpen)
            {
                return;
            }

            isPurchaseConfirmationOpen = false;
            CurrentRun = MarketAreaFlow.CloseCardDetail(CurrentRun);
            resourceFeedbackMessage = string.Empty;
            RefreshRunUi();
        }

        public void ConfirmReservation()
        {
            if (CurrentRun == null)
            {
                return;
            }

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = ResourceLedger.AddFundingCash(CurrentRun, 1);
            resourceFeedbackMessage = string.Empty;
            RefreshRunUi();
        }

        public void AddRevenueForDevelopment()
        {
            if (CurrentRun == null)
            {
                return;
            }

            if (isPurchaseConfirmationOpen)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = ResourceLedger.AddRevenue(CurrentRun, 1);
            resourceFeedbackMessage = string.Empty;
            RefreshRunUi();
        }

        public void AddEarnedCashForDevelopment()
        {
            AddRevenueForDevelopment();
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

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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
            purchaseConfirmationView = ProjectShell.EnsurePurchaseConfirmationView(uiRoot);
            cardDetailView = ProjectShell.EnsureCardDetailView(uiRoot);
            marketTapeDevControls = ProjectShell.EnsureMarketTapeDevControls(uiRoot);
            resourceDevControls = ProjectShell.EnsureResourceDevControls(uiRoot);
            runProgressControls = ProjectShell.EnsureRunProgressControls(uiRoot);

            marketTapeView.SetMarketCardSelectedHandler(OpenMarketCardDetail);
            marketTapeView.SetCurrentMarketCardReleasedHandler(ReleaseCurrentMarketCard);
            marketTapeView.SetCurrentMarketCardReservedHandler(ReserveCurrentMarketCard);
            marketTapeView.SetCurrentMarketCardUnreservedHandler(UnreserveCurrentMarketCard);
            portfolioSummaryView.SetStockSaleSelectedHandler(SellStockSlot);

            purchaseConfirmationView.ConfirmButton.onClick.RemoveListener(ConfirmPurchase);
            purchaseConfirmationView.ConfirmButton.onClick.AddListener(ConfirmPurchase);

            purchaseConfirmationView.BackButton.onClick.RemoveListener(ClosePurchaseConfirmation);
            purchaseConfirmationView.BackButton.onClick.AddListener(ClosePurchaseConfirmation);

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
            resourceDevControls.EarnedCashButton.onClick.RemoveListener(AddRevenueForDevelopment);
            resourceDevControls.EarnedCashButton.onClick.AddListener(AddRevenueForDevelopment);

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
            marketTapeView.Show(CurrentRun, pendingPurchaseFailureCardRuntimeId);
            purchaseConfirmationView.Show(CurrentRun, isPurchaseConfirmationOpen);
            pendingPurchaseFailureCardRuntimeId = string.Empty;
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

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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

            if (isPurchaseConfirmationOpen)
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
            pendingPurchaseFailureCardRuntimeId = result.Succeeded
                ? string.Empty
                : result.FailedCardRuntimeId;
            RefreshRunUi();
        }

        private void ApplyFailedPurchaseAndCloseWorkingState(PurchasePaymentResult result)
        {
            isPurchaseConfirmationOpen = false;
            CurrentRun = MarketAreaFlow.CloseCardDetail(result.Run);
            resourceFeedbackMessage = result.Message;
            pendingPurchaseFailureCardRuntimeId = result.FailedCardRuntimeId;
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

        private void ReleaseCurrentMarketCard(AssetCardRuntimeData selectedCard, Vector2 screenPosition)
        {
            if (CurrentRun == null || selectedCard == null)
            {
                return;
            }

            if (isPurchaseConfirmationOpen)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            if (IsInsidePortfolio(screenPosition))
            {
                CurrentRun = MarketAreaFlow.OpenMarketCardDetail(CurrentRun, selectedCard);
                if (CurrentRun.CardDetail.SelectedCard == null)
                {
                    RefreshRunUi();
                    return;
                }

                var result = PurchasePayment.ConfirmPurchase(CurrentRun);
                if (result.Succeeded)
                {
                    ApplyPaymentResult(result);
                    return;
                }

                ApplyFailedPurchaseAndCloseWorkingState(result);
                return;
            }

            RefreshRunUi();
        }

        private void ReserveCurrentMarketCard(AssetCardRuntimeData selectedCard)
        {
            if (CurrentRun == null || selectedCard == null)
            {
                return;
            }

            if (isPurchaseConfirmationOpen)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            CurrentRun = MarketAreaFlow.OpenMarketCardDetail(CurrentRun, selectedCard);
            ApplyReservationResult(ReservationAction.ConfirmReservation(CurrentRun));
        }

        private void UnreserveCurrentMarketCard(AssetCardRuntimeData selectedCard)
        {
            if (CurrentRun == null || selectedCard == null)
            {
                return;
            }

            if (isPurchaseConfirmationOpen)
            {
                return;
            }

            ClearPortfolioSaleSelection();
            ApplyReservationResult(ReservationAction.UnreserveMarketCard(CurrentRun, selectedCard));
        }

        private bool IsInsidePortfolio(Vector2 screenPosition)
        {
            if (portfolioSummaryView == null || portfolioSummaryView.Panel == null)
            {
                return false;
            }

            var rectTransform = portfolioSummaryView.Panel.GetComponent<RectTransform>();
            return rectTransform != null
                && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition);
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
