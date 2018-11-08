using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patch.Common.Commands
{
    internal partial class FrmDetails : Form
    {
        private static readonly IDictionary<ScriptStatus, string> status = new Dictionary<ScriptStatus, string>
        {
            {ScriptStatus.UnRun,"未执行" },
            {ScriptStatus.UnChecked,"未选择" },
            {ScriptStatus.Failed,"失败" },
            {ScriptStatus.Ignore,"已忽略" },
            {ScriptStatus.Success,"成功" },
        };
        private readonly SqlAdoExecutor.SqlHelper sqlHelper;
        private readonly ScriptFiles files;
        private readonly string rootPath;
        private Thread thRun;
        private Thread thWait;
        private DialogResult failedAction = DialogResult.None;
        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);
        private readonly string storePath;
        private readonly IWatch watch;
        private readonly Stack<string> newDirs;
        private readonly IDictionary<ScriptType, List<string>> newObjs;
        private readonly bool needStore;
        private bool hasError = false;
        private bool hasRunned = false;
        private bool isRunning = false;

        public CommandStatus CommandStatus { get; private set; } = CommandStatus.Inited;

        public FrmDetails()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            this.btnOk.Enabled = false;
            this.pnlControl.Visible = false;
            this.pnlError.Visible = false;
            this.Shown += FrmDetails_Shown;

            this.dataGridView1.AutoGenerateColumns = false;
            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnRetry.Click += BtnRetry_Click;
            this.btnAbort.Click += BtnAbort_Click;
            this.btnIgnore.Click += BtnIgnore_Click;

            this.btnRun.Click += BtnRun_Click;

            this.dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            this.dataGridView1.ChangeDataGridViewSelectedLinkColor(SystemColors.HighlightText);
            this.FormClosing += FrmDetails_FormClosing;
        }

        public FrmDetails(SqlAdoExecutor.SqlHelper sqlHelper, ScriptFiles files, string rootPath, string storePath, IWatch watch, Stack<string> newDirs, IDictionary<ScriptType, List<string>> newObjs) : this()
        {
            this.sqlHelper = sqlHelper;
            this.files = files;
            this.rootPath = rootPath;
            this.needStore = !string.IsNullOrEmpty(storePath);
            this.storePath = storePath;
            this.watch = watch;
            this.newDirs = newDirs;
            this.newObjs = newObjs;
            this.SetDetails();
        }

        private void FrmDetails_Shown(object sender, System.EventArgs e)
        {
            var groups = new Dictionary<Item, DataGridViewGroupCell>();
            var subitems = new Dictionary<Item, List<DataGridViewGroupCell>>();

            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                row.Selected = false;
                if (row.DataBoundItem is Item item && row.Cells[0] is DataGridViewGroupCell cell)
                {
                    if (item.IsGroup)
                    {
                        groups.Add(item, cell);
                    }
                    else
                    {
                        if (!subitems.ContainsKey(item.Parent))
                        {
                            subitems.Add(item.Parent, new List<DataGridViewGroupCell>());
                        }
                        subitems[item.Parent].Add(cell);
                    }
                }
            }

            foreach (var @group in groups)
            {
                var children = subitems[@group.Key];
                @group.Value.AddChildCellRange(children);
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            this.btnRun.Visible = false;
            thRun = new Thread(() => { Run(); });
            thRun.Start();
        }

        private void ToggleErrors(bool show)
        {
            this.pnlError.Visible = show;
            this.pnlControl.Visible = show;
        }

        private void Run()
        {
            this.CommandStatus = CommandStatus.Running;
            isRunning = true;
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (row.DataBoundItem is Item item)
                {
                    if (!item.IsGroup)
                    {
                        if (item.Status != ScriptStatus.UnRun)
                        {
                            continue;
                        }

                        if (row.Cells[0] is DataGridViewGroupCell cell && cell.ParentCell.Collapsed)
                        {
                            cell.ParentCell.Expand();
                        }
                        this.dataGridView1.CurrentCell = row.Cells[0];
                        row.Selected = true;

                        if (this.needStore)
                        {
                            var name = item.ScriptFile.GetObjectName();
                            if (item.Type == ScriptType.Proc || item.Type == ScriptType.Function ||
                                item.Type == ScriptType.View || item.Type == ScriptType.Trigger)
                            {
                                if (sqlHelper.IsExsit(name))
                                {
                                    var path = Path.Combine(storePath, item.Type.ToString());
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                        newDirs.Push(path);
                                    }
                                    item.Parent.StoreFile = path;

                                    var storeFile = Path.Combine(path, item.Name + ".sql");
                                    if (sqlHelper.StoreObj(storeFile, item.Type, name, null))
                                    {
                                        item.StoreFile = storeFile;
                                        item.ScriptFile.StoreFile = storeFile;
                                    }
                                }
                                else
                                {
                                    if (!this.newObjs.ContainsKey(item.Type))
                                    {
                                        this.newObjs.Add(item.Type, new List<string>());
                                    }

                                    this.newObjs[item.Type].Add(name);
                                }

                                watch?.AddValue(1);
                            }
                        }

                        var result = TryExecuteFile(item);

                        this.Invoke((Action<bool>)ToggleErrors, !result);
                        if (!result)
                        {
                            hasError = true;
                            break;
                        }

                        watch?.AddValue(1);
                    }
                }
            }

            if (!hasError)
            {
                this.Invoke((Action<bool>)ToggleErrors, false);
                var success = files.Count(x => x.Status == ScriptStatus.Success);
                var failed = files.Count(x => x.Status == ScriptStatus.Failed);
                var ignore = files.Count(x => x.Status == ScriptStatus.Ignore);
                var unrun = files.Count(x => x.Status == ScriptStatus.UnRun);
                var info = $"成功 {success}；失败 {failed}；忽略 {ignore}；未执行 {unrun}";
                lblInfo.Text = info;
                watch?.Info(info);
                this.btnOk.Enabled = true;
                this.btnCancel.Enabled = false;
                Application.DoEvents();
                this.CommandStatus = CommandStatus.Complete;
            }
            else
            {
                thWait = new Thread(() =>
                {
                    while (this.thRun.ThreadState != ThreadState.Stopped)
                    {
                        Thread.Sleep(10);
                    }
                    if (this.hasError && this.failedAction == DialogResult.Abort)
                    {
                        this.DialogResult = DialogResult.No;
                        this.Close();
                    }
                });
                thWait.Start();
                this.CommandStatus = CommandStatus.Failed;
            }

            isRunning = false;
            hasRunned = true;
            if (this.thRun != null && this.thRun.ThreadState != ThreadState.Stopped)
            {
                thRun.Abort();
            }
        }
        private bool TryExecuteFile(Item item)
        {
            this.lblInfo.Text = $"正在执行脚本 {item.Name}";
            bool retry;
            do
            {
                var flag = ExecuteFile(item.File, out string message);
                if (flag)
                {
                    item.Status = ScriptStatus.Success;
                    item.ScriptFile.Status = ScriptStatus.Success;
                    this.dataGridView1.Refresh();
                    Application.DoEvents();
                    return true;
                }

                retry = false;
                item.Status = ScriptStatus.Failed;
                this.lblError.Text = $"执行脚本失败，原因:{Environment.NewLine}{message}{Environment.NewLine}是否重试？";
                this.Invoke((Action<bool>)ToggleErrors, true);
                this.btnRetry.Focus();
                this.dataGridView1.Refresh();
                this.Activate();
                Application.DoEvents();

                resetEvent.WaitOne();
                if (failedAction == DialogResult.Abort)
                {
                    item.Status = ScriptStatus.Failed;
                    item.ScriptFile.Status = ScriptStatus.Failed;
                    return false;
                }

                if (failedAction == DialogResult.Retry)
                {
                    retry = true;
                }
                else if (failedAction == DialogResult.Ignore)
                {
                    item.Status = ScriptStatus.Ignore;
                    item.ScriptFile.Status = ScriptStatus.Ignore;
                    this.dataGridView1.Refresh();
                    Application.DoEvents();
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
                    return false;
                }
            }

            message = null;
            return true;
        }

        private void BtnRetry_Click(object sender, EventArgs e)
        {
            this.failedAction = DialogResult.Retry;
            this.resetEvent.Set();
        }

        private void BtnIgnore_Click(object sender, EventArgs e)
        {
            this.failedAction = DialogResult.Ignore;
            this.resetEvent.Set();
        }

        private void BtnAbort_Click(object sender, EventArgs e)
        {
            var dialog = MessageBox.Show(Owner, "中止运行会停止执行后续脚本，确定要中止吗？", "提示", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                this.failedAction = DialogResult.Abort;
                this.resetEvent.Set();
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            var row = dgv.Rows[e.RowIndex];
            if (row.DataBoundItem is Item item)
            {
                var col = this.dataGridView1.Columns[e.ColumnIndex];
                if (col == colPath)
                {
                    if (item.IsGroup)
                        Helper.OpenFolder(item.File);
                    else
                        Helper.OpenFile(item.File);
                }
                else if (col == colStorePath)
                {
                    if (item.IsGroup)
                        Helper.OpenFolder(item.StoreFile);
                    else
                        Helper.OpenFile(item.StoreFile);
                }
            }
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void FrmDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.isRunning)
            {
                var dialog = MessageBox.Show(Owner, "中止运行会停止执行后续脚本，确定要中止吗？", "提示", MessageBoxButtons.YesNo);
                if (dialog != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (hasRunned && !hasError)
            {
                this.DialogResult = DialogResult.OK;
            }

            if (this.thWait != null && this.thWait.ThreadState != ThreadState.Stopped)
            {
                thWait.Abort();
            }
            if (this.thRun != null && this.thRun.ThreadState != ThreadState.Stopped)
            {
                thRun.Abort();
            }
        }

        private void SetDetails()
        {
            var items = new List<Item>();
            foreach (var group in this.files.GroupBy(x => x.Group)
                .OrderBy(x => x.Key, StringComparer.Default))
            {
                var path = Path.Combine(rootPath, group.Key);

                var grp = new Item()
                {
                    Name = group.Key,
                    File = path,
                    IsGroup = true,
                    Children = new List<Item>()
                };
                items.Add(grp);
                foreach (var file in group.OrderBy(x => x.Name, StringComparer.Default))
                {
                    var fitm = new Item()
                    {
                        Name = file.Name,
                        File = file.File,
                        Type = file.Type,
                        Status = file.Status,
                        StoreFile = file.StoreFile,
                        ScriptFile = file,
                        Parent = grp
                    };
                    items.Add(fitm);
                    grp.Children.Add(fitm);
                }
            }

            this.dataGridView1.DataSource = items;
        }


        private class Item
        {
            public string File { get; set; }
            public ScriptType Type { get; set; }
            public bool IsGroup { get; set; }
            public string StoreFile { get; set; }
            public ScriptStatus? Status { get; set; }
            public string StatusStr
            {
                get
                {
                    return Status.HasValue ? status[Status.Value] : null;
                }
            }
            public string Name { get; set; }
            public ScriptFile ScriptFile { get; set; }
            public List<Item> Children { get; set; }
            public Item Parent { get; set; }
        }
    }
}
