using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace QuickShare.Uninstaller
{
    static class Program
    {
        [STAThread]
        private static void Main()
        {
            if (!GetAssembly(Application.ExecutablePath, out var rawAssembly))
            {
                MessageBox.Show("文件已被损坏。", "提示");
            }
            else
            {
                var path = Path.GetTempPath();
                var fileName = Guid.NewGuid().ToString("N") + ".exe";
                var file = Path.Combine(path, fileName);
                File.WriteAllBytes(file, rawAssembly);
                var pi = new ProcessStartInfo(file)
                {
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Verb = "runas"//管理员权限
                };
                Process.Start(pi);
            }
        }

        private static bool GetAssembly(string executablePath, out byte[] rawAssembly)
        {
            try
            {
                FileStream fileStream = new FileStream(executablePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                int length = (int)fileStream.Length;//总长度
                fileStream.Seek(64, SeekOrigin.Begin);
                BinaryReader binaryReader = new BinaryReader(fileStream);

                var len = binaryReader.ReadInt32();//壳长度
                var innerLength = length - len;
                fileStream.Seek(len, SeekOrigin.Begin);
                rawAssembly = new byte[innerLength];
                fileStream.Read(rawAssembly, 0, innerLength);
                fileStream.Close();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            rawAssembly = null;
            return false;
        }
    }
}
