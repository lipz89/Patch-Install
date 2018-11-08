using System;
using System.IO;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Installer.SubForm
{
    public partial class FrmDirectory : Form, IStep
    {
        private const string chooseUsers = "谁可以使用 $[Name]";
        private const string subtitle = "选择 $[Name] 要安装的文件夹。";
        private const string info = "在安装 $[Name] 之前请检阅许可证协议。如果你接受协议中所有条款，单击下方的勾选框。单击 [下一步(N)] 继续。";
        private const string newdSizeFmt = "所需空间：{0}";
        private const string freeSizeFmt = "剩余空间：{0}";
        private long freeSize = 0;
        private FolderBrowserDialog dialog;
        public FrmDirectory()
        {
            InitializeComponent();

            this.txtInstallPath.TextChanged += Changed;
            this.btnChangePath.Click += BtnChangePath_Click;
        }

        private void BtnChangePath_Click(object sender, EventArgs e)
        {
            dialog = dialog ?? new FolderBrowserDialog
            {
                SelectedPath = this.txtInstallPath.Text
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (dialog.SelectedPath.EndsWith(Context.AppInfo.AppName, StringComparison.OrdinalIgnoreCase))
                {
                    this.txtInstallPath.Text = dialog.SelectedPath;
                }
                else
                {
                    this.txtInstallPath.Text = Path.Combine(dialog.SelectedPath, Context.AppInfo.AppName);
                }
            }
        }

        private void Changed(object sender, EventArgs e)
        {
            freeSize = Global.GetFreeSize(txtInstallPath.Text);
            this.lblFreeSize.Text = string.Format(freeSizeFmt, Global.GetSizeString(freeSize));
            this.InstallChanged?.Invoke(sender, new InstallEventArgs()
            {
                AllowNext = freeSize > this.Context.NeedSize
            });
        }

        public string Title { get; } = "选择安装位置";
        public string SubTitle { get; private set; }
        private InstallContext Context { get; set; }
        public void SetContext(InstallContext context, EventHandler<InstallEventArgs> handler)
        {
            this.Context = context;
            this.InstallChanged += handler;
            this.SubTitle = context.ConvertVars(subtitle);
            this.lblInfo.Text = context.ConvertVars(info);
            this.txtInstallPath.Text = context.InstInfo.InstallPath.Trim("\\/".ToCharArray());
            this.lblNeedSize.Text = string.Format(newdSizeFmt, Global.GetSizeString(context.NeedSize));
            this.txtInstallPath.Enabled = context.InstInfo.AllowChangeInstallPath;
            this.btnChangePath.Enabled = context.InstInfo.AllowChangeInstallPath;

            this.gbChooseUser.Visible = this.Context.InstInfo.AllowChangeUser;
            this.chkAllUsers.Checked = this.Context.InstInfo.ForAllUser;
            this.chkOnlyMe.Checked = !this.Context.InstInfo.ForAllUser;
            this.gbChooseUser.Text = context.ConvertVars(chooseUsers);
        }

        public void ChangeContext()
        {
            this.Context.InstInfo.InstallPath = this.txtInstallPath.Text.Trim();
            this.Context.InstInfo.ForAllUser = this.chkAllUsers.Checked;
        }

        public void ActiveStep(bool fromPrev)
        {
        }

        private event EventHandler<InstallEventArgs> InstallChanged;
    }
}
