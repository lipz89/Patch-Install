using System;
using System.IO;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patcher.SubForm
{
    public partial class FrmComplete : Form, IStep, IProcess
    {
        private string lastZipFile;
        public FrmComplete()
        {
            InitializeComponent();
            this.VisibleChanged += FrmComplete_VisibleChanged;
        }

        private void FrmComplete_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                this.panel1.Visible = true;
            }
        }

        public string Title { get; } = "完成";
        public string SubTitle { get; } = "您已成功配置了所有步骤。";
        private Context Context { get; set; }
        public void SetChangeHandler(EventHandler handler)
        {
            if (!isInitChanged)
            {
                this.OnChanged += handler;
                this.txtZipFile.TextChanged += Changed;
                isInitChanged = true;
            }
        }

        public void SetContext(Context context, bool? isFromPrev=null)
        {
            this.Context = context;
            if (isFromPrev != true) { return; }
            if (string.IsNullOrEmpty(this.txtZipFile.Text) && !string.IsNullOrEmpty(Context.AppInfo.TargetName))
            {
                this.txtZipFile.Text = Context.AppInfo.TargetName + ".zip";
                lastZipFile = this.txtZipFile.Text;
                this.Context.OutputName = this.txtZipFile.Text?.Trim();
            }
        }

        private bool isInitChanged;
        private void Changed(object sender, EventArgs e)
        {
            var zip = this.txtZipFile.Text.Trim();
            var chars = Path.GetInvalidFileNameChars();
            if (zip.IndexOfAny(chars) >= 0)
            {
                this.txtZipFile.Text = lastZipFile;
            }
            else if (this.Context.OutputName != zip)
            {
                if (!zip.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    zip += ".zip";
                }
                lastZipFile = zip;
                this.Context.OutputName = zip;
                this.OnChanged?.Invoke(sender, e);
            }
        }
        public void ChangeContext()
        {
            var zip = this.txtZipFile.Text?.Trim();
            if (!zip.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                zip += ".zip";
            }
            this.Context.OutputName = zip;
        }

        public bool Check()
        {
            if (string.IsNullOrEmpty(this.txtZipFile.Text.Trim()))
            {
                MessageBox.Show("请填写输出包文件名称", "提示", MessageBoxButtons.OK);
                this.txtZipFile.Focus();
                return false;
            }
            return true;
        }

        private event EventHandler OnChanged;

        private void Log(string message, string type)
        {
            lbLog.Items.Add(string.Format($"[{type}]:{message}"));
            lbLog.SelectedIndex = lbLog.Items.Count - 1;
        }
        public void Begin()
        {
            this.panel1.Visible = false;
        }

        public void End()
        {
            this.panel1.Visible = true;
        }
        public void Info(string message)
        {
            Log(message, "信息");
        }

        public void Warn(string message)
        {
            Log(message, "警告");
        }

        public void Error(string message)
        {
            Log(message, "错误");
        }
    }
}
