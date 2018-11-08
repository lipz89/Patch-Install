namespace QuickShare.Patch.Common
{
    public class GlobalPath
    {
        public const string ProgramFiles = "${ProgFiles}";
        public const string Desktop = "${Desktop}";
        public const string StartMenu = "${StartMenu}";
        public const string ApplicationData = "${AppData}";
        public const string LocalApplicationData = "${LocalAppData}";
        public const string Startup = "${Startup}";
        public const string Templates = "${Templates}";
        public const string System = "${System}";

        public const string InstallPath = "${InstPath}";
        public const string IconGroup = "${IconGroup}";

        public const string AppName = "AppName";
        public const string FullName = "Name";
        public static string[] GetGlobalVars()
        {
            return new string[]
            {
                AppName,
                FullName,
            };
        }
        public static string[] GetInstallPaths()
        {
            return new string[]
            {
                ProgramFiles,
            };
        }
        public static string[] GetShortCutPaths()
        {
            return new string[]
            {
                Desktop,
                IconGroup,
                StartMenu,
            };
        }
        public static string[] GetFilePaths()
        {
            return new string[]
            {
                InstallPath,
                ApplicationData,
                Desktop,
                Templates,
                Startup,
                System,
            };
        }
    }
}