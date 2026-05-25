using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class MissionSettlement
    {
        public static int Calculate(MissionDefinitionData mission, OwnedAssetState ownedAssets)
        {
            if (mission == null || ownedAssets == null)
            {
                return 0;
            }

            switch (mission.Template)
            {
                case MissionTemplate.FastEntry:
                case MissionTemplate.Concentration:
                case MissionTemplate.TwoTagStable:
                    return CountTargetCards(ownedAssets, mission.TargetTags) * mission.SettlementRewardPerCard;
                case MissionTemplate.Foil:
                    return (CountTargetCards(ownedAssets, mission.TargetTags) + CountTargetFoils(ownedAssets, mission.TargetTags))
                        * mission.SettlementRewardPerCard;
                case MissionTemplate.HighValue:
                    return SumTargetValue(ownedAssets, mission.TargetTags);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mission), mission.Template, "Unknown mission settlement template.");
            }
        }

        private static int CountTargetCards(OwnedAssetState ownedAssets, IReadOnlyList<TagData> targetTags)
        {
            var count = 0;
            foreach (var card in ownedAssets.OwnedCards)
            {
                if (MatchesTarget(card, targetTags))
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountTargetFoils(OwnedAssetState ownedAssets, IReadOnlyList<TagData> targetTags)
        {
            var count = 0;
            foreach (var card in ownedAssets.OwnedCards)
            {
                if (card.IsFoil && MatchesTarget(card, targetTags))
                {
                    count++;
                }
            }

            return count;
        }

        private static int SumTargetValue(OwnedAssetState ownedAssets, IReadOnlyList<TagData> targetTags)
        {
            var total = 0;
            foreach (var card in ownedAssets.OwnedCards)
            {
                if (MatchesTarget(card, targetTags))
                {
                    total += card.Value;
                }
            }

            return total;
        }

        private static bool MatchesTarget(AssetCardRuntimeData card, IReadOnlyList<TagData> targetTags)
        {
            if (card == null || card.Card == null)
            {
                return false;
            }

            if (targetTags == null || targetTags.Count == 0)
            {
                return true;
            }

            foreach (var targetTag in targetTags)
            {
                if (HasTag(card, targetTag))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasTag(AssetCardRuntimeData card, TagData targetTag)
        {
            if (targetTag == null)
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
    }
}
