using System;
using System.Diagnostics;
using System.IO;

namespace QuickShare.Patch.Common.Util
{
    public class ShortcutHelper
    {
        /// <summary>
        /// 创建网站快捷方式
        /// </summary>
        /// <param name="path">快捷方式完全路径</param>
        /// <param name="url">URL地址</param>
        public static void CreateWebShortcutFile(string path, string url)
        {
            // Create shortcut file, based on Title
            var objWriter = File.CreateText(path);
            // Write URL to file
            objWriter.WriteLine("[InternetShortcut]");
            objWriter.WriteLine("URL=\"" + url + "\"");
            // Close file
            objWriter.Close();
        }

        private const string format = @"Set WshShell=CreateObject('WScript.Shell')
        Set oShellLink = WshShell.CreateShortcut('{0}')
        oShellLink.TargetPath='{1}'
        oShellLink.Arguments='{2}'
        oShellLink.WorkingDirectory='{3}'
        oShellLink.WindowStyle=1
        oShellLink.Description='{4}'
        oShellLink.Save";

        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="path">快捷方式完全路径</param>
        /// <param name="target">目标文件</param>
        /// <param name="args">参数</param>
        /// <param name="desc">描述</param>
        public static void CreateShortcutFile(string path, string target, string args = null, string desc = null)
        {
            target = target.Replace("\\", "/");
            var workDir = target.Remove(target.LastIndexOf("/"));
            var vbs = string.Format(format, path, target, args, workDir, desc);
            vbs = vbs.Replace("'", "\"");
            var fileName = Guid.NewGuid().ToString("N") + ".vbs";
            File.WriteAllText(fileName, vbs);
            var p = Process.Start(fileName);
            p.WaitForExit();
            File.Delete(fileName);
        }
    }
}