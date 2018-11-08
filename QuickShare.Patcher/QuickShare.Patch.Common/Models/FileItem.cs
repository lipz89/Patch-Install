using System;
using System.IO;

namespace QuickShare.Patch.Common.Models
{
    [Serializable]
    public class FileItem
    {
        public string Path { get; set; }
        public string Name
        {
            get { return new FileInfo(Path).Name; }
        }
        public string TargetDir { get; set; }
        public FileOverride FileOverride { get; set; }

        public string Key { get; set; }
        public bool IsMain { get; set; }
    }
}