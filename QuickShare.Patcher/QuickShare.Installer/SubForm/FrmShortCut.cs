using System;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Installer.SubForm
{
    public partial class FrmShortCut : Form, IStep
    {
        private const string subtitle = "创建桌面快捷方式，并选择用于 $[Name] 快捷方式的“开始菜单组”文件夹";
        public FrmShortCut()
        {
            InitializeComponent();

            this.chkDesktopShortcut.CheckedChanged += ChkDesktopShortcut_CheckedChanged;
        }

        private void ChkDesktopShortcut_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = this.chkDesktopShortcut.Checked;
        }

        public string Title { get; } = "快捷方式和“开始菜单组”文件夹";
        public string SubTitle { get; private set; }
        private InstallContext Context { get; set; }
        public void SetContext(InstallContext context, EventHandler<InstallEventArgs> handler)
        {
            this.Context = context;
            this.SubTitle = context.ConvertVars(subtitle);
            this.chkDesktopShortcut.Checked = true;
            this.txtIconGroup.Text = context.InstInfo.IconGroup;
            this.chkIconGroup.Checked = true;
            this.txtIconGroup.Enabled = context.InstInfo.AllowChangeIconGroup;
        }

        public void ChangeContext()
        {
            this.Context.InstInfo.IconGroup = this.txtIconGroup.Text.Trim();
            this.Context.CreateShortcuts = this.chkDesktopShortcut.Checked;
            this.Context.CreateIconGroup = this.chkIconGroup.Checked && this.chkDesktopShortcut.Checked;
        }

        public void ActiveStep(bool fromPrev)
        {
        }
    }
}
