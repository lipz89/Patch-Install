using System.IO;

namespace QuickShare.Patch.Common.Util
{
    public static class FileHelper
    {
        public static void RemoveReadonly(string path)
        {
            if (File.Exists(path))
            {
                var attrs = File.GetAttributes(path);
                if ((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attrs = attrs & ~FileAttributes.ReadOnly;
                    File.SetAttributes(path, attrs);
                }
            }
        }
    }
}
