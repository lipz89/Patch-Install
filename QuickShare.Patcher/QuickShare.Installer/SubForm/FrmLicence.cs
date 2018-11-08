using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Installer.SubForm
{
    public partial class FrmLicence : Form, IStep
    {
        private const string info = "如果你接受协议中的条款，单击下方的勾选框。必须要接受协议才能安装 $[Name]。单击 [下一步(N)] 继续。";
        private const string subtitle = "在安装 $[Name] 之前请检阅授权条款";
        public FrmLicence()
        {
            InitializeComponent();
            this.chkAgree.CheckedChanged += ChkAgree_CheckedChanged;
        }

        private void ChkAgree_CheckedChanged(object sender, EventArgs e)
        {
            this.InstallChanged?.Invoke(this, new InstallEventArgs()
            {
                AllowNext = chkAgree.Checked
            });
        }

        public string Title { get; } = "许可证协议";
        public string SubTitle { get; private set; }
        private InstallContext Context { get; set; }
        public void SetContext(InstallContext context, EventHandler<InstallEventArgs> handler)
        {
            this.Context = context;
            this.InstallChanged += handler;
            this.lblInfo.Text = context.ConvertVars(info);
            this.SubTitle = context.ConvertVars(subtitle);
            if (context.InstInfo.LicenceFile.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
            {
                this.txtLicence.LoadFile(context.InstInfo.LicenceFile);
            }
            else
            {
                this.txtLicence.Text = File.ReadAllText(context.InstInfo.LicenceFile, Encoding.GetEncoding("GB18030"));
            }
        }

        public void ChangeContext()
        {
        }

        public void ActiveStep(bool fromPrev)
        {
            if (fromPrev)
            {
                this.InstallChanged?.Invoke(this, new InstallEventArgs()
                {
                    AllowNext = chkAgree.Checked
                });
            }
        }

        private event EventHandler<InstallEventArgs> InstallChanged;
    }
}
