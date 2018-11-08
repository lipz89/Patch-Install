using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Commands;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace TestPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.Log = new Log();
            //Reg.Test();
            //var test = new TestCompress();
            //test.TestZip();
            //test.TestUnZip();

            ////var testshort = new TestShortCut();
            ////testshort.CreateShortcutFile();
            //TestOrderByBool();
            //TestFolders();
            //TestFolders2();

            //StartTextReplacer();

            //TestCmd();
            //TestText();
            //TestSql();
            //TestSqlAdo();
            //TestIISConfig();

            new CharTest().Run();
            Console.ReadKey();
        }

        private static void TestIISConfig()
        {
            var iiscfg = new QuickShare.Patcher.IISPlugin.Command();
            iiscfg.SetContext(null, new Log());
            iiscfg.Init();
            iiscfg.Run();
            Console.WriteLine("配置完成");
            Console.ReadKey();
            iiscfg.Rollback();
            Console.WriteLine("还原完成");
        }
        private static void TestSqlAdo()
        {
            var ctx = new Context()
            {
                AppInfo = new AppInfo(false),
                InstInfo = new InstInfo() { InstallPath = @"F:\\Test" }
            };
            var insCtx = new InstallContext(ctx);
            var tr = new SqlAdoExecutor();
            tr.SetContext(insCtx, new Log(), "", @"E:\Tests\TestScripts");
            tr.Run();
            tr.Rollback();
        }
        private static void TestSql()
        {
            var tr = new SqlCmdExecutor();
            tr.SetContext(null, new Log(), "", @"E:\Tests\TestScripts");
            tr.Run();
        }

        private static void TestText()
        {
            var tr = new TextReplacer();
            tr.SetContext(null, new Log(), "", "D:\\Program Files\\Winning.FrameWork.Config __HRP_PATH__>D:\\Program Files".Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void TestCmd()
        {
            RunCommand(@"E:\Projects\10-接口平台DEP\04-标准采配接口\02-数据库\DataBase\123.vbs", "", null);

            Console.WriteLine("end");
        }

        private static void RunCommand(string target, string args, string workDir)
        {
            if (!string.IsNullOrEmpty(target) && File.Exists(target))
            {
                //process?.Info("命令：" + target);
                if (!string.IsNullOrEmpty(args))
                {
                    //process?.Info("参数：" + args);
                }
                var pinfo = new ProcessStartInfo(target, args)
                {
                    WorkingDirectory = workDir ?? AppDomain.CurrentDomain.BaseDirectory
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

        private static void StartTextReplacer()
        {
            var target =
                @"E:\Projects\QuickShare.Patcher\QuickShare.Patcher.TextReplacer\bin\Release\QuickShare.Patcher.TextReplacer.exe";
            var args = new string[]
            {
                @"E:\Projects\QuickShare.Patcher\QuickShare.Patcher.TextReplacer\bin\Release\Winning.FrameWork.Config",
                @"E:\Projects\QuickShare.Patcher\QuickShare.Patcher.TextReplacer\bin\Release\Winning.FrameWork.Unicode.Config",
                @"__HRP_PATH__>E:\Projects\QuickShare.Patcher\QuickShare.Patcher.TextReplacer\bin\Release"
            };

            var str = string.Join(" ", args.Select(x => "\"" + x + "\"").ToArray());
            Console.WriteLine("命令：" + target);
            if (!string.IsNullOrEmpty(str))
            {
                Console.WriteLine("参数：" + str);
            }
            var pinfo = new ProcessStartInfo(target, str)
            {
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
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

        private static void TestOrderByBool()
        {
            var list = new List<FileItem>
            {
                new FileItem() {Path = "1", TargetDir = "1"},
                new FileItem() {Path = "2", TargetDir = "2"},
                new FileItem() {Path = "3", TargetDir = "3", IsMain = true},
                new FileItem() {Path = "4", TargetDir = "4"},
                new FileItem() {Path = "5", TargetDir = "5"},
            };

            foreach (var fi in list.OrderByDescending(x => x.IsMain))
            {
                Console.WriteLine(fi.Path + "   " + fi.TargetDir + "   " + fi.IsMain);
            }
        }

        private static void TestFolders()
        {
            var type = typeof(Environment.SpecialFolder);
            Environment.SpecialFolder folder;
            string path;
            for (int i = 0; i < 44; i++)
            {
                if (Enum.IsDefined(type, i))
                {
                    folder = (Environment.SpecialFolder)i;
                    path = Environment.GetFolderPath(folder);
                    Console.WriteLine(folder + ":\t" + path);
                }
            }
        }


        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
        private const int MAX_PATH = 260;
        private static void TestFolders2()
        {
            for (int i = 0; i < 100; i++)
            {
                var path = GetPath(i);
                if (!string.IsNullOrEmpty(path))
                    Console.WriteLine("{0:0000}:{1}", i, path);
            }
        }
        private static string GetPath(int nFolder)
        {
            StringBuilder sbPath = new StringBuilder(MAX_PATH);
            SHGetFolderPath(IntPtr.Zero, nFolder, IntPtr.Zero, 0, sbPath);
            return sbPath.ToString();
        }
    }


    class TestCompress
    {
        public void TestZip()
        {
            var folder = @"F:\HRP";
            var di = new DirectoryInfo(folder);
            var files = di.GetFiles("*.*", SearchOption.AllDirectories);

            Compress.Zip("test1.zip", folder);
            Compress.AppendFileToZip("test1.zip", new FileInfo("F:/readme.txt"));
            //Compress.Zip("test2.zip", files, folder);
            //Compress.Zip("test3.zip", files.Select(x => x.FullName).ToArray(), folder);
        }

        public void TestUnZip()
        {
            Compress.UnZip("test1.zip", "test1");
            //Compress.UnZip("test2.zip", "test2");
            //Compress.UnZip("test3.zip", "test3");
        }
    }

    class Log : ILog, IWatch
    {
        public void Info(string message)
        {
            Console.WriteLine("信息：" + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine("警告：" + message);
        }

        public void Error(string message)
        {
            Console.WriteLine("错误：" + message);
        }

        public void SetStep(string stepInfo)
        {
            Console.WriteLine("步骤：" + stepInfo);
        }

        public void SetMaxValue(int maxValue, int value)
        {
        }

        public void AddValue(int value)
        {
        }

        public void Complete()
        {
            Console.WriteLine("全部成功。");
        }

        public void Faild()
        {
            Console.WriteLine("失败。");
        }
    }

    class TestShortCut
    {
        public void CreateShortcutFile()
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/test.lnk";
            ShortcutHelper.CreateShortcutFile(dir, "C:/windows/notepad.exe");
        }
    }

    class Reg
    {
        public static void Test()
        {
            var regInfo = new RegInfo
            {
                DisplayName = "AnotepadApp",
                DisplayIcon = "C:\\Windows\\notepad.exe",
                DisplayVersion = "1.0",
                ExeArgs = "",
                ExePath = "C:\\Windows\notepad.exe",
                ExeKey = "AnotepadApp.exe",
                IconGroupName = "icon",
                InstallPath = "C:\\Windows",
                UninstallString = "C:\\Windows\\notepad.exe",
                Publisher = "Publisher",
                EstimatedSize = 20100,
                URLInfoAbout = "http://baidu.com",
                AppKey = "AnotepadApp"
            };
            var flag = RegisterHelper.Write(regInfo);

            RegisterHelper.Delete(RegInfo.ProductUninstKey, "AnotepadApp");
            RegisterHelper.Delete(RegInfo.ProductDirRegkey, "AnotepadApp.exe");
        }
    }

    class CharTest
    {
        private int index = 0;
        private int pagesize = 100;
        public void Run()
        {
            bool go = false;
            do
            {
                for (int i = 0; i < pagesize; i++)
                {
                    if (index >= char.MaxValue)
                    {
                        Console.WriteLine("没有下一页了");
                        return;
                    }
                    Console.Write(index);
                    Console.Write("\t");
                    Console.Write(index.ToString("x8"));
                    Console.Write("\t");
                    Console.Write((char)index);

                    if (i % 3 == 2)
                    {
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("\t\t");
                    }
                    index++;
                }
                Console.WriteLine();
                Console.Write("Y继续，其他键退出：");
                var ley = Console.ReadKey();
                Console.WriteLine();
                go = ley.KeyChar == 'Y' || ley.KeyChar == 'y';
            } while (go);
        }
    }
}
