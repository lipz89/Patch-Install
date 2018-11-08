using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace QuickShare.Patch.Common.Util
{
    public static class Compress
    {
        private static readonly ArchiveEncoding ArchiveEncoding = new ArchiveEncoding() { Default = Encoding.Default };
        public static readonly WriterOptions WriterOptions = new WriterOptions(CompressionType.Deflate) { ArchiveEncoding = ArchiveEncoding };
        public static readonly ReaderOptions ReaderOptions = new ReaderOptions() { ArchiveEncoding = ArchiveEncoding };

        public static bool Zip(string tarFile, string folder)
        {
            if (string.IsNullOrEmpty(tarFile))
            {
                Global.Log?.Error($"没有指定目标文件");
                return false;
            }

            if (string.IsNullOrEmpty(folder))
            {
                Global.Log?.Error($"没有指定要压缩的文件目录");
                return false;
            }
            using (var archive = ZipArchive.Create())
            {
                try
                {
                    if (Directory.Exists(folder))
                    {
                        archive.AddAllFromDirectory(folder);
                        Global.Log?.Info("已添加所有文件，正在压缩...");
                        archive.SaveTo(tarFile, WriterOptions);
                    }
                    else
                    {
                        Global.Log?.Error($"没有找到文件夹 [{folder}]");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return false;
                }
            }

            return true;
        }
        public static bool Zip(string tarFile, string[] files, string rootPath = null)
        {
            var fis = files.Select(x => new FileInfo(x)).ToArray();
            return Zip(tarFile, fis, rootPath);
        }

        public static bool Zip(string tarFile, FileInfo[] files, string rootPath = null)
        {

            if (string.IsNullOrEmpty(tarFile))
            {
                Global.Log?.Error($"没有指定目标文件");
                return false;
            }

            if (files == null || !files.Any())
            {
                Global.Log?.Error($"没有指定要压缩的文件集合");
                return false;
            }

            if (!string.IsNullOrEmpty(rootPath))
            {
                rootPath = new DirectoryInfo(rootPath).FullName;
            }
            using (var archive = ZipArchive.Create())
            {
                try
                {
                    foreach (var file in files)
                    {
                        if (file.Exists)
                        {
                            Global.Log?.Info($"正在添加文件 [{file.FullName}]");
                            var key = file.FullName;
                            if (!string.IsNullOrEmpty(rootPath))
                            {
                                if (!key.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
                                {
                                    Global.Log?.Error($"排除文件 [{file.FullName}]");
                                    continue;
                                }

                                key = key.Remove(0, rootPath.Length);
                            }
                            archive.AddEntry(key, file.OpenRead(), true);
                        }
                        else
                        {
                            Global.Log?.Error($"没有找到文件 [{file}]");
                            return false;
                        }
                    }
                    Global.Log?.Info("已添加所有文件，正在压缩...");
                    archive.SaveTo(tarFile, WriterOptions);
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return false;
                }
            }

            return true;
        }

        public static bool Zip(string tarFile, Dictionary<string, string> files)
        {
            if (string.IsNullOrEmpty(tarFile))
            {
                Global.Log?.Error($"没有指定目标文件");
                return false;
            }

            if (files == null || !files.Any())
            {
                Global.Log?.Error($"没有指定要压缩的文件集合");
                return false;
            }

            using (var archive = ZipArchive.Create())
            {
                try
                {
                    foreach (var file in files)
                    {
                        if (File.Exists(file.Value))
                        {
                            Global.Log?.Info($"正在添加文件 [{file.Value}]");
                            archive.AddEntry(file.Key, File.OpenRead(file.Value), true);
                        }
                        else
                        {
                            Global.Log?.Error($"没有找到文件 [{file}]");
                            return false;
                        }
                    }
                    Global.Log?.Info("已添加所有文件，正在压缩...");
                    archive.SaveTo(tarFile, WriterOptions);
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return false;
                }
            }

            return true;
        }

        public static bool AppendFileToZip(string tarFile, FileInfo file)
        {
            var path = Path.GetTempFileName();
            using (var archive = ZipArchive.Open(tarFile,ReaderOptions))
            {
                try
                {
                    archive.AddEntry(file.Name, file.OpenRead());
                    archive.SaveTo(path, WriterOptions);
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return false;
                }
            }

            File.Copy(path, tarFile, true);
            return true;
        }
        public static bool AppendStreamToZip(string tarFile, string key, Stream file)
        {
            var path = Path.GetTempFileName();
            using (var archive = ZipArchive.Open(tarFile, ReaderOptions))
            {
                try
                {
                    archive.AddEntry(key, file);
                    archive.SaveTo(path, WriterOptions);
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return false;
                }
            }

            File.Copy(path, tarFile, true);
            return true;
        }

        public static bool UnZip(string tarFile, string targetFolder)
        {
            using (var archive = ZipArchive.Open(tarFile, ReaderOptions))
            {
                try
                {
                    archive.EntryExtractionBegin += (t, e) => Global.Log?.Info($"抽取 [{e.Item.Key}]");
                    var opts = new ExtractionOptions { ExtractFullPath = true, Overwrite = true };
                    archive.WriteToDirectory(targetFolder, opts);
                    return true;
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return false;
                }
            }
        }

        public static Stream UnZipToStream(string tarFile, string key)
        {
            using (var archive = ZipArchive.Open(tarFile, ReaderOptions))
            {
                try
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.Key == key)
                        {
                            using (var entryStream = entry.OpenEntryStream())
                            {
                                Global.Log?.Info($"抽取 [{entry.Key}]");
                                var stream = new MemoryStream();
                                const int bufSize = 0x1000;
                                byte[] buf = new byte[bufSize];
                                int bytesRead = 0;
                                while ((bytesRead = entryStream.Read(buf, 0, bufSize)) > 0)
                                    stream.Write(buf, 0, bytesRead);
                                stream.Seek(0, SeekOrigin.Begin);
                                return stream;
                            }
                        }
                    }

                    return null;
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return null;
                }
            }
        }
        public static string UnZipToTempFile(string tarFile, string key)
        {
            using (var archive = ZipArchive.Open(tarFile, ReaderOptions))
            {
                try
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.Key == key)
                        {
                            Global.Log?.Info($"抽取 [{entry.Key}]");
                            var path = Path.Combine(Path.GetTempPath(), key);
                            entry.WriteToFile(path, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                            return path;
                        }
                    }

                    return null;
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return null;
                }
            }
        }
        public static long GetUnZipSize(string tarFile)
        {
            using (var archive = ZipArchive.Open(tarFile, ReaderOptions))
            {
                try
                {
                    var size = archive.TotalUncompressSize;
                    return size;
                }
                catch (Exception e)
                {
                    Global.Log?.Error(e.Message);
                    return 0;
                }
            }
        }
    }
}