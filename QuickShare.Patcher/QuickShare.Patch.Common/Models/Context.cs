using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class Context
    {
        public static readonly string DataKey = "data.bin";
        public static readonly string LicenceRtfKey = "licence.rtf";
        public static readonly string LicenceTxtKey = "licence.txt";
        public static readonly string UninstallerKey = "unins.exe";
        public InstInfo InstInfo { get; set; } = new InstInfo();
        public AppInfo AppInfo { get; set; }

        public List<string> PageItemKeys
        {
            get
            {
                var init = GlobalPath.GetGlobalVars().ToList();
                if (this.Pages == null)
                    return init;
                var customs = this.Pages?.SelectMany(x => x.Items.Select(i => i.Key)).ToList();
                init.AddRange(customs);
                return init;
            }
        }

        public List<FileItem> Files { get; set; }
        public List<ShortCutInfo> ShortCuts { get; set; }
        public List<CmdInfo> Commands { get; set; }
        public List<PageInfo> Pages { get; set; }
        public string OutputName { get; set; }
    }
}