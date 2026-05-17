# 01. 주식 데이터와 투자 철학 자원 전환

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

기존 자산/전문 자원 기반 런 데이터를 주식/투자 철학 기반으로 전환한다. 플레이어가 새 런을 시작하면 포트폴리오 대상은 주식으로 표시되고, 투자 철학 자원은 독서/명상/인내로 관리되어야 한다. 기존 자원 원장과 HUD 기반은 재사용하되, 명칭과 한도 규칙은 새 PRD를 따른다.

이 이슈는 이후 모든 delta 이슈의 데이터 기반이다. 아직 시장 테이프, 구매, 예약, 호일, 매도 흐름을 완성하지 않아도 되지만, 그 흐름들이 사용할 새 카드/자원/수익 명칭이 안정적으로 준비되어야 한다.

## Existing implementation conflicts

- `RunStaticDataSet`는 현재 `AssetCardData` 중심의 단일 자산 카드 목록을 제공한다.
- `RunModels`는 `AssetCardData`, `AssetCardRuntimeData`, `AssetRarity`, `ProfessionalResourceCost`, `ResourceType.Research/Credit/Commodity` 등 이전 도메인 이름을 기준으로 한다.
- `ResourceLedger`와 `ResourceHud`는 전문 자원 총량 제한과 기존 칩 이름을 전제로 한다.
- 기존 테스트는 리서치/신용/원자재와 전문 자원 총량 10을 기대한다.

## Refactor approach

- 데이터와 런타임 모델의 public 의미를 주식과 투자 철학으로 바꾸되, 이후 이슈가 작게 이어질 수 있도록 기존 원장/부트스트랩 흐름은 최대한 유지한다.
- `ResourceLedger`의 책임은 유지하고, 투자 철학 총량 10과 각 종류별 5 제한을 같은 캡 처리 경로에 얹는다.
- 카드 데이터는 호일 값, 배당금, 덱 포함 수량을 담을 수 있도록 확장하고, 실제 호일 합성 처리는 별도 이슈에서 붙인다.
- 테스트는 새 명칭과 한도 규칙을 기준으로 갱신한다.

## Acceptance criteria

- [x] 새 런의 포트폴리오 카드 도메인은 주식으로 표시되며, 사용자-facing 텍스트에서 자산 용어가 새 흐름의 주 개념으로 노출되지 않는다.
- [x] 리서치/신용/원자재는 독서/명상/인내로 교체된다.
- [x] 투자 철학은 총 10개, 각 종류별 5개 보유 한도를 가진다.
- [x] 투자 철학 초과 획득 시 기존 보유량은 유지되고 신규 초과분만 버려진다.
- [x] 현금과 딜은 투자 철학 총량/개별 한도에 포함되지 않는다.
- [x] 주식 데이터는 기본 가치, 기본 배당금, 호일 가치, 호일 배당금, 비용, 덱 포함 수량 min/max를 표현할 수 있다.
- [x] 기존 `implemented-needs-editor-check` 자원 원장 작업은 완료 기반으로 간주하고, 필요한 변경만 덧입힌다.

## Completion notes

- `RunStaticDataSet` 기본 런 데이터와 `MvpRunStaticData.asset`을 주식 시드로 전환했다.
- `AssetCardData`는 기존 타입명을 유지하되 `CardDomain.Stock`, 기본 가치/배당금, 호일 가치/배당금, 덱 포함 수량 min/max를 표현한다.
- `ResourceType`은 독서/명상/인내를 canonical 투자 철학 자원으로 노출하고, 기존 Research/Credit/Commodity 이름은 후속 이슈 충돌을 줄이기 위한 호환 별칭으로 남겼다.
- `ResourceLedger`는 투자 철학 총 10, 각 종류별 5 한도를 적용하며 신규 초과분만 버린다.
- `ProjectShell`과 `MainGame.unity`의 자원 버튼/라벨을 독서/명상/인내로 갱신했다.
- `docs/agents/class-inventory.md`를 새 데이터 shape와 자원 책임에 맞게 갱신했다.

## Verification

- RED: 새 `AddInvestmentPhilosophy`, `ResourceType.Reading/Meditation/Patience`, 주식 카드 필드가 없어 EditMode 컴파일이 실패함을 확인했다.
- GREEN: Unity EditMode `AssetManager.Tests.EditMode` 71/71 passed.
- PlayMode/UI wiring: Unity PlayMode `AssetManager.Tests.PlayMode` 32/32 passed.
- Unity 수동 확인은 별도로 실행하지 않았다. PlayMode가 씬 부트스트랩, 자원 버튼, 카드 결제 UI 연결을 검증했다.

## Remaining risks

- 구매/예약/카드 상세보기 구현 내부에는 `AssetCardData`, `ProfessionalResourceCost`, `Research/Credit/Commodity` 같은 호환 이름이 남아 있다. 이번 이슈에서는 public behavior와 사용자-facing 표면을 우선 전환하고, 전역 rename은 후속 delta 이슈와 충돌하지 않도록 보류했다.
- `운용가치`, `운용 수익`, `환매 압력` 등 broader result/pressure 용어는 후속 이슈 범위에 남아 있다.

## Blocked by

None - can start immediately
