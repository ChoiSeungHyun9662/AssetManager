using System;
using System.Collections.Generic;

namespace AssetManager
{
    public static class StockTagCatalog
    {
        public static readonly TagData Technology = new TagData("technology", "Technology", TagType.Sector);
        public static readonly TagData Consumer = new TagData("consumer", "Consumer", TagType.Sector);
        public static readonly TagData Energy = new TagData("energy", "Energy", TagType.Sector);
        public static readonly TagData Financials = new TagData("financials", "Financials", TagType.Sector);
        public static readonly TagData Industrials = new TagData("industrials", "Industrials", TagType.Sector);

        private static readonly IReadOnlyList<TagData> Tags = new[]
        {
            Technology,
            Consumer,
            Energy,
            Financials,
            Industrials
        };

        public static IReadOnlyList<TagData> AllowedStockTags => Tags;

        public static bool HasExactlyOneAllowedStockTag(AssetCardData card)
        {
            if (card == null || card.CardDomain != CardDomain.Stock || card.Tags.Count != 1)
            {
                return false;
            }

            return IsAllowedStockTag(card.Tags[0]);
        }

        public static bool IsAllowedStockTag(TagData tag)
        {
            return tag != null
                && tag.TagType == TagType.Sector
                && IsAllowedStockTag(tag.Id);
        }

        public static bool IsAllowedStockTag(string tagId)
        {
            if (string.IsNullOrEmpty(tagId))
            {
                return false;
            }

            foreach (var tag in Tags)
            {
                if (string.Equals(tag.Id, tagId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
