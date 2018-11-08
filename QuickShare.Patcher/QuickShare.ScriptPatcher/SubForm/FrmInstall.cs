using System;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.ScriptPatcher.SubForm
{
    public partial class FrmInstall : Form, IStep, IWatch
    {
        public FrmInstall()
        {
            InitializeComponent();
        }

        public string Title { get; } = "正在执行";
        public string SubTitle { get; } = "正在执行脚本，请稍等";
        private PatchContext Context { get; set; }
        public void SetContext(PatchContext context, EventHandler<InstallEventArgs> handler)
        {
            this.Context = context;
        }

        public void ChangeContext()
        {
        }

        public void ActiveStep()
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
            this.Info("脚本执行已完成！");
            this.SetStep("脚本执行已完成");
        }

        public void Faild()
        {
            this.Info("脚本执行失败！");
            this.SetStep("脚本执行失败");
        }
    }
}
