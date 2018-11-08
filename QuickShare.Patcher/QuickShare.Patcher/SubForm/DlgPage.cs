using System;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patcher.SubForm
{
    public partial class DlgPage : Form
    {
        public PageInfo PageInfo { get; private set; }
        private readonly DlgPageItem dlgPageItem;

        public DlgPage()
        {
            InitializeComponent();

            dlgPageItem = new DlgPageItem();

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
            this.btnPerview.Click += BtnPerview_Click;

            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
        }

        private void BtnPerview_Click(object sender, EventArgs e)
        {
            var page = GetPageInfo();
            if (page != null)
            {
                var form = new FrmMain(page);
                form.ShowDialog(this);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (dlgPageItem.ShowDialog(null, this) == DialogResult.OK)
            {
                var file = dlgPageItem.PageItem;
                ListViewItem item = lvCmds.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Key);
                var index = lvCmds.Items.Count;
                if (item != null)
                {
                    index = lvCmds.Items.IndexOf(item);
                    item?.Remove();
                }

                item = new ListViewItem(file.Key)
                {
                    Tag = file
                };
                item.SubItems.AddRange(new string[] { file.Label, file.Type.ToString() });
                this.lvCmds.Items.Insert(index, item);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var listViewItem = lvCmds.SelectedItems[0];
            var file = listViewItem.Tag as PageItem;
            if (dlgPageItem.ShowDialog(file, this) == DialogResult.OK)
            {
                var index = lvCmds.Items.IndexOf(listViewItem);
                listViewItem.Remove();
                file = dlgPageItem.PageItem;

                ListViewItem item = lvCmds.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Key);
                if (item != null)
                {
                    index = lvCmds.Items.IndexOf(item);
                    item.Remove();
                }

                item = new ListViewItem(file.Key)
                {
                    Tag = file
                };
                item.SubItems.AddRange(new string[] { file.Label, file.Type.ToString() });
                this.lvCmds.Items.Insert(index, item);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var item = lvCmds.SelectedItems[0];
            var index = lvCmds.Items.IndexOf(item);
            lvCmds.SelectedItems[0].Remove();
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
            }
        }

        private void LvCmds_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.BtnEdit_Click(sender, e);
        }

        private void LvCmds_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnDelete.Enabled = lvCmds.SelectedItems.Count > 0;
            this.btnEdit.Enabled = lvCmds.SelectedItems.Count > 0;
            this.btnUp.Enabled = lvCmds.SelectedItems.Count > 0 && lvCmds.SelectedIndices[0] > 0;
            this.btnDown.Enabled = lvCmds.SelectedItems.Count > 0 && lvCmds.SelectedIndices[0] < lvCmds.Items.Count - 1;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (GetPageInfo() == null) return;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private PageInfo GetPageInfo()
        {
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                MessageBox.Show("请填写页面标题。", "提示", MessageBoxButtons.OK);
                txtTitle.Focus();
                return null;
            }

            if (string.IsNullOrEmpty(txtSubTitle.Text.Trim()))
            {
                MessageBox.Show("请填写页面副标题。", "提示", MessageBoxButtons.OK);
                txtSubTitle.Focus();
                return null;
            }

            if (this.lvCmds.Items.Count == 0)
            {
                MessageBox.Show("请添加配置项。", "提示", MessageBoxButtons.OK);
                this.btnAdd.Focus();
                return null;
            }

            this.PageInfo.Title = txtTitle.Text.Trim();
            this.PageInfo.SubTitle = txtSubTitle.Text.Trim();
            this.PageInfo.EnableText = txtEnableText.Text.Trim();
            this.PageInfo.Items = lvCmds.Items
                .OfType<ListViewItem>()
                .Select(x => x.Tag)
                .OfType<PageItem>()
                .Select((x, i) =>
                {
                    x.Index = i;
                    return x;
                })
                .ToList();
            return this.PageInfo;
        }

        public DialogResult ShowDialog(PageInfo item, IWin32Window owner = null)
        {
            this.PageInfo = item ?? new PageInfo() { Editable = true };
            this.txtTitle.Text = this.PageInfo.Title;
            this.txtSubTitle.Text = this.PageInfo.SubTitle;
            this.txtEnableText.Text = this.PageInfo.EnableText;

            this.lvCmds.Items.Clear();
            if (PageInfo.Items != null)
            {
                foreach (var itemItem in PageInfo.Items.OrderBy(x => x.Index))
                {
                    var lbi = new ListViewItem(itemItem.Key) { Tag = itemItem };
                    lbi.SubItems.AddRange(new string[] { itemItem.Label, itemItem.Type.ToString() });
                    this.lvCmds.Items.Add(lbi);
                }
            }

            this.btnAdd.Enabled = PageInfo.Editable;
            this.btnDelete.Enabled = PageInfo.Editable;
            this.btnDown.Enabled = PageInfo.Editable;
            this.btnEdit.Enabled = PageInfo.Editable;
            this.btnUp.Enabled = PageInfo.Editable;
            this.lvCmds.Enabled = PageInfo.Editable;

            if (owner != null)
            {
                return this.ShowDialog(owner);
            }
            else
            {
                return this.ShowDialog();
            }
        }
    }
}
