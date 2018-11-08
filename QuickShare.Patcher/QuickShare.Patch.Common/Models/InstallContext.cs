using System.Linq;

namespace QuickShare.Patch.Common.Models
{
    public class InstallContext : Context
    {
        public string PackageFile { get; set; }
        public long NeedSize { get; set; }
        public bool IsFull { get; private set; }
        public bool IsMakeStore { get; set; }
        public string Operator { get; private set; }
        public bool RunWhenFinish { get; set; }
        public bool RunReadmeWhenFinish { get; set; }
        public bool CreateShortcuts { get; set; }
        public bool CreateIconGroup { get; set; }
        public bool CreateUninstall { get; set; }
        public UninstallData UninstallData { get; set; }
        public bool IsSuccess { get; set; }
        public InstallContext(Context context)
        {
            base.AppInfo = context.AppInfo;
            IsFull = (base.AppInfo?.LastVersion == null);
            Operator = (IsFull ? "安装" : "升级");
            base.InstInfo = context.InstInfo;
            base.Commands = context.Commands;
            base.Files = context.Files;
            base.Pages = context.Pages;
            base.ShortCuts = context.ShortCuts;
        }

        public string GetPageItemValue(string key)
        {
            return base.Pages?.SelectMany((PageInfo x) => x.Items).FirstOrDefault((PageItem x) => x.Key == key)?.Value;
        }
    }
}