using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class UninstallData
    {
        public string InstallPath { get; set; }
        public string UninstallKey { get; set; }
        public string UninstallTarget { get; set; }
        public string UninstallArgs { get; set; }
        public string AppName { get; set; }
        public List<string> Shortcuts { get; set; }

        public void SaveToFile(string file)
        {
            var str = new StringBuilder();
            str.AppendLine($"UninstallKey={UninstallKey}");
            str.AppendLine($"InstallPath={InstallPath}");
            str.AppendLine($"AppName={AppName}");

            if (Shortcuts != null && Shortcuts.Any())
            {
                str.AppendLine("@@Files:");
                foreach (var shortcut in Shortcuts)
                {
                    str.AppendLine(shortcut);
                }
            }

            var text = str.ToString();
            BinarySerializer.Serialize(text, file);
        }

        public static UninstallData LoadFrom(string file)
        {
            var text = BinarySerializer.Deserialize<string>(file);
            var lines = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var data = new UninstallData() { Shortcuts = new List<string>() };
            var isFile = false;
            foreach (var line in lines)
            {
                if (isFile)
                {
                    data.Shortcuts.Add(line);
                }
                else if (line.Equals("@@Files:"))
                {
                    isFile = true;
                }
                else if (line.StartsWith("AppName="))
                {
                    data.AppName = line.Replace("AppName=", "");
                }
                else if (line.StartsWith("UninstallKey="))
                {
                    data.UninstallKey = line.Replace("UninstallKey=", "");
                }
                else if (line.StartsWith("InstallPath="))
                {
                    data.InstallPath = line.Replace("InstallPath=", "");
                }
            }

            return data;
        }
    }
}