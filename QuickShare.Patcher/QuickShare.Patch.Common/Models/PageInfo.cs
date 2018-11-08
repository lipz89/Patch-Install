using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class PageInfo
    {
        public int Index { get; set; }
        public string EnableText { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public List<PageItem> Items { get; set; }
        public string Privoder { get; set; }
        public bool Editable { get; set; }

        public string Keys
        {
            get
            {
                if (this.Items == null)
                {
                    return "[未定义配置项]";
                }

                return string.Join(",", this.Items.Select(x => x.Key).ToArray());
            }
        }

        public bool IsPlugin { get; internal set; }
        public string CustomPageTypeName { get; internal set; }
    }
}