using System;
using System.Collections.Generic;
using System.Text;

namespace AssetManager
{
    public static class MarketCardFormatter
    {
        public static string Format(AssetCardRuntimeData card, bool isReserved)
        {
            if (card.Card.CardDomain == CardDomain.ConsumableResource)
            {
                return FormatConsumableResourceCard(card.Card);
            }

            var builder = new StringBuilder();
            builder.Append("₩");
            builder.Append(card.Card.CashCost);
            builder.Append("  ");
            builder.Append(FormatProfessionalCosts(card.Card.ProfessionalCosts));
            builder.Append("        ↗");
            builder.Append(card.Card.Income);
            if (card.Card.GrantsExtraBuyAction)
            {
                builder.Append("  +↺");
            }

            builder.AppendLine();
            builder.Append("■ ");
            builder.AppendLine(card.Card.DisplayName);
            var tags = FormatTags(card.Card.Tags);
            if (tags != string.Empty)
            {
                builder.AppendLine(tags);
            }

            builder.Append("◆");
            builder.Append(card.Card.Value);
            if (isReserved)
            {
                builder.Append("  예약");
            }

            return builder.ToString();
        }

        private static string FormatConsumableResourceCard(AssetCardData card)
        {
            var builder = new StringBuilder();
            builder.Append("₩");
            builder.Append(card.CashCost);
            builder.AppendLine();
            builder.Append("■ ");
            builder.AppendLine(card.Rarity.ToString());
            builder.Append(ResourceLedger.GetResourceDisplayName(card.ProvidedResourceType));
            builder.Append(" +");
            builder.Append(card.ProvidedResourceAmount);

            return builder.ToString();
        }

        private static string FormatProfessionalCosts(IReadOnlyList<ProfessionalResourceCost> costs)
        {
            if (costs.Count == 0)
            {
                return "—";
            }

            var builder = new StringBuilder();
            for (var i = 0; i < costs.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(GetResourceToken(costs[i].ResourceType));
                builder.Append(costs[i].Amount);
            }

            return builder.ToString();
        }

        private static string GetResourceToken(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Reading:
                    return "R";
                case ResourceType.Meditation:
                    return "M";
                case ResourceType.Patience:
                    return "P";
                case ResourceType.Deal:
                    return "D";
                case ResourceType.Cash:
                    return "₩";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        private static string FormatTags(IReadOnlyList<TagData> tags)
        {
            if (tags.Count == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            for (var i = 0; i < tags.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(" / ");
                }

                builder.Append(tags[i].DisplayName);
            }

            return builder.ToString();
        }
    }
}
