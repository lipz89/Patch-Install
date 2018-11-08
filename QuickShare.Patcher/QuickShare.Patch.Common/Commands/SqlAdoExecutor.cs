using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patch.Common.Commands
{
    public class SqlAdoExecutor : BaseCommand
    {
        public static string CmdHelper = $@"请以文件夹名称组织脚本，其中视图，函数，存储过程，触发器文件夹命名要求如下：
    视图 序号.{ScriptType.View}--自定义视图相关脚本文件夹，文件名称应为序号.视图名称
    函数 序号.{ScriptType.Function}--自定义函数相关脚本文件夹，文件名称应为序号.函数名称
    存储过程 序号.{ScriptType.Proc}--自定义存储过程相关脚本文件夹，文件名称应为序号.存储过程名称
    触发器 序号.{ScriptType.Trigger}--自定义触发器相关脚本文件夹，文件名称应为序号.触发器名称
该命令将按照文件夹序号顺序执行文件夹中的脚本，
文件夹中按照文件名序号顺序执行脚本，
该命令可对现有的视图，函数，存储过程，触发器对象进行备份。";

        public static string CmdDesc = $@"该命令需要两个参数：
    1，path  //脚本存放文件夹
    2，showdetails:true/false  //配置是否显示明细界面，默认true

" + CmdHelper;

        public static string CmdKey = "$(SqlAdoExecutor)";

        private readonly IDictionary<ScriptType, List<string>> newObjs = new Dictionary<ScriptType, List<string>>();

        private static readonly IDictionary<ScriptType, string> keywords = new Dictionary<ScriptType, string>
        {
            {ScriptType.Proc, "PROCEDURE"},
            {ScriptType.Function, "FUNCTION"},
            {ScriptType.View, "VIEW"},
            {ScriptType.Trigger, "TRIGGER"}
        };

        private readonly Stack<string> newDirs = new Stack<string>();
        private string storePath;
        private string scriptPath;
        private string connectionString;
        private bool needStore;
        private SqlHelper sqlHelper;
        private ScriptFiles files;
        private bool isShowDetails = true;

        public override void SetContext(InstallContext context, IWatch watch, string name = null, params string[] args)
        {
            this.needStore = !context.IsFull;
            base.SetContext(context, watch, name, args);
        }

        public void Init(PatchContext context, IWatch watch)
        {
            this.Watch = watch;
            this.needStore = context.IsNeedStore;
            this.files = context.Files;
            this.scriptPath = context.Path;
            this.connectionString = context.ConnectionString;
            this.isShowDetails = context.ShowDetails;

            this.CanRun = InternalInit(); ;
            this.CanRollback = this.CanRun;
            this.CommandStatus = CommandStatus.Inited;
        }

        public override void Init()
        {
            if (initArgs != null && initArgs.Any())
            {
                files = GetFiles(initArgs[0]);
                this.scriptPath = initArgs[0];
                var cfg = initArgs.FirstOrDefault(x => x.StartsWith("showdetails:"));
                if (cfg != null)
                {
                    cfg = cfg.Replace("showdetails:", "").Trim();
                    isShowDetails = cfg.Equals("1") || cfg.Equals("true", StringComparison.OrdinalIgnoreCase);
                }
            }
            else
            {
                files = null;
            }

            this.CanRun = InternalInit();
            this.CanRollback = this.CanRun;
            this.CommandStatus = CommandStatus.Inited;
        }

        public override int GetProcessValue()
        {
            if (this.CanRun)
            {
                var processValue = files.Count(x => x.Status == ScriptStatus.UnRun);
                if (this.needStore)
                {
                    processValue += files.Count(x => x.Status == ScriptStatus.UnRun &&
                                                     (x.Type == ScriptType.Proc || x.Type == ScriptType.Function ||
                                                      x.Type == ScriptType.View || x.Type == ScriptType.Trigger));
                }

                return processValue;
            }

            return 0;
        }

        public override bool Run()
        {
            if (!CanRun)
            {
                return true;
            }


            Watch?.Info($"正在执行脚本");
            if (isShowDetails)
            {
                var details = new FrmDetails(sqlHelper, files, scriptPath, storePath, Watch, newDirs, newObjs);
                if (this.Owner is Form from)
                {
                    details.Icon = from.Icon;
                }
                var da = details.ShowDialog(Owner);
                this.CommandStatus = details.CommandStatus;
                var result = da == DialogResult.OK;
                if (result)
                {
                    Watch?.Info("全部脚本执行结束。");
                    this.CommandStatus = CommandStatus.Complete;
                }
                else
                {
                    Watch?.Faild();
                    this.CommandStatus = CommandStatus.Failed;
                }
                return result;
            }
            else
            {
                var reset = new AutoResetEvent(false);
                var result = false;
                new Thread(() =>
                {
                    result = TryExecute();
                    reset.Set();
                    Thread.CurrentThread.Abort();
                }).Start();

                this.CommandStatus = CommandStatus.Running;
                reset.WaitOne();
                if (!result)
                {
                    Watch?.Faild();
                    this.CommandStatus = CommandStatus.Failed;
                    return false;
                }

                this.CommandStatus = CommandStatus.Complete;
                return true;
            }
        }

        private bool InternalInit()
        {
            if (files != null && files.Count > 0)
            {
                Watch?.Info($"{files.Count(x => x.Status == ScriptStatus.UnRun)} 个待执行脚本");
            }
            else
            {
                Watch?.Info("没有待执行的脚本");
                return false;
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                if (ReadConfig(out string config))
                {
                    connectionString = config;
                }
                else
                {
                    Watch?.Info("未配置数据库，取消执行。");
                    return false;
                }
            }

            if (this.needStore)
            {
                var dir = $"Store{DateTime.Now:yyyyMMdd}";
                storePath = Path.Combine(scriptPath, dir);
                if (!Directory.Exists(storePath))
                {
                    Directory.CreateDirectory(storePath);
                    newDirs.Push(storePath);
                }
            }
            else
            {
                storePath = null;
            }

            this.sqlHelper = new SqlHelper(connectionString);
            return true;
        }

        private bool TryExecute()
        {
            if (files.Any(x => x.Status == ScriptStatus.UnRun))
            {
                foreach (var script in files.ToSorted())
                {
                    if (script.Status == ScriptStatus.UnChecked)
                    {
                        continue;
                    }
                    var name = script.GetObjectName();
                    string storeFile = null;
                    if (this.needStore)
                    {
                        if (script.Type == ScriptType.Proc || script.Type == ScriptType.Function ||
                            script.Type == ScriptType.View || script.Type == ScriptType.Trigger)
                        {
                            if (sqlHelper.IsExsit(name))
                            {
                                var path = Path.Combine(storePath, script.Type.ToString());
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                    newDirs.Push(path);
                                }

                                storeFile = Path.Combine(path, script.Name + ".sql");
                                sqlHelper.StoreObj(storeFile, script.Type, name, Watch);
                            }
                            else
                            {
                                if (!this.newObjs.ContainsKey(script.Type))
                                {
                                    this.newObjs.Add(script.Type, new List<string>());
                                }

                                this.newObjs[script.Type].Add(name);
                            }

                            Watch?.AddValue(1);
                        }
                    }

                    Watch?.Info($"执行脚本 {name} , {script.File}");
                    var result = TryExecuteFile(script);
                    this.files.Update(script.File, script.Status, storeFile);
                    if (!result)
                    {
                        return false;
                    }

                    Watch?.AddValue(1);
                }
            }

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
            if (this.needStore)
            {
                Watch?.Info("正在还原数据库对象");
                Restore(ScriptType.Trigger);
                Restore(ScriptType.Proc);
                Restore(ScriptType.Function);
                Restore(ScriptType.View);

                Watch?.Info("正在删除新增数据库对象");
                foreach (var type in newObjs.Keys)
                {
                    foreach (var name in newObjs[type])
                    {
                        sqlHelper.DeleteObj(type, name);
                        Watch?.AddValue(-1);
                    }
                }

                Watch?.Info("正在删除数据库对象备份文件夹");
                while (newDirs.Count > 0)
                {
                    var dir = newDirs.Pop();
                    Directory.Delete(dir, true);
                }
            }

            this.RollbackStatus = RollbackStatus.Complete;
            return true;
        }

        private void Restore(ScriptType type)
        {
            var path = Path.Combine(storePath, type.ToString());
            if (Directory.Exists(path))
            {
                var scripts = Directory.GetFiles(path, "*.sql", SearchOption.TopDirectoryOnly);
                foreach (var script in scripts)
                {
                    ExecuteFile(script, out string message);
                    File.Delete(script);
                    Watch?.AddValue(-2);
                }
            }
        }


        public static ScriptFiles GetFiles(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return null;
            }

            var scriptPath = Path.Combine(Environment.CurrentDirectory, dir);
            var pathDir = new DirectoryInfo(scriptPath);
            if (!pathDir.Exists)
            {
                return null;
            }

            var result = new ScriptFiles();
            var subFolders = pathDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var subFolder in subFolders)
            {
                var groupName = subFolder.Name;
                var type = ScriptType.Other;
                foreach (var key in keywords.Keys)
                {
                    if (groupName.EndsWith("." + key, StringComparison.OrdinalIgnoreCase))
                    {
                        type = key;
                    }
                }

                var scripts = subFolder.GetFiles("*.sql", SearchOption.TopDirectoryOnly);
                if (scripts.Length > 0)
                {
                    foreach (var fileInfo in scripts)
                    {
                        var scf = new ScriptFile()
                        {
                            Group = groupName,
                            File = fileInfo.FullName,
                            Type = type,
                            Status = ScriptStatus.UnRun
                        };
                        result.Add(scf);
                    }
                }
            }
            return result;
        }

        private bool ReadConfig(out string args)
        {
            if (FrmSqlConfig.GetConfig(this.Owner, out string server, out string dbName, out string userId,
                out string password))
            {
                args =
                    $"data source={server};initial catalog={dbName};user id={userId};password={password};MultipleActiveResultSets=True";
                return true;
            }

            args = null;
            return false;
        }

        private bool TryExecuteFile(ScriptFile file)
        {
            bool retry;
            do
            {
                var flag = ExecuteFile(file.File, out string message);
                if (flag)
                {
                    file.Status = ScriptStatus.Success;
                    return true;
                }

                retry = false;
                var dr = Helper.AbortRetryIgnore(this.Owner, $"执行脚本失败，是否重试？失败原因:{Environment.NewLine}{message}");
                if (dr == DialogResult.Abort)
                {
                    Watch?.Error($"执行脚本失败，失败原因:{message}");
                    Watch?.Faild();
                    file.Status = ScriptStatus.Failed;
                    return false;
                }

                if (dr == DialogResult.Retry)
                {
                    Watch?.Info("重试该脚本。");
                    retry = true;
                }
                else if (dr == DialogResult.Ignore)
                {
                    Watch?.Info("忽略该脚本。");
                    file.Status = ScriptStatus.Ignore;
                    return true;
                }
            } while (retry);

            return true;
        }

        private bool ExecuteFile(string file, out string message)
        {
            var statements = sqlHelper.ParseCommands(file);
            foreach (var statement in statements)
            {
                if (!sqlHelper.TryExecuteNonQuery(statement, out message))
                {
                    Watch?.Error(message.Trim());
                    return false;
                }
            }

            message = null;
            return true;
        }
        internal class SqlHelper
        {
            private readonly SqlConnection connection;

            public SqlHelper(string connectionString)
            {
                connection = new SqlConnection(connectionString);
            }

            public void DeleteObj(ScriptType type, string name)
            {
                string sql = $"DROP {keywords[type]} {name}";
                TryExecuteNonQuery(sql, out string message);
            }

            public bool StoreObj(string path, ScriptType type, string name, IWatch watch)
            {
                if (TrySqlHelpText(name, out List<string> lines, out string message))
                {
                    lines.Add("GO");
                    lines.Insert(0, "GO");
                    lines.Insert(0, $"\tDROP {keywords[type]} {name}");
                    lines.Insert(0, $"IF EXISTS (SELECT 1 FROM sys.objects WHERE name='{name}')");
                    File.WriteAllLines(path, lines.ToArray(), Encoding.UTF8);
                    return true;
                }
                else
                {
                    watch?.Error(message);
                    return false;
                }
            }

            public bool IsExsit(string name)
            {
                try
                {
                    var sql = $"SELECT COUNT(*) FROM sys.objects WHERE name = '{name}'";
                    connection.Open();
                    var cmd = new SqlCommand(sql, connection);
                    var obj = cmd.ExecuteScalar();
                    return obj.ToString() == "1";
                }
                finally
                {
                    connection.Close();
                }
            }

            public bool TryExecuteScalar(string sql, out object obj, out string message)
            {
                try
                {
                    connection.Open();
                    var cmd = new SqlCommand(sql, connection);
                    obj = cmd.ExecuteScalar();
                    message = null;
                    return true;
                }
                catch (Exception e)
                {
                    obj = null;
                    message = e.Message;
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }

            public bool TryExecuteNonQuery(string sql, out string message)
            {
                try
                {
                    connection.Open();
                    var cmd = new SqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                    message = null;
                    return true;
                }
                catch (Exception e)
                {
                    message = e.Message;
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }

            public bool TrySqlHelpText(string name, out List<string> lines, out string message)
            {
                try
                {
                    var sql = $"EXEC sys.sp_helptext @objname = N'{name}'";
                    connection.Open();
                    var cmd = new SqlCommand(sql, connection);
                    var reader = cmd.ExecuteReader();
                    lines = new List<string>();
                    while (reader.Read())
                    {
                        var line = reader.GetString(0);
                        line = line.TrimEnd();
                        lines.Add(line);
                    }

                    message = null;
                    return true;
                }
                catch (Exception e)
                {
                    message = e.Message;
                    lines = null;
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }

            public IEnumerable<string> ParseCommands(string filePath)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        while (true)
                        {
                            var statement = ReadNextStatementFromStream(reader);
                            if (statement == null)
                            {
                                yield break;
                            }

                            if (!string.IsNullOrEmpty(statement))
                            {
                                yield return statement.Trim();
                            }
                        }
                    }
                }
            }

            private string ReadNextStatementFromStream(StreamReader reader)
            {
                var builder = new StringBuilder();

                while (true)
                {
                    var lineOfText = reader.ReadLine();
                    if (lineOfText == null)
                    {
                        if (builder.Length > 0)
                            return builder.ToString();

                        return null;
                    }

                    lineOfText = lineOfText.Trim();

                    if (lineOfText.ToUpper() == "GO")
                        break;

                    if (string.IsNullOrEmpty(lineOfText) || lineOfText.StartsWith("--") ||
                        lineOfText.StartsWith("use", StringComparison.OrdinalIgnoreCase))
                        continue;

                    builder.Append(lineOfText + Environment.NewLine);
                }

                return builder.ToString();
            }
        }
    }

    public enum ScriptType
    {
        Proc,
        Function,
        View,
        Trigger,
        Other,
    }

    public enum ScriptStatus
    {
        UnChecked,
        UnRun,
        Ignore,
        Failed,
        Success,
    }

    public class ScriptFile
    {
        public string File { get; set; }
        public ScriptType Type { get; set; }
        public string Group { get; set; }
        public string StoreFile { get; set; }
        public ScriptStatus Status { get; set; }
        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(File); }
        }

        public string GetObjectName()
        {
            var rst = Name;
            if (!rst.Contains("."))
            {
                return rst;
            }

            var sp = rst.Split(new[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
            rst = sp[1].Trim();
            if (string.IsNullOrEmpty(rst))
            {
                return Name;
            }

            return rst;
        }
    }

    public static class ListExtension
    {
        public static List<ScriptFile> ToSorted(this IEnumerable<ScriptFile> list)
        {
            return list.OrderBy(x => x.Group, StringComparer.Default).ThenBy(x => x.Name, StringComparer.Default)
                .ToList();
        }
    }

    public class ScriptFiles : List<ScriptFile>
    {
        public void Update(string file, ScriptStatus status, string storePath)
        {
            var first = this.FirstOrDefault(x => x.File == file);
            if (first != null)
            {
                first.Status = status;
                first.StoreFile = storePath;
            }
        }
    }

    public class StringComparer : IComparer<string>
    {
        private StringComparer()
        {
        }

        private static IComparer<string> _default;

        public static IComparer<string> Default
        {
            get { return _default ?? (_default = new StringComparer()); }
        }

        public int Compare(string x, string y)
        {
            if (x == null && y == null) { return 0; }
            if (x == null) { return 1; }
            if (y == null) { return -1; }

            var xv = x.Split('.')[0];
            var yv = y.Split('.')[0];
            if (xv.Length != yv.Length)
            {
                return xv.Length - yv.Length;
            }

            return string.Compare(xv, yv, StringComparison.OrdinalIgnoreCase);
        }
    }
}