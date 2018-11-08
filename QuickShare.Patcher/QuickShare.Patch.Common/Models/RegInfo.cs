using System;
using Microsoft.Win32;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class RegInfo
    {
        public static RegistryKey Root { get; } = Registry.LocalMachine;
        public static string ProductDirRegkey { get; } = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
        public static string ProductUninstKey { get; } = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public string AppKey { get; set; }
        public string ExeKey { get; set; }
        public string DisplayIcon { get; set; }
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public int EstimatedSize { get; set; }
        public string URLInfoAbout { get; set; }
        public string Publisher { get; set; }
        public string UninstallString { get; set; }
        public string IconGroupName { get; set; }
        public string ExePath { get; set; }
        public string ExeArgs { get; set; }
        public string InstallPath { get; set; }
    }
}