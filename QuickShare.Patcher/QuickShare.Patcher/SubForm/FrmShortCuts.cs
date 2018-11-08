using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class FrmShortCuts : Form, IStep
    {
        private readonly DlgShortCut dlgShortCut;
        private bool isItemAdded;
        public FrmShortCuts()
        {
            InitializeComponent();

            this.dlgShortCut = new DlgShortCut();

            this.btnAdd.Click += BtnAdd_Click;
            this.btnEdit.Click += BtnEdit_Click;
            this.btnDelete.Click += BtnDelete_Click;
            this.lvShortCuts.MouseDoubleClick += LvShortCuts_MouseDoubleClick;
            this.lvShortCuts.SelectedIndexChanged += LvShortCuts_SelectedIndexChanged;
            this.btnDelete.Enabled = false;
            this.btnEdit.Enabled = false;
        }

        private void LvShortCuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnDelete.Enabled = lvShortCuts.SelectedItems.Count > 0;
            this.btnEdit.Enabled = lvShortCuts.SelectedItems.Count > 0;
        }

        private bool isInitChanged;

        private void LvShortCuts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.BtnEdit_Click(sender, e);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var item = lvShortCuts.SelectedItems[0];
            var index = lvShortCuts.Items.IndexOf(item);
            lvShortCuts.SelectedItems[0].Remove();
            this.OnChanged?.Invoke(sender, e);
            if (lvShortCuts.Items.Count > index)
            {
                lvShortCuts.Items[index].Selected = true;
            }
            else if (lvShortCuts.Items.Count > 0)
            {
                lvShortCuts.Items[index - 1].Selected = true;
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var listViewItem = lvShortCuts.SelectedItems[0];
            var file = listViewItem.Tag as ShortCutInfo;
            if (dlgShortCut.ShowDialog(file, this) == DialogResult.OK)
            {
                var index = lvShortCuts.Items.IndexOf(listViewItem);
                listViewItem.Remove();
                file = dlgShortCut.ShortCutInfo;

                ListViewItem item = lvShortCuts.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Key);
                if (item != null)
                {
                    index = lvShortCuts.Items.IndexOf(item);
                    item.Remove();
                }

                item = new ListViewItem(file.Key);
                item.SubItems.Add(file.Command);
                item.Tag = file;
                this.lvShortCuts.Items.Insert(index, item);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (dlgShortCut.ShowDialog(null, this) == DialogResult.OK)
            {
                var file = dlgShortCut.ShortCutInfo;
                ListViewItem item = lvShortCuts.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Key);
                var index = lvShortCuts.Items.Count;
                if (item != null)
                {
                    index = lvShortCuts.Items.IndexOf(item);
                    item?.Remove();
                }

                item = new ListViewItem(file.Key);
                item.SubItems.Add(file.Command);
                item.Tag = file;
                this.lvShortCuts.Items.Insert(index, item);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        public string Title { get; } = "开始菜单和快捷方式";
        public string SubTitle { get; } = "创建开始菜单文件夹和快捷方式。";
        private Context Context { get; set; }

        public void SetChangeHandler(EventHandler handler)
        {
            if (!isInitChanged)
            {
                this.OnChanged += handler;
                this.txtIconGroupName.TextChanged += Changed;
                this.chkCreateWebLnk.CheckedChanged += Changed;
                this.chkAllowChangIconGroup.CheckedChanged += Changed;
                this.chkCreateUninstallLnk.CheckedChanged += Changed;
                isInitChanged = true;
            }
        }

        public void SetContext(Context context, bool? isFromPrev=null)
        {
            this.Context = context;
            if (isFromPrev != true) { return; }

            this.dlgShortCut.Init(this.Context);
            this.Context.InstInfo.IconGroup = this.Context.AppInfo.AppName;
            this.chkCreateWebLnk.Checked = this.Context.AppInfo.WebSite != null;
            this.chkAllowChangIconGroup.Checked = Context.InstInfo.AllowChangeIconGroup;
            this.chkCreateWebLnk.Checked = Context.InstInfo.CreateWebSiteLnk;
            this.chkCreateUninstallLnk.Checked = Context.InstInfo.CreateUninstallLnk;
            this.txtIconGroupName.Text = this.Context.InstInfo.IconGroup;

            InitDefaultShortCuts();
            if (!isItemAdded)
            {
                if (this.Context.ShortCuts != null && this.Context.ShortCuts.Any())
                {
                    foreach (var shortCut in this.Context.ShortCuts)
                    {
                        ListViewItem item = new ListViewItem(shortCut.Key);
                        item.SubItems.Add(shortCut.Command);
                        item.Tag = shortCut;
                        this.lvShortCuts.Items.Add(item);
                    }

                    isItemAdded = true;
                }
            }
        }

        private void InitDefaultShortCuts()
        {
            var runable = this.Context.GetExecutables().FirstOrDefault();
            if (runable == null)
                return;
            if (this.Context.ShortCuts == null)
            {
                this.Context.ShortCuts = new List<ShortCutInfo>();
                this.Context.ShortCuts.Add(new ShortCutInfo
                {
                    Name = this.Context.AppInfo.AppName,
                    TargetDir = GlobalPath.Desktop,
                    Target = runable,
                });
                this.Context.ShortCuts.Add(new ShortCutInfo
                {
                    Name = this.Context.AppInfo.AppName,
                    TargetDir = GlobalPath.IconGroup,
                    Target = runable,
                });
                isItemAdded = false;
            }
        }
        public void ChangeContext()
        {
            this.Context.InstInfo.AllowChangeIconGroup = chkAllowChangIconGroup.Checked;
            this.Context.InstInfo.CreateWebSiteLnk = chkCreateWebLnk.Checked;
            this.Context.InstInfo.CreateUninstallLnk = chkCreateUninstallLnk.Checked;
            this.Context.InstInfo.IconGroup = this.txtIconGroupName.Text.Trim();

            if (lvShortCuts.Items.Count > 0)
            {
                this.Context.ShortCuts = lvShortCuts.Items.OfType<ListViewItem>().Select(x => x.Tag).OfType<ShortCutInfo>()
                        .ToList();
            }
        }

        public bool Check()
        {
            return true;
        }

        private event EventHandler OnChanged;

        private void Changed(object sender, EventArgs e)
        {
            this.OnChanged?.Invoke(sender, e);
        }
    }
}
