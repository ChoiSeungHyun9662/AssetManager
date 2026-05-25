# 01. v3 주식 태그와 영업일 기준선

Status: done

Type: AFK

## Parent

- `.scratch/v3/prd_v3.md`

## User stories covered

- 1, 2, 3, 61, 62, 70

## What to build

v3의 가장 작은 실행 기준선을 만든다. 모든 주식은 기술, 소비재, 에너지, 금융, 공업 중 정확히 하나의 산업/테마 태그를 가진다. 태그는 카드 자체 효과가 아니며, 카드 표면과 검사 가능한 런 데이터에서만 먼저 드러난다.

같은 슬라이스에서 v3 영업일 기준도 교체한다. 플레이 가능한 분기는 1/2 회계년도에 8영업일, 3 회계년도에 10영업일을 사용한다. 이 변경은 이후 미션과 Mr.Market 제안이 얹힐 시간 구조를 만든다.

## Acceptance criteria

- [x] 모든 주식 데이터는 Technology, Consumer, Energy, Financials, Industrials 중 정확히 하나의 태그를 가진다.
- [x] 태그가 없는 주식이나 허용되지 않은 태그를 가진 주식은 테스트에서 실패한다.
- [x] 카드 표면 또는 현재 카드 표시 경로에서 주식 태그가 확인 가능하다.
- [x] 태그 추가가 주식별 고유 효과나 카드 텍스트 효과를 만들지 않는다.
- [x] 1/2 회계년도 플레이 가능 분기는 8영업일로 시작한다.
- [x] 3 회계년도 플레이 가능 분기는 10영업일로 시작한다.
- [x] 기존 분기 이동과 휴가 분기 라우팅은 v3에서 명시적으로 바뀐 영업일 수 외에는 유지된다.
- [x] 오래된 Deal-as-payment 규칙이 v3 기준선 작업에서 되살아나지 않는다.
- [x] 관련 EditMode 테스트가 태그 유효성, 영업일 수, 분기 라우팅을 검증한다.
- [x] 최소 PlayMode 또는 UI-facing 검증이 카드 태그 표시와 남은 영업일 표시를 확인한다.

## Blocked by

None - can start immediately

## Implementation notes

- Added `StockTagCatalog` as the public v3 tag catalog and validation surface.
- Updated generated default data and `MvpRunStaticData.asset` so starter stocks use exactly one allowed v3 tag.
- Kept tags as classification/display data only; no stock-specific effect text or Deal-as-payment behavior was added.
- Updated calendar defaults to 8 business days for FY1/FY2 playable quarters and 10 business days for FY3 playable quarters.
- Updated EditMode and PlayMode/UI-facing tests for tag validation, market-card tag display, remaining-business-day display, and v3 calendar counts.

## Verification

- RED: Unity EditMode compile failed as expected when `StockTagCatalogTests` referenced missing `StockTagCatalog`.
- GREEN: Unity EditMode `AssetManager.Tests.EditMode` passed 123/123 for the tag slice after adding `StockTagCatalog` and default v3 tags.
- Final EditMode: Unity `AssetManager.Tests.EditMode` passed 124/124 on 2026-05-25, results `.scratch/test-results/editmode-20260525-215413-results.xml`.
- PlayMode first rerun: Unity `AssetManager.Tests.PlayMode` failed 59/60 because one vacation routing test still expected 4-day quarters.
- Final PlayMode: Unity `AssetManager.Tests.PlayMode` passed 60/60 on 2026-05-25 after updating that test to 8-day FY1/FY2 quarters, results `.scratch/test-results/playmode-20260525-215553-results.xml`.
