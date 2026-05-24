# 18. 단일 예약 카드 잠금 버튼 토글

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD_20260520_feedback_overhaul.md`

## What to build

예약을 기존 영업일 소비 행동에서 시장 카드 1장을 고정하는 즉시 토글 조작으로 바꾼다. 예약은 별도 드롭존, 드래그 임계값, 모달을 사용하지 않는다. 예약은 주식 카드 전용이며 소모형 자원 카드는 예약 버튼을 표시하지 않는다.

일반 주식 카드를 호버하면 원래 시장 카드 아래에 자물쇠 예약 버튼을 표시한다. 예약 버튼을 클릭하면 해당 주식이 예약되고, 카드의 아래 면이 예약 버튼의 아래 면 위치까지 내려간다. 예약된 주식 카드를 호버하면 내려간 카드 위에 예약 해제 버튼을 표시한다. 예약 해제 버튼을 클릭하면 즉시 예약이 해제되고 카드는 원래 시장 줄 위치로 복귀한다.

예약 가능 카드는 한 장뿐이다. 이미 예약된 카드가 있을 때 다른 카드를 예약하면 기존 예약은 자동 해제되고 새 카드만 예약된다. 예약과 예약 해제는 영업일을 소비하지 않고 딜을 지급하지 않으며 환매 압력/월세 밀림을 증가시키지 않는다. 예약된 카드는 내려간 위치에 고정되어 보이며, 그 내려간 위치가 클릭, 드래그, 호버 기준이 된다. 예약된 카드는 시장 진행, 당김, 갱신에서도 남아 있어야 하고, 일반 카드와 같은 매수 규칙으로 매수할 수 있어야 한다. 카드 또는 예약/해제 버튼 위에 포인터가 있으면 버튼은 계속 표시된다. 예약/해제 버튼 클릭은 카드 클릭 매수로 전파되지 않는다.

## Acceptance criteria

- [x] 일반 주식 카드를 호버하면 원래 시장 카드 아래에 자물쇠 예약 버튼이 표시된다.
- [x] 소모형 자원 카드를 호버해도 예약 버튼은 표시되지 않는다.
- [x] 예약 버튼 클릭은 해당 주식 카드를 예약하고 카드 클릭 매수로 전파되지 않는다.
- [x] 예약된 카드는 카드 아래 면이 예약 버튼 아래 면 위치까지 내려간 상태로 고정된다.
- [x] 예약과 예약 해제는 확인 모달 없이 즉시 반영된다.
- [x] 예약은 영업일을 소비하지 않고 딜을 지급하지 않으며 월세 밀림을 증가시키지 않는다.
- [x] 동시에 예약 가능한 카드는 한 장뿐이다.
- [x] 새 카드를 예약하면 기존 예약은 자동 해제되고 새 카드만 예약된다.
- [x] 예약된 주식 카드를 호버하면 내려간 카드 위에 예약 해제 버튼이 표시된다.
- [x] 예약 해제 버튼 클릭은 해당 주식 카드를 즉시 예약 해제하고 카드 클릭 매수로 전파되지 않는다.
- [x] 예약 해제된 카드는 버튼 위치를 역산 기준으로 쓰지 않고 정상 시장 줄로 돌아간다.
- [x] 카드에서 예약/해제 버튼으로 포인터를 이동해도 버튼은 사라지지 않는다.
- [x] 예약된 카드의 클릭, 드래그, 호버 기준은 내려간 시각 위치다.
- [x] 예약된 카드는 시장 진행, 당김, 갱신에서 제거되거나 이동하지 않고 남아 있다.
- [x] 예약된 카드를 짧게 놓으면 일반 카드와 같은 매수 확인 모달 흐름을 사용한다.
- [x] 예약된 카드를 포트폴리오 영역에 놓으면 일반 카드와 같은 즉시 매수 흐름을 사용한다.
- [x] 예약된 카드를 포트폴리오 영역 밖에 드래그해 놓으면 아무 액션도 일어나지 않고 내려간 예약 위치로 복귀한다.
- [x] 예약된 카드 매수 성공 시 예약이 해제되고 기존 구매 후 슬롯 보충 규칙이 실행된다.
- [x] 관련 EditMode 테스트가 단일 예약, 자동 이전 예약 해제, 비소비/비보상, 시장 진행/갱신 유지, 매수 시 예약 해제를 검증한다.
- [x] 관련 PlayMode 테스트가 주식 전용 예약 버튼, 예약 해제 버튼, 버튼 hover 유지, 버튼 클릭 전파 차단, 내려간 위치 기준 호버/매수 조작을 검증한다.

## Completion notes

- RED/GREEN: updated EditMode reservation tests from the old consuming reservation behavior to the new single-card toggle behavior. The first RED was a missing unreserve rule API; GREEN made reservation non-consuming/non-rewarding, single-slot, and auto-releasing.
- RED/GREEN: updated PlayMode coverage from legacy card-detail reservation to current-market card-local buttons. The RED first failed on missing reserve/unreserve button surfaces; GREEN added stock-only hover buttons, click propagation blocking, hover persistence, and lowered-card interaction positioning.
- Compatibility correction: old MVP smoke tests that expected Deal, 월세 밀림, and bankruptcy from reservation were updated because those expectations conflict with the 2026-05-20 amendment PRD.
- Verification: Unity EditMode `AssetManager.Tests.EditMode` passed 113/113; Unity PlayMode `AssetManager.Tests.PlayMode` passed 56/56.
- Unity manual check: not run. Automated PlayMode covered the specified pointer hover/click/drag flows.

## Blocked by

- `.scratch/stock-rules-overhaul/issues/17-market-card-drag-and-portfolio-drop-purchase.md`
