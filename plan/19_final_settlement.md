# 19. 최종 정산과 평가

## 1. 목적

이 문서는 런 완료 시 표시되는 최종 정산 화면과 최종 평가 규칙을 정의한다.

최종 정산은 플레이어가 3회계년도 4Q까지 완료하고, 환매 압력으로 인한 런 실패가 발생하지 않았을 때 진입하는 결과 화면이다.

```text
3회계년도 4Q 마감
→ 환매 압력 실패 없음
→ 최종 정산
```

최종 정산의 목적은 다음과 같다.

```text
- 최종 운용가치 표시
- 최종 평가 등급 표시
- 총 운용 수익 표시
- 보유 자산 수 표시
- 환매 압력 표시
- 운용 코멘트 표시
```

---

## 2. 최종 정산 진입 조건

최종 정산은 다음 조건을 만족할 때 진입한다.

```text
- 3회계년도 4Q를 완료했다.
- 분기 마감 처리가 끝났다.
- 환매 압력이 10 미만이다.
- 런 실패가 발생하지 않았다.
```

진입 흐름:

```text
3회계년도 4Q 마지막 영업일 종료
→ 분기 마감 정산
→ 분기 목표 달성률 계산
→ 환매 압력 처리
→ 환매 압력 10 미만
→ 최종 정산
```

환매 압력이 10 이상이면 최종 정산으로 가지 않는다.

```text
환매 압력 10 이상
→ 대규모 환매 발생
→ 런 실패
```

---

## 3. 최종 정산 화면 표시 정보

최종 정산 화면에는 다음 정보를 표시한다.

```text
- 최종 운용가치
- 최종 평가
- 총 운용 수익
- 보유 자산 수
- 환매 압력
- 운용 코멘트
```

예시:

```text
최종 정산

최종 운용가치: 180
최종 평가: A · 스타 펀드매니저

총 운용 수익: 92
보유 자산: 24장
환매 압력: 7 / 10

운용 코멘트:
높은 환매 압력을 감수하며 스타급 성과를 낸 공격적인 운용입니다.
```

최종 화면은 긴 리포트가 아니라 결과 발표에 가깝게 구성한다.
회계년도별/분기별 상세 리포트는 최종 정산 화면에서 표시하지 않는다.

---

## 4. 최종 운용가치

최종 운용가치는 보유 자산 카드들의 운용가치 합계이다.

```text
최종 운용가치
= 보유 자산 카드들의 운용가치 합계
```

운용가치는 자산 카드가 가진 고유 점수이다.
스플렌더의 승점과 같은 개념이다.

최종 운용가치에 포함하는 것:

```text
- 보유 자산 카드의 운용가치
```

포함하지 않는 것:

```text
- 현금
- 리서치
- 신용
- 원자재
- 딜
- 예약 카드
- 시장 카드
- 운용 수익
```

예시:

```text
보유 자산:
성장 기술주 운용가치 5
국채 ETF 운용가치 3
원자재 펀드 운용가치 4

최종 운용가치 = 12
```

현재 현금이 많아도 최종 운용가치에는 포함하지 않는다.

---

## 5. 최종 평가

최종 평가는 최종 운용가치 기준으로 산정한다.

```text
최종 평가
= 최종 운용가치 기준
```

환매 압력은 최종 평가 등급을 낮추지 않는다.

```text
환매 압력 높음
→ 최종 등급 하락 없음
```

환매 압력은 별도의 운용 코멘트에 사용한다.

---

## 6. 최종 평가 등급 구조

최종 평가는 `등급 문자 + 칭호` 조합의 5단계로 구성한다.

```text
D · 생존한 운용자
C · 신중한 운용자
B · 유능한 펀드매니저
A · 스타 펀드매니저
S · 전설적인 펀드매니저
```

기준값은 테이블 조정형으로 둔다.

초기 임시 기준값:

| 등급 | 칭호 | 임시 운용가치 기준 |
|---|---|---:|
| D | 생존한 운용자 | 0+ |
| C | 신중한 운용자 | 50+ |
| B | 유능한 펀드매니저 | 100+ |
| A | 스타 펀드매니저 | 150+ |
| S | 전설적인 펀드매니저 | 200+ |

이 수치는 최종 밸런스 값이 아니라 초기 테스트용 기본값이다.

---

## 7. 최종 평가 판정 방식

최종 운용가치가 각 등급의 최소 기준값 이상이면 해당 등급 후보가 된다.
그중 가장 높은 등급을 최종 평가로 사용한다.

예시:

```text
최종 운용가치 137
→ B · 유능한 펀드매니저

최종 운용가치 180
→ A · 스타 펀드매니저

최종 운용가치 211
→ S · 전설적인 펀드매니저
```

판정 방식:

```text
FinalRatingTable에서
MinManagementValue <= FinalManagementValue인 항목 중
MinManagementValue가 가장 높은 항목 선택
```

---

## 8. 총 운용 수익

총 운용 수익은 전체 런 동안 발생한 운용 성과 현금 합계이다.

포함:

```text
- 보유 자산의 영업일 시작 현금
- 분기 마감 정산 수익
- 기타 운용 성과로 분류된 현금
```

제외:

```text
- 자원 확보로 얻은 현금
```

자원 확보로 얻은 현금은 조달 현금이므로 총 운용 수익에 포함하지 않는다.

```text
조달 현금
= 조달 현금
≠ 운용 수익
```

총 운용 수익 계산:

```text
총 운용 수익
= 1회계년도 운용 수익
+ 2회계년도 운용 수익
+ 3회계년도 운용 수익
```

---

## 9. 보유 자산 수

보유 자산 수는 매수 완료된 자산 카드 수이다.

```text
보유 자산 수
= Owned 상태의 자산 카드 수
```

포함:

```text
- 시장에서 매수한 자산
- 예약 카드에서 매수한 자산
```

제외:

```text
- 예약 구역에 아직 남아 있는 카드
- 시장 테이프 카드
- 제거된 카드
```

---

## 10. 환매 압력 표시

최종 정산 화면에는 최종 환매 압력을 표시한다.

예시:

```text
환매 압력: 7 / 10
```

환매 압력이 10 이상이면 이미 런 실패가 발생했어야 하므로 최종 정산에 진입할 수 없다.

```text
환매 압력 10 이상
→ 런 실패
→ 최종 정산 없음
```

---

## 11. 운용 코멘트

운용 코멘트는 최종 등급과 환매 압력 단계를 함께 기준으로 결정한다.

```text
운용 코멘트
= 최종 평가 등급 × 환매 압력 단계
```

최종 평가 등급:

```text
D / C / B / A / S
```

환매 압력 단계:

```text
Low / Medium / High
```

총 15개 코멘트를 준비할 수 있다.

```text
5등급 × 3단계 = 15개 코멘트
```

운용 코멘트는 최종 등급을 바꾸지 않는다.
플레이어가 어떤 리스크 성향으로 운용했는지 해석해주는 역할이다.

---

## 12. 환매 압력 단계

환매 압력 단계는 테이블 조정형으로 둔다.

초기 임시값:

```text
낮음: 0~2
중간: 3~5
높음: 6+
```

구현 enum 예시:

```csharp
public enum RedemptionPressureLevel
{
    Low,
    Medium,
    High
}
```

단계 데이터 예시:

```text
Low
min_redemption_pressure: 0

Medium
min_redemption_pressure: 3

High
min_redemption_pressure: 6
```

---

## 13. 운용 코멘트 예시

### 13.1 S 등급

```text
S + Low
→ 압도적인 성과와 안정성을 모두 달성한 운용입니다.

S + Medium
→ 적절한 리스크를 감수해 전설적인 성과를 낸 운용입니다.

S + High
→ 높은 환매 압력을 감수하고도 전설적인 성과를 낸 공격적 운용입니다.
```

---

### 13.2 A 등급

```text
A + Low
→ 안정적으로 큰 성과를 낸 우수한 운용입니다.

A + Medium
→ 균형 잡힌 리스크 관리로 스타급 성과를 달성한 운용입니다.

A + High
→ 높은 환매 압력을 감수하며 스타급 성과를 낸 공격적인 운용입니다.
```

---

### 13.3 B 등급

```text
B + Low
→ 안정적인 관리 속에서 준수한 성과를 낸 운용입니다.

B + Medium
→ 적절한 리스크와 성장 추구가 균형을 이룬 운용입니다.

B + High
→ 리스크를 적극적으로 감수해 유의미한 성과를 만든 운용입니다.
```

---

### 13.4 C 등급

```text
C + Low
→ 신중했지만 성장 동력이 부족했던 운용입니다.

C + Medium
→ 리스크와 성과 모두 제한적인 보수적 운용입니다.

C + High
→ 리스크는 컸지만 성과로 충분히 이어지지 못한 운용입니다.
```

---

### 13.5 D 등급

```text
D + Low
→ 손실은 억제했지만 성장 기회를 살리지 못한 운용입니다.

D + Medium
→ 일정한 리스크를 감수했지만 성과가 부족했던 운용입니다.

D + High
→ 높은 환매 압력에도 성과가 부족했던 위태로운 운용입니다.
```

문구는 최종 톤 조정 단계에서 변경할 수 있다.
구조는 `등급 × 환매 압력 단계`로 유지한다.

---

## 14. 최종 정산에서 표시하지 않는 것

최종 정산 화면은 전체 런 결과를 짧게 보여주는 화면이다.

표시하지 않는 것:

```text
- 회계년도별 운용 수익 상세
- 분기별 운용 수익 상세
- 각 분기별 마감 시나리오 목록
- 카드별 수익 기여도
- 전체 자산 목록 상세
```

이 정보들은 필요하다면 별도 상세 리포트 화면에서 후순위로 제공할 수 있다.
MVP 최종 정산 화면에서는 생략한다.

---

## 15. 최종 정산 처리 순서

최종 정산 진입 시 처리 순서는 다음과 같다.

```text
1. 최종 운용가치 계산
2. 최종 평가 등급 산정
3. 총 운용 수익 조회
4. 보유 자산 수 조회
5. 최종 환매 압력 조회
6. 환매 압력 단계 산정
7. 운용 코멘트 산정
8. 최종 정산 UI 표시
```

---

## 16. FinalRunSummaryData

최종 정산 화면 데이터 예시:

```csharp
public class FinalRunSummaryData
{
    public int FinalManagementValue;
    public FinalRatingData FinalRating;

    public int TotalEarnedCash;
    public int OwnedAssetCount;
    public int RedemptionPressure;

    public RedemptionPressureLevel RedemptionPressureLevel;
    public FinalManagementCommentData ManagementComment;
}
```

---

## 17. FinalRatingData

최종 평가 데이터 예시:

```csharp
public class FinalRatingData
{
    public string Id;
    public string Grade;
    public string Title;
    public int MinManagementValue;
}
```

예시 데이터:

```text
id: rating_d
grade: D
title: 생존한 운용자
min_management_value: 0

id: rating_c
grade: C
title: 신중한 운용자
min_management_value: 50

id: rating_b
grade: B
title: 유능한 펀드매니저
min_management_value: 100

id: rating_a
grade: A
title: 스타 펀드매니저
min_management_value: 150

id: rating_s
grade: S
title: 전설적인 펀드매니저
min_management_value: 200
```

---

## 18. FinalManagementCommentData

운용 코멘트 데이터 예시:

```csharp
public class FinalManagementCommentData
{
    public string Id;

    public string FinalRatingGrade;
    public RedemptionPressureLevel RedemptionPressureLevel;

    public string Comment;
}
```

예시 데이터:

```text
id: comment_a_high
final_rating_grade: A
redemption_pressure_level: High
comment: 높은 환매 압력을 감수하며 스타급 성과를 낸 공격적인 운용입니다.
```

---

## 19. 최종 운용가치 계산 함수

```csharp
int CalculateFinalManagementValue()
{
    return OwnedAssets.Sum(asset =>
        asset.BaseData.ManagementValue
    );
}
```

보유 자산만 계산 대상이다.

```text
OwnedAssets만 포함
ReservedAssets 제외
MarketCards 제외
```

---

## 20. 최종 평가 판정 함수

```csharp
FinalRatingData GetFinalRating(int finalManagementValue)
{
    return FinalRatingTable
        .Where(rating =>
            finalManagementValue >= rating.MinManagementValue
        )
        .OrderByDescending(rating =>
            rating.MinManagementValue
        )
        .First();
}
```

---

## 21. 환매 압력 단계 판정 함수

```csharp
RedemptionPressureLevel GetRedemptionPressureLevel(
    int redemptionPressure
)
{
    return RedemptionPressureLevelTable
        .Where(data =>
            redemptionPressure >= data.MinRedemptionPressure
        )
        .OrderByDescending(data =>
            data.MinRedemptionPressure
        )
        .First()
        .Level;
}
```

---

## 22. 운용 코멘트 판정 함수

```csharp
FinalManagementCommentData GetManagementComment(
    string finalRatingGrade,
    RedemptionPressureLevel pressureLevel
)
{
    return FinalManagementCommentTable
        .First(comment =>
            comment.FinalRatingGrade == finalRatingGrade
            && comment.RedemptionPressureLevel == pressureLevel
        );
}
```

---

## 23. 최종 정산 데이터 생성 함수

```csharp
FinalRunSummaryData BuildFinalRunSummary()
{
    int finalManagementValue = CalculateFinalManagementValue();

    FinalRatingData finalRating =
        GetFinalRating(finalManagementValue);

    RedemptionPressureLevel pressureLevel =
        GetRedemptionPressureLevel(CurrentRedemptionPressure);

    FinalManagementCommentData comment =
        GetManagementComment(
            finalRating.Grade,
            pressureLevel
        );

    return new FinalRunSummaryData
    {
        FinalManagementValue = finalManagementValue,
        FinalRating = finalRating,

        TotalEarnedCash = TotalEarnedCash,
        OwnedAssetCount = OwnedAssets.Count,
        RedemptionPressure = CurrentRedemptionPressure,

        RedemptionPressureLevel = pressureLevel,
        ManagementComment = comment
    };
}
```

---

## 24. 최종 정산 화면 표시 예시

```csharp
void ShowFinalSettlement()
{
    CurrentRunState = RunState.Completed;

    FinalRunSummaryData summary = BuildFinalRunSummary();

    FinalSettlementView.Show(summary);
}
```

UI 예시:

```text
최종 정산

최종 운용가치: 180
최종 평가: A · 스타 펀드매니저

총 운용 수익: 92
보유 자산: 24장
환매 압력: 7 / 10

운용 코멘트:
높은 환매 압력을 감수하며 스타급 성과를 낸 공격적인 운용입니다.
```

---

## 25. 실패 화면과의 차이

환매 압력으로 런 실패가 발생하면 최종 정산 화면으로 가지 않는다.

```text
환매 압력 10 이상
→ 런 실패 화면
```

실패 화면은 별도 화면이다.
실패 화면에서는 최종 평가 등급을 표시하지 않는다.

실패 화면에 표시할 수 있는 정보:

```text
- 실패 사유: 대규모 환매 발생
- 도달 회계년도 / 분기
- 현재 운용가치
- 총 운용 수익
- 보유 자산 수
- 환매 압력
```

---

## 26. 구현 시 주의사항

```text
- 최종 정산은 3회계년도 4Q 완료 후에만 진입한다.
- 환매 압력 10 이상이면 최종 정산으로 가지 않는다.
- 최종 운용가치는 보유 자산 카드의 운용가치 합계이다.
- 현금은 최종 운용가치에 포함하지 않는다.
- 예약 카드는 최종 운용가치에 포함하지 않는다.
- 최종 평가는 운용가치 기준이다.
- 환매 압력은 최종 등급을 낮추지 않는다.
- 환매 압력은 운용 코멘트 산정에 사용한다.
- 총 운용 수익에는 조달 현금을 포함하지 않는다.
- 최종 정산 화면에서는 회계년도별/분기별 상세 리포트를 표시하지 않는다.
- 등급 기준값과 운용 코멘트 문구는 데이터 테이블로 조정 가능하게 둔다.
