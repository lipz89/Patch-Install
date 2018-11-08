using System;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class FrmPages : Form, IStep
    {
        private readonly DlgPage dlgCommand;
        private readonly DlgPagePlugins dlgPagePlugins;
        private bool isItemAdded;
        private bool isInitChanged;

        public FrmPages()
        {
            InitializeComponent();

            dlgCommand = new DlgPage();
            dlgPagePlugins = new DlgPagePlugins();

            this.btnAdd.Click += BtnAdd_Click;
            this.btnEdit.Click += BtnEdit_Click;
            this.btnDelete.Click += BtnDelete_Click;
            this.btnUp.Click += BtnUp_Click;
            this.btnDown.Click += BtnDown_Click;
            this.lvCmds.MouseDoubleClick += LvCmds_MouseDoubleClick;
            this.lvCmds.SelectedIndexChanged += LvCmds_SelectedIndexChanged;
            this.btnDelete.Enabled = false;
            this.btnEdit.Enabled = false;
            this.btnUp.Enabled = false;
            this.btnDown.Enabled = false;
            this.btnPlugins.Click += BtnPlugins_Click;
        }

        private void BtnPlugins_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == dlgPagePlugins.ShowDialog(this))
            {
                if (dlgPagePlugins.Infos != null && dlgPagePlugins.Infos.Any())
                {
                    foreach (var file in dlgPagePlugins.Infos)
                    {
                        ListViewItem item = lvCmds.Items.OfType<ListViewItem>()
                            .FirstOrDefault(x => x.Text == file.Title);
                        if (item == null)
                        {
                            item = new ListViewItem(file.Title)
                            {
                                Tag = file
                            };
                            item.SubItems.Add(file.Keys);
                            this.lvCmds.Items.Add(item);
                            this.OnChanged?.Invoke(sender, e);
                        }
                    }
                }
            }
        }

        public string Title { get; } = "创建自定义页面";
        public string SubTitle { get; } = "通过自定义页面设置安装时的步骤，这些步骤将逐个显示在安装页面前面。";
        private Context Context { get; set; }

        private void LvCmds_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnDelete.Enabled = lvCmds.SelectedItems.Count > 0;
            this.btnEdit.Enabled = lvCmds.SelectedItems.Count > 0;
            this.btnUp.Enabled = lvCmds.SelectedItems.Count > 0 && lvCmds.SelectedIndices[0] > 0;
            this.btnDown.Enabled = lvCmds.SelectedItems.Count > 0 && lvCmds.SelectedIndices[0] < lvCmds.Items.Count - 1;
        }

        private void LvCmds_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.BtnEdit_Click(sender, e);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (dlgCommand.ShowDialog(null, this) == DialogResult.OK)
            {
                var file = dlgCommand.PageInfo;
                ListViewItem item = lvCmds.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Title);
                var index = lvCmds.Items.Count;
                if (item != null)
                {
                    index = lvCmds.Items.IndexOf(item);
                    item.Remove();
                }

                item = new ListViewItem(file.Title)
                {
                    Tag = file
                };
                item.SubItems.Add(file.Keys);
                this.lvCmds.Items.Insert(index, item);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var listViewItem = lvCmds.SelectedItems[0];
            var file = listViewItem.Tag as PageInfo;
            if (dlgCommand.ShowDialog(file, this) == DialogResult.OK)
            {
                var index = lvCmds.Items.IndexOf(listViewItem);
                listViewItem.Remove();
                file = dlgCommand.PageInfo;

                ListViewItem item = lvCmds.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Title);
                if (item != null)
                {
                    index = lvCmds.Items.IndexOf(item);
                    item.Remove();
                }

                item = new ListViewItem(file.Title)
                {
                    Tag = file
                };
                item.SubItems.Add(file.Keys);
                this.lvCmds.Items.Insert(index, item);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var item = lvCmds.SelectedItems[0];
            var index = lvCmds.Items.IndexOf(item);
            lvCmds.SelectedItems[0].Remove();
            this.OnChanged?.Invoke(sender, e);
            if (lvCmds.Items.Count > index)
            {
                lvCmds.Items[index].Selected = true;
            }
            else if (lvCmds.Items.Count > 0)
            {
                lvCmds.Items[index - 1].Selected = true;
            }
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            var item = lvCmds.SelectedItems[0];
            var index = lvCmds.Items.IndexOf(item);
            if (index > 0)
            {
                lvCmds.SelectedItems[0].Remove();
                lvCmds.Items.Insert(index - 1, item);
                item.Selected = true;
                lvCmds.Focus();

                LvCmds_SelectedIndexChanged(sender, e);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            var item = lvCmds.SelectedItems[0];
            var index = lvCmds.Items.IndexOf(item);
            if (index < lvCmds.Items.Count - 1)
            {
                lvCmds.SelectedItems[0].Remove();
                lvCmds.Items.Insert(index + 1, item);
                item.Selected = true;
                lvCmds.Focus();

                LvCmds_SelectedIndexChanged(sender, e);
                this.OnChanged?.Invoke(sender, e);
            }
        }
        public void SetChangeHandler(EventHandler handler)
        {
            if (!isInitChanged)
            {
                this.OnChanged += handler;
                isInitChanged = true;
            }
        }

        public void SetContext(Context context, bool? isFromPrev = null)
        {
            this.Context = context;
            if (isFromPrev != true) { return; }
            if (!isItemAdded)
            {
                if (this.Context.Pages != null)
                {
                    foreach (var shortCut in this.Context.Pages.OrderBy(x => x.Index))
                    {
                        ListViewItem item = new ListViewItem(shortCut.Title);
                        item.SubItems.Add(shortCut.Keys);
                        item.Tag = shortCut;
                        this.lvCmds.Items.Add(item);
                    }
                }

                var plugins = PageHelper.GetPluginPages();
                this.btnPlugins.Enabled = plugins.Any();
                if (plugins.Any())
                {
                    dlgPagePlugins.SetPlugins(plugins);
                }

                isItemAdded = true;
            }
        }

        public void ChangeContext()
        {
            this.Context.Pages = lvCmds.Items
                    .OfType<ListViewItem>()
                    .Select(x => x.Tag)
                    .OfType<PageInfo>()
                    .Select((x, i) =>
                    {
                        x.Index = i;
                        return x;
                    })
                    .ToList();
        }

        public bool Check()
        {
            var allKeys = this.Context.PageItemKeys.ToList();
            var rekeys = allKeys.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).Where(x => x.Value > 1).Select(x => x.Key);
            if (rekeys.Any())
            {
                var msg = string.Join(Environment.NewLine, rekeys.ToArray());
                MessageBox.Show($"存在以下重复键，无法保存：{Environment.NewLine}{msg}。", "提示", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private event EventHandler OnChanged;
    }
}
