using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Commands;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class DlgCommand : Form
    {
        private Context context;
        public CmdInfo CmdInfo { get; private set; }
        private ToolTip toolTip = new ToolTip();

        public DlgCommand()
        {
            InitializeComponent();

            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnGlobal.Click += BtnGlobal_Click;
            this.lvGlobal.MouseDoubleClick += LvGlobal_MouseDoubleClick;

            this.pnlGlobal.Visible = false;
            this.MinimumSize = new Size(500, 300);
            this.Height = 300;

            this.lvGlobal.Columns[0].Width = this.lvGlobal.Width - 30;

            this.SizeChanged += DlgCommand_SizeChanged;
            this.toolTip.SetToolTip(lblToolTip, "未选择命令");
            this.cmbTarget.SelectedIndexChanged += CmbTarget_SelectedIndexChanged;
        }

        private void CmbTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            var target = cmbTarget.Text.Trim();
            var desc = BaseCommand.GetCommandDesc(target);
            this.toolTip.SetToolTip(lblToolTip, desc);
        }

        private void DlgCommand_SizeChanged(object sender, EventArgs e)
        {
            if (this.pnlGlobal.Visible)
            {
                this.pnlGlobal.Height = this.Height - 320;
                this.lvGlobal.Columns[0].Width = this.lvGlobal.Width - 30;
            }
        }

        private void LvGlobal_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvGlobal.SelectedItems.Count == 1)
            {
                var item = lvGlobal.SelectedItems[0];
                var val = item.Text;
                var txt = this.txtArgs.Text;
                var start = this.txtArgs.SelectionStart;
                var length = this.txtArgs.SelectionLength;
                var pre = txt.Substring(0, start);
                var post = txt.Substring(start + length);
                var newTxt = pre + val + post;
                this.txtArgs.Text = newTxt;
            }
        }

        private void BtnGlobal_Click(object sender, EventArgs e)
        {
            this.pnlGlobal.Visible = !this.pnlGlobal.Visible;
            if (this.pnlGlobal.Visible)
            {
                this.btnGlobal.Text = "<<宏(&M)";
                this.Height += 200;
                this.MinimumSize = new Size(500, 500);
                this.pnlGlobal.Height = this.Height - 320;
            }
            else
            {
                this.btnGlobal.Text = "宏(&M)>>";
                this.MinimumSize = new Size(500, 300);
                this.Height -= 200;
            }
        }

        public void Init(Context context)
        {
            this.context = context;
            this.cmbTarget.Items.Clear();
            this.cmbTarget.Items.AddRange(context.GetCmds());
            if (this.cmbTarget.Items.Count > 0) this.cmbTarget.SelectedIndex = 0;
            
            this.InitGlobal();
        }

        private void InitGlobal()
        {
            this.lvGlobal.ShowGroups = true;
            this.lvGlobal.Items.Clear();
            var vars = this.context.PageItemKeys;
            if (vars.Any())
            {
                var grp = new ListViewGroup("变量");
                this.lvGlobal.Groups.Add(grp);
                foreach (var @var in vars)
                {
                    var item = new ListViewItem($"$[{@var}]", grp);
                    this.lvGlobal.Items.Add(item);
                }
            }

            var paths = GlobalPath.GetInstallPaths().Concat(GlobalPath.GetShortCutPaths()).Concat(GlobalPath.GetFilePaths());
            if (paths.Any())
            {
                var grp = new ListViewGroup("路径");
                this.lvGlobal.Groups.Add(grp);
                foreach (var path in paths)
                {
                    var item = new ListViewItem(path, grp);
                    this.lvGlobal.Items.Add(item);
                }
            }

            if (this.context.Files != null && this.context.Files.Any())
            {
                var grp = new ListViewGroup("文件");
                this.lvGlobal.Groups.Add(grp);
                foreach (var file in this.context.Files)
                {
                    var item = new ListViewItem(file.Key, grp);
                    this.lvGlobal.Items.Add(item);
                }
            }

            this.lvGlobal.EnableGroupEvent();
            this.lvGlobal.EnableSort();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                MessageBox.Show("请填写命令的名称。", "提示", MessageBoxButtons.OK);
                txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(cmbTarget.Text.Trim()))
            {
                MessageBox.Show("请选择命令执行目标。", "提示", MessageBoxButtons.OK);
                cmbTarget.Focus();
                return;
            }
            this.CmdInfo.Name = txtName.Text.Trim();
            if (!string.IsNullOrEmpty(this.txtArgs.Text.Trim()))
            {
                this.CmdInfo.ArgList = txtArgs.Text.Trim()
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                this.CmdInfo.ArgList = null;
            }

            this.CmdInfo.Target = cmbTarget.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public DialogResult ShowDialog(CmdInfo item, IWin32Window owner = null)
        {
            this.CmdInfo = item ?? new CmdInfo();
            this.txtName.Text = this.CmdInfo.Name;
            if (this.CmdInfo.Target != null)
            {
                this.cmbTarget.SelectedItem = this.CmdInfo.Target;
            }

            if (this.CmdInfo.ArgList != null)
            {
                this.txtArgs.Text = string.Join(Environment.NewLine, this.CmdInfo.ArgList);
            }
            else
            {
                this.txtArgs.Text = "";
            }

            if (owner != null)
            {
                return this.ShowDialog(owner);
            }
            else
            {
                return this.ShowDialog();
            }
        }
    }
}
