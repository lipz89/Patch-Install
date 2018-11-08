using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using QuickShare.Installer.SubForm;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Installer
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

        private Dictionary<int, Form> forms = new Dictionary<int, Form>();
        private InstallContext context;
        private string filePath;
        private bool isInstalling;

        private OpenFileDialog ofd;
        private string confirmMessage;
        private Action<InstallContext> confirmAction;

        private int Step { get; set; } = 1;

        private void FrmStep_Load(object sender, EventArgs e)
        {
            var welcome = new FrmWelcome();
            welcome.TopLevel = false;
            welcome.FormBorderStyle = FormBorderStyle.None;
            this.pnlMain.Controls.Add(welcome);
            welcome.Visible = true;
            forms.Add(1, welcome);

            welcome.btnOpen.Click += BtnOpen_Click;

            this.btnCancel.Click += BtnCancel_Click;
            this.btnNext.Click += BtnNext_Click;
            this.btnPrev.Click += BtnPrev_Click;
            this.btnInstall.Click += BtnInstallClick;
            this.btnFinish.Click += BtnFinish_Click;

            OnStepChanged();
        }

        private void BtnFinish_Click(object sender, EventArgs e)
        {
            if (this.forms[Step] is IStep step)
            {
                step.ChangeContext();
            }

            if (context.IsFull && context.IsSuccess)
            {
                if (context.RunWhenFinish)
                {
                    if (!string.IsNullOrEmpty(context.InstInfo.MainEntery))
                    {
                        var mainEntity = context.ConvertPath(context.InstInfo.MainEntery);
                        var pinfo = new ProcessStartInfo(mainEntity, context.InstInfo.MainEnteryArgs)
                        {
                            WorkingDirectory = context.ConvertPath(context.InstInfo.InstallPath)
                        };
                        Process.Start(pinfo);
                    }
                }

                if (context.RunReadmeWhenFinish)
                {
                    if (!string.IsNullOrEmpty(context.InstInfo.ReadmeFile))
                    {
                        var readme = context.ConvertPath(context.InstInfo.ReadmeFile);
                        Process.Start("notepad.exe", readme);
                    }
                }
            }

            this.Close();
            Application.Exit();
        }

        private void BtnInstallClick(object sender, EventArgs e)
        {
            BtnNext_Click(sender, e);
            this.pnlControl.Enabled = false;
            var process = pnlMain.Controls.OfType<FrmInstall>().FirstOrDefault();
            if (process == null)
            {
                return;
            }

            isInstalling = true;
            Global.Log = process;
            new Thread(() =>
            {
                var installer = new InstallProcess(this.context, process, this);
                installer.Install();
                this.pnlControl.Enabled = true;
                this.btnPrev.Enabled = false;
                this.btnCancel.Enabled = false;
                isInstalling = false;
                Thread.CurrentThread.Abort();
            }).Start();
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            this.forms[this.Step].Visible = false;
            this.Step--;
            this.btnNext.Enabled = true;
            this.btnInstall.Enabled = true;
            this.btnFinish.Enabled = true;
            OnStepChanged(false);
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
            if (!string.IsNullOrEmpty(this.confirmMessage))
            {
                var da = MessageBox.Show(this, this.confirmMessage, "提示", MessageBoxButtons.YesNo);
                if (da != DialogResult.Yes)
                {
                    return;
                }

                this.confirmMessage = null;
                this.confirmAction?.Invoke(this.context);
            }
            this.forms[this.Step].Visible = false;
            this.Step++;
            OnStepChanged();
            this.forms[this.Step].Visible = true;
        }

        private void GoToWelcome()
        {
            this.forms[this.Step].Visible = false;
            this.Step = 1;
            OnStepChanged(false);
            this.forms[this.Step].Visible = true;
        }


        private bool WhenClosing()
        {
            if (Step > 1 && Step < this.forms.Count)
            {
                var dlg = MessageBox.Show(this, "当前安装未完成，是否确定退出？", "提示", MessageBoxButtons.YesNo);
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
            this.ClearForms();
        }

        private void OnStepChanged(bool arrow = true)
        {
            this.pnlTitle.Visible = Step != 1 && Step != forms.Count;
            this.pnlControl.Visible = Step != 1;
            this.lblStep.Text = $"({Step}/{forms.Count}):";
            this.btnPrev.Visible = Step > 1;
            this.btnInstall.Visible = Step == forms.Count - 2;
            this.btnFinish.Visible = Step == forms.Count;
            this.btnNext.Visible = !btnInstall.Visible && !this.btnFinish.Visible;
            if (this.forms[Step] is IStep step)
            {
                step.ActiveStep(arrow);
                this.lblTitle.Text = step.Title;
                this.lblSubTitle.Text = step.SubTitle;
            }
            else if (this.forms[Step] is Form panel && panel.Tag is Page info)
            {
                if (arrow)
                {
                    info.Init();
                }

                this.lblTitle.Text = info.PageInfo.Title;
                this.lblSubTitle.Text = info.PageInfo.SubTitle;
            }
            else
            {
                this.lblTitle.Text = "";
                this.lblSubTitle.Text = "";
            }

            if (!arrow)
            {
                this.btnNext.Enabled = true;
                this.btnInstall.Enabled = true;
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (this.LoadFile(out var message))
            {
                this.ClearForms();
                AddFrom(new FrmAppInfo());
                if (context.IsFull && this.context.InstInfo.LicenceFile != null) AddFrom(new FrmLicence());
                if (context.IsFull) AddFrom(new FrmDirectory());
                if (context.IsFull) AddFrom(new FrmShortCut());
                if (context.Pages != null)
                {
                    foreach (var pageInfo in this.context.Pages.OrderBy(x => x.Index))
                    {
                        AddPage(PageHelper.CreatePage(this, pageInfo));
                    }
                }

                AddFrom(new FrmInstall());
                AddFrom(new FrmComplete());
                GoNext();
            }
            else if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(this, message, "信息", MessageBoxButtons.OK);
            }
        }

        private bool LoadFile(out string message)
        {
            message = null;
            ofd = ofd ?? new OpenFileDialog
            {
                Multiselect = false,
                Title = "选择压缩文件",
                Filter = "压缩文件(*.zip)|*.zip",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                RestoreDirectory = false
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                filePath = ofd.FileName;
                var dataFile = Compress.UnZipToTempFile(filePath, Context.DataKey);
                if (dataFile != null)
                {
                    this.context = new InstallContext(ContextUtil.FromFile(dataFile));
                    this.context.PackageFile = filePath;
                    this.context.InstInfo.LicenceFile = Compress.UnZipToTempFile(filePath, Context.LicenceTxtKey) ?? Compress.UnZipToTempFile(filePath, Context.LicenceRtfKey);
                    this.context.NeedSize = Compress.GetUnZipSize(filePath);
                    return true;
                }
                else
                {
                    message = "压缩文件中不包含安装数据。";
                }
            }

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
            if (!string.IsNullOrEmpty(e.ConfirmMessage))
            {
                this.confirmMessage = e.ConfirmMessage;
                this.confirmAction = e.ConfirmAction;
            }
        }

        private void AddPage(Page page)
        {
            page.InstallChanged += Step_InstallChanged;
            var key = forms.Keys.Max() + 1;
            var form = page.Form;
            forms.Add(key, form);
            form.Visible = false;
            this.pnlMain.Controls.Add(form);
        }

        private void ClearForms()
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
    }
}
