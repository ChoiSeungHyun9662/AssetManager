using System;
using System.Collections.Generic;

namespace AssetManager
{
    public sealed class MissionConfirmationActionResult
    {
        public MissionConfirmationActionResult(RunSessionState run, bool confirmed)
        {
            Run = run ?? throw new ArgumentNullException(nameof(run));
            Confirmed = confirmed;
        }

        public RunSessionState Run { get; }
        public bool Confirmed { get; }
    }

    public static class MissionConfirmationAction
    {
        public static MissionConfirmationActionResult Evaluate(RunSessionState run)
        {
            if (run == null)
            {
                throw new ArgumentNullException(nameof(run));
            }

            if (run.Missions == null || run.Missions.HasConfirmedMission)
            {
                return new MissionConfirmationActionResult(run, false);
            }

            for (var i = 0; i < run.Missions.CandidateSlots.Count; i++)
            {
                var slot = run.Missions.CandidateSlots[i];
                if (slot.IsEmpty || !IsClear(slot.Candidate, run.OwnedAssets))
                {
                    continue;
                }

                return new MissionConfirmationActionResult(Confirm(run, i, slot.Candidate), true);
            }

            return new MissionConfirmationActionResult(run, false);
        }

        private static bool IsClear(MissionDefinitionData mission, OwnedAssetState ownedAssets)
        {
            if (mission == null || ownedAssets == null)
            {
                return false;
            }

            switch (mission.Template)
            {
                case MissionTemplate.FastEntry:
                case MissionTemplate.Concentration:
                    return CountOwnedTargetTagStocks(ownedAssets, mission.TargetTags) >= mission.ClearTargetCount;
                case MissionTemplate.Foil:
                    return CountOwnedTargetTagFoils(ownedAssets, mission.TargetTags) >= mission.ClearTargetCount;
                case MissionTemplate.HighValue:
                    return HasOwnedTargetTagStockAtValue(ownedAssets, mission.TargetTags, mission.ClearTargetCount);
                case MissionTemplate.TwoTagStable:
                    return HasEachTargetTagCount(ownedAssets, mission.TargetTags, mission.ClearTargetCount);
                default:
                    return false;
            }
        }

        private static RunSessionState Confirm(
            RunSessionState run,
            int confirmedSlotIndex,
            MissionDefinitionData confirmedMission)
        {
            var updatedSlots = new List<MissionCandidateSlotState>();
            for (var i = 0; i < run.Missions.CandidateSlots.Count; i++)
            {
                var slot = run.Missions.CandidateSlots[i];
                updatedSlots.Add(i == confirmedSlotIndex
                    ? new MissionCandidateSlotState(slot.Candidate, slot.HasSpentMulligan)
                    : new MissionCandidateSlotState(null, true));
            }

            return WithMissions(
                run,
                new MissionRunState(updatedSlots, run.Missions.NextMissionDrawIndex, confirmedMission));
        }

        private static int CountOwnedTargetTagStocks(OwnedAssetState ownedAssets, IReadOnlyList<TagData> targetTags)
        {
            var count = 0;
            foreach (var card in ownedAssets.OwnedCards)
            {
                if (HasAnyTargetTag(card, targetTags))
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountOwnedTargetTagFoils(OwnedAssetState ownedAssets, IReadOnlyList<TagData> targetTags)
        {
            var count = 0;
            foreach (var card in ownedAssets.OwnedCards)
            {
                if (card.IsFoil && HasAnyTargetTag(card, targetTags))
                {
                    count++;
                }
            }

            return count;
        }

        private static bool HasOwnedTargetTagStockAtValue(
            OwnedAssetState ownedAssets,
            IReadOnlyList<TagData> targetTags,
            int minimumValue)
        {
            foreach (var card in ownedAssets.OwnedCards)
            {
                if (card.Value >= minimumValue && HasAnyTargetTag(card, targetTags))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasEachTargetTagCount(
            OwnedAssetState ownedAssets,
            IReadOnlyList<TagData> targetTags,
            int requiredPerTag)
        {
            if (targetTags == null || targetTags.Count == 0)
            {
                return false;
            }

            foreach (var tag in targetTags)
            {
                var count = 0;
                foreach (var card in ownedAssets.OwnedCards)
                {
                    if (HasTag(card, tag))
                    {
                        count++;
                    }
                }

                if (count < requiredPerTag)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasAnyTargetTag(AssetCardRuntimeData card, IReadOnlyList<TagData> targetTags)
        {
            if (targetTags == null)
            {
                return false;
            }

            foreach (var tag in targetTags)
            {
                if (HasTag(card, tag))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasTag(AssetCardRuntimeData card, TagData targetTag)
        {
            if (card == null || targetTag == null || card.Card == null)
            {
                return false;
            }

            foreach (var tag in card.Card.Tags)
            {
                if (tag != null && tag.Id == targetTag.Id)
                {
                    return true;
                }
            }

            return false;
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
