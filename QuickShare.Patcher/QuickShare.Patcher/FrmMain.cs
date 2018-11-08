using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;
using QuickShare.Patcher.SubForm;

namespace QuickShare.Patcher
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            this.btnSave.Enabled = false;

            this.Load += FrmStep_Load;
            this.Closing += FrmMain_Closing;
        }

        public FrmMain(PageInfo pageInfo)
        {
            InitializeComponent();
            this.Text = "自定义页面预览";
            this.btnSave.Visible = false;
            this.lblStep.Text = "(?/?):";
            this.lblTitle.Text = pageInfo.Title;
            this.lblSubTitle.Text = pageInfo.SubTitle;
            var page = PageHelper.CreatePage(this, pageInfo);
            this.pnlMain.Controls.Add(page.Form);
            page.Form.Visible = true;
            page.InstallChanged += (sender, args) => this.btnNext.Enabled = args.AllowNext;
            this.btnCancel.Click += (sender, args) => this.Close();
        }

        private Dictionary<int, Form> forms = new Dictionary<int, Form>();
        private bool isSaved;
        private Context context;
        private string filePath;
        private bool isBuilding = false;

        private int Step { get; set; } = 1;

        private void FrmStep_Load(object sender, EventArgs e)
        {
            var welcome = new FrmWelcome();
            this.pnlMain.Controls.Add(SetFrom(welcome));
            welcome.Visible = true;
            forms.Add(1, welcome);

            welcome.btnCreateNew.Click += BtnCreateNew_Click;
            welcome.btnCreatePatcher.Click += BtnCreatePatcher_Click;
            welcome.btnOpen.Click += BtnOpen_Click;

            this.btnCancel.Click += BtnCancel_Click;
            this.btnNext.Click += BtnNext_Click;
            this.btnPrev.Click += BtnPrev_Click;
            this.btnBuild.Click += BtnBuild_Click;
            this.btnSave.Click += BtnSave_Click;
            this.btnFinish.Click += BtnFinish_Click;

            OnStepChanged(true);
        }

        private void BtnFinish_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnBuild_Click(object sender, EventArgs e)
        {
            this.pnlControl.Enabled = false;
            isBuilding = true;
            new Thread(Build).Start();
        }

        private void Build()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            if (WhenClosing())
            {
                path = filePath.Replace("/", "\\");
                path = path.Remove(path.LastIndexOf("\\"));
            }

            path = Path.Combine(path, context.OutputName);
            var process = BeginProcess();
            Global.Log = process;

            Global.Log?.Info($"开始执行打包，当前时间{DateTime.Now:s}");
            Global.Log?.Info("生成数据文件...");

            var dataPath = filePath;
            if (filePath.EndsWith(".psc", StringComparison.OrdinalIgnoreCase))
            {
                dataPath = dataPath.Replace(".psc", ".bin");
                this.context.SaveToFile(dataPath);
            }

            Global.Log?.Info("收集程序文件...");
            var files = context.Files.ToDictionary(x => x.Key, x => x.Path);
            files.Add(Context.DataKey, dataPath);
            var uninsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Context.UninstallerKey);
            if (File.Exists(uninsPath))
            {
                files.Add(Context.UninstallerKey, uninsPath);
            }

            if (!string.IsNullOrEmpty(context.InstInfo.LicenceFile))
            {
                var licencePath = context.InstInfo.LicenceFile.Trim();
                if (!files.ContainsKey(licencePath))
                {
                    if (context.InstInfo.LicenceFile.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(Context.LicenceRtfKey, licencePath);
                    }
                    else if (context.InstInfo.LicenceFile.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(Context.LicenceTxtKey, licencePath);
                    }
                }
            }

            if (Compress.Zip(path, files))
            {
                Global.Log?.Info("打包成功。");
                Global.Log?.Info($"输出结果：{path}");
                this.btnBuild.Visible = false;
            }
            else
            {
                Global.Log?.Error("打包失败。");
            }
            this.pnlControl.Enabled = true;
            isBuilding = false;
            Thread.CurrentThread.Abort();
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            this.forms[this.Step].Visible = false;
            this.Step--;
            this.btnNext.Enabled = true;
            this.btnBuild.Enabled = true;
            this.btnFinish.Enabled = true;
            OnStepChanged(false);
            this.forms[this.Step].Visible = true;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            var step = this.forms[Step] as IStep;
            if (step != null)
            {
                if (!step.Check())
                {
                    return;
                }
                step.ChangeContext();
            }
            GoNext();
        }

        private void GoNext()
        {
            this.forms[this.Step].Visible = false;
            this.Step++;
            OnStepChanged(true);
            this.forms[this.Step].Visible = true;
        }

        private void GoToWelCome()
        {
            this.forms[this.Step].Visible = false;
            this.Step = 1;
            OnStepChanged(false);
            this.forms[this.Step].Visible = true;
        }

        private bool WhenClosing()
        {
            if (!isSaved && Step > 1)
            {
                var dlg = MessageBox.Show("当前脚本未保存，是否需要保存？", "提示", MessageBoxButtons.YesNoCancel);
                if (dlg == DialogResult.Yes)
                {
                    Save();
                }
                else if (dlg == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        private void FrmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isBuilding)
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
            GoToWelCome();
            this.ClearForms();
        }

        private void OnStepChanged(bool isFromPrev)
        {
            this.panel1.Visible = Step != 1;
            this.pnlControl.Visible = Step != 1;
            this.lblStep.Text = $"({Step}/{forms.Count}):";
            this.btnPrev.Visible = Step > 2;
            this.btnNext.Visible = Step < forms.Count;
            this.btnSave.Visible = Step > 1;
            this.btnBuild.Visible = Step == forms.Count;
            this.btnFinish.Visible = Step == forms.Count;
            var step = this.forms[Step] as IStep;
            if (step != null)
            {
                step.SetContext(context, isFromPrev);
                this.lblTitle.Text = step.Title;
                this.lblSubTitle.Text = step.SubTitle;
            }
            else
            {
                this.lblTitle.Text = "";
                this.lblSubTitle.Text = "";
            }
        }


        private void BtnCreateNew_Click(object sender, EventArgs e)
        {
            filePath = null;
            isSaved = false;
            context = new Context()
            {
                AppInfo = new AppInfo(true)
            };
            this.ClearForms();
            AddStep(SetFrom(new FrmAppInfo(true)));
            AddStep(SetFrom(new FrmDirectory()));
            AddStep(SetFrom(new FrmFiles()));
            AddStep(SetFrom(new FrmShortCuts()));
            AddStep(SetFrom(new FrmPages()));
            AddStep(SetFrom(new FrmCommands()));
            AddStep(SetFrom(new FrmRun()));
            AddStep(SetFrom(new FrmComplete()));
            GoNext();
        }

        private void BtnCreatePatcher_Click(object sender, EventArgs e)
        {
            filePath = null;
            isSaved = false;
            context = new Context()
            {
                AppInfo = new AppInfo(false)
            };
            this.ClearForms();
            AddStep(SetFrom(new FrmAppInfo(false)));
            AddStep(SetFrom(new FrmFiles()));
            AddStep(SetFrom(new FrmCommands()));
            AddStep(SetFrom(new FrmComplete()));
            GoNext();
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (this.LoadFile())
            {
                isSaved = true;
                var isFull = this.context.AppInfo.LastVersion == null;
                this.ClearForms();
                AddStep(SetFrom(new FrmAppInfo(isFull)));
                if (isFull) AddStep(SetFrom(new FrmDirectory()));
                AddStep(SetFrom(new FrmFiles()));
                AddStep(SetFrom(new FrmShortCuts()));
                AddStep(SetFrom(new FrmPages()));
                AddStep(SetFrom(new FrmCommands()));
                if (isFull) AddStep(SetFrom(new FrmRun()));
                AddStep(SetFrom(new FrmComplete()));
                GoNext();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private SaveFileDialog sfd;
        private OpenFileDialog ofd;
        private bool Save()
        {
            foreach (var step in this.pnlMain.Controls.OfType<IStep>())
            {
                step.ChangeContext();
            }
            if (filePath == null)
            {
                sfd = sfd ?? new SaveFileDialog
                {
                    Title = "保存脚本文件",
                    Filter = "脚本文件(*.psc)|*.psc|二进制文件(*.bin)|*.bin",
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    RestoreDirectory = false,
                    FileName = context?.AppInfo?.TargetName
                };

                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    filePath = sfd.FileName;
                }
            }
            if (filePath != null)
            {
                if (this.context.SaveToFile(filePath))
                {
                    this.isSaved = true;
                    this.btnSave.Enabled = false;
                    return true;
                }
            }

            return false;
        }

        private bool LoadFile()
        {
            ofd = ofd ?? new OpenFileDialog
            {
                Multiselect = false,
                Title = "选择脚本文件",
                Filter = "脚本文件(*.psc)|*.psc|二进制文件(*.bin)|*.bin",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                RestoreDirectory = false
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                filePath = ofd.FileName;
                this.context = ContextUtil.FromFile(filePath);
                return this.context != null;
            }

            return false;
        }

        private void AddStep(Form form)
        {
            var key = forms.Keys.Max() + 1;
            forms.Add(key, SetFrom(form));
            form.Visible = false;
            this.pnlMain.Controls.Add(form);
        }
        private Form SetFrom(Form form)
        {
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            form.FormBorderStyle = FormBorderStyle.None;
            var step = form as IStep;
            if (step != null)
            {
                step.SetContext(context);
                step.SetChangeHandler(Step_OnChanged);
            }
            return form;
        }

        private IProcess BeginProcess()
        {
            var process = pnlMain.Controls.OfType<IProcess>().FirstOrDefault();
            if (process != this.forms[this.Step])
            {
                this.forms[this.Step].Visible = false;
                (process as Form).Visible = true;
            }
            process.Begin();
            return process;
        }

        private void Step_OnChanged(object sender, EventArgs e)
        {
            this.isSaved = false;
            this.btnSave.Enabled = true;
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
