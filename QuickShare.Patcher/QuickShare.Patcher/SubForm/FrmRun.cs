using System;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class FrmRun : Form, IStep
    {
        public FrmRun()
        {
            InitializeComponent();
        }

        private bool isInitChanged;

        public string Title { get; } = "安装完成后运行";
        public string SubTitle { get; } = "请指定当前安装完成后要执行的动作。";
        private Context Context { get; set; }

        public void SetChangeHandler(EventHandler handler)
        {
            if (!isInitChanged)
            {
                this.OnChanged += handler;
                this.txtArgs.TextChanged += Changed;
                this.cmbReadme.SelectedIndexChanged += Changed;
                this.cmbRunable.SelectedIndexChanged += Changed;
                isInitChanged = true;
            }
        }

        public void SetContext(Context context, bool? isFromPrev = null)
        {
            this.Context = context;
            if (isFromPrev != true) { return; }
            this.cmbRunable.TryChangeComboItems( this.Context.GetExecutables());
            this.cmbReadme.TryChangeComboItems( this.Context.GetTexts());

            this.txtArgs.Text = Context.InstInfo.MainEnteryArgs;

            if (Context.InstInfo.MainEntery != null)
                this.cmbRunable.SelectedItem = Context.InstInfo.MainEntery;
            if (Context.InstInfo.ReadmeFile != null)
                this.cmbReadme.SelectedItem = Context.InstInfo.ReadmeFile;
        }

        private void Changed(object sender, EventArgs e)
        {
            this.OnChanged?.Invoke(sender, e);
        }

        public void ChangeContext()
        {
            this.Context.InstInfo.MainEntery = !string.IsNullOrEmpty(this.cmbRunable.Text.Trim())
                ? this.cmbRunable.Text.Trim() : null;
            this.Context.InstInfo.MainEnteryArgs = !string.IsNullOrEmpty(this.txtArgs.Text.Trim())
                ? this.txtArgs.Text.Trim() : null;
            this.Context.InstInfo.ReadmeFile = !string.IsNullOrEmpty(this.cmbReadme.Text.Trim())
                ? this.cmbReadme.Text.Trim() : null;
        }

        public bool Check()
        {
            if (string.IsNullOrEmpty(cmbRunable.Text))
            {
                MessageBox.Show("请选择主程序。", "提示", MessageBoxButtons.OK);
                cmbRunable.Focus();
                return false;
            }
            return true;
        }

        private event EventHandler OnChanged;
    }
}
