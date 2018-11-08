using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Commands;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;
using StringComparer = QuickShare.Patch.Common.Commands.StringComparer;

namespace QuickShare.ScriptPatcher.SubForm
{
    public partial class FrmResult : Form, IStep
    {
        private PatchContext context;

        public FrmResult()
        {
            InitializeComponent();

            this.pnlInfo2.Visible = false;
            this.pnlInfo1.Dock = DockStyle.Fill;

            this.listView1.AddGroupRightClick(grp =>
            {
                var tag = grp.Tag as string;
                Helper.OpenFolder(tag);
            });
            this.listView1.MouseDoubleClick += ListView1_MouseDoubleClick;
            //this.listView1.ColumnClick += listView1_ColumnClick;
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                var item = this.listView1.SelectedItems[0];
                if (item.Tag is string tag)
                {
                    Helper.OpenFolder(tag);
                }
                else if (item.Tag is ScriptFile file)
                {
                    var x = 0;
                    var index = 0;
                    foreach (ColumnHeader column in this.listView1.Columns)
                    {
                        x += column.Width;
                        if (e.X < x && index < item.SubItems.Count)
                        {
                            break;
                        }
                        index++;
                    }
                    Helper.OpenFile(index < 3 ? file.File : file.StoreFile);
                }
            }
        }

        public string Title { get; } = "执行结果";
        public string SubTitle { get; } = "所有脚本执行的结果";
        public void SetContext(PatchContext context, EventHandler<InstallEventArgs> handler)
        {
            this.context = context;
        }

        public void ChangeContext()
        {
        }

        private readonly IDictionary<ScriptStatus, string> status = new Dictionary<ScriptStatus, string>
        {
            {ScriptStatus.UnRun,"未执行" },
            {ScriptStatus.UnChecked,"未选择" },
            {ScriptStatus.Failed,"失败" },
            {ScriptStatus.Ignore,"已忽略" },
            {ScriptStatus.Success,"成功" },
        };

        public void ActiveStep()
        {
            var success = this.context.Files.Count(x => x.Status == ScriptStatus.Success);
            var failed = this.context.Files.Count(x => x.Status == ScriptStatus.Failed);
            var ignore = this.context.Files.Count(x => x.Status == ScriptStatus.Ignore);
            var unrun = this.context.Files.Count(x => x.Status == ScriptStatus.UnRun);
            var info = $"成功 {success}；失败 {failed}；忽略 {ignore}；未执行 {unrun}";
            if (this.context.ShowDetails)
            {
                this.pnlInfo1.Dock = DockStyle.None;
                this.pnlInfo1.Visible = false;
                this.pnlInfo2.Dock = DockStyle.Fill;
                this.pnlInfo2.Visible = true;
                this.lblInfo2.Text = info;
            }
            else
            {
                this.pnlInfo2.Dock = DockStyle.None;
                this.pnlInfo2.Visible = false;
                this.pnlInfo1.Dock = DockStyle.Fill;
                this.pnlInfo1.Visible = true;
                this.listView1.Items.Clear();
                foreach (var group in this.context.Files.GroupBy(x => x.Group).OrderBy(x => x.Key, StringComparer.Default))
                {
                    var path = Path.Combine(this.context.Path, group.Key);
                    var lvg = new ListViewGroup(group.Key) { Tag = path };
                    this.listView1.Groups.Add(lvg);

                    foreach (var file in group.OrderBy(x => x.Name, StringComparer.Default))
                    {
                        var fitm = new ListViewItem(file.Name, lvg) { Tag = file };
                        fitm.SubItems.AddRange(new string[] { status[file.Status], file.File, file.StoreFile });
                        this.listView1.Items.Add(fitm);
                    }
                }

                this.listView1.EnableGroupEvent();
                this.lblInfo.Text = info;
            }
        }
    }
}
