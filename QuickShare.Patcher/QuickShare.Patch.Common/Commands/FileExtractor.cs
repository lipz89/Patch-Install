using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace QuickShare.Patch.Common.Commands
{
    public class FileExtractor : BaseCommand
    {
        private string storePath;
        private List<string> newFiles = new List<string>();
        private Stack<string> newDirs = new Stack<string>();
        public override void Init()
        {
            if (InstallContext.Files != null && InstallContext.Files.Any())
            {
                this.CanRun = true;
            }

            if (!InstallContext.IsFull)
            {
                this.CanRollback = true;
            }
            this.CommandStatus = CommandStatus.Inited;
        }

        public override int GetProcessValue()
        {
            if (this.CanRun)
            {
                var val = InstallContext.Files.Count;
                if (!InstallContext.IsFull)
                {
                    val += 10;
                }

                if (InstallContext.IsMakeStore)
                {
                    val += InstallContext.Files.Count;
                }

                return val;
            }

            return 0;
        }

        public override bool Run()
        {
            if (!CanRun)
            {
                return true;
            }
            this.CommandStatus = CommandStatus.Running;
            if (InstallContext.IsMakeStore)
            {
                Watch?.Info("正在备份历史文件");
                this.Store();
                Watch?.AddValue(10);
            }
            else if (!Directory.Exists(InstallContext.InstInfo.InstallPath))
            {
                Directory.CreateDirectory(InstallContext.InstInfo.InstallPath);
                newDirs.Push(InstallContext.InstInfo.InstallPath);
            }

            Watch?.Info("开始文件提取");
            if (!UnZip(InstallContext.PackageFile, InstallContext.Files, InstallContext.InstInfo.InstallPath))
            {
                Watch?.Faild();
                this.CommandStatus = CommandStatus.Failed;
                return false;
            }
            Watch?.Info("文件提取完成");
            this.CommandStatus = CommandStatus.Complete;
            return true;
        }

        public override bool Rollback()
        {
            if (!this.CanRollback)
            {
                return true;
            }
            if (this.CommandStatus != CommandStatus.Complete &&
                this.CommandStatus != CommandStatus.Failed &&
                this.CommandStatus != CommandStatus.Running)
            {
                return true;
            }

            this.RollbackStatus = RollbackStatus.Rollbacking;
            if (InstallContext.IsMakeStore)
            {
                Watch?.Info("正在还原历史文件");
                this.ReStore();
                Watch?.AddValue(-10);
            }

            Watch?.SetStep("正在删除文件");
            foreach (var newFile in newFiles)
            {
                File.Delete(newFile);
                Watch?.AddValue(-1);
            }

            while (newDirs.Count > 0)
            {
                var dir = newDirs.Pop();
                Directory.Delete(dir, true);
            }

            this.RollbackStatus = RollbackStatus.Complete;
            return true;
        }

        private bool UnZip(string tarFile, List<FileItem> files, string installPath)
        {
            using (var archive = ZipArchive.Open(tarFile, Compress.ReaderOptions))
            {
                try
                {
                    var entries = archive.Entries.ToDictionary(x => x.Key.Replace("/", "\\"));
                    ZipArchiveEntry entry;
                    ExtractionOptions extractionOptions = new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true,
                        PreserveFileTime = true
                    };
                    if (entries.ContainsKey(Context.UninstallerKey))
                    {
                        entry = entries[Context.UninstallerKey];
                        var path = Path.Combine(installPath, Context.UninstallerKey);
                        entry.WriteToFile(path, extractionOptions);
                    }
                    foreach (var file in files)
                    {
                        if (entries.TryGetValue(file.Key, out entry))
                        {
                            Watch?.Info($"抽取 [{file.Name}]");
                            var path = file.Path;
                            FileHelper.RemoveReadonly(path);
                            var info = new FileInfo(path);
                            if (!Directory.Exists(info.Directory.FullName))
                            {
                                Directory.CreateDirectory(info.Directory.FullName);
                                newDirs.Push(info.Directory.FullName);
                            }

                            var flag = ExtractionFile(entry, info, file.FileOverride, extractionOptions);
                            if (!flag)
                            {
                                return false;
                            }
                        }

                        Watch?.AddValue(1);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Watch?.Error(e.Message);
                    return false;
                }
            }
        }

        private bool ExtractionFile(ZipArchiveEntry entry, FileInfo info, FileOverride fileOverride, ExtractionOptions extractionOptions)
        {
            var isExist = info.Exists;
            if (fileOverride == FileOverride.Never && isExist)
            {
                Watch?.Info($"文件已存在 [{info.FullName}]。");
                return true;
            }
            if (fileOverride == FileOverride.Newer && isExist && entry.LastModifiedTime <= info.LastWriteTime)
            {
                Watch?.Info($"文件已存在 [{info.FullName}]。");
                return true;
            }

            bool retry;
            do
            {
                retry = false;
                try
                {
                    entry.WriteToFile(info.FullName, extractionOptions);
                    if (!isExist)
                    {
                        newFiles.Add(info.FullName);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    if (fileOverride == FileOverride.Try)
                    {
                        Watch?.Info($"覆盖文件失败，自动忽略并继续。失败原因:{e.Message}");
                        return true;
                    }
                    var dr = Helper.AbortRetryIgnore(this.Owner, $"覆盖文件失败，是否重试？失败原因:{Environment.NewLine}{e.Message}");
                    if (dr == DialogResult.Abort)
                    {
                        Watch?.Error($"覆盖文件失败，失败原因:{e.Message}");
                        Watch?.Faild();
                        return false;
                    }
                    if (dr == DialogResult.Retry)
                    {
                        Watch?.Info($"覆盖文件失败，正在重试。失败原因:{e.Message}");
                        retry = true;
                    }
                    else if (dr == DialogResult.Ignore)
                    {
                        Watch?.Info($"覆盖文件失败，忽略该操作。失败原因:{e.Message}");
                        return true;
                    }
                }
            } while (retry);

            return true;
        }



        private void Store()
        {
            var dir = $"Store{DateTime.Now:yyyyMMdd}";
            storePath = Path.Combine(this.InstallContext.InstInfo.InstallPath, dir);
            if (!Directory.Exists(storePath))
            {
                Directory.CreateDirectory(storePath);
                newDirs.Push(storePath);
            }
            foreach (var file in InstallContext.Files)
            {
                if (File.Exists(file.Path))
                {
                    var name = file.Path.Replace(InstallContext.InstInfo.InstallPath, storePath);
                    var info = new FileInfo(name);
                    if (!Directory.Exists(info.Directory.FullName))
                    {
                        Directory.CreateDirectory(info.Directory.FullName);
                        newDirs.Push(info.Directory.FullName);
                    }
                    File.Copy(file.Path, name);
                    Watch?.AddValue(1);
                }
            }
        }
        private void ReStore()
        {
            var files = Directory.GetFiles(storePath, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var name = file.Replace(storePath, InstallContext.InstInfo.InstallPath);
                File.Copy(file, name, true);
                File.Delete(file);
                Watch?.AddValue(-1);
            }
        }
    }
}