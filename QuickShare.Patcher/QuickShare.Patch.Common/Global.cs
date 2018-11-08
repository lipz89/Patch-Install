using System.IO;

namespace QuickShare.Patch.Common
{
    public static class Global
    {
        public static ILog Log { get; set; }
        public static long GetFreeSize(string path)
        {
            try
            {
                var root = Path.GetPathRoot(path);
                long totalSize = 0;
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    if (drive.Name == root)
                    {
                        totalSize = drive.TotalFreeSpace;
                    }
                }
                return totalSize;
            }
            catch
            {
                return 0;
            }
        }

        private const int Unit = 1024;
        public static string GetSizeString(long size)
        {
            if (size < Unit)
            {
                return size.ToString("N1") + " B";
            }

            size /= Unit;
            if (size < Unit)
            {
                return size.ToString("N1") + " KB";
            }

            size /= Unit;
            if (size < Unit)
            {
                return size.ToString("N1") + " MB";
            }

            size /= Unit;
            return size.ToString("N1") + " GB";
        }
    }
}