# Asset Manager

Asset Manager는 제한된 영업일 안에서 주식, 투자 철학, 시장 테이프, 미션, Mr.Market 제안, 월세 밀림을 관리해 최종 가치를 높이는 주식 포트폴리오 로그라이트 게임이다.
이 컨텍스트는 기획 문서와 구현에서 쓰는 도메인 언어를 고정한다.

## Language

### 진행 단위

**런**:
1회계년도부터 3회계년도까지 이어지는 전체 플레이.
_Avoid_: 게임, 판, 세션

**파산**:
월세 밀림이 한도에 도달해 런이 최종 정산으로 가지 못하고 중단되는 종료 상태.
_Avoid_: 런 실패, 게임 오버, 대규모 환매

**회계년도**:
런을 구성하는 하위 진행 단위이며 여러 분기로 구성된다.
_Avoid_: 연도, 월드, 챕터

**분기**:
여러 영업일로 구성된 중간 목표 단위.
_Avoid_: 스테이지, 라운드

**영업일**:
플레이어가 주요 행동 1회를 확정할 수 있는 최소 진행 단위.
_Avoid_: 턴, 행동력

**다음 영업일**:
현재 영업일을 종료하고 다음 영업일로 넘어가는 입력.
_Avoid_: 턴 종료, 턴 넘기기

**4Q 휴가**:
1회계년도와 2회계년도의 4Q에 표시되는 비플레이 요약 구간.
_Avoid_: 휴식 스테이지, 보너스 분기

**분기 마감**:
마지막 영업일 종료 후 현금 흐름, 미션 수익, 목표 달성률, 월세 밀림을 확정하고 다음 일정으로 넘어가는 처리 구간.
_Avoid_: 스테이지 마감, 라운드 종료

### 주식과 성과

**주식 카드**:
시장에 등장하며 매수되면 포트폴리오에 편입될 수 있는 카드.
_Avoid_: 자산 카드, 투자 카드, 상품 카드

**시장 카드**:
시장 슬롯에 표시되어 매수하거나 예약할 수 있는 카드.
_Avoid_: 매물, 상점 카드

**예약 주식**:
예약으로 시장 슬롯에 고정된 주식 카드.
_Avoid_: 예약 카드, 보류 카드, 킵 카드

**보유 주식**:
매수 확정으로 포트폴리오에 편입된 주식 카드.
_Avoid_: 보유 자산, 소유 카드, 획득 카드

**포트폴리오**:
플레이어가 현재 보유한 주식들의 묶음.
_Avoid_: 덱, 인벤토리

**산업 태그**:
주식 카드마다 정확히 1개 붙는 산업/테마 분류.
사용 가능한 값은 기술, 소비재, 에너지, 금융, 공업이다.
_Avoid_: 속성, 키워드, 카드 효과 태그

**희귀도**:
주식 카드의 가치 체감, 정렬, 밸런싱에 쓰이는 등급.
_Avoid_: 레어도, 최종 평가 등급

**가치**:
주식 카드가 가진 평가용 점수.
_Avoid_: 운용가치, AUM, 카드 AUM, 승점

**최종 가치**:
현재 또는 런 종료 시점의 보유 주식 가치 합계.
Mr.Market 영구 가치 델타를 반영한다.
_Avoid_: 최종 운용가치, 최종 AUM, 총점

**배당금**:
보유 주식이 영업일 시작에 제공하는 현금.
_Avoid_: 인컴, 이자, 수입

**현금 흐름**:
분기 목표 판정에 쓰는 현금성 성과.
v3 기준 현금 흐름은 배당금과 주식 매도 현금의 합이다.
_Avoid_: 운용 수익, 분기 수익, 매출

**미션 수익**:
확정 미션이 분기말 현재 포트폴리오 기준으로 만드는 평가 수익.
현금에 더하지 않지만 월세 밀림 판정에는 항상 포함한다.
_Avoid_: 보너스 현금, 정산 현금

**분기 평가 목표**:
월세 밀림을 피하기 위해 분기말에 달성해야 하는 평가 기준.
_Avoid_: 분기 목표 수익, 스테이지 목표

**목표 달성률**:
`(현금 흐름 + 미션 수익) / 분기 평가 목표`.
_Avoid_: 달성도, 성공률

**총 수익**:
런 전체 동안 누적된 현금 흐름.
미션 수익과 조달 현금은 포함하지 않는다.
_Avoid_: 총매출, 총 운용 수익

**최종 평가**:
최종 가치를 기준으로 산정하는 런 종료 등급.
_Avoid_: 점수 등급, 랭크

**최종 정산**:
런을 끝까지 완료했을 때 최종 가치, 최종 평가, 총 수익, 총 미션 수익, 월세 밀림, 운용 코멘트를 보여주는 정상 종료 결과 흐름.
_Avoid_: 결과 화면, 엔딩 화면

### 미션

**미션**:
런 시작 시 후보로 제시되는 투자 논문.
한 런에 정확히 1장만 확정될 수 있다.
_Avoid_: 퀘스트, 업적, 카드 효과

**미션 후보**:
런 시작 직후 생성되는 3개의 미션 선택지.
각 후보 슬롯은 1회 멀리건할 수 있고, 멀리건을 쓴 뒤에는 수동 폐기할 수 있다.
_Avoid_: 미션 덱 손패, 선택 보상

**확정 미션**:
클리어 조건을 가장 먼저 만족해 런의 단일 미션으로 고정된 미션.
동시에 여러 후보가 만족되면 왼쪽 슬롯이 우선한다.
_Avoid_: 활성 퀘스트 목록

**클리어 조건**:
미션이 확정되는 최초 조건.
정산 공식과 분리된다.
_Avoid_: 보상 공식

**정산 공식**:
확정 미션이 매 분기말 미션 수익을 계산하는 공식.
_Avoid_: 클리어 조건

### 자원과 행동

**현금**:
주식과 소모형 자원 카드 매수에 사용하는 기본 결제 자원.
_Avoid_: 돈, 골드, 자금

**조달 현금**:
소모형 자원 카드로 얻은 현금을 현금 흐름과 구분하기 위한 획득 출처 분류.
_Avoid_: 수익 현금, 운용 수익

**투자 철학**:
주식 매수 비용 슬롯에 배치하는 독서, 명상, 인내 칩의 묶음.
_Avoid_: 전문 자원, 특수 자원, 컬러 자원

**독서**:
독서 비용 슬롯에 배치하는 투자 철학.
_Avoid_: 리서치, 연구

**명상**:
명상 비용 슬롯에 배치하는 투자 철학.
_Avoid_: 신용, 크레딧

**인내**:
인내 비용 슬롯에 배치하는 투자 철학.
_Avoid_: 원자재, 커머디티, 소재

**딜**:
투자 철학 마스터리로 전환하는 포트폴리오 성장 보상 자원.
_Avoid_: 딜 칩, 조커, 와일드 칩, 결제 대체 자원

**투자 철학 마스터리**:
딜을 소비해 올리는 영구 할인 수치.
같은 종류의 주식 매수 투자 철학 비용을 낮춘다.
_Avoid_: 숙련도, 패시브 할인, 딜 할인

**비용 슬롯**:
주식 매수 시 투자 철학을 요구하는 결제 자리.
_Avoid_: 칸, 요구 자원

**투자 철학 한도**:
독서, 명상, 인내 합계와 각 개별 보유 제한.
_Avoid_: 자원 한도, 전문 자원 한도

**주식 매수**:
시장 주식이나 예약 주식을 결제해 보유 주식으로 편입하는 행동.
_Avoid_: 자산 매수, 구매, 투자 실행

**매수 출처**:
주식 매수가 일반 시장 주식에서 일어났는지 예약 주식에서 일어났는지 남기는 분류.
_Avoid_: 구매 출처, 획득 경로

**예약**:
시장 슬롯의 주식 카드 1장을 그 위치에 잠그는 무료 행동.
예약은 영업일, 딜, 월세 밀림을 바꾸지 않는다.
_Avoid_: 보류, 킵, 예약 구역 이동

**행동 확정**:
검토 중인 행동이 실제로 실행되어 영업일 소비를 일으키는 순간.
예약과 주식 매도는 행동 확정 대상이지만 영업일을 소비하지 않는다.
_Avoid_: 액션 선택, 실행 클릭

**영업일 소비**:
주식 매수, 소모형 자원 카드 매수, 다음 영업일 입력으로 현재 영업일이 종료되는 것.
_Avoid_: 턴 소모, 행동력 소비

**추가 매수권**:
주로 Mr.Market 템포형 제안으로 발생하는 같은 영업일의 추가 매수 권리.
주식, 예약 주식, 소모형 자원 카드를 살 수 있고 예약은 이 권리를 소비하지 않는다.
_Avoid_: 추가 행동, 보너스 턴

### 시장과 Mr.Market

**시장 영역**:
시장 테이프, 포트폴리오, 자원, 다음 영업일을 조작하는 보드의 주요 조작 영역.
_Avoid_: 카드 상세보기 화면, 자원 확보 화면

**시장**:
시장 영역의 기본 상태.
_Avoid_: 현재 시장, 시장 테이프

**시장 슬롯**:
1x8 시장 테이프의 한 칸.
_Avoid_: 시장 구역, 예약 슬롯

**시장 테이프**:
8개의 시장 슬롯으로 구성된 카드 흐름 구조.
_Avoid_: 매도 임박/현재/예비 3구역, 상점 목록

**시장 테이프 진행**:
새 영업일 시작 시 비예약 카드가 왼쪽으로 한 칸 이동하고 가장 왼쪽 비예약 카드가 사라지는 처리.
_Avoid_: 시장 리롤, 전체 갱신

**시장 테이프 갱신**:
분기 시작 시 예약되지 않은 시장 슬롯을 새 카드로 교체하는 처리.
_Avoid_: 시장 테이프 진행, 슬롯 보충

**시장 테이프 당김**:
시장에 빈칸이 생겼을 때 오른쪽 비예약 카드들을 왼쪽으로 압축하는 처리.
_Avoid_: 산 자리 보충

**소모형 자원 카드**:
시장에 등장하며 매수 시 현금 또는 투자 철학을 지급하고 사라지는 카드.
포트폴리오에 들어가지 않는다.
_Avoid_: 중앙 은행, 자원 확보 액션

**Mr.Market 제안**:
분기 시작 시 시장 슬롯에 배치되는 공개 매수 조건.
별도의 시장 상황 시스템은 만들지 않고, Mr.Market 제안이 곧 시장 상황이다.
_Avoid_: 시장 뉴스, 별도 이벤트 레이어

**결제 완화형 제안**:
1~2번 슬롯에 배치되는 Mr.Market 제안.
결제 순간에만 적용되고 영구 흔적을 남기지 않는다.
_Avoid_: 할인 카드 효과

**성격 변환형 제안**:
3~5번 슬롯에 배치되는 Mr.Market 제안.
비용을 바꾸거나 영구 가치/배당 델타를 남길 수 있다.
_Avoid_: 카드 고유 효과

**템포형 제안**:
6~8번 슬롯에 배치되는 Mr.Market 제안.
추가 매수권 또는 투자 철학 보너스를 줄 수 있다.
_Avoid_: 무료 보너스 타일

**영구 제안 흔적**:
성격 변환형 제안으로 주식에 저장되는 가치 델타와 배당 델타.
호일 합성 시 재료 3장의 흔적을 합산한다.
_Avoid_: 상세 변경 로그

### 위험

**월세 밀림**:
분기 평가 목표 미달로 증가하며 한도에 도달하면 파산을 일으키는 누적 리스크 수치.
_Avoid_: 환매 압력, 스트레스, 리스크

**월세 밀림 단계**:
최종 운용 코멘트 산정을 위해 월세 밀림을 나눈 구간.
_Avoid_: 위험 등급, 리스크 단계

**운용 코멘트**:
최종 평가 등급과 월세 밀림 단계의 조합으로 선택되는 최종 정산 문구.
_Avoid_: 결과 코멘트, 평가 문구

## Relationships

- A **런** contains three **회계년도**.
- A **회계년도** contains one or more **분기**.
- A playable **분기** contains several **영업일**.
- 1회계년도와 2회계년도 playable **분기** contain 8 **영업일** each.
- 3회계년도 playable **분기** contain 10 **영업일** each.
- A **영업일** is consumed when **주식 매수**, **소모형 자원 카드 매수**, or **다음 영업일** is confirmed.
- **예약** and **주식 매도** do not consume a **영업일**.
- **분기 마감** happens after the last **영업일** of a **분기** ends.
- **분기 마감** includes mission settlement, **목표 달성률** calculation, **월세 밀림** handling, and the next schedule decision.
- A **주식 카드** can be a **시장 카드**, **예약 주식**, **보유 주식**, or removed from play.
- Every **주식 카드** has exactly one **산업 태그**.
- **산업 태그** is used by **미션**, settlement formulas, and portfolio direction, not by card-specific effects.
- A **시장 카드** can become a **예약 주식** through **예약**.
- A **시장 카드** or **예약 주식** can become a **보유 주식** through **주식 매수**.
- **주식 매수** records **매수 출처** as either market-origin or reservation-origin.
- Only **보유 주식** contributes to **최종 가치**, **배당금**, **미션 수익**, and **최종 정산**.
- **최종 가치** uses final stock value, including relevant **영구 제안 흔적**.
- **배당금** uses final stock dividend, including relevant **영구 제안 흔적**.
- **주식 매도** price ignores **영구 제안 흔적**.
- **미션 후보** are generated immediately at run start.
- A run can have exactly one **확정 미션**.
- The first **미션 후보** whose **클리어 조건** is satisfied becomes the **확정 미션**.
- If multiple **미션 후보** clear simultaneously, the leftmost candidate wins.
- **정산 공식** runs at every **분기 마감** after confirmation, including the same quarter if the mission was confirmed during that quarter.
- **미션 수익** is not cash and does not increase **현금** or **총 수익**.
- **목표 달성률** uses **현금 흐름 + 미션 수익**.
- **현금 흐름** is **배당금 + 주식 매도 현금**.
- **조달 현금** increases **현금** but does not count as **현금 흐름**.
- **딜** is not a payment wildcard.
- **딜** can be consumed to increase one **투자 철학 마스터리**.
- **투자 철학 마스터리** reduces matching **투자 철학** costs during **주식 매수**.
- **시장 테이프** has exactly eight **시장 슬롯**.
- **시장 테이프 진행** affects non-reserved market cards but does not move **예약 주식**.
- **시장 테이프 갱신** replaces non-reserved market cards but keeps **예약 주식**.
- **시장 테이프 당김** moves only non-reserved cards.
- **예약** locks a market stock in its market slot and does not grant **딜**, increase **월세 밀림**, advance the **시장 테이프**, or consume one **영업일**.
- Only one **예약 주식** can exist at a time.
- **Mr.Market 제안** is slot-bound, not card-bound.
- **Mr.Market 제안** applies only to **주식 카드**.
- A **소모형 자원 카드** on an offer slot leaves the offer inactive and unconsumed.
- A stock on an offer slot must be bought through the offer terms.
- Failed offer purchase attempts consume no cost, no offer, no **영업일**, and no **추가 매수권**.
- Unused **Mr.Market 제안** are discarded at **분기 마감**.
- **예약** preserves the card, not the **Mr.Market 제안**.
- A reserved stock uses the current quarter offer on its slot if one exists.
- **추가 매수권** can buy market stocks, reserved stocks, and consumable resource cards.
- **예약** during **추가 매수권** preserves the extra buy right.
- Pressing **다음 영업일** discards unused **추가 매수권**.
- **추가 매수권** can chain if a tempo offer grants another extra buy right.
- **월세 밀림** at 10 or higher causes immediate **파산**.
- **파산** does not proceed to **최종 정산**.
- **최종 정산** is reached only when the run is completed without **파산**.
- **최종 평가** is based on **최종 가치**, not **월세 밀림**.
- **운용 코멘트** uses both **최종 평가** and **월세 밀림 단계**.

## Example Dialogue

> **Dev:** "예약 주식도 최종 가치에 포함하나요?"
> **Domain expert:** "아니요. **예약 주식**은 아직 **보유 주식**이 아니므로 **최종 가치**, **배당금**, **미션 수익**에 포함하지 않습니다."
>
> **Dev:** "소모형 현금 카드로 얻은 현금도 분기 목표에 반영하나요?"
> **Domain expert:** "아니요. 그 현금은 **조달 현금**입니다. 같은 **현금** 잔고에 더해지지만 **현금 흐름**에는 포함하지 않습니다."
>
> **Dev:** "예약하면 딜을 받고 월세 밀림도 오르나요?"
> **Domain expert:** "아니요. v3 기준 **예약**은 무료 슬롯 고정 행동입니다. **딜**도 주지 않고 **월세 밀림**도 올리지 않으며 **영업일**도 소비하지 않습니다."
>
> **Dev:** "Mr.Market 제안 슬롯 위의 주식을 그냥 일반 매수해도 되나요?"
> **Domain expert:** "아니요. 제안 슬롯 위의 **주식 카드**는 반드시 그 **Mr.Market 제안** 조건으로만 매수합니다. 실패한 시도는 비용, 제안, 영업일, 추가 매수권을 소비하지 않습니다."

## Flagged Ambiguities

- "AUM" was used for the card score concept. Resolved: use **가치** and **최종 가치**.
- "자산" was used for portfolio cards. Resolved: use **주식** and **보유 주식**.
- "운용 수익" was used for cash performance. Resolved: use **현금 흐름** for quarter target cash contribution and **총 수익** for run-wide cash result.
- "전문 자원", "리서치", "신용", and "원자재" were used for cost resources. Resolved: use **투자 철학**, **독서**, **명상**, and **인내**.
- "환매 압력" was used for risk. Resolved: use **월세 밀림**.
- "런 실패" and "대규모 환매" were used for failure state/reason. Resolved: use **파산**.
- "딜" was previously described as a purchase-payment wildcard. Resolved: **딜** is not placed into **비용 슬롯** and does not reduce cash cost directly; it converts into **투자 철학 마스터리**.
- "예약 구역" was used for reserved cards. Resolved: **예약** locks a stock in its **시장 슬롯**; there is no separate reservation area.
- "예약" was previously risky and rewarded **딜**. Resolved: v3 **예약** is free and changes only the card's slot lock state.
- "예약 최대 3장" was used in older docs. Resolved: v3 allows only one **예약 주식** at a time.
- "중앙 은행" and "자원 확보" were used for resources. Resolved: resources come from **소모형 자원 카드** in the 1x8 market.
- "카드 상세보기" was used as a separate market state. Resolved: card information is shown through hover/quote UI without leaving **시장**.
- "매도 임박 / 현재 시장 / 예비 시장" were used as market zones. Resolved: use the single **1x8 시장 테이프** and numbered **시장 슬롯**.
- "분기 목표 수익" was used for cash-only target checks. Resolved: v3 uses **분기 평가 목표** checked by **현금 흐름 + 미션 수익**.
- "시장 상황" was proposed as a separate system. Resolved: **Mr.Market 제안** is the market situation.
- "추가 매수권은 중첩되지 않는다" was used in older docs. Resolved: v3 allows tempo chains; balance comes from offer frequency, placement, and values.
