using System;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Installer.SubForm
{
    public partial class FrmInstall : Form, IStep, IWatch
    {
        private const string subtitle = "$[Name] 正在安装，请稍候...";
        public FrmInstall()
        {
            InitializeComponent();
        }

        public string Title { get; } = "正在安装";
        public string SubTitle { get; private set; }
        private InstallContext Context { get; set; }
        public void SetContext(InstallContext context, EventHandler<InstallEventArgs> handler)
        {
            this.Context = context;
            this.SubTitle = this.Context.ConvertVars(subtitle);
        }

        public void ChangeContext()
        {
        }

        public void ActiveStep(bool fromPrev)
        {
        }

        private void Log(string message)
        {
            this.lbInfos.Items.Add(message);
            this.lbInfos.SelectedIndex = this.lbInfos.Items.Count - 1;
        }
        public void Info(string message)
        {
            this.Log(message);
        }

        public void Warn(string message)
        {
            this.Log("[警告]" + message);
        }

        public void Error(string message)
        {
            this.Log("[错误]" + message);
        }

        public void SetStep(string stepInfo)
        {
            this.lblStep.Text = stepInfo;
        }
        public void SetMaxValue(int maxValue, int value)
        {
            if (maxValue > 0)
                this.progressBar1.Maximum = maxValue;
            AddValue(value);
        }
        public void AddValue(int value)
        {
            var val = this.progressBar1.Value + value;
            if (val >= 0 && val <= progressBar1.Maximum)
                this.progressBar1.Value = val;
        }
        public void Complete()
        {
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.Info("安装完成！");
            this.SetStep("已完成");
        }

        public void Faild()
        {
            this.Info("安装失败！");
            this.SetStep("安装失败");
        }
    }
}
