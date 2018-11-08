using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Commands;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Util
{
    public class InstallProcess
    {
        private readonly InstallContext context;
        private readonly IWatch process;
        private readonly IWin32Window owner;

        private const int ValueMin = 1;
        private const int ValueReady = 2;
        private const int ValueCommand = 10;

        private readonly IList<Cmd> commands = new List<Cmd>();
        private readonly IList<Cmd> executed = new List<Cmd>();

        public InstallProcess(InstallContext context, IWatch process, IWin32Window owner = null)
        {
            this.context = context;
            this.process = process;
            this.owner = owner;
            this.process?.SetStep("正在准备安装");
            this.process?.Info("正在准备安装");

        }

        private void Init()
        {
            context.InstInfo.InstallPath = context.ConvertPath(context.InstInfo.InstallPath);
            context.Files.ForEach(x => x.Path = context.ConvertPath(x.Key));

            context.UninstallData = new UninstallData()
            {
                UninstallKey = context.AppInfo.AppName,
                InstallPath = context.InstInfo.InstallPath,
                AppName = context.AppInfo.FullName,
                Shortcuts = new List<string>()
            };

            var index = 0;

            var fileExtractor = new FileExtractor();
            fileExtractor.SetContext(context, process);
            commands.Add(new Cmd() { Index = index++, Command = fileExtractor });

            var shortcutCreator = new ShortcutCreator();
            shortcutCreator.SetContext(context, process);
            commands.Add(new Cmd() { Index = index++, Command = shortcutCreator });

            foreach (var cmdInfo in context.Commands)
            {
                cmdInfo.ArgList = cmdInfo.ArgList?.Select(x => context.ConvertPath(context.ConvertVars(x))).ToArray() ?? new string[0];
                var cmd = BaseCommand.GetCommand(cmdInfo.Target);
                if (cmd != null)
                {
                    cmd.SetContext(context, process, cmdInfo.Name, cmdInfo.ArgList);
                    commands.Add(new Cmd() { Index = index++, Command = cmd });
                }
                else
                {
                    commands.Add(new Cmd() { Index = index++, CmdInfo = cmdInfo });
                }
            }

            var uninsCreator = new UninsCreator();
            uninsCreator.SetContext(context, process);
            commands.Add(new Cmd() { Index = index++, Command = uninsCreator });

            var registerEditor = new RegisterEditor();
            registerEditor.SetContext(context, process);
            commands.Add(new Cmd() { Index = index++, Command = registerEditor });

            process?.SetMaxValue(GetProcessValue(), ValueMin);
        }

        private int GetProcessValue()
        {
            int maxValue = ValueMin;
            maxValue += ValueReady;

            if (commands != null && commands.Any())
            {
                foreach (var command in commands)
                {
                    if (command.Command?.CanRun == true)
                    {
                        maxValue += command.Command.GetProcessValue();
                    }
                    else if (command.CmdInfo != null)
                    {
                        maxValue += ValueCommand;
                    }
                }
            }
            return maxValue;
        }

        public void Install()
        {
            Init();

            process?.Info("开始" + context.Operator);

            process?.AddValue(ValueReady);


            if (Run())
            {
                context.IsSuccess = true;
                process?.Complete();
            }
            else
            {
                context.IsSuccess = false;
                process?.Error("正在回滚。");
                Rollback();
                process?.Info("完成回滚。");
            }
        }

        private bool Run()
        {
            if (commands != null && commands.Any())
            {
                foreach (var command in commands.OrderBy(x => x.Index))
                {
                    if (command.Command != null)
                    {
                        command.Command.SetOwner(owner);
                        command.Command.Init();
                        if (command.Command.CanRun)
                        {
                            executed.Add(command);
                            if (!command.Command.Run())
                            {
                                process?.Faild();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        RunCommand(command.CmdInfo, context.InstInfo.InstallPath);
                    }
                }
            }

            return true;
        }

        private void Rollback()
        {
            foreach (var cmd in executed.OrderByDescending(x => x.Index))
            {
                if (cmd.Command != null)
                {
                    cmd.Command.Rollback();
                }
                else if (cmd.CmdInfo != null)
                {

                }
            }
        }
        private void RunCommand(CmdInfo command, string installPath)
        {
            process?.Info("开始命令 " + command.Name);
            var target = context.ConvertPath(command.Target);
            if (!string.IsNullOrEmpty(target) && File.Exists(target))
            {
                process?.Info("命令：" + target);
                if (!string.IsNullOrEmpty(command.Args))
                {
                    process?.Info("参数：" + command.Args);
                }

                var pinfo = new ProcessStartInfo(target, command.Args)
                {
                    WorkingDirectory = installPath
                };
                var proc = Process.Start(pinfo);

                if (proc != null)
                {
                    proc.WaitForExit();
                    proc.Close();
                    proc.Dispose();
                    proc = null;
                }
            }
        }
        class Cmd
        {
            public int Index { get; set; }
            public ICommand Command { get; set; }
            public CmdInfo CmdInfo { get; set; }
        }
    }
}