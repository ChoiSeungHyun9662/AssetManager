# 07a. 읽기 전용 1x8 포트폴리오 카드 보드

Status: done

## Parent

- `.scratch/stock-rules-overhaul/PRD.md`
- `.scratch/stock-rules-overhaul/mvp-issue-mapping.md`

## What to build

기존 포트폴리오 텍스트 보유 목록을 읽기 전용 1x8 카드 보드로 대체한다. 포트폴리오 보드는 `OwnedAssetState.StockSlots`의 순서와 빈 슬롯을 그대로 보여주며, 플레이어가 현재 8칸 포트폴리오의 점유 상태, 빈칸, 호일 결과를 시장 테이프처럼 한눈에 읽을 수 있어야 한다.

이번 이슈는 UI 표시 전환만 다룬다. 포트폴리오 카드 클릭, 카드 호버 확대, 매도 선택, 매도 버튼, 결제/구매/예약 조작은 추가하지 않는다. 주식 매도 조작은 후속 `08-stock-sale-and-revenue-tracking.md` 범위에서 포트폴리오 카드 보드 위에 붙인다.

## Existing implementation conflicts

- `PortfolioSummaryView`는 보유 주식 목록을 텍스트로 표시한다.
- `OwnedAssetState.StockSlots`는 8칸 포트폴리오 순서와 빈칸을 표현하지만, 현재 UI는 이 슬롯 표면을 직접 보여주지 않는다.
- 호일 합성 후 남는 빈 슬롯은 규칙상 보존되지만, 텍스트 목록만으로는 플레이어가 빈 프레임 위치를 볼 수 없다.
- 후속 주식 매도 이슈는 포트폴리오 카드 조작 표면이 필요하지만, 이번 이슈에서 조작까지 섞으면 범위가 커진다.

## Refactor approach

- 포트폴리오 표시를 `OwnedAssetState.OwnedCards` 텍스트 목록이 아니라 `OwnedAssetState.StockSlots` 기반 슬롯 보드로 전환한다.
- 포트폴리오 보드는 항상 8개의 고정 카드 프레임을 렌더링한다.
- 빈 슬롯은 텍스트 없는 빈 카드 프레임으로 남긴다.
- 보유 주식 슬롯은 이름, 등급, 현재 가치, 배당금을 표시하고, 호일 주식은 일반 주식과 시각적으로 구분한다.
- 시장 카드 표시 언어를 최대한 재사용하되, 비용, 구매, 예약, 결제, 매도 액션은 포트폴리오 슬롯에 노출하지 않는다.

## Acceptance criteria

- [x] 새 런에서 포트폴리오는 항상 8개의 카드 프레임으로 보인다.
- [x] 빈 포트폴리오 슬롯은 텍스트 없는 빈 카드 프레임으로 보인다.
- [x] 주식 구매 후 해당 주식은 `OwnedAssetState.StockSlots` 기준 가장 왼쪽 빈 포트폴리오 카드 슬롯에 보인다.
- [x] 보유 주식 슬롯에는 이름, 등급, 현재 가치, 배당금이 보인다.
- [x] 호일 주식 슬롯은 일반 주식 슬롯과 시각적으로 구분된다.
- [x] 호일 합성 후 결과 호일 주식은 첫 보유 슬롯에 보이고, 소비된 슬롯은 빈 카드 프레임으로 남는다.
- [x] 포트폴리오 슬롯에는 비용, 구매, 예약, 결제, 매도 액션이 보이지 않는다.
- [x] 기존 포트폴리오 텍스트 보유 목록은 새 플레이 경로에서 노출되지 않는다.

## TDD success criteria

- RED: PlayMode 테스트로 새 런의 포트폴리오 영역이 8개 슬롯 프레임을 제공하지 못하는 현재 동작을 잡는다.
- GREEN: 포트폴리오 슬롯 보드가 8개 프레임을 렌더링하고, 빈 슬롯은 빈 카드 프레임으로 표시되게 한다.
- RED: PlayMode 테스트로 주식 구매 후 첫 빈 슬롯에 구매 주식 카드 정보가 표시되지 않는 현재 동작을 잡는다.
- GREEN: 구매 후 `OwnedAssetState.StockSlots` 순서대로 포트폴리오 보드가 갱신되게 한다.
- RED: PlayMode 테스트로 호일 합성 후 소비 슬롯이 텍스트 목록에서 사라질 뿐 빈 프레임 위치로 보존되지 않는 표시 문제를 잡는다.
- GREEN: 호일 결과 슬롯과 소비된 빈 슬롯 프레임을 동시에 검증한다.

## Verification

- PlayMode: 포트폴리오 카드 보드 표시, 구매 후 슬롯 갱신, 호일 합성 후 빈 프레임 보존, 기존 텍스트 목록 미노출을 검증한다.
- EditMode: 새 규칙을 추가하지 않으므로 기존 포트폴리오/호일 규칙 회귀가 의심될 때만 관련 EditMode를 실행한다.
- Unity manual check: 가능하면 실제 Main Game Scene에서 새 런, 주식 구매, 호일 합성 직후 포트폴리오 1x8 보드의 읽힘을 확인한다.

## Completion notes

- Implemented `PortfolioSummaryView` as a read-only 1x8 slot board backed by `OwnedAssetState.StockSlots`.
- Left empty slots as visible blank frames and kept the legacy owned-cards text object empty in the new play path.
- Added PlayMode coverage for new-run blank frames, purchase slot update, no portfolio action/cost text, and foil merge result plus consumed blank slot preservation.
- Verified with `powershell -ExecutionPolicy Bypass -File scripts\Run-UnityBatchmode.ps1 -Mode PlayMode -AssemblyNames AssetManager.Tests.PlayMode` on 2026-05-18.
- Unity manual Main Game Scene check was not run; automated PlayMode coverage exercised the same shell UI creation and interaction paths.

## Post-completion correction

2026-05-19 UX decision supersedes the fixed empty-slot board presentation:

- The visible portfolio UI is now an `Owned Stock Card 1~N` compressed card row, capped at 8 visible cards.
- The display still derives from `OwnedAssetState.StockSlots`, but it walks slots left-to-right and skips `null`/non-owned slots.
- Empty stock slots no longer render blank frames and have no click target.
- After foil merge, an internal state such as `[FOIL, null, Other]` is shown as `[FOIL][Other]`; the displayed card keeps its original `StockSlots` index for later actions such as sale.
- The legacy `Portfolio Slot 1~8` scene objects were renamed/restructured to `Owned Stock Card 1~8`.

## Blocked by

- `06-stock-purchase-payment-and-portfolio-cap.md`
- `07-foil-merge-and-stock-removal.md`
