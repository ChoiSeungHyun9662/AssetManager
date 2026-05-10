# 04. 시장 영역 상태와 카드 상세보기

Status: ready-for-agent

## Parent

.scratch/mvp/PRD.md

## What to build

시장 영역이 Market, CardDetail, GainLiquidity 중 하나의 상태만 가지도록 만들고, 시장 카드 또는 예약 카드를 클릭하면 카드 상세보기로 전환되게 한다. 카드 상세보기는 팝업이 아니라 시장 영역을 대체하며, 닫기를 누르면 영업일을 소비하지 않고 Market 상태로 돌아와야 한다.

## Code work

- MarketAreaState module을 구현해 단일 활성 상태를 관리한다.
- CardDetailState를 구현해 선택 카드, 매수 출처, 결제 상태, 추가 매수권 중 열렸는지 여부를 기록한다.
- 시장 카드 클릭과 예약 카드 클릭을 CardDetail 진입으로 연결한다.
- CardDetail 진입 시 선택 카드, 매수 출처, 카드 표시 데이터, 결제 상태 초기화, 예약 버튼 표시 여부를 준비한다.
- CardDetail 닫기 시 선택 카드와 결제 대기 상태를 초기화한다.
- 다음 영업일 버튼은 Market 상태에서만 활성화되게 한다.

## Unity Editor work

- 시장 영역 안에 Market Panel과 CardDetail Panel을 만든다.
- CardDetail Panel에는 카드 이미지 placeholder, 이름, 설명, 현금 비용, 전문 자원 비용, 운용가치, 운용 수익, 태그, 희귀도, 닫기 버튼, 매수 버튼, 예약 버튼 위치를 배치한다.
- Market Panel과 CardDetail Panel이 동시에 보이지 않게 연결한다.
- 시장 카드 UI를 클릭하면 CardDetail Panel로 전환되게 이벤트를 연결한다.
- 닫기 버튼을 Market 상태 복귀에 연결한다.
- 다음 영업일 버튼이 CardDetail 상태에서 비활성화되게 연결한다.

## Verification

- Play Mode에서 시장 카드 클릭 시 시장 영역이 카드 상세보기로 대체된다.
- 닫기 클릭 시 시장 테이프로 돌아오고 남은 영업일이 줄지 않는다.
- CardDetail 상태에서는 다른 시장 카드 클릭, 중앙 은행 클릭, 다음 영업일 버튼 입력이 동작하지 않는다.
- Market 상태로 돌아오면 다음 영업일 버튼이 다시 활성화된다.

## Acceptance criteria

- [ ] 시장 영역은 Market, CardDetail, GainLiquidity 중 하나만 표시한다.
- [ ] 시장 카드 클릭은 CardDetail 상태로 진입한다.
- [ ] 예약 카드 클릭을 위한 진입 hook이 준비된다.
- [ ] CardDetail 닫기는 영업일을 소비하지 않는다.
- [ ] CardDetail 닫기는 선택 카드와 결제 대기 상태를 정리한다.
- [ ] CardDetail 상태에서 다음 영업일 버튼은 비활성화된다.
- [ ] 예약 버튼은 시장 카드 상세보기에서만 표시할 수 있는 조건을 가진다.

## Blocked by

- .scratch/mvp/issues/03-market-tape-display-draw-advance-and-refresh.md

## User stories covered

7, 8, 13, 14, 22, 28

## Comments
