using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuickShare.Patch.Common.Commands
{
    public class SqlCmdExecutor : BaseCommand
    {
        public static string CmdDesc = @"该命令需要一个参数：
    脚本存放文件夹
该命令将按照字母顺序逐一执行文件夹及子文件夹中的所有sql脚本。
该命令不会对现有的数据库对象进行备份，应提醒用户自行备份。";
        public static string CmdKey = "$(SqlCmdExecutor)";
        private static readonly Regex errorRegex = new Regex(@"消息 \d*，级别 \d*");
        private static readonly Regex errorRegex2 = new Regex(@"消息 \d*，级别 \d*");
        private readonly List<string> files = new List<string>();
        private string config;

        public override void Init()
        {
            this.CanRun = CheckFiles(initArgs);
            this.CommandStatus = CommandStatus.Inited;
        }

        public override int GetProcessValue()
        {
            if (this.CanRun)
            {
                return files.Count;
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
            foreach (var file in files.OrderBy(x => x))
            {
                if (!File.Exists(file))
                {
                    continue;
                }
                Watch?.Info($"执行脚本： [{file}]");
                var args = $"{config} -i {file}";
                if (!Start("sqlcmd.exe", args))
                {
                    this.CommandStatus = CommandStatus.Failed;
                    return false;
                }
                Watch?.AddValue(1);
            }

            this.CommandStatus = CommandStatus.Complete;
            return true;
        }

        public override bool Rollback()
        {
            return true;
        }

        private bool Start(string name, string args)
        {
            try
            {
                var iserror = false;
                Process process = new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        FileName = name,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    }
                };
                if (!string.IsNullOrEmpty(args))
                {
                    process.StartInfo.Arguments = args;
                }
                process.Start();

                var output = process.StandardOutput.ReadToEnd();
                if (string.IsNullOrEmpty(output))
                {
                    Watch?.Info("执行成功。");
                }
                else
                {
                    iserror = errorRegex.IsMatch(output) || errorRegex2.IsMatch(output);
                    if (iserror)
                    {
                        Watch?.Error(output);
                    }
                    else
                    {
                        Watch?.Info(output);
                    }
                }
                process.WaitForExit();
                process.Close();
                return !iserror;
            }
            catch (Exception ex)
            {
                Watch?.Error("脚本执行错误：" + ex.Message);
                return false;
            }
        }

        private bool CheckFiles(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false;
            }

            var dir = args[0];
            if (string.IsNullOrEmpty(dir))
            {
                return false;
            }

            dir = Path.Combine(Environment.CurrentDirectory, dir);

            if (!Directory.Exists(dir))
            {
                return false;
            }

            files.AddRange(Directory.GetFiles(dir, "*.sql", SearchOption.AllDirectories));
            if (files.Count > 0)
            {
                Watch?.Info($"检查到 {files.Count} 个脚本待执行");
            }
            else
            {
                Watch?.Info("没检查到待执行的脚本");
                return false;
            }
            if (!ReadConfig(out string config))
            {
                this.config = config;
                Watch?.Info("未配置数据库，取消执行。");
                return false;
            }

            return true;
        }

        private bool ReadConfig(out string args)
        {
            if (FrmSqlConfig.GetConfig(this.Owner, out string server, out string dbName, out string userId, out string password))
            {
                args = $"-U {userId} -P {password} -S {server} -d {dbName}";
                return true;
            }

            args = null;
            return false;
        }

        //        private static void GetConfig(out string server, out string dbName, out string userId, out string password)
        //        {
        //#if DEBUG
        //            server = ".";
        //            dbName = "Test";
        //            userId = "sa";
        //            password = "111111";
        //#else
        //                server = ReadLine("请输入服务器\\实例名：");
        //                dbName = ReadLine("请输入数据库名：");
        //                userId = ReadLine("请输入用户名：");
        //                password = ReadLine("请输入密码：", true) ?? "";
        //#endif
        //        }

        //public static bool TestDb(string args)
        //{
        //    Process process = new Process
        //    {
        //        StartInfo =
        //        {
        //            WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
        //            FileName = "sqlcmd",
        //            CreateNoWindow = true,
        //            UseShellExecute = false,
        //            RedirectStandardInput = true,
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //        }
        //    };
        //    if (!string.IsNullOrEmpty(args))
        //    {
        //        process.StartInfo.Arguments = $"{args} -Q \"SELECT TOP 1 1 FROM SYS.tables\"";
        //    }
        //    process.Start();
        //    var output = process.StandardOutput.ReadToEnd();
        //    process.WaitForExit();
        //    process.Close();
        //    var result = !string.IsNullOrEmpty(output);
        //    return result;
        //}
        public override bool CanRollback { get; protected set; } = false;
    }
}