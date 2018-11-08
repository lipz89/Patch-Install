using System;
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
    public partial class FrmDbObjects : Form, IStep
    {
        private PatchContext context;

        public FrmDbObjects()
        {
            InitializeComponent();

            this.listView1.MouseDoubleClick += ListView1_MouseDoubleClick;
            this.listView1.AddGroupRightClick(grp =>
            {
                var tag = grp.Tag as string;
                Helper.OpenFolder(tag);
            });
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 1)
            {
                var item = this.listView1.SelectedItems[0];
                var tag = item.Tag as string;
                Helper.OpenFile(tag);
            }
        }

        public string Title { get; } = "展示要执行的脚本";
        public string SubTitle { get; } = "按照执行顺序分组展示要执行的脚本";
        public void SetContext(PatchContext context, EventHandler<InstallEventArgs> handler)
        {
            this.context = context;
            this.InstallChanged += handler;
        }

        public void ChangeContext()
        {
        }

        public void ActiveStep()
        {
            if (this.context.Files != null && this.context.Files.Any())
            {
                this.listView1.Items.Clear();
                foreach (var group in this.context.Files.GroupBy(x => x.Group)
                    .OrderBy(x => x.Key, StringComparer.Default))
                {
                    var path = Path.Combine(this.context.Path, group.Key);
                    var lvg = new ListViewGroup(group.Key) { Tag = path };
                    this.listView1.Groups.Add(lvg);
                    foreach (var file in group.OrderBy(x => x.Name, StringComparer.Default))
                    {
                        var fitm = new ListViewItem(file.Name, lvg) { Tag = file.File };
                        fitm.SubItems.Add(file.File);
                        this.listView1.Items.Add(fitm);
                    }
                }
                this.listView1.EnableGroupEvent();

                this.lblInfo.Text = $"共计 {this.context.Files.Count} 个脚本";
            }
            else
            {
                this.InstallChanged?.Invoke(this, new InstallEventArgs()
                {
                    AllowNext = false
                });
                MessageBox.Show(this, "您选择的文件夹不包含脚本，或者未按规定方式组织脚本，\r\n请重新选择目录。", "提示", MessageBoxButtons.OK);
            }
        }
        private event EventHandler<InstallEventArgs> InstallChanged;
    }
}
