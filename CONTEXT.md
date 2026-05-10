# Asset Manager

Asset Manager는 제한된 영업일 안에서 자산 카드, 자원, 시장 테이프, 환매 압력을 관리해 최종 운용가치를 높이는 자산 운용 로그라이트 게임이다.
이 컨텍스트는 기획 문서와 구현에서 쓰는 도메인 언어를 고정한다.

## Language

### 진행 단위

**런**:
1회계년도부터 3회계년도까지 이어지는 전체 플레이.
_Avoid_: 게임, 판, 세션

**런 실패**:
런 진행이 중단되어 최종 정산으로 가지 않는 종료 상태.
_Avoid_: 게임 오버, 파산

**회계년도**:
런을 구성하는 하위 진행 단위이며, 여러 분기로 구성된다.
_Avoid_: 연도, 월드, 챕터

**분기**:
여러 영업일로 구성된 중간 목표 단위.
_Avoid_: 스테이지, 라운드

**영업일**:
플레이어가 한 번의 주요 행동을 확정할 수 있는 최소 진행 단위.
_Avoid_: 턴, 행동력

**다음 영업일**:
현재 영업일을 종료하고 다음 영업일로 넘어가는 입력.
_Avoid_: 턴 종료, 턴 넘기기

**4Q 휴가**:
1회계년도와 2회계년도의 4Q에 표시되는 비플레이 요약 구간.
_Avoid_: 휴식 스테이지, 보너스 분기

**분기 마감**:
마지막 영업일 종료 후 분기 운용 성과를 확정하고 다음 일정으로 넘어가는 처리 구간.
_Avoid_: 스테이지 마감, 라운드 종료

### 자산과 성과

**자산 카드**:
플레이어가 시장이나 예약 구역에서 매수해 포트폴리오에 편입할 수 있는 카드.
_Avoid_: 투자 카드, 상품 카드

**시장 카드**:
시장 테이프에 표시되어 매수하거나 예약할 수 있는 자산 카드.
_Avoid_: 매물, 상점 카드

**예약 카드**:
예약 액션으로 시장에서 예약 구역으로 가져온 자산 카드.
_Avoid_: 보류 카드, 킵 카드

**보유 자산**:
매수 확정으로 포트폴리오에 편입된 자산 카드.
_Avoid_: 소유 카드, 획득 카드

**포트폴리오**:
플레이어가 현재 보유한 자산들의 묶음.
_Avoid_: 덱, 인벤토리

**태그**:
자산 카드에 붙어 정산, 분류, 효과 조건에 쓰이는 표식.
_Avoid_: 속성, 키워드

**자산군 태그**:
보유 자산을 큰 카테고리로 분류하는 태그.
_Avoid_: 자산 타입, 카테고리

**일반 태그**:
정산, 효과, 시너지 조건에 쓰이는 비자산군 태그.
_Avoid_: 보조 태그, 효과 태그

**희귀도**:
자산 카드의 가치 체감, 정렬, 밸런싱에 쓰이는 등급.
_Avoid_: 레어도, 등급

**운용가치**:
자산 카드가 가진 고유 점수.
_Avoid_: AUM, 카드 AUM, 승점

**현재 운용가치**:
특정 시점의 보유 자산 운용가치 합계.
_Avoid_: 현재 AUM, 중간 점수

**최종 운용가치**:
런 종료 시점의 보유 자산 운용가치 합계.
_Avoid_: 최종 AUM, 총점

**운용 수익**:
보유 자산의 영업일 시작 현금, 분기 마감 정산 등 운용 성과로 얻은 현금.
_Avoid_: 인컴, 수입, 이익, 매출

**정산 수익**:
분기 마감 정산으로 얻어 운용 수익에 포함되는 현금.
_Avoid_: 보너스 현금, 정산 보상

**분기 마감 정산**:
보유 자산의 태그 등을 기준으로 분기 마감에서 정산 수익을 계산하는 처리.
_Avoid_: 분기 보상, 정산 보상 계산

**조달 현금**:
자원 확보로 얻은 현금을 운용 수익과 구분하기 위한 획득 출처 분류.
_Avoid_: 운용 수익, 수익 현금

**분기 운용 수익**:
해당 분기 동안 누적된 운용 수익.
_Avoid_: 분기 수입, 분기 매출

**분기 목표 운용 수익**:
해당 분기에서 환매 압력을 피하기 위해 달성해야 하는 운용 수익 기준.
_Avoid_: 스테이지 목표, 목표 매출

**목표 달성률**:
분기 운용 수익이 분기 목표 운용 수익에 도달한 비율.
_Avoid_: 달성도, 성공률

**총 운용 수익**:
런 전체 동안 누적된 운용 수익.
_Avoid_: 총수입, 총매출

**최종 평가**:
최종 운용가치를 기준으로 산정하는 런 종료 등급.
_Avoid_: 점수 등급, 랭크

**최종 정산**:
런을 끝까지 완료했을 때 최종 운용가치, 최종 평가, 운용 코멘트를 보여주는 정상 종료 결과 흐름.
_Avoid_: 결과 화면, 엔딩 화면

### 자원과 행동

**현금**:
자산 매수에 사용하는 기본 결제 자원.
_Avoid_: 돈, 골드, 자금

**기본 자원**:
현금과 전문 자원으로 구성된 자원 확보 대상 자원 묶음.
_Avoid_: 일반 자원, 유동성 자원

**전문 자원**:
자산 매수의 전문 비용 슬롯에 배치하는 리서치, 신용, 원자재의 묶음.
_Avoid_: 특수 자원, 컬러 자원

**리서치**:
리서치 비용 슬롯에 배치하는 전문 자원.
_Avoid_: 연구

**신용**:
신용 비용 슬롯에 배치하는 전문 자원.
_Avoid_: 크레딧

**원자재**:
원자재 비용 슬롯에 배치하는 전문 자원.
_Avoid_: 커머디티, 소재

**딜**:
전문 자원 비용 슬롯을 대체하고 기본 현금 비용을 낮추는 예약 보상 자원.
_Avoid_: 딜 칩, 조커, 와일드 칩

**비용 슬롯**:
자산 매수 시 전문 자원이나 딜을 배치하는 카드 상세보기의 결제 자리.
_Avoid_: 칸, 요구 자원

**전문 자원 한도**:
리서치, 신용, 원자재 합계의 보유 제한.
_Avoid_: 자원 한도, 전문 자원 최대치

**딜 한도**:
딜의 최대 보유 제한.
_Avoid_: 딜 칩 한도, 조커 한도

**자산 매수**:
시장 카드나 예약 카드를 결제해 보유 자산으로 편입하는 행동.
_Avoid_: 구매, 투자 실행

**매수 출처**:
자산 매수가 시장 카드에서 일어났는지 예약 카드에서 일어났는지 남기는 분류.
_Avoid_: 구매 출처, 획득 경로

**예약**:
시장 카드 1장을 예약 구역으로 가져오고 딜과 환매 압력을 함께 얻는 행동.
_Avoid_: 보류, 킵

**자원 확보**:
영업일 하나를 사용해 기본 자원을 확보하는 행동.
_Avoid_: 유동성 확보, 자원 획득, 채집

**행동 확정**:
검토 중인 행동이 실제로 실행되어 영업일 소비를 일으키는 순간.
_Avoid_: 액션 선택, 실행 클릭

**영업일 소비**:
행동 확정으로 현재 영업일이 종료되는 것.
_Avoid_: 턴 소모, 행동력 소비

**추가 매수권**:
자산 매수 후 특정 규칙으로 발생하는 같은 영업일의 자산 매수 전용 권리.
_Avoid_: 추가 행동, 보너스 턴

### 시장과 위험

**중앙 은행**:
시장에서 자원 확보에 진입하기 위해 조작하는 보드 요소.
_Avoid_: 중앙은행, 자원통, 자원 버튼, 은행

**예약 구역**:
예약 카드가 매수되기 전까지 머무는 보드의 보관 영역.
_Avoid_: 예약 슬롯, 보관함

**시장 영역**:
시장, 카드 상세보기, 자원 확보 중 하나로 전환되는 보드의 주요 조작 영역.
_Avoid_: 화면, 패널

**시장**:
시장 카드, 예약 카드, 중앙 은행, 다음 영업일을 조작할 수 있는 시장 영역의 기본 상태.
_Avoid_: 현재 시장, 시장 테이프

**카드 상세보기**:
선택한 자산 카드의 정보와 매수 또는 예약 행동을 표시하는 카드 확인 상태.
_Avoid_: 카드 팝업, 상세 팝업

**시장 테이프**:
매도 임박, 현재 시장, 예비 시장으로 구성된 자산 카드의 흐름 구조.
_Avoid_: 상점, 마켓 목록

**매도 임박**:
다음 시장 테이프 진행 때 제거될 시장 테이프 구역.
_Avoid_: 만료 예정, 버려질 카드

**현재 시장**:
플레이어가 가장 직접적으로 매수나 예약을 고려하는 시장 테이프 구역.
_Avoid_: 현재 매물, 메인 시장

**예비 시장**:
다음 시장 테이프 진행 때 현재 시장으로 이동할 카드 구역.
_Avoid_: 대기 시장, 미래 시장

**시장 테이프 진행**:
매도 임박 카드를 제거하고 현재 시장과 예비 시장 카드를 한 단계씩 이동시키는 처리.
_Avoid_: 시장 테이프 전체 갱신, 시장 리롤

**시장 테이프 갱신**:
기존 시장 테이프를 전부 제거하고 새 카드로 다시 구성하는 처리.
_Avoid_: 시장 테이프 진행, 슬롯 보충

**슬롯 보충**:
특정 빈 시장 슬롯에 새 자산 카드 1장을 채우는 처리.
_Avoid_: 시장 테이프 갱신

**환매 압력**:
예약과 분기 목표 미달로 증가하며 한도에 도달하면 런 실패를 일으키는 누적 리스크 수치.
_Avoid_: 스트레스, 리스크, 위기 수치

**환매 압력 단계**:
최종 운용 코멘트 산정을 위해 환매 압력을 나눈 구간.
_Avoid_: 위험 등급, 리스크 단계

**대규모 환매**:
환매 압력이 10 이상이 되었을 때 발생하는 런 실패 사유.
_Avoid_: 파산, 게임 오버

**운용 코멘트**:
최종 평가 등급과 환매 압력 단계의 조합으로 선택되는 최종 정산 문구.
_Avoid_: 결과 코멘트, 평가 문구

## Relationships

- A **런** contains three **회계년도**.
- A **회계년도** contains one or more **분기**.
- A playable **분기** contains several **영업일**.
- A **영업일** is consumed when **자산 매수**, **예약**, **자원 확보**, or **다음 영업일** is confirmed.
- **행동 확정** causes **영업일 소비**.
- **영업일 소비** either starts the next **영업일** or enters **분기 마감**.
- **분기 마감** happens after the last **영업일** of a **분기** ends.
- **분기 마감** includes **분기 마감 정산**, **목표 달성률** calculation, **환매 압력** handling, and the next schedule decision.
- A **자산 카드** can be a **시장 카드**, **예약 카드**, **보유 자산**, or removed from play.
- A **자산 카드** can have **태그**, **자산군 태그**, **일반 태그**, and **희귀도**.
- **자산군 태그** and **일반 태그** are both kinds of **태그**.
- A **시장 카드** can become a **예약 카드** in the **예약 구역** through **예약**.
- A **시장 카드** or **예약 카드** can become a **보유 자산** through **자산 매수**.
- **자산 매수** records **매수 출처** as either market-origin or reservation-origin.
- **카드 상세보기** does not consume a **영업일** by itself; **자산 매수** or **예약** from it does.
- Only **보유 자산** contributes to **최종 운용가치**, **운용 수익**, and **분기 마감 정산**.
- **현재 운용가치** and **최종 운용가치** use the same calculation at different run moments.
- **운용 수익** includes cash from owned assets at business-day start and 분기 마감 정산 수익.
- **분기 마감 정산** produces **정산 수익**.
- **정산 수익** counts as **운용 수익** and contributes to **분기 운용 수익**.
- **분기 운용 수익** is compared with **분기 목표 운용 수익** to calculate **목표 달성률**.
- **목표 달성률** determines whether **환매 압력** increases at **분기 마감**.
- **총 운용 수익** is the run-wide total of **운용 수익**.
- **조달 현금** increases the same **현금** balance but is tracked as a funding source, not **운용 수익**.
- **기본 자원** is exactly **현금** plus **전문 자원**.
- **전문 자원** is exactly **리서치**, **신용**, and **원자재**.
- **전문 자원 한도** applies to the combined amount of **리서치**, **신용**, and **원자재**.
- **딜** is not a **기본 자원** or **전문 자원**, but can replace a **전문 자원** cost slot.
- **딜 한도** applies only to **딜** and does not block **예약**.
- **시장 영역** can be in **시장**, **카드 상세보기**, or **자원 확보**.
- **중앙 은행** is used from **시장** to enter **자원 확보**.
- **시장 테이프** contains three zones: **매도 임박**, **현재 시장**, and **예비 시장**.
- **시장 테이프 진행** affects market cards but does not affect **예약 구역** or **예약 카드**.
- **시장 테이프 갱신** affects market cards but does not affect **예약 구역** or **예약 카드**.
- **예약** grants **딜**, increases **환매 압력**, advances the **시장 테이프**, and consumes one **영업일**.
- **자원 확보** can grant **기본 자원**, but never **딜**.
- **환매 압력** at 10 or higher causes immediate **런 실패** with **대규모 환매** as the failure reason.
- **런 실패** does not proceed to **최종 정산**.
- **최종 정산** is reached only when the run is completed without **런 실패**.
- **최종 평가** is based on **최종 운용가치**, not **환매 압력**.
- **환매 압력 단계** is derived from **환매 압력**.
- **운용 코멘트** uses both **최종 평가** and **환매 압력 단계**.

## Example Dialogue

> **Dev:** "예약 카드는 운용가치에 포함해서 최종 평가에 넣나요?"
> **Domain expert:** "아니요. **예약 카드**는 아직 **보유 자산**이 아니므로 **운용가치**, **운용 수익**, **분기 마감 정산**에 포함하지 않습니다."
>
> **Dev:** "플레이어가 자원 확보로 현금을 얻으면 분기 목표 운용 수익에도 반영하나요?"
> **Domain expert:** "아니요. 그 현금은 **조달 현금**입니다. 같은 **현금** 잔고에 더해지지만, 획득 출처가 **자원 확보**이므로 **운용 수익**에는 포함하지 않습니다."
>
> **Dev:** "시장 카드 예약 후에는 시장을 어떻게 바꾸나요?"
> **Domain expert:** "예약한 자리를 먼저 **슬롯 보충**하고, 그 다음 **시장 테이프 진행**을 합니다. **시장 테이프 갱신**은 회계년도 시작처럼 시장 전체를 새로 구성할 때만 씁니다."

## Flagged Ambiguities

- "AUM" was used for the card score concept. Resolved: use **운용가치**; never use "AUM" in planning or user-facing text.
- "턴" was used for the smallest action unit. Resolved: use **영업일** in planning and user-facing text.
- "스테이지" was used for the middle progress unit. Resolved: use **분기** in planning and user-facing text.
- "인컴" was used for cash from owned assets. Resolved: use **운용 수익** instead, including for business-day-start cash from owned assets.
- "**현재 운용가치**" and **최종 운용가치** use the same calculation but different timing. Resolved: use **현재 운용가치** for an in-run snapshot and **최종 운용가치** only at run end.
- "운용 수익" and "조달 현금" both increase **현금**. Resolved: **조달 현금** is not a separate resource balance; it is an accounting classification for cash gained through **자원 확보**.
- "**분기 마감**", **분기 마감 정산**, and **정산 수익** are distinct. Resolved: **분기 마감** is the whole end-of-quarter segment, **분기 마감 정산** is the calculation step, and **정산 수익** is the cash produced by that step.
- "**분기 운용 수익**", **분기 목표 운용 수익**, and **목표 달성률** are distinct. Resolved: compare the first to the second to calculate the third.
- "**총 운용 수익**" is not current cash. Resolved: it is the run-wide accumulated **운용 수익** and excludes **조달 현금**.
- "기본 자원" could be mistaken as only non-cash resources. Resolved: **기본 자원** means **현금** plus **전문 자원**; **딜** is excluded.
- "**전문 자원 한도**" and **딜 한도** are separate. Resolved: **전문 자원 한도** applies to **리서치** + **신용** + **원자재**; **딜 한도** applies only to **딜**.
- "딜 칩" was used for the reservation reward resource. Resolved: use **딜** as the canonical term.
- "자원통" was used for the liquidity entry point. Resolved: use **중앙 은행** as the canonical term.
- "중앙은행" could be used without spacing. Resolved: use **중앙 은행** with a space.
- "유동성 확보" was used for the action that grants basic resources. Resolved: use **자원 확보**, because the action can grant **전문 자원** as well as **현금**.
- **중앙 은행** granting **전문 자원** is a game abstraction, not a realistic finance model. Resolved: accept this for the board-game resource loop.
- "시장 영역" was briefly removed as too implementation-facing, but too many rules depend on it. Resolved: keep **시장 영역** as the board area that switches between market interaction, **카드 상세보기**, and **자원 확보**.
- "시장" could mean the default market interaction state, **현재 시장**, or **시장 테이프**. Resolved: **시장** means the default state of **시장 영역**; use **현재 시장** for the tape zone and **시장 테이프** for the card flow.
- The market tape zone names **매도 임박**, **현재 시장**, and **예비 시장** are canonical and should be kept.
- "대규모 환매" should not be used as a run state. Resolved: use **런 실패** for the state and **대규모 환매** for the failure reason.
- "카드 상세보기" is UI-facing but carries an important action boundary. Resolved: keep **카드 상세보기** because entering or closing it is free, while confirming **자산 매수** or **예약** from it consumes a **영업일**.
- "비용 슬롯" is UI-facing but carries an important resource rule. Resolved: keep **비용 슬롯** because **전문 자원** and **딜** interact through slot placement.
- "**행동 확정**" and **영업일 소비** are not interchangeable. Resolved: **행동 확정** is the trigger; **영업일 소비** is the consequence.
- "**매수 출처**" is not the same as card state. Resolved: it records whether **자산 매수** came from a **시장 카드** or **예약 카드**.
- "**태그**" has two canonical kinds. Resolved: use **자산군 태그** for asset category grouping and **일반 태그** for settlement/effect conditions.
- "**희귀도**" is a card property, not a final result grade. Resolved: use **최종 평가** for run-end rating.
- "시장 테이프 진행" and "시장 테이프 갱신" are not interchangeable. Resolved: **진행** means cards move one step; **갱신** means the whole market tape is rebuilt.
- "예약 슬롯" was proposed as a domain term. Resolved: use **예약 구역** instead; slot-level language belongs to implementation details.
- "예약 카드" could read as only a state or only a location. Resolved: **예약 카드** means an asset card brought from the market into the **예약 구역** by **예약**.
- "예약" and "자산 매수" are distinct. Resolved: **예약** brings a market card into the **예약 구역** and creates risk; **자산 매수** turns a market or reserved card into a **보유 자산**.
- "**회계년도**" was described as an upper unit in a way that could read larger than **런**. Resolved: **런** is the larger concept; **회계년도** is a child unit inside a **런**.
