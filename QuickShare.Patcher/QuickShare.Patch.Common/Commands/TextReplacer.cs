using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuickShare.Patch.Common.Commands
{
    public class TextReplacer : BaseCommand
    {
        public static string CmdDesc = @"该命令需要 文件，占位符和替换内容 两种参数，
其中占位符和替换内容以>分割，每一行内容是一个参数。例如：
    ${InstPath}\App.Config
    ${InstPath}\Image.Config
    __PATH__>${InstPath}
    __APPNAME__>$[Name]
表示将要替换两个文件，分别是安装目录下的App.Config文件和Image.Config文件，
分别将占位符__PATH__替换为安装目录，将占位符__APPNAME__替换为应用程序全名。
注意：如果文件包含中文，则文件的编码格式不允许为ASCII。";
        public static string CmdKey = "$(TextReplacer)";
        private readonly List<string> files = new List<string>();
        private readonly List<string> placeholders = new List<string>();
        private readonly Dictionary<string, string> dics = new Dictionary<string, string>();

        public override bool Run()
        {
            if (!CanRun)
            {
                return true;
            }

            this.CommandStatus = CommandStatus.Running;

            var flag = true;
            foreach (var file in files)
            {
                flag &= Replace(file, dics);
            }

            this.CommandStatus = flag ? CommandStatus.Complete : CommandStatus.Failed;
            return flag;
        }

        public override void Init()
        {
            if (initArgs != null)
            {
                foreach (var s in initArgs)
                {
                    if (s.Contains(">"))
                    {
                        placeholders.Add(s);
                    }
                    else
                    {
                        files.Add(s);
                    }
                }
            }

            if (files.Any() && placeholders.Any())
            {
                Watch?.Info($"当前命令包含 {files.Count} 个待处理的文件，{dics.Count} 对替换内容。");
                foreach (var placeholder in placeholders)
                {
                    var sp = placeholder.Split(">".ToCharArray(), 2);
                    dics[sp[0].Trim()] = sp[1].Trim();
                }
                this.CanRun = true;
            }
            else
            {
                Watch?.Warn("没有要替换的文件或占位符。");
                this.CanRun = false;
            }
            this.CommandStatus = CommandStatus.Inited;
        }

        public override bool Rollback()
        {
            return true;
        }

        public override int GetProcessValue()
        {
            if (this.CanRun)
            {
                return files.Count;
            }
            return 0;
        }
        private bool Replace(string file, Dictionary<string, string> dic)
        {
            Watch?.Info($"开始替换文件: {file}");
            if (File.Exists(file))
            {
                try
                {
                    var content = File.ReadAllText(file);
                    foreach (var pair in dic)
                    {
                        content = content.Replace(pair.Key, pair.Value);
                    }

                    File.WriteAllText(file, content);
                    Watch?.Info("替换成功。");
                    return true;
                }
                catch (Exception ex)
                {
                    Watch?.Error("替换失败：" + ex.Message);
                    return false;
                }
            }

            Watch?.Error("替换失败：文件不存在。");
            return false;
        }

        public override bool CanRollback { get; protected set; } = false;
    }
}