using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using QuickShare.Patch.Common.Commands;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Util
{
    public static class ContextUtil
    {

        public static string[] GetExecutables(this Context context)
        {
            if (context.Files == null || context.Files.Count == 0) return new string[0];

            return context.Files.Where(x => x.Key.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.IsMain).Select(x => x.Key)
                .ToArray();
        }
        public static string[] GetTexts(this Context context)
        {
            if (context.Files == null || context.Files.Count == 0) return new string[0];

            return context.Files.Where(x => x.Key.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)).Select(x => x.Key)
                .ToArray();
        }
        public static string[] GetCmds(this Context context)
        {
            if (context.Files == null || context.Files.Count == 0) return new string[0];
            Func<FileItem, bool> filter = x => x.Key.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                                               || x.Key.EndsWith(".bat", StringComparison.OrdinalIgnoreCase)
                                               || x.Key.EndsWith(".vbs", StringComparison.OrdinalIgnoreCase);
            var cmds = BaseCommand.GetCommands();
            var runables = context.Files.Where(filter).Select(x => x.Key).ToArray();
            return cmds.Concat(runables).ToArray();
        }

        public static bool SaveToFile(this Context context, string file)
        {
            if (file.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
            {
                return BinarySerializer.Serialize(context, file);
            }
            if (file.EndsWith(".psc", StringComparison.OrdinalIgnoreCase))
            {
                var json = JsonConvert.SerializeObject(context);
                File.WriteAllText(file, json);
                return true;
            }

            return false;
        }

        public static Context FromFile(string file)
        {
            if (file.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
            {
                var ctx = BinarySerializer.Deserialize<Context>(file);
                return ctx;
            }
            if (file.EndsWith(".psc", StringComparison.OrdinalIgnoreCase))
            {
                var json = File.ReadAllText(file);
                var ctx = JsonConvert.DeserializeObject<Context>(json);
                return ctx;
            }

            return null;
        }
        public static Stream SaveToStream(this Context context)
        {
            return BinarySerializer.Serialize(context);
        }

        private static readonly Regex rgPath = new Regex("\\$\\{.*?\\}");
        private static readonly Regex rgVar = new Regex("\\$\\[(.*?)\\]");
        public static string ConvertPath(this InstallContext context, string path)
        {
            if (path == null)
                return null;
            while (rgPath.IsMatch(path))
            {
                path = rgPath.Replace(path, match =>
                {
                    switch (match.Value)
                    {
                        case GlobalPath.Desktop:
                            if (context.InstInfo.ForAllUser)
                                return GetPath(25) + "\\";
                            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
                        case GlobalPath.StartMenu:
                            if (context.InstInfo.ForAllUser)
                                return GetPath(23) + "\\";
                            return Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\";
                        case GlobalPath.Startup:
                            if (context.InstInfo.ForAllUser)
                                return GetPath(24) + "\\";
                            return Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\";
                        case GlobalPath.ProgramFiles:
                            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\";
                        case GlobalPath.ApplicationData:
                            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\";
                        case GlobalPath.LocalApplicationData:
                            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\";
                        case GlobalPath.Templates:
                            return Environment.GetFolderPath(Environment.SpecialFolder.Templates) + "\\";
                        case GlobalPath.System:
                            return Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\";
                        case GlobalPath.InstallPath:
                            return context.InstInfo.InstallPath + "\\";
                        case GlobalPath.IconGroup:
                            return Path.Combine(GlobalPath.StartMenu, context.InstInfo.IconGroup) + "\\";
                    }

                    return match.Value;
                });
            }
            path = path.Replace("/", "\\").Replace("\\\\", "\\");
            while (path.Contains("\\\\"))
            {
                path = path.Replace("\\\\", "\\");
            }
            return path;
        }


        public static string ConvertVars(this InstallContext context, string var)
        {
            if (var == null) return null;
            var result = rgVar.Replace(var, match =>
            {
                switch (match.Groups[1].Value)
                {
                    case GlobalPath.AppName:
                        return context.AppInfo.AppName;
                    case GlobalPath.FullName:
                        return context.AppInfo.FullName;
                }

                var val = context.GetPageItemValue(match.Groups[1].Value);
                return val ?? match.Value;
            });
            return context.ConvertPath(result);
        }
        private static string GetPath(int nFolder)
        {
            StringBuilder sbPath = new StringBuilder(MAX_PATH);
            SHGetFolderPath(IntPtr.Zero, nFolder, IntPtr.Zero, 0, sbPath);
            return sbPath.ToString();
        }
        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
        private const int MAX_PATH = 260;
    }
}