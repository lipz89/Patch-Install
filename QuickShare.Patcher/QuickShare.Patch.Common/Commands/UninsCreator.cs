using System;
using System.IO;

namespace QuickShare.Patch.Common.Commands
{
    public class UninsCreator : BaseCommand
    {
        public override void Init()
        {
            if (InstallContext.CreateUninstall)
            {
                this.CanRun = true;
                this.CanRollback = true;
            }
            this.CommandStatus = CommandStatus.Inited;
        }

        public override int GetProcessValue()
        {
            if (this.CanRun)
            {
                return 10;
            }

            return 0;
        }

        public override bool Run()
        {
            if (!this.CanRun)
            {
                return true;
            }
            this.CommandStatus = CommandStatus.Running;

            Watch?.SetStep("创建卸载程序");
            var uninsDataPath = Path.Combine(InstallContext.InstInfo.InstallPath, "unins.data");
            InstallContext.UninstallData.SaveToFile(uninsDataPath);
            Watch?.AddValue(10);
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
            Watch?.SetStep("删除卸载程序");
            var uninsDataPath = Path.Combine(InstallContext.InstInfo.InstallPath, "unins.data");
            if (File.Exists(uninsDataPath))
            {
                File.Delete(uninsDataPath);
            }
            Watch?.AddValue(-10);

            this.RollbackStatus = RollbackStatus.Complete;
            return true;
        }
    }
}