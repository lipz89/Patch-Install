using System;
using System.IO;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Installer.SubForm
{
    public partial class FrmAppInfo : Form, IStep
    {
        public FrmAppInfo()
        {
            InitializeComponent();
        }

        public string Title { get; } = "应用程序信息";
        public string SubTitle { get; } = "请确定以下关于应用程序的基本信息。";
        private InstallContext Context { get; set; }
        private bool canGoNext = true;
        private string confirmMessage;

        public void SetContext(InstallContext context, EventHandler<InstallEventArgs> handler)
        {
            canGoNext = true;
            this.Context = context;
            this.InstallChanged += handler;
            label5.Visible = valLastVersion.Visible = !this.Context.IsFull;
            if (context?.AppInfo == null)
            {
                this.valAppName.Text = "";
                this.valAppVersion.Text = "";
                this.valLastVersion.Text = "";
                this.valPublisher.Text = "";
                this.valWebSite.Text = "";
                this.lblInfo.Text = "您选择的压缩包不包含安装/升级数据。";
                canGoNext = false;
            }
            else
            {
                var appInfo = context.AppInfo;
                this.valAppName.Text = appInfo.AppName;
                this.valAppVersion.Text = appInfo.Version;
                this.valPublisher.Text = appInfo.Publisher;
                this.valWebSite.Text = appInfo.WebSite;
                this.valLastVersion.Text = appInfo.LastVersion;

                this.Context.InstInfo.InstallPath = Path.Combine(this.Context.ConvertPath(this.Context.InstInfo.InstallPath), context.AppInfo.AppName);

                if (RegisterHelper.Read(appInfo.AppName, out var regInfo))
                {
                    if (regInfo == null)
                    {
                        if (!context.IsFull)
                        {
                            lblCurrentInfo.Text = $"当前计算机没有安装 {appInfo.AppName}，无法进行升级操作。";
                            canGoNext = false;
                        }
                        else
                        {
                            lblCurrentInfo.Text = $"当前计算机没有安装 {appInfo.AppName}。";
                        }
                    }
                    else
                    {
                        var currentApp = $"{regInfo.DisplayName}.{regInfo.DisplayVersion}";
                        if (context.IsFull)
                        {
                            if (string.Compare(regInfo.DisplayVersion, context.AppInfo.Version, StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                lblCurrentInfo.Text =
                                    $"当前计算机已安装 {currentApp}，高于当前版本。{Environment.NewLine}若要重新安装请先卸载{currentApp}。";
                                canGoNext = false;
                            }
                            else
                            {
                                lblCurrentInfo.Text = $"当前计算机已安装 {currentApp}。";

                                this.confirmMessage = $"当前计算机已安装 {currentApp}。{Environment.NewLine}安装 {appInfo.FullName} 会覆盖 {currentApp}。{Environment.NewLine}是否确定要覆盖安装？";

                                this.Context.InstInfo.InstallPath = regInfo.InstallPath;
                                this.Context.InstInfo.IconGroup = regInfo.IconGroupName;
                                this.Context.InstInfo.AllowChangeInstallPath = false;
                                this.Context.InstInfo.AllowChangeIconGroup = false;
                            }
                        }
                        else
                        {
                            this.Context.InstInfo.InstallPath = regInfo.InstallPath;
                            this.Context.InstInfo.MainEntery = regInfo.ExePath;
                            this.Context.InstInfo.MainEnteryArgs = regInfo.ExeArgs;
                            if (appInfo.LastVersion != regInfo.DisplayVersion)
                            {
                                lblCurrentInfo.Text = $"无法从 {currentApp} 升级到 {appInfo.FullName}。";
                                canGoNext = false;
                            }
                            else
                            {
                                lblCurrentInfo.Text = $"准备从 {currentApp} 升级到 {appInfo.FullName}。";
                            }
                        }
                    }
                }
                else
                {
                    lblCurrentInfo.Text = "读取注册表失败，无法进行安装。";
                    canGoNext = false;
                }
            }
        }

        public void ChangeContext()
        {
        }

        public void ActiveStep(bool fromPrev)
        {
            var e = new InstallEventArgs()
            {
                AllowNext = canGoNext
            };
            if (!string.IsNullOrEmpty(this.confirmMessage))
            {
                e.ConfirmMessage = this.confirmMessage;
                e.ConfirmAction = ctx => ctx.IsMakeStore = true;
                this.confirmMessage = null;
            }

            this.lblTipNext.Visible = canGoNext;
            this.InstallChanged?.Invoke(this, e);
        }

        private event EventHandler<InstallEventArgs> InstallChanged;
    }
}
