# 03. 시장 테이프 표시와 카드 드로우

Status: done

## Parent

.scratch/mvp/PRD.md

## What to build

시장 테이프를 실제 Unity 화면에 표시하고, 회계년도 시작 시 갱신, 같은 회계년도 내 다음 분기 시작 시 진행, 슬롯 보충, 중복 출현 방지 규칙을 구현한다. 플레이어는 매도 임박, 현재 시장, 예비 시장에 놓인 테스트 카드를 볼 수 있어야 한다.

## Code work

- MarketTape module을 구현한다.
- 시장 테이프 구역은 SellImminent, Current, Upcoming으로 구성한다.
- MarketConfigData의 슬롯 수를 사용해 세 구역을 같은 수로 채운다. MVP 기본값은 각 구역 3장이다.
- 회계년도 시작에는 시장 테이프 갱신을 수행한다.
- 같은 회계년도 내 다음 분기 시작에는 시장 테이프 진행을 수행한다.
- 시장 테이프 진행은 매도 임박 구역 전체 제거, 현재 시장 구역 전체를 매도 임박으로 이동, 예비 시장 구역 전체를 현재 시장으로 이동, 예비 시장 구역 전체 보충 순서로 처리한다.
- 슬롯 보충은 특정 빈 슬롯 하나를 새 카드로 채우는 처리로 분리한다.
- 보유 자산, 예약 카드, 제거된 카드, 현재 시장 테이프에 있는 카드는 새 카드 후보에서 제외한다.

## Unity Editor work

- 시장 영역에 매도 임박, 현재 시장, 예비 시장 3개 구역을 표시한다.
- 각 구역에 카드 슬롯 프리팹 또는 임시 카드 UI를 배치한다.
- 카드 슬롯에는 카드 이름, 현금 비용, 운용가치, 인컴 정도의 최소 정보를 표시한다.
- 개발용 버튼 또는 로그로 시장 테이프 진행과 갱신을 수동 확인할 수 있게 한다.
- 회계년도 시작과 다음 분기 시작 이벤트에 시장 테이프 처리를 연결한다.

## Verification

- Play Mode에서 런 시작 시 세 구역이 모두 카드로 채워진다.
- 같은 회계년도 다음 분기 시작 시 매도 임박 구역 3장 전체가 제거되고, 현재 시장 3장은 모두 매도 임박으로, 예비 시장 3장은 모두 현재 시장으로 이동하며, 예비 시장 3장이 보충된다.
- 회계년도 시작 시 기존 시장 테이프 카드가 제거되고 세 구역이 새 카드로 채워진다.
- 화면에 같은 카드가 동시에 중복 표시되지 않는다.

## Acceptance criteria

- [x] 시장 테이프는 매도 임박, 현재 시장, 예비 시장을 표시한다.
- [x] 각 구역 슬롯 수는 같은 값으로 데이터 조정 가능하며 MVP 기본값은 3장이다.
- [x] 시장 테이프 진행과 시장 테이프 갱신은 서로 다른 함수 또는 명확히 구분된 흐름으로 구현된다.
- [x] 회계년도 시작에는 갱신이 실행된다.
- [x] 같은 회계년도 내 다음 분기 시작에는 진행이 실행된다.
- [x] 4Q 휴가 자체에서는 시장 테이프 처리가 발생하지 않는다.
- [x] 새 카드 보충 후보에서 보유, 예약, 제거, 현재 표시 카드를 제외한다.

## Blocked by

- .scratch/mvp/issues/01-unity-mvp-run-shell-and-data-bootstrap.md
- .scratch/mvp/issues/02-calendar-business-day-and-schedule-loop.md

## User stories covered

15, 18, 19, 20, 21, 56, 58

## Comments

2026-05-10 TDD progress:

- RED/GREEN: added `MarketTapeTests.RefreshFillsConfiguredSlotsWithoutShowingDuplicateCards`; implemented `MarketTape.Refresh` public interface and deterministic MVP draw order.
- RED/GREEN: added `RunBootstrapperTests.CreateNewRunRefreshesMarketTape`; connected 새 런 시작 to 회계년도 시작 시장 테이프 갱신.
- RED/GREEN: added `MarketTapeTests.AdvanceRemovesSellImminentAndRefillsUpcomingMarket`; implemented `MarketTape.Advance` separately from `Refresh`.
- Correction: the first implementation note was wrong. Market tape 진행 now moves whole zones with equal slot counts: remove all 매도 임박 cards, move all 현재 시장 cards to 매도 임박, move all 예비 시장 cards to 현재 시장, then refill all 예비 시장 slots.
- RED/GREEN: added schedule boundary tests for same 회계년도 next 분기 진행, 4Q 휴가 no-op, and next 회계년도 1Q 갱신.
- RED/GREEN: added candidate exclusion coverage for 보유 자산, 예약 카드, 제거된 카드, and currently visible 시장 테이프 cards.
- GREEN: added runtime-created 시장 테이프 UI panels for 매도 임박/현재 시장/예비 시장 and temporary card text showing card name, 현금 cost, 운용가치, 인컴.
- GREEN: added development buttons for 시장 테이프 진행 and 시장 테이프 갱신.
- GREEN: corrected MVP market config to 3/3/3 and expanded MVP default card data to 24 simple test cards, enough for initial 9-card display, same-year advances, and the first fiscal-year refresh acceptance path.
- Verification: escalated Unity EditMode batchmode passed, 20/20 tests.
- Verification: escalated Unity PlayMode batchmode passed, 6/6 tests.
- Verification: `rg "AUM|턴|스테이지" Asset Manager/Assets/_AssetManager/Scripts Asset Manager/Assets/_AssetManager/Tests` returned no matches.

2026-05-10 correction after intent review:

- `to-issues` review: kept this as the existing vertical slice rather than creating a new child issue, because the requested behavior is the core acceptance rule of 03 itself.
- PRD and this issue were clarified so 시장 테이프 진행 is explicitly whole-zone movement with equal slot counts and MVP default 3 cards per zone.
- RED: corrected tests failed as expected before the implementation fix, 18/20 EditMode passed and 2/20 failed on whole-zone advance expectations.
- GREEN: `MarketTape.Advance` now removes all 매도 임박 cards, moves all 현재 시장 cards to 매도 임박, moves all 예비 시장 cards to 현재 시장, and refills 예비 시장.
- GREEN: default `MarketConfigData` and `MvpRunStaticData.asset` now use 3/3/3.
- Verification: corrected escalated Unity EditMode batchmode passed, 20/20 tests.
- Verification: corrected escalated Unity PlayMode batchmode passed, 6/6 tests.

Manual Unity checklist:

- Open `Assets/_AssetManager/Scenes/MainGame.unity`.
- Enter Play Mode and confirm the market area shows `매도 임박`, `현재 시장`, and `예비 시장`.
- Confirm each visible card line includes name, `현금`, `운용가치`, and `인컴`.
- Click `시장 테이프 진행` and confirm the previous 매도 임박 3 cards disappear, 현재 시장 3 cards move to 매도 임박, 예비 시장 3 cards move to 현재 시장, and 예비 시장 receives 3 new cards without duplicate visible cards.
- Click `시장 테이프 갱신` and confirm all zones refill without duplicate visible cards.
- Progress through 1회계년도 3Q 마감 into 4Q 휴가 and confirm the market tape does not change during the vacation transition.
- Click `계속` from 4Q 휴가 and confirm 2회계년도 1Q starts with a refreshed market tape.
