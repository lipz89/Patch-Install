using System;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class DlgFile : Form
    {
        public FileItem FileItem { get; set; }
        private readonly OpenFileDialog dlg = new OpenFileDialog();
        private string[] filePaths;

        public DlgFile()
        {
            InitializeComponent();

            this.dlg.Multiselect = false;
            this.dlg.Title = "选择文件";
            this.dlg.RestoreDirectory = false;
            filePaths = GlobalPath.GetFilePaths();
            this.cmbTargetDir.Items.AddRange(filePaths);
            if (this.cmbTargetDir.Items.Count > 0) this.cmbTargetDir.SelectedIndex = 0;
            this.cmbOverride.DataSource = EnumHelper.GetFileOverridePairs().ToList();
            this.cmbOverride.DisplayMember = "Value";
            this.cmbOverride.ValueMember = "Key";
            this.cmbOverride.SelectedValue = FileOverride.Try;

            this.btnSelect.Click += BtnSelect_Click;
            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFile.Text.Trim()))
            {
                MessageBox.Show("请选择要添加的文件。", "提示", MessageBoxButtons.OK);
                txtFile.Focus();
                return;
            }
            if (string.IsNullOrEmpty(cmbTargetDir.Text.Trim()))
            {
                MessageBox.Show("请选择要目标目录。", "提示", MessageBoxButtons.OK);
                cmbTargetDir.Focus();
                return;
            }
            var targetDir = cmbTargetDir.Text.Trim();
            if (filePaths.All(x => !targetDir.Contains(x)))
            {
                MessageBox.Show("目标目录不合法。", "提示", MessageBoxButtons.OK);
                cmbTargetDir.Focus();
                return;
            }
            this.FileItem.Path = txtFile.Text.Trim();
            this.FileItem.TargetDir = cmbTargetDir.Text.Trim();
            this.FileItem.FileOverride = (FileOverride)cmbOverride.SelectedValue;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public DialogResult ShowDialog(FileItem item, IWin32Window owner = null)
        {
            this.FileItem = item ?? new FileItem()
            {
                TargetDir = GlobalPath.GetFilePaths().FirstOrDefault(),
                FileOverride = FileOverride.Try
            };

            this.txtFile.Text = FileItem.Path;
            this.cmbTargetDir.SelectedItem = FileItem.TargetDir;
            this.cmbOverride.SelectedItem = FileItem.FileOverride;

            if (owner != null)
            {
                return this.ShowDialog(owner);
            }
            else
            {
                return this.ShowDialog();
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            this.dlg.FileName = this.FileItem.Path;

            if (this.dlg.ShowDialog(this) == DialogResult.OK)
            {
                this.txtFile.Text = dlg.FileName;
            }
        }
    }
}
