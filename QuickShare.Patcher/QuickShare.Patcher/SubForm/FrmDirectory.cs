using System;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patcher.SubForm
{
    public partial class FrmDirectory : Form, IStep
    {
        public FrmDirectory()
        {
            InitializeComponent();

            this.btnSelectLicence.Click += BtnSelectLicence_Click;
        }
        private bool isInitChanged;

        private void BtnSelectLicence_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Multiselect = false,
                Title = "选择授权文件",
                Filter = "授权文件(*.rtf,*.txt)|*.rtf;*.txt|富文本文件(*.rtf)|*.rtf|文本文件(*.txt)|*.txt|所有文件(*.*)|*.*"
            };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                this.txtLicence.Text = dlg.FileName;
            }
        }

        private void Changed(object sender, EventArgs e)
        {
            this.OnChanged?.Invoke(sender, e);
        }

        public string Title { get; } = "应用程序目录与授权信息";
        public string SubTitle { get; } = "请指定关于应用程序默认安装的目录与授权信息。";
        private Context Context { get; set; }

        public void SetChangeHandler(EventHandler handler)
        {
            if (!isInitChanged)
            {
                this.OnChanged += handler;
                var paths = GlobalPath.GetInstallPaths();
                this.cmbInstallPath.Items.AddRange(paths);
                if (this.cmbInstallPath.Items.Count > 0) this.cmbInstallPath.SelectedIndex = 0;
                this.cmbInstallPath.SelectedIndexChanged += Changed;
                this.chkAllowChange.CheckedChanged += Changed;
                this.txtLicence.TextChanged += Changed;
                this.chkForAll.CheckedChanged += Changed;
                this.chkAllowChangeUsers.CheckedChanged += Changed;
                isInitChanged = true;
            }
        }

        public void SetContext(Context context, bool? isFromPrev=null)
        {
            this.Context = context;
            if (isFromPrev != true) { return; }
            this.chkAllowChange.Checked = Context.InstInfo.AllowChangeInstallPath;
            if (Context.InstInfo.InstallPath != null)
            {
                this.cmbInstallPath.SelectedItem = Context.InstInfo.InstallPath;
            }

            this.txtLicence.Text = Context.InstInfo.LicenceFile ?? "";
            this.chkAllowChangeUsers.Checked = Context.InstInfo.AllowChangeUser;
            this.chkForAll.Checked = Context.InstInfo.ForAllUser;
        }

        public void ChangeContext()
        {
            this.Context.InstInfo = this.Context.InstInfo ?? new InstInfo();
            this.Context.InstInfo.AllowChangeInstallPath = chkAllowChange.Checked;
            this.Context.InstInfo.AllowChangeUser = chkAllowChangeUsers.Checked;
            this.Context.InstInfo.ForAllUser = chkForAll.Checked;
            this.Context.InstInfo.InstallPath = cmbInstallPath.Text.Trim();
            if (!string.IsNullOrEmpty(txtLicence.Text))
            {
                this.Context.InstInfo.LicenceFile = txtLicence.Text.Trim();
            }
            else
            {
                this.Context.InstInfo.LicenceFile = null;
            }
        }

        public bool Check()
        {
            if (string.IsNullOrEmpty(cmbInstallPath.Text.Trim()))
            {
                MessageBox.Show("请选择安装目录。", "提示", MessageBoxButtons.OK);
                cmbInstallPath.Focus();
                return false;
            }

            return true;
        }

        private event EventHandler OnChanged;
    }
}
