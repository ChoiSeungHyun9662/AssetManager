# 19. 최종 정산과 평가

## 1. 목적

이 문서는 런 완료 후 표시되는 최종 정산 화면과 최종 평가 규칙을 정의한다.

최종 정산은 플레이어가 3회계연도 4Q까지 완료하고 파산하지 않았을 때 진입하는 결과 화면이다.

```text
3회계연도 4Q 마감
-> 월세 밀림 10 미만
-> 최종 정산
```

최종 정산의 목적은 다음과 같다.

```text
- 최종 가치 표시
- 최종 평가 등급 표시
- 총 수익 표시
- 총 미션 수익 표시
- 보유 주식 수 표시
- 월세 밀림 표시
- 운용 코멘트 표시
```

---

## 2. 진입 조건

최종 정산은 다음 조건을 모두 만족할 때 진입한다.

```text
- 3회계연도 4Q를 완료했다.
- 분기 마감 처리가 끝났다.
- 월세 밀림이 10 미만이다.
- 파산이 발생하지 않았다.
```

진입 흐름:

```text
3회계연도 4Q 마지막 영업일 종료
-> 분기 마감 정산
-> 분기 목표 달성률 계산
-> 월세 밀림 처리
-> 월세 밀림 10 미만
-> 최종 정산
```

월세 밀림이 10 이상이면 최종 정산으로 가지 않는다.

```text
월세 밀림 10 이상
-> 파산
-> 게임 오버
```

---

## 3. 화면 표시 정보

최종 정산 화면에는 다음 정보를 표시한다.

```text
- 최종 가치
- 최종 평가
- 총 수익
- 총 미션 수익
- 보유 주식 수
- 월세 밀림
- 운용 코멘트
```

예시:

```text
최종 정산

최종 가치: 180
최종 평가: A · 슈퍼 개미

총 수익: 92
총 미션 수익: 18
보유 주식: 7 / 8
월세 밀림: 7 / 10

운용 코멘트
높은 월세 밀림을 감수하면서도 강한 포트폴리오를 완성했습니다.
```

최종 화면은 긴 리포트가 아니라 결과 발표에 가깝게 구성한다.
회계연도별, 분기별 상세 리포트는 MVP 최종 정산 화면에서는 생략한다.

---

## 4. 최종 가치

최종 가치는 보유 주식 가치의 합계다.

```text
최종 가치 = 보유 주식의 가치 합계
```

최종 가치에 포함되는 것:

```text
- 보유 중인 일반 주식의 최종 가치
- 보유 중인 호일 주식의 최종 가치
```

최종 가치는 Mr.Market 영구 가치 델타를 반영한다.
호일 주식은 재료 3장의 Mr.Market 영구 가치 델타를 합산한 뒤 0 미만으로 내려가지 않게 계산한다.

최종 가치에 포함되지 않는 것:

```text
- 현금
- 독서
- 명상
- 인내
- 딜
- 총 수익
- 시장 카드
- 예약된 주식
```

현금이 많아도 최종 가치에는 포함하지 않는다.
평가는 플레이어가 끝까지 들고 간 주식 포트폴리오의 힘을 기준으로 한다.

예시:

```text
보유 주식:
성장 기술주 가치 5
국밥 ETF 가치 3
호일 배당 귀족주 가치 9

최종 가치 = 17
```

---

## 5. 최종 평가

최종 평가는 최종 가치 기준으로 결정한다.

```text
최종 평가
= 최종 가치 기준
```

월세 밀림은 최종 평가 등급을 낮추지 않는다.

```text
월세 밀림 높음
-> 최종 등급 하락 없음
```

월세 밀림은 별도의 운용 코멘트에 사용한다.

---

## 6. 최종 평가 등급 구조

최종 평가는 `등급 문자 + 칭호` 조합의 5단계로 구성한다.

```text
D · 생존형 투자자
C · 신중한 투자자
B · 유능한 포트폴리오 매니저
A · 슈퍼 개미
S · 전설적인 투자자
```

기준값은 데이터 테이블 조정 대상으로 둔다.

초기 테스트용 기준값:

| 등급 | 칭호 | 임시 최종 가치 기준 |
|---|---|---:|
| D | 생존형 투자자 | 0+ |
| C | 신중한 투자자 | 50+ |
| B | 유능한 포트폴리오 매니저 | 100+ |
| A | 슈퍼 개미 | 150+ |
| S | 전설적인 투자자 | 200+ |

수치는 최종 밸런스 값이 아니라 초기 테스트용 기본값이다.

---

## 7. 최종 평가 판정 방식

최종 가치가 각 등급의 최소 기준값 이상이면 해당 등급 후보가 된다.
그중 가장 높은 등급을 최종 평가로 사용한다.

예시:

```text
최종 가치 137
-> B · 유능한 포트폴리오 매니저

최종 가치 180
-> A · 슈퍼 개미

최종 가치 211
-> S · 전설적인 투자자
```

판정 방식:

```text
FinalRatingTable에서
MinFinalValue <= FinalValue인 항목 중
MinFinalValue가 가장 높은 항목 선택
```

---

## 8. 총 수익

총 수익은 전체 런 동안 수익으로 분류된 현금의 합계다.

포함:

```text
- 배당금
- 주식 매도 수익
- 그 밖에 수익으로 명시된 현금
```

제외:

```text
- 소모형 자원 카드로 획득한 현금
- 시작 현금
- 단순 환급 또는 비용 취소
```

총 수익 계산:

```text
총 수익
= 1회계연도 수익
+ 2회계연도 수익
+ 3회계연도 수익
```

총 수익은 최종 등급 판정 기준이 아니다.
플레이어가 런 동안 얼마나 많은 현금 흐름을 만들었는지 보여주는 보조 지표다.
미션 수익은 현금이 아니므로 총 수익에 포함하지 않고, 필요하면 별도 총 미션 수익으로 표시한다.

---

## 9. 보유 주식 수

보유 주식 수는 최종 시점에 포트폴리오 슬롯을 점유한 주식 수다.

```text
보유 주식 수 = 포트폴리오에서 점유된 슬롯 수
```

일반 주식 1장은 1칸을 차지한다.
호일 주식 1장은 1칸을 차지한다.

표시 예시:

```text
보유 주식: 7 / 8
```

예약된 주식과 시장 카드는 보유 주식 수에 포함하지 않는다.

---

## 10. 월세 밀림 표시

최종 정산 화면에는 최종 월세 밀림을 표시한다.

예시:

```text
월세 밀림: 7 / 10
```

월세 밀림이 10 이상이면 이미 파산이 발생했으므로 최종 정산에 진입할 수 없다.

```text
월세 밀림 10 이상
-> 파산
-> 최종 정산 없음
```

---

## 11. 운용 코멘트

운용 코멘트는 최종 등급과 월세 밀림 단계를 함께 기준으로 결정한다.

```text
운용 코멘트 = 최종 평가 등급 x 월세 밀림 단계
```

최종 평가 등급:

```text
D / C / B / A / S
```

월세 밀림 단계:

```text
Low / Medium / High
```

총 15개의 코멘트를 준비할 수 있다.

```text
5등급 x 3단계 = 15개 코멘트
```

운용 코멘트는 최종 등급을 바꾸지 않는다.
플레이어가 어떤 리스크 성향으로 투자했는지 해석해주는 역할이다.

---

## 12. 월세 밀림 단계

월세 밀림 단계는 데이터 테이블 조정 대상으로 둔다.

초기 테스트용 값:

```text
낮음: 0~2
중간: 3~5
높음: 6+
```

구현 enum 예시:

```csharp
public enum RentArrearsLevel
{
    Low,
    Medium,
    High
}
```

단계 데이터 예시:

```text
Low
min_rent_arrears: 0

Medium
min_rent_arrears: 3

High
min_rent_arrears: 6
```

---

## 13. 운용 코멘트 예시

### 13.1 S 등급

```text
S + Low
-> 압도적인 가치와 안정성을 모두 달성한 투자입니다.

S + Medium
-> 적절한 리스크를 감수해 전설적인 성과를 냈습니다.

S + High
-> 월세가 턱밑까지 왔지만, 결과만큼은 전설적입니다.
```

### 13.2 A 등급

```text
A + Low
-> 안정적으로 강한 포트폴리오를 완성했습니다.

A + Medium
-> 균형 잡힌 리스크 관리로 뛰어난 성과를 냈습니다.

A + High
-> 아슬아슬한 생활감 속에서도 슈퍼 개미의 체급을 증명했습니다.
```

### 13.3 B 등급

```text
B + Low
-> 안정적인 관리 속에 준수한 성과를 냈습니다.

B + Medium
-> 리스크와 성장 추구가 균형을 이룬 투자였습니다.

B + High
-> 월세 독촉을 견디며 의미 있는 포트폴리오를 만들었습니다.
```

### 13.4 C 등급

```text
C + Low
-> 조심스럽게 버텼지만 성장 동력은 부족했습니다.

C + Medium
-> 성과와 리스크 모두 제한적인 보수적 투자였습니다.

C + High
-> 생활 압박은 커졌지만 성과가 충분히 따라오지는 못했습니다.
```

### 13.5 D 등급

```text
D + Low
-> 잃지는 않았지만 큰 기회도 만들지 못했습니다.

D + Medium
-> 일정한 리스크를 감수했지만 성과가 부족했습니다.

D + High
-> 월세도 밀리고 성과도 부족한 위태로운 투자였습니다.
```

문구는 최종 톤 조정 단계에서 변경할 수 있다.
구조는 `등급 x 월세 밀림 단계`로 유지한다.

---

## 14. 최종 정산에서 표시하지 않는 것

최종 정산 화면은 전체 런 결과를 간단히 보여주는 화면이다.

표시하지 않는 것:

```text
- 회계연도별 수익 상세
- 분기별 수익 상세
- 각 분기별 마감 시나리오 목록
- 카드별 수익 기여도
- 전체 주식 목록 상세
```

이 정보가 필요하다면 별도 상세 리포트 화면에서 우선순위로 제공할 수 있다.
MVP 최종 정산 화면에서는 생략한다.

---

## 15. 처리 순서

최종 정산 진입 후 처리 순서는 다음과 같다.

```text
1. 최종 가치 계산
2. 최종 평가 등급 결정
3. 총 수익 조회
4. 총 미션 수익 조회
5. 보유 주식 수 조회
6. 최종 월세 밀림 조회
7. 월세 밀림 단계 결정
8. 운용 코멘트 결정
9. 최종 정산 UI 표시
```

---

## 16. 상태 데이터 예시

최종 정산 화면 데이터:

```csharp
public class FinalRunSummaryData
{
    public int FinalValue;
    public FinalRatingData FinalRating;

    public int TotalRevenue;
    public int TotalMissionRevenue;
    public int OwnedStockSlotCount;
    public int MaxStockSlotCount;
    public int RentArrears;

    public RentArrearsLevel RentArrearsLevel;
    public FinalManagementCommentData ManagementComment;
}
```

최종 평가 데이터:

```csharp
public class FinalRatingData
{
    public string Id;
    public string Grade;
    public string Title;
    public int MinFinalValue;
}
```

운용 코멘트 데이터:

```csharp
public class FinalManagementCommentData
{
    public string Id;

    public string FinalRatingGrade;
    public RentArrearsLevel RentArrearsLevel;

    public string Comment;
}
```

---

## 17. 처리 예시

```csharp
int CalculateFinalValue()
{
    return Portfolio.Stocks.Sum(stock =>
        stock.CurrentValue
    );
}
```

```csharp
FinalRatingData GetFinalRating(int finalValue)
{
    return FinalRatingTable
        .Where(rating =>
            finalValue >= rating.MinFinalValue
        )
        .OrderByDescending(rating =>
            rating.MinFinalValue
        )
        .First();
}
```

```csharp
FinalRunSummaryData BuildFinalRunSummary()
{
    int finalValue = CalculateFinalValue();

    FinalRatingData finalRating =
        GetFinalRating(finalValue);

    RentArrearsLevel rentArrearsLevel =
        GetRentArrearsLevel(CurrentRentArrears);

    FinalManagementCommentData comment =
        GetManagementComment(
            finalRating.Grade,
            rentArrearsLevel
        );

    return new FinalRunSummaryData
    {
        FinalValue = finalValue,
        FinalRating = finalRating,

        TotalRevenue = TotalRevenue,
        TotalMissionRevenue = TotalMissionRevenue,
        OwnedStockSlotCount = Portfolio.OccupiedSlotCount,
        MaxStockSlotCount = Portfolio.MaxSlotCount,
        RentArrears = CurrentRentArrears,

        RentArrearsLevel = rentArrearsLevel,
        ManagementComment = comment
    };
}
```

```csharp
void ShowFinalSettlement()
{
    CurrentRunState = RunState.Completed;

    FinalRunSummaryData summary = BuildFinalRunSummary();

    FinalSettlementView.Show(summary);
}
```

---

## 18. 파산 화면과의 차이

월세 밀림으로 파산이 발생하면 최종 정산 화면으로 가지 않는다.

```text
월세 밀림 10 이상
-> 파산 화면
```

파산 화면은 별도 화면이다.
파산 화면에서는 최종 평가 등급을 표시하지 않는다.

파산 화면에서 표시할 수 있는 정보:

```text
- 실패 사유: 월세 밀림
- 도달 회계연도 / 분기
- 현재 가치
- 총 수익
- 보유 주식 수
- 월세 밀림
```

---

## 19. 구현 시 주의사항

```text
- 최종 정산은 3회계연도 4Q 완료 뒤에만 진입한다.
- 월세 밀림이 10 이상이면 최종 정산으로 가지 않는다.
- 최종 가치는 보유 주식 가치의 합계다.
- 최종 가치는 Mr.Market 영구 가치 델타를 반영한다.
- 현금은 최종 가치에 포함하지 않는다.
- 예약된 주식은 최종 가치에 포함하지 않는다.
- 최종 평가는 최종 가치 기준이다.
- 월세 밀림은 최종 등급을 낮추지 않는다.
- 월세 밀림은 운용 코멘트 결정에 사용한다.
- 총 수익에는 소모형 자원 카드로 얻은 현금을 포함하지 않는다.
- 총 수익에는 미션 수익을 포함하지 않는다.
- 등급 기준값과 코멘트 문구는 데이터 테이블로 조정 가능하게 둔다.
```
