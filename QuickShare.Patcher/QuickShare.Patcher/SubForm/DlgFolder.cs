using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class DlgFolder : Form
    {
        public List<FileItem> FileItems { get; set; }
        private readonly FolderBrowserDialog dlg = new FolderBrowserDialog();
        private string[] filePaths;

        public DlgFolder()
        {
            InitializeComponent();

            this.dlg.Description = "选择文件夹";
            this.dlg.ShowNewFolderButton = false;
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
            if (string.IsNullOrEmpty(txtFolder.Text.Trim()))
            {
                MessageBox.Show("请选择要添加的文件夹。", "提示", MessageBoxButtons.OK);
                txtFolder.Focus();
                return;
            }
            if (string.IsNullOrEmpty(cmbTargetDir.Text.Trim()))
            {
                MessageBox.Show("请选择要目标目录。", "提示", MessageBoxButtons.OK);
                txtFolder.Focus();
                return;
            }
            var targetDir = cmbTargetDir.Text.Trim();
            if (filePaths.All(x => !targetDir.Contains(x)))
            {
                MessageBox.Show("目标目录不合法。", "提示", MessageBoxButtons.OK);
                txtFolder.Focus();
                return;
            }

            this.FileItems = new List<FileItem>();
            var dir = new DirectoryInfo(txtFolder.Text.Trim());
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (var fileInfo in files)
            {
                var fi = new FileItem
                {
                    Path = fileInfo.FullName,
                    FileOverride = (FileOverride)cmbOverride.SelectedValue
                };
                fi.TargetDir = targetDir + fileInfo.DirectoryName.Replace(dir.FullName, "");
                this.FileItems.Add(fi);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                this.txtFolder.Text = dlg.SelectedPath;
            }
        }
    }
}
