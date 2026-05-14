# MVP Play Mode QA Checklist

Use this checklist after automated EditMode and PlayMode tests pass. The goal is to verify the MVP through Unity-visible controls, not just through rule code.

## Setup

- Open `Assets/_AssetManager/Scenes/MainGame.unity`.
- Confirm `Main Game Shell` has `MvpRunStaticData.asset` assigned, or that play mode logs the temporary MVP default fallback.
- Enter Play Mode and confirm the HUD starts at `1회계년도 1Q`, `4영업일`, `환매 압력 0/10`.
- Confirm the visible market has three zones: `매도 임박`, `현재 시장`, `예비 시장`, with three card buttons each.
- Confirm the QA/dev controls are visible only while the market area is active: market tape advance/refresh and resource add buttons.

## Smoke Scenarios

### 1. Basic Progression

- Click `다음 영업일` four times.
- Expected: remaining 영업일 reaches 0 and the `분기 마감` panel appears.
- Expected: the panel shows 분기 운용 수익, 분기 목표, 목표 달성률, and 환매 압력 change.

### 2. Market Card Purchase

- Use resource dev buttons to add the professional resources required by a current market card.
- Click a current market card and confirm 카드 상세보기 appears.
- Place required chips into 비용 슬롯.
- Confirm final cash cost is visible and `매수` becomes enabled.
- Click `매수`.
- Expected: one 보유 자산 is added, only the purchased market slot is refilled, 영업일 is consumed, and next-day 운용 수익 is applied.

### 3. Liquidity

- Click `중앙 은행`.
- Expected: 자원 확보 panel opens and `다음 영업일` is disabled.
- Click `현금` twice.
- Expected: 현금 increases by 2, 분기 운용 수익 does not increase, the panel closes, and one 영업일 is consumed.

### 4. Reservation

- Click a current market card.
- Click `예약`.
- Expected: the card appears in 예약 구역, 딜 increases by 1, 환매 압력 increases by 1, market tape advances, and one 영업일 is consumed.

### 5. Redemption Failure

- Use the debugger/inspector or a temporary seeded test state to set 환매 압력 to 9.
- Reserve a market card.
- Expected: 환매 압력 reaches 10, `런 실패` appears with `대규모 환매 발생`, and schedule continuation is unavailable.

### 6. Final Settlement

- Use a seeded QA state at `3회계년도 4Q` with 환매 압력 below 10, or play through to that point.
- Finish the final quarter and click `계속`.
- Expected: `최종 정산` appears and shows 최종 운용가치, 최종 평가, 총 운용 수익, 보유 자산 수, 환매 압력, and 운용 코멘트.

## Regression Map

- `MvpSmokeScenarioTests` covers the six smoke scenarios above through PlayMode UI/state flows.
- `RunCalendarTests`, `BusinessDayFlowTests`, `MarketAreaFlowTests`, `MarketTapeTests`, `ResourceLedgerTests`, `PurchasePaymentTests`, `LiquidityActionTests`, `ReservationActionTests`, `QuarterSettlementTests`, `FinalSettlementTests`, and related EditMode suites cover pure rule regressions.
- `MainGameShellBootstrapTests` covers shell wiring, UI visibility, button enablement, and summary panels.

## Pass Criteria

- All automated EditMode tests pass.
- All automated PlayMode tests pass.
- Manual Play Mode confirms the six smoke scenarios without missing scene, prefab, ScriptableObject, or UI references.
- Any failed manual step is recorded against issue 14 before the issue is closed.
