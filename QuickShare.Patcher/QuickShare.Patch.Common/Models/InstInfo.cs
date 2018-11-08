using System;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class InstInfo
    {
        public string InstallPath { get; set; }
        public bool AllowChangeInstallPath { get; set; }
        public string IconGroup { get; set; }
        public bool AllowChangeIconGroup { get; set; }
        public bool CreateWebSiteLnk { get; set; } = true;
        public bool CreateUninstallLnk { get; set; } = true;
        public string MainEntery { get; set; }
        public string MainEnteryArgs { get; set; }
        public string LicenceFile { get; set; }
        public string ReadmeFile { get; set; }
        public bool ForAllUser { get; set; }
        public bool AllowChangeUser { get; set; } = true;
    }
}

