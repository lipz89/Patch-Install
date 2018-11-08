using System;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patcher.SubForm
{
    public partial class FrmAppInfo : Form, IStep
    {
        private readonly bool isFull;

        public FrmAppInfo()
        {
            InitializeComponent();
        }

        private bool isInitChanged;

        private void TxtAppName_TextChanged(object sender, EventArgs e)
        {
            this.OnChanged?.Invoke(sender, e);
        }

        public FrmAppInfo(bool isFull) : this()
        {
            this.isFull = isFull;
            label5.Visible = txtLastVersion.Visible = !isFull;
        }

        public string Title { get; } = "应用程序信息";
        public string SubTitle { get; } = "请指定关于应用程序的基本信息。";
        private Context Context { get; set; }
        public void SetChangeHandler(EventHandler handler)
        {
            if (!isInitChanged)
            {
                this.OnChanged += handler;
                this.txtAppName.TextChanged += TxtAppName_TextChanged;
                this.txtAppVersion.TextChanged += TxtAppName_TextChanged;
                this.txtLastVersion.TextChanged += TxtAppName_TextChanged;
                this.txtPublisher.TextChanged += TxtAppName_TextChanged;
                this.txtWebSite.TextChanged += TxtAppName_TextChanged;
                isInitChanged = true;
            }
        }

        public void SetContext(Context context, bool? isFromPrev=null)
        {
            this.Context = context;
            this.txtAppName.Text = this.Context.AppInfo.AppName;
            this.txtAppVersion.Text = this.Context.AppInfo.Version;
            this.txtLastVersion.Text = this.Context.AppInfo.LastVersion;
            this.txtPublisher.Text = this.Context.AppInfo.Publisher;
            this.txtWebSite.Text = this.Context.AppInfo.WebSite;
        }

        public void ChangeContext()
        {
            this.Context.AppInfo.AppName = txtAppName.Text.Trim();
            this.Context.AppInfo.Version = txtAppVersion.Text.Trim();
            this.Context.AppInfo.Publisher = txtPublisher.Text.Trim();
            this.Context.AppInfo.WebSite = txtWebSite.Text.Trim();
            this.Context.AppInfo.LastVersion = txtLastVersion.Text.Trim();

            if (string.IsNullOrEmpty(this.Context.AppInfo.WebSite))
            {
                this.Context.AppInfo.WebSite = null;
            }

            if (string.IsNullOrEmpty(this.Context.AppInfo.LastVersion))
            {
                this.Context.AppInfo.LastVersion = null;
            }

            this.Context.InstInfo.IconGroup = this.Context.InstInfo.IconGroup ?? this.Context.AppInfo.AppName;
        }

        public bool Check()
        {
            if (string.IsNullOrEmpty(txtAppName.Text.Trim()))
            {
                MessageBox.Show("应用程序名称不能为空。", "提示", MessageBoxButtons.OK);
                txtAppName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtAppVersion.Text.Trim()))
            {
                MessageBox.Show("应用程序版本不能为空。", "提示", MessageBoxButtons.OK);
                txtAppVersion.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtPublisher.Text.Trim()))
            {
                MessageBox.Show("应用程序出版人不能为空。", "提示", MessageBoxButtons.OK);
                txtPublisher.Focus();
                return false;
            }
            if (!isFull && string.IsNullOrEmpty(txtLastVersion.Text.Trim()))
            {
                MessageBox.Show("上一版本不能为空。", "提示", MessageBoxButtons.OK);
                txtLastVersion.Focus();
                return false;
            }

            return true;
        }

        private event EventHandler OnChanged;
    }
}
