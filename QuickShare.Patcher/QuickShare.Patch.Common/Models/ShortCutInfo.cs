using System;
using System.IO;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class ShortCutInfo
    {
        public string TargetDir { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public string Args { get; set; }

        public string Key
        {
            get { return Path.Combine(TargetDir, Name); }
        }
        public string Command
        {
            get { return Target + " " + Args; }
        }
    }
}