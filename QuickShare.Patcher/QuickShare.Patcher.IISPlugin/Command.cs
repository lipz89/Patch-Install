using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using QuickShare.Patch.Common.Commands;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.IISPlugin
{
    public class Command : BaseCommand
    {
        private int processValue = 10;
        private string backupName;
        private string target;
        private string siteAddress;
        private readonly Regex error = new Regex("^ERROR.*message:.*$", RegexOptions.IgnoreCase);
        private const string LIST_BACKUP = "list backup {0}";
        private const string ADD_BACKUP = "add backup {0}";
        private const string RESTORE_BACKUP = "restore  backup {0}";
        private const string DELETE_BACKUP = "delete  backup {0}";
        private const string LIST_POOL = "list apppool {0}";
        private const string ADD_POOL = "add apppool /name:{0} /managedRuntimeVersion:\"v4.0\" /managedPipelineMode:\"Integrated\"";
        private const string LIST_SITE = "list site {0}";
        private const string ADD_SITE = "add site /name:{0} /physicalPath:{1} /bindings:{2}";
        private const string SET_SITE_POOL = "set site /site.name:{0} /[path='/'].applicationPool:{1}";
        private const string START_SITE = "start site {0}";
        public override void Init()
        {
            target = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"system32\inetsrv\appcmd.exe");
            this.CanRun = File.Exists(target);

            if (!CanRun)
            {
                Watch?.Warn("当前计算机未安装IIS。");
            }
            this.CanRollback = this.CanRun;

            this.CommandStatus = CommandStatus.Inited;
        }

        public override int GetProcessValue()
        {
            return processValue;
        }

        public override bool Run()
        {
            if (!CanRun)
            {
                return true;
            }
            this.CommandStatus = CommandStatus.Running;
            Watch?.Info("正在配置IIS");
            MakeBackup();

            var siteName = this.InstallContext.GetPageItemValue("IIS_SiteName");
            var poolName = this.InstallContext.GetPageItemValue("IIS_PoolName");
            var hostName = this.InstallContext.GetPageItemValue("IIS_HostName");
            var type = this.InstallContext.GetPageItemValue("IIS_BindType");
            var ip = this.InstallContext.GetPageItemValue("IIS_IpAddress");
            var port = this.InstallContext.GetPageItemValue("IIS_Port");
            var logicPath = this.InstallContext.GetPageItemValue("IIS_LogicPath");
            logicPath = this.InstallContext.ConvertVars(logicPath);

            var host = hostName;
            if (string.IsNullOrEmpty(hostName))
            {
                host = ip == "*" ? "localhost" : ip;
            }

            if (port != "80")
            {
                host += $":{port}";
            }
            siteAddress = $"{type}://{host}";

            //var siteName = "NewSite";
            //var poolName = "NewSite";
            //var hostName = "www.newsite.com";
            //var type = "http";
            //var ip = "*";
            //var port = "4049";
            //var logicPath = @"E:\Projects\09-物资云\物资云.药品V2.0\代码\QuickShare.Cloud\QuickShare.Cloud.Web";

            if (!MakeAppPool(poolName))
            {
                Watch?.Faild();
                this.CommandStatus = CommandStatus.Failed;
                return false;
            }

            var bindings = $"{type}/{ip}:{port}:{hostName}";
            if (!MakeSite(siteName, logicPath, bindings))
            {
                Watch?.Faild();
                this.CommandStatus = CommandStatus.Failed;
                return false;
            }

            if (SetSitePool(siteName, poolName))
            {
                StartSite(siteName, siteAddress);
            }

            Watch?.Info($"IIS站点 {siteName} 部署完成。");
            this.CommandStatus = CommandStatus.Complete;
            return true;
        }

        public override bool Rollback()
        {
            this.RollbackStatus = RollbackStatus.Rollbacking;
            var result = RestoreBackup(backupName);
            Watch?.AddValue(-processValue);
            DoCmd(string.Format(DELETE_BACKUP, backupName));

            this.RollbackStatus = result ? RollbackStatus.Complete : RollbackStatus.Failed;
            return result;
        }

        private bool StartSite(string siteName, string site)
        {
            Watch?.Info($"启动网站 {siteName}");
            var result = DoCmd(string.Format(START_SITE, siteName));
            var rst = !error.IsMatch(result);
            if (rst)
            {
                Watch?.Info($"启动网站 {siteName} 成功：{result}");
                return true;
            }

            Watch?.Error($"启动网站 {siteName} 失败：{result}");
            return false;
        }

        private bool SetSitePool(string siteName, string poolName)
        {
            Watch?.Info($"设置网站应用程序池 {siteName} -> {poolName}");
            var result = DoCmd(string.Format(SET_SITE_POOL, siteName, poolName));
            var rst = !error.IsMatch(result);
            if (rst)
            {
                return true;
            }

            Watch?.Error($"设置网站应用程序池 {siteName} -> {poolName} 失败：{result}");
            return false;
        }
        private bool MakeSite(string siteName, string logicPath, string bindings)
        {
            Watch?.Info($"检查网站 {siteName} 是否存在");
            var result = DoCmd(string.Format(LIST_SITE, siteName));
            if (!result.Contains(siteName))
            {
                Watch?.Info($"添加网站 {siteName}");
                result = DoCmd(string.Format(ADD_SITE, siteName, logicPath, bindings));
                var rst = !error.IsMatch(result);
                if (rst)
                {
                    return true;
                }

                Watch?.Error($"添加网站 {siteName} 失败：{result}");
                return false;
            }

            Watch?.Error($"网站 {siteName} 已存在");
            return false;
        }
        private bool MakeAppPool(string poolName)
        {
            Watch?.Info($"检查应用程序池 {poolName} 是否存在");
            var result = DoCmd(string.Format(LIST_POOL, poolName));
            if (!result.Contains(poolName))
            {
                Watch?.Info($"正在添加应用程序池 {poolName}");
                result = DoCmd(string.Format(ADD_POOL, poolName));
                var rst = !error.IsMatch(result);
                if (rst)
                {
                    return true;
                }

                Watch?.Error($"添加应用程序池 {poolName} 失败：{result}");
                return false;
            }

            Watch?.Info($"应用程序池 {poolName} 已存在");
            return true;
        }

        private bool MakeBackup()
        {
            Watch?.Info("正在备份IIS设置");
            do
            {
                var name = $"CFG_BACKUP_{DateTime.Now:yyyyMMddHHmmss}";
                var result = DoCmd(string.Format(LIST_BACKUP, name));
                if (!result.Contains(name))
                {
                    result = DoCmd(string.Format(ADD_BACKUP, name));
                    var rst = !error.IsMatch(result);
                    if (rst)
                    {
                        this.backupName = name;
                        return true;
                    }

                    Watch?.Warn($"备份IIS配置失败：{result}");
                    return false;
                }
            } while (true);
        }

        private bool RestoreBackup(string backupName)
        {
            Watch?.Info("正在还原IIS配置");
            var result = DoCmd(string.Format(RESTORE_BACKUP, backupName));
            var rst = !error.IsMatch(result);
            if (rst)
            {
                return true;
            }

            Watch?.Error($"还原IIS配置失败：{result}");
            return false;
        }

        private string DoCmd(string args)
        {
            var pinfo = new ProcessStartInfo(target, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            var proc = Process.Start(pinfo);

            if (proc != null)
            {
                var message = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                proc.Close();
                proc.Dispose();
                proc = null;
                return message;
            }

            return null;
        }
    }
}
