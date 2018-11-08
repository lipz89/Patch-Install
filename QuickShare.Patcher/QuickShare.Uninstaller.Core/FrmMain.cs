using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Uninstaller.Core
{
    public partial class FrmMain : Form, IWatch
    {
        private const string Message = "{0} 已成功地从你的计算机移除。";
        private const string Confirm = "您确实要完全移除 {0} ，其及所有的组件？";
        private UninstallData uninstallData;
        private const int ValueMin = 1;
        private const int ValueReady = 2;
        private const int ValueShortcut = 10;
        private const int ValueReg = 10;
        private const int ValueDir = 10;
        private RegInfo regInfo;
        private readonly ManualResetEvent reset = new ManualResetEvent(false);
        private Thread th, thMain;

        private int value = 0;
        public FrmMain()
        {
            InitializeComponent();
            this.lblConfirm.Text = Confirm;

            CheckForIllegalCrossThreadCalls = false;

            this.button1.Click += Button1_Click;

            this.Load += FrmMain_Load;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnOk.Click += BtnOk_Click;
            this.Closing += FrmMain_Closing;
        }

        private void FrmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (thMain != null && thMain.ThreadState != ThreadState.Stopped)
            {
                e.Cancel = true;
                return;
            }
            if (th != null)
            {
                th.Abort();
                th = null;
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.pnlConfirm.Visible = false;
            th = new Thread(StartUninstall);
            th.Start();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, "unins.data");
                if (!File.Exists(path))
                {
                    MessageBox.Show("没有找到相关信息，无法继续卸载。", "提示", MessageBoxButtons.OK);
                    this.Close();
                    return;
                }

                Environment.CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
                var data = UninstallData.LoadFrom(path);
                if (data != null)
                {
                    this.label1.Text = "检查卸载信息";
                    uninstallData = data;
                    Global.Log = this;
                    if (RegisterHelper.Read(uninstallData.UninstallKey, out RegInfo info))
                    {
                        regInfo = info;
                    }

                    this.lblConfirm.Text = string.Format(Confirm, data.AppName);
                }
                else
                {
                    MessageBox.Show("没有找到相关信息，无法继续卸载。", "提示", MessageBoxButtons.OK);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "提示", MessageBoxButtons.OK);
                this.Close();
            }
        }

        private void StartUninstall()
        {
            thMain = new Thread(Uninstall);
            thMain.Start();
            this.reset.Reset();
            this.reset.WaitOne();
            Thread.Sleep(100);
            this.Close();
        }

        private void Button1_Click(object sender, System.EventArgs e)
        {
            this.Height = 375;
            this.button1.Visible = false;
            this.listBox1.Visible = true;
        }

        private void Uninstall()
        {
            try
            {
                var installPath = regInfo?.InstallPath ?? uninstallData.InstallPath;
                value = ValueMin;
                var max = ValueMin + ValueReady + ValueShortcut + ValueDir;
                var delFiles = Directory.Exists(installPath);
                string[] files = new string[0];
                if (delFiles)
                {
                    files = Directory.GetFiles(installPath, "*", SearchOption.AllDirectories);
                    max += files.Length;
                }

                if (uninstallData.Shortcuts != null)
                {
                    max += ValueShortcut;
                }

                if (regInfo != null)
                {
                    max += ValueReg;
                }

                SetMaxValue(max, value);
                AddValue(value += ValueReady);
                if (delFiles)
                {
                    SetStep("正在删除文件");
                    foreach (var fileInfo in files)
                    {
                        if (File.Exists(fileInfo))
                        {
                            FileHelper.RemoveReadonly(fileInfo);
                            File.Delete(fileInfo);
                        }

                        Info($"删除[{fileInfo}]");
                        AddValue(value += 1);
                    }
                }

                if (uninstallData.Shortcuts != null)
                {
                    SetStep("正在删除快捷方式");
                    foreach (var shortcut in uninstallData.Shortcuts)
                    {
                        if (File.Exists(shortcut))
                        {
                            FileHelper.RemoveReadonly(shortcut);
                            File.Delete(shortcut);
                        }
                    }

                    AddValue(value += ValueShortcut);
                }
                var iconDir = regInfo.IconGroupName;
                if (Directory.Exists(iconDir))
                {
                    Directory.Delete(iconDir, true);
                }
                if (regInfo != null)
                {
                    SetStep("正在删除注册表");
                    RegisterHelper.Delete(RegInfo.ProductDirRegkey, regInfo.ExeKey);
                    RegisterHelper.Delete(RegInfo.ProductUninstKey, uninstallData.UninstallKey);
                }

                AddValue(value += ValueReg);
                if (delFiles)
                {
                    Directory.Delete(installPath, true);
                }

                AddValue(value += ValueDir);

                string msg = string.Format(Message, uninstallData.AppName);
                MessageBox.Show(this, msg, "提示", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "提示", MessageBoxButtons.OK);
            }
            finally
            {
                reset.Set();
                thMain.Abort();
            }
        }

        private void Log(string message)
        {
            this.listBox1.Items.Add(message);
            if (this.listBox1.Visible)
            {
                this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
            }
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
            this.label1.Text = stepInfo;
        }
        public void SetMaxValue(int maxValue, int value)
        {
            if (maxValue > 0)
                this.progressBar1.Maximum = maxValue;
            AddValue(value);
        }
        public void AddValue(int value)
        {
            var val = progressBar1.Value + value;
            if (val >= 0 && val <= progressBar1.Maximum)
                this.progressBar1.Value = val;
        }
        public void Complete()
        {
            this.progressBar1.Value = this.progressBar1.Maximum;
            this.Info("卸载完成！");
            this.SetStep("卸载完成！");
        }

        public void Faild()
        {
        }
    }
}
