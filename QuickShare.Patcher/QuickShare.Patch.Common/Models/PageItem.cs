using System;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class PageItem
    {
        public int Index { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public PageItemType Type { get; set; }
    }
}