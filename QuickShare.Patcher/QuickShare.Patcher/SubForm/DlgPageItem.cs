using System;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class DlgPageItem : Form
    {
        public PageItem PageItem { get; private set; }

        public DlgPageItem()
        {
            InitializeComponent();
            this.cmbType.DataSource = EnumHelper.GetPageItemTypePairs().ToList();
            this.cmbType.DisplayMember = "Value";
            this.cmbType.ValueMember = "Key";
            this.cmbType.SelectedValue = PageItemType.TextBox;
            this.cmbDefault.Visible = false;
            this.cmbDefault.Items.AddRange(new[] { "False", "True" });
            this.cmbDefault.SelectedIndex = 0;
            this.cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;

            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = (PageItemType)cmbType.SelectedValue;
            if (type == PageItemType.CheckBox)
            {
                this.cmbDefault.Visible = true;
                this.cmbDefault.SelectedIndex = 0;
                this.txtDefault.Visible = false;
                this.chkIsRequired.Enabled = false;
            }
            else
            {
                this.cmbDefault.Visible = false;
                this.txtDefault.Visible = true;
                this.chkIsRequired.Enabled = true;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text.Trim()))
            {
                MessageBox.Show("请填写键名称。", "提示", MessageBoxButtons.OK);
                txtKey.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtLabel.Text.Trim()))
            {
                MessageBox.Show("请填写标签名称。", "提示", MessageBoxButtons.OK);
                txtLabel.Focus();
                return;
            }

            this.PageItem.Key = txtKey.Text.Trim();
            this.PageItem.Label = txtLabel.Text.Trim();
            this.PageItem.IsRequired = chkIsRequired.Checked;
            this.PageItem.Type = (PageItemType)cmbType.SelectedValue;
            if (this.PageItem.Type == PageItemType.CheckBox)
            {
                this.PageItem.DefaultValue = cmbDefault.Text.Trim();
            }
            else
            {
                this.PageItem.DefaultValue = txtDefault.Text.Trim();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public DialogResult ShowDialog(PageItem item, IWin32Window owner = null)
        {
            this.PageItem = item ?? new PageItem();
            this.txtKey.Text = this.PageItem.Key;
            this.txtLabel.Text = this.PageItem.Label;
            this.chkIsRequired.Checked = this.PageItem.IsRequired;
            this.cmbType.SelectedValue = this.PageItem.Type;
            this.txtDefault.Text = this.PageItem.DefaultValue;

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
