using System;
using System.Collections.Generic;

namespace AssetManager
{
    public sealed class MissionCandidateActionResult
    {
        public MissionCandidateActionResult(RunSessionState run, bool succeeded, string message)
        {
            Run = run;
            Succeeded = succeeded;
            Message = message ?? string.Empty;
        }

        public RunSessionState Run { get; }
        public bool Succeeded { get; }
        public string Message { get; }
    }

    public static class MissionCandidateAction
    {
        private const int CandidateSlotCount = 3;

        public static MissionRunState CreateInitialState(IReadOnlyList<MissionDefinitionData> missionPool)
        {
            if (missionPool == null)
            {
                throw new ArgumentNullException(nameof(missionPool));
            }

            if (missionPool.Count < CandidateSlotCount)
            {
                throw new InvalidOperationException("Mission pool requires at least three missions.");
            }

            var slots = new List<MissionCandidateSlotState>();
            for (var i = 0; i < CandidateSlotCount; i++)
            {
                slots.Add(new MissionCandidateSlotState(missionPool[i], false));
            }

            return new MissionRunState(slots, CandidateSlotCount);
        }

        public static MissionCandidateActionResult Mulligan(RunSessionState run, int slotIndex)
        {
            if (run != null && run.Missions.HasConfirmedMission)
            {
                return new MissionCandidateActionResult(run, false, "Mission already confirmed.");
            }

            if (!TryGetSlot(run, slotIndex, out var slot))
            {
                return new MissionCandidateActionResult(run, false, "미션 후보 슬롯이 없습니다.");
            }

            if (slot.IsEmpty || slot.HasSpentMulligan)
            {
                return new MissionCandidateActionResult(run, false, "이미 멀리건한 미션 후보입니다.");
            }

            var replacement = DrawReplacementMission(run, slot.Candidate);
            var updatedSlots = ReplaceSlot(
                run.Missions.CandidateSlots,
                slotIndex,
                new MissionCandidateSlotState(replacement, true));
            var updatedRun = WithMissions(
                run,
                new MissionRunState(updatedSlots, AdvanceDrawIndex(run.StaticData.Missions, run.Missions.NextMissionDrawIndex)));

            return new MissionCandidateActionResult(updatedRun, true, string.Empty);
        }

        public static MissionCandidateActionResult Discard(RunSessionState run, int slotIndex)
        {
            if (run != null && run.Missions.HasConfirmedMission)
            {
                return new MissionCandidateActionResult(run, false, "Mission already confirmed.");
            }

            if (!TryGetSlot(run, slotIndex, out var slot))
            {
                return new MissionCandidateActionResult(run, false, "미션 후보 슬롯이 없습니다.");
            }

            if (slot.IsEmpty || !slot.HasSpentMulligan)
            {
                return new MissionCandidateActionResult(run, false, "멀리건 후 폐기할 수 있습니다.");
            }

            var updatedSlots = ReplaceSlot(
                run.Missions.CandidateSlots,
                slotIndex,
                new MissionCandidateSlotState(null, true));
            var updatedRun = WithMissions(run, new MissionRunState(updatedSlots, run.Missions.NextMissionDrawIndex));

            return new MissionCandidateActionResult(updatedRun, true, string.Empty);
        }

        private static bool TryGetSlot(RunSessionState run, int slotIndex, out MissionCandidateSlotState slot)
        {
            slot = null;
            if (run == null
                || run.Missions == null
                || slotIndex < 0
                || slotIndex >= run.Missions.CandidateSlots.Count)
            {
                return false;
            }

            slot = run.Missions.CandidateSlots[slotIndex];
            return true;
        }

        private static MissionDefinitionData DrawReplacementMission(RunSessionState run, MissionDefinitionData rejectedMission)
        {
            var pool = run.StaticData.Missions;
            var drawIndex = run.Missions.NextMissionDrawIndex;
            for (var attempts = 0; attempts < pool.Count; attempts++)
            {
                var candidate = pool[(drawIndex + attempts) % pool.Count];
                if (!IsVisibleCandidate(run.Missions.CandidateSlots, candidate)
                    && candidate.Id != rejectedMission.Id)
                {
                    return candidate;
                }
            }

            return pool[drawIndex % pool.Count];
        }

        private static int AdvanceDrawIndex(IReadOnlyList<MissionDefinitionData> missionPool, int currentIndex)
        {
            if (missionPool.Count == 0)
            {
                return currentIndex;
            }

            return (currentIndex + 1) % missionPool.Count;
        }

        private static bool IsVisibleCandidate(
            IEnumerable<MissionCandidateSlotState> slots,
            MissionDefinitionData candidate)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.Candidate.Id == candidate.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private static IReadOnlyList<MissionCandidateSlotState> ReplaceSlot(
            IReadOnlyList<MissionCandidateSlotState> slots,
            int slotIndex,
            MissionCandidateSlotState replacement)
        {
            var updatedSlots = new List<MissionCandidateSlotState>(slots);
            updatedSlots[slotIndex] = replacement;
            return updatedSlots;
        }

        private static RunSessionState WithMissions(RunSessionState run, MissionRunState missions)
        {
            return new RunSessionState(
                run.State,
                run.StaticData,
                run.Calendar,
                run.Resources,
                run.Performance,
                run.AssetCards,
                run.MarketTape,
                run.Reservation,
                run.OwnedAssets,
                run.BusinessDay,
                run.RedemptionPressure,
                run.CardDetail,
                run.LiquidityAction,
                run.QuarterEndResult,
                run.FailureReason,
                run.InvestmentPhilosophyMastery,
                run.DealRewards,
                missions);
        }
    }
}
