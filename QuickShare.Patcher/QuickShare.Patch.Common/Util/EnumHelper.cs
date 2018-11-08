using System.Collections.Generic;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Util
{
    public static class EnumHelper
    {
        private static readonly IDictionary<FileOverride, string> pairsFileOverride = new Dictionary<FileOverride, string>
        {
            { FileOverride.Always,"总是覆盖"},
            { FileOverride.Never,"从不覆盖"},
            { FileOverride.Try,"尝试覆盖"},
            { FileOverride.Newer,"较新时覆盖"},
        };

        public static string GetDescription(FileOverride fileOverride)
        {
            if (pairsFileOverride.ContainsKey(fileOverride))
            {
                return pairsFileOverride[fileOverride];
            }
            return pairsFileOverride[FileOverride.Try];
        }

        public static IDictionary<FileOverride, string> GetFileOverridePairs()
        {
            return pairsFileOverride;
        }

        private static readonly IDictionary<PageItemType, string> pairsPageItemType = new Dictionary<PageItemType, string>
        {
            { PageItemType.TextBox,"文本框"},
            { PageItemType.CheckBox,"复选框"},
        };

        public static string GetDescription(PageItemType pageItemType)
        {
            if (pairsPageItemType.ContainsKey(pageItemType))
            {
                return pairsPageItemType[pageItemType];
            }
            return pairsPageItemType[PageItemType.TextBox];
        }

        public static IDictionary<PageItemType, string> GetPageItemTypePairs()
        {
            return pairsPageItemType;
        }
    }
}