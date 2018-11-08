using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patch.Common.Commands
{
    public class ShortcutCreator : BaseCommand
    {
        private string iconDir;
        private readonly List<ShortCutInfo> shortCutInfos = new List<ShortCutInfo>();
        private bool createIconDir;

        public override void Init()
        {
            InstallContext.UninstallData.UninstallTarget =
                Path.Combine(InstallContext.InstInfo.InstallPath, Context.UninstallerKey);

            InstallContext.CreateUninstall = InstallContext.InstInfo.CreateUninstallLnk &&
                                             File.Exists(InstallContext.UninstallData.UninstallTarget);

            this.iconDir = InstallContext.ConvertPath(GlobalPath.IconGroup);

            if (InstallContext.ShortCuts != null && InstallContext.ShortCuts.Any() && InstallContext.CreateShortcuts)
            {
                if (InstallContext.CreateIconGroup && !string.IsNullOrEmpty(InstallContext.InstInfo.IconGroup))
                {

                    if (InstallContext.CreateUninstall)
                    {
                        shortCutInfos.Add(new ShortCutInfo()
                        {
                            Name = "Uninstall.lnk",
                            TargetDir = iconDir,
                            Target = InstallContext.UninstallData.UninstallTarget
                        });
                    }

                    if (InstallContext.InstInfo.CreateWebSiteLnk &&
                        !string.IsNullOrEmpty(InstallContext.AppInfo.WebSite))
                    {
                        shortCutInfos.Add(new ShortCutInfo()
                        {
                            Name = "website.url",
                            TargetDir = iconDir,
                            Target = InstallContext.AppInfo.WebSite
                        });
                    }
                }
            }

            if (InstallContext.ShortCuts != null)
            {
                shortCutInfos.AddRange(InstallContext.ShortCuts);
            }

            if (shortCutInfos.Any())
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
                return shortCutInfos?.Count ?? 0;
            }

            return 0;
        }

        public override bool Run()
        {
            if (!CanRun)
            {
                return true;
            }
            this.CommandStatus = CommandStatus.Running;
            Watch?.SetStep("创建快捷方式");
            if (!Directory.Exists(iconDir))
            {
                Directory.CreateDirectory(iconDir);
                this.createIconDir = true;
            }

            foreach (var shortcut in shortCutInfos)
            {
                var name = shortcut.Name;
                if (!name.EndsWith(".url", StringComparison.OrdinalIgnoreCase) && !name.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    name += ".lnk";
                }
                var targetPath = InstallContext.ConvertPath(shortcut.TargetDir);
                if (Directory.Exists(targetPath))
                {
                    var path = Path.Combine(targetPath, name);
                    FileHelper.RemoveReadonly(path);
                    if (name.EndsWith(".url", StringComparison.OrdinalIgnoreCase))
                    {
                        ShortcutHelper.CreateWebShortcutFile(path, shortcut.TargetDir);
                    }
                    else
                    {
                        var target = InstallContext.ConvertPath(shortcut.Target);
                        ShortcutHelper.CreateShortcutFile(path, target, shortcut.Args);
                    }

                    Watch?.Info("-->" + path);
                    Watch?.AddValue(1);
                    InstallContext.UninstallData.Shortcuts.Add(path);
                }
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
            Watch?.SetStep("删除快捷方式");
            Watch?.SetStep("正在删除快捷方式");
            foreach (var path in InstallContext.UninstallData.Shortcuts)
            {
                File.Delete(path);
                Watch?.AddValue(-1);
            }

            if (createIconDir && Directory.Exists(iconDir))
            {
                Directory.Delete(iconDir, true);
            }


            this.RollbackStatus = RollbackStatus.Complete;
            return true;
        }
    }
}