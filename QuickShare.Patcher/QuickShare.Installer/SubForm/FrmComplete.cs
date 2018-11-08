using System;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Installer.SubForm
{
    public partial class FrmComplete : Form, IStep
    {
        public FrmComplete()
        {
            InitializeComponent();
        }

        public string Title { get; }
        public string SubTitle { get; }
        private InstallContext Context { get; set; }
        public void SetContext(InstallContext context, EventHandler<InstallEventArgs> handler)
        {
            this.Context = context;
        }

        public void ChangeContext()
        {
            this.Context.RunWhenFinish = this.chkRun.Checked;
            this.Context.RunReadmeWhenFinish = this.chkShowReadme.Checked;
        }

        public void ActiveStep(bool fromPrev)
        {
            if (this.Context.IsSuccess)
            {
                this.chkRun.Checked = true;
                this.chkRun.Text = $"运行 {this.Context.AppInfo.FullName} (&R)";
                this.chkRun.Visible = !string.IsNullOrEmpty(this.Context.InstInfo.MainEntery);

                if (this.Context.IsFull)
                {
                    this.lblInfo.Text = $"{this.Context.AppInfo.FullName} 已安装在你的系统。";
                    this.lblTitle.Text = $"正在完成 {this.Context.AppInfo.FullName} 安装向导";

                    this.chkShowReadme.Visible = !string.IsNullOrEmpty(this.Context.InstInfo.ReadmeFile);
                }
                else
                {
                    this.lblInfo.Text = $"{this.Context.AppInfo.AppName} 已经升级到版本 {this.Context.AppInfo.Version}。";
                    this.lblTitle.Text = $"正在完成 {this.Context.AppInfo.AppName} 升级向导";

                    this.chkRun.Checked = false;
                    this.chkRun.Visible = false;
                    this.chkShowReadme.Visible = false;
                }
            }
            else
            {
                this.chkRun.Visible = false;
                this.chkShowReadme.Visible = false;
                if (this.Context.IsFull)
                {
                    this.lblInfo.Text = $"{this.Context.AppInfo.FullName} 安装失败。";
                    this.lblTitle.Text = $"{this.Context.AppInfo.FullName} 安装失败";
                }
                else
                {
                    this.lblInfo.Text = $"{this.Context.AppInfo.AppName} 已经升级到版本 {this.Context.AppInfo.Version} 失败。";
                    this.lblTitle.Text = $"{this.Context.AppInfo.AppName} 升级失败";
                }
            }
        }
    }
}
