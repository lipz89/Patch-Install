using System.IO;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patch.Common.Commands
{
    public class RegisterEditor : BaseCommand
    {
        private RegInfo regInfo;

        public override void Init()
        {
            var iconDir = InstallContext.ConvertPath(GlobalPath.IconGroup);
            var mainEntity = InstallContext.ConvertPath(InstallContext.InstInfo.MainEntery);
            regInfo = new RegInfo
            {
                DisplayName = InstallContext.AppInfo.AppName,
                DisplayIcon = mainEntity,
                DisplayVersion = InstallContext.AppInfo.Version,
                ExeArgs = InstallContext.InstInfo.MainEnteryArgs,
                EstimatedSize = (int)(InstallContext.NeedSize / 1024),
                ExePath = mainEntity,
                ExeKey = new FileInfo(mainEntity).Name,
                IconGroupName = iconDir,
                InstallPath = InstallContext.InstInfo.InstallPath,
                Publisher = InstallContext.AppInfo.Publisher,
                URLInfoAbout = InstallContext.AppInfo.WebSite,
                AppKey = InstallContext.AppInfo.AppName,
                UninstallString = InstallContext.CreateUninstall ? InstallContext.UninstallData.UninstallTarget : null
            };
            this.CanRun = true;
            this.CanRollback = true;
            this.CommandStatus = CommandStatus.Inited;
        }

        public override int GetProcessValue()
        {
            return 10;
        }

        public override bool Run()
        {
            if (!this.CanRun)
            {
                return true;
            }
            this.CommandStatus = CommandStatus.Running;

            Watch?.SetStep("写注册表");
            Watch?.Info("正在写入注册表");
            if (!RegisterHelper.Write(regInfo))
            {
                Watch?.Faild();
                this.CommandStatus = CommandStatus.Failed;
                return false;
            }

            this.CommandStatus = CommandStatus.Complete;
            return true;
        }

        public override bool Rollback()
        {
            if (!this.CanRollback)
            {
                return true;
            }
            if (this.CommandStatus != CommandStatus.Complete &&
                this.CommandStatus != CommandStatus.Failed &&
                this.CommandStatus != CommandStatus.Running)
            {
                return true;
            }

            this.RollbackStatus = RollbackStatus.Rollbacking;

            Watch?.SetStep("删除注册表");
            Watch?.Info("正在删除注册表");
            RegisterHelper.Delete(RegInfo.ProductUninstKey, regInfo.AppKey);
            RegisterHelper.Delete(RegInfo.ProductDirRegkey, regInfo.ExeKey);


            this.RollbackStatus = RollbackStatus.Complete;
            return true;
        }
    }
}