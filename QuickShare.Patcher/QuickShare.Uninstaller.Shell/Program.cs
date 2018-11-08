using System;
using System.IO;

namespace QuickShare.Uninstaller.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            Press("QuickShare.Uninstaller.exe", "QuickShare.Uninstaller.Core.exe", "unins.exe");
        }

        internal static void Press(string shellName, string innerName, string outName)
        {
            if (!File.Exists(shellName))
            {
                Console.WriteLine("没找到外壳程序。");
            }
            if (!File.Exists(innerName))
            {
                Console.WriteLine("没找到核心程序。");
            }

            var array1 = GetStream(shellName);
            var array2 = GetStream(innerName);
            Save(outName, array1, array2);
        }
        private static byte[] GetStream(string shellName)
        {
            Stream stream = File.OpenRead(shellName);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        private static void Save(string path, byte[] buffer, byte[] innerBuffer)
        {
            using (FileStream fStream = File.Create(path))
            {
                using (BinaryWriter bReader = new BinaryWriter(fStream))
                {
                    bReader.Write(buffer);
                    bReader.Write(innerBuffer);

                    fStream.Seek(64, SeekOrigin.Begin);
                    bReader.Write(buffer.Length);
                    bReader.Write(innerBuffer.Length);
                }
            }
        }
    }
}
