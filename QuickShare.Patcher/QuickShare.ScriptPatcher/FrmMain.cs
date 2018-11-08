using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Commands;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;
using QuickShare.ScriptPatcher.SubForm;

namespace QuickShare.ScriptPatcher
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            this.Load += FrmStep_Load;
            this.Closing += FrmMain_Closing;
        }

        private readonly Dictionary<int, Control> forms = new Dictionary<int, Control>();
        private readonly PatchContext context = new PatchContext();
        private FolderBrowserDialog fbd;
        private bool isInstalling;
        private bool isRunned;
        private bool isConfigDb;
        private bool isChooseScriptDir;
        private string lblInfo;
        private string lblInfo2;
        private readonly FrmWelcome welcome = new FrmWelcome();
        private int Step { get; set; } = 1;

        private void FrmStep_Load(object sender, EventArgs e)
        {
            AddFrom(welcome);
            welcome.Visible = true;
            welcome.btnOpen.Click += BtnOpen_Click;
            welcome.btnConfig.Click += BtnConfig_Click;
            welcome.btnHelper.Click += BtnHelper_Click;

            this.btnCancel.Click += BtnCancel_Click;
            this.btnNext.Click += BtnNext_Click;
            this.btnPrev.Click += BtnPrev_Click;
            this.btnInstall.Click += BtnInstallClick;
            this.btnFinish.Click += BtnFinish_Click;

            OnStepChanged();
        }

        private void BtnHelper_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, SqlAdoExecutor.CmdHelper, "帮助", MessageBoxButtons.OK);
        }

        private bool ReadConfig()
        {
            if (FrmSqlConfig.GetConfig(this, out string server, out string dbName, out string userId, out string password))
            {
                lblInfo = $"数据库配置：{Environment.NewLine}服务器：  {server}{Environment.NewLine}数据库：  {dbName}";
                this.context.ConnectionString = $"data source={server};initial catalog={dbName};user id={userId};password={password};MultipleActiveResultSets=True";
                isConfigDb = true;
                welcome.SetLabel(lblInfo, lblInfo2);
                return true;
            }

            welcome.SetLabel(lblInfo, lblInfo2);
            return false;
        }

        private void BtnFinish_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void BtnInstallClick(object sender, EventArgs e)
        {
            BtnNext_Click(sender, e);
            Begin();
        }

        private void Begin()
        {
            this.pnlControl.Enabled = false;
            var process = pnlMain.Controls.OfType<FrmInstall>().FirstOrDefault();
            if (process == null)
            {
                return;
            }

            isInstalling = true;
            Global.Log = process;
            var installer = new SqlAdoExecutor();
            installer.SetOwner(this);
            installer.Init(this.context, process);
            process.SetMaxValue(installer.GetProcessValue(), 0);
            if (!installer.Run())
            {
                if (installer.CommandStatus == CommandStatus.Running &&
                    MessageBox.Show(this, "脚本执行出错，是否需要还原？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    installer.Rollback();
                }
            }
            else
            {
                process.Complete();
            }

            this.pnlControl.Enabled = true;
            this.btnPrev.Enabled = false;
            this.btnCancel.Enabled = false;
            isInstalling = false;
            isRunned = true;
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            this.forms[this.Step].Visible = false;
            this.Step--;
            this.btnNext.Enabled = true;
            this.btnInstall.Enabled = true;
            this.btnFinish.Enabled = true;
            OnStepChanged();
            this.forms[this.Step].Visible = true;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (this.forms[Step] is IStep step)
            {
                step.ChangeContext();
            }
            GoNext();
        }

        private void GoNext()
        {
            this.forms[this.Step].Visible = false;
            this.Step++;
            this.forms[this.Step].Visible = true;
            OnStepChanged();
            if (this.forms[this.Step] is FrmInstall)
            {
                Begin();
            }
        }

        private void GoToWelcome()
        {
            this.forms[this.Step].Visible = false;
            this.Step = 1;
            OnStepChanged();
            this.forms[this.Step].Visible = true;
            this.ClearForm();
        }

        private void ClearForm()
        {
            var listToRemove = new List<int>();
            foreach (var kv in forms)
            {
                if (kv.Key > 1)
                {
                    this.pnlMain.Controls.Remove(kv.Value);
                    listToRemove.Add(kv.Key);
                }
            }

            foreach (var i in listToRemove)
            {
                forms.Remove(i);
            }
        }


        private bool WhenClosing()
        {
            if (Step > 1 || isRunned)
            {
                return true;
            }
            if (Step < this.forms.Count)
            {
                var dlg = MessageBox.Show(this, "脚本执行工作未完成，是否确定退出？", "提示", MessageBoxButtons.YesNo);
                if (dlg == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void FrmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isInstalling)
            {
                e.Cancel = true;
                return;
            }
            if (!WhenClosing())
            {
                e.Cancel = true;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (!WhenClosing())
            {
                return;
            }
            GoToWelcome();
        }

        private void OnStepChanged()
        {
            this.pnlTitle.Visible = Step != 1 && Step != forms.Count;
            this.pnlControl.Visible = Step != 1;
            this.lblStep.Text = $"({Step}/{forms.Count}):";
            this.btnPrev.Visible = Step > 1;
            this.btnInstall.Visible = Step == forms.Count - 2;
            this.btnFinish.Visible = Step == forms.Count;
            this.btnNext.Visible = !btnInstall.Visible && !this.btnFinish.Visible;
            Application.DoEvents();
            if (this.forms[Step] is IStep step)
            {
                step.ActiveStep();
                this.lblTitle.Text = step.Title;
                this.lblSubTitle.Text = step.SubTitle;
            }
            else
            {
                this.lblTitle.Text = $"步骤{Step}";
                this.lblSubTitle.Text = "";
            }

            if (Step == 1)
            {
                isChooseScriptDir = false;
                lblInfo2 = null;
            }
            welcome.SetLabel(lblInfo, lblInfo2);
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            this.context.Path = @"E:\Tests\TestScripts";
            this.context.Files = SqlAdoExecutor.GetFiles(context.Path);
            this.context.ConnectionString =
                "data source=.;initial catalog=test;user id=sa;password=111111;MultipleActiveResultSets=True";
            GoToNext();
            return;
            if (this.LoadFile() && isConfigDb)
            {
                GoToNext();
            }
        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            if (ReadConfig() && isChooseScriptDir)
            {
                GoToNext();
            }
        }

        private void GoToNext()
        {
            this.context.ShowDetails = welcome.chkShowDetails.Checked;
            if (!this.context.ShowDetails) AddFrom(new FrmDbObjects());
            AddFrom(new FrmInstall());
            /*if (!this.context.ShowDetails)*/
            AddFrom(new FrmResult());
            GoNext();
        }

        private bool LoadFile()
        {
            fbd = fbd ?? new FolderBrowserDialog()
            {
                Description = "选择脚本所在文件夹",
            };

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                this.context.Path = fbd.SelectedPath;
                isChooseScriptDir = true;
                lblInfo2 = $"脚本目录：\r\n{this.context.Path}";
                welcome.SetLabel(lblInfo, lblInfo2);

                this.context.Files = SqlAdoExecutor.GetFiles(context.Path);
                if (this.context.Files != null && this.context.Files.Any())
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(this, "您选择的文件夹不包含脚本，或者未按规定方式组织脚本，\r\n请重新选择目录。", "提示", MessageBoxButtons.OK);
                    return false;
                }
            }

            welcome.SetLabel(lblInfo, lblInfo2);
            return false;
        }

        private void AddFrom(Form form)
        {
            var key = 1;
            if (forms.Any())
                key = forms.Keys.Max() + 1;

            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            form.FormBorderStyle = FormBorderStyle.None;
            if (form is IStep step)
            {
                step.SetContext(context, Step_InstallChanged);
            }
            forms.Add(key, form);
            form.Visible = false;
            this.pnlMain.Controls.Add(form);
        }

        private void Step_InstallChanged(object sender, InstallEventArgs e)
        {
            this.btnNext.Enabled = e.AllowNext;
            this.btnInstall.Enabled = e.AllowNext;
        }
    }
}
