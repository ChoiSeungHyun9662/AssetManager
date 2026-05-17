# 07. 동일 주식 3장 호일 합성과 종목 제거

Status: ready-for-agent

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

동일 주식 3장을 보유하면 즉시 1장의 호일 주식으로 합치는 규칙을 구현한다. 호일 주식은 덱에 별도 카드로 존재하지 않고, 같은 종목 3장 보유의 결과 상태다. 호일 완성 후 시장, 예약 슬롯, 주식 덱에 남아 있는 같은 종목 주식은 모두 이번 게임에서 제거한다.

호일이 완성되면 가장 먼저 보유한 같은 종목 주식 슬롯이 호일 주식으로 바뀌고, 나머지 두 슬롯은 빈칸으로 남는다. 이후 새 주식은 포트폴리오의 가장 왼쪽 빈칸에 들어간다.

## Existing implementation conflicts

- `AssetCardData`와 `AssetCardRuntimeData`는 호일 상태, 호일 가치/배당금, 종목별 min/max 덱 수량을 표현하지 않는다.
- `OwnedAssetState`는 순서 있는 보유 목록이지만 포트폴리오 빈칸 보존과 슬롯 병합 규칙이 없다.
- `MarketTape`의 removed 처리는 개별 카드 제거 중심이고, 종목 전체 제거를 알지 못한다.
- 별도 `ReservationState` 기반 예약 카드 제거 로직은 시장 슬롯 예약으로 바뀌어야 한다.

## Refactor approach

- 주식에는 동일 종목 판정을 위한 안정적인 stock id와 획득 순서를 둔다.
- 포트폴리오 슬롯 서비스에서 세 번째 동일 주식 구매 후 즉시 병합을 처리한다.
- 병합 결과는 earliest acquired 슬롯에 남기고, 나머지 슬롯은 비워 둔다.
- 종목 제거는 주식 덱, 시장 슬롯, 예약 상태를 한 번에 청소하는 규칙으로 만들고, 시장 빈칸은 당김으로 후처리한다.

## Acceptance criteria

- [ ] 동일 주식 3장을 보유하는 순간 즉시 호일 주식 1장으로 합쳐진다.
- [ ] 포트폴리오가 8/8이어도 세 번째 동일 주식 구매가 즉시 호일로 이어지는 경우 구매를 허용한다.
- [ ] 호일 결과는 가장 먼저 획득한 같은 종목 주식 슬롯에 배치된다.
- [ ] 합쳐진 나머지 두 포트폴리오 슬롯은 빈칸으로 유지된다.
- [ ] 호일 주식은 같은 등급을 유지하되, 가치와 배당금은 데이터에 지정된 호일 값을 사용한다.
- [ ] 호일 완성 후 주식 덱의 같은 종목 주식은 모두 제거된다.
- [ ] 호일 완성 후 시장과 예약 슬롯의 같은 종목 주식은 모두 제거되고, 생긴 빈칸은 시장 테이프 당김으로 메운다.
- [ ] 예약된 같은 종목 주식이 제거되면 예약 수가 함께 감소한다.

## Blocked by

- `05-market-slot-reservation.md`
- `06-stock-purchase-payment-and-portfolio-cap.md`
