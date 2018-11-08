using System;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class DlgShortCut : Form
    {
        public ShortCutInfo ShortCutInfo { get; private set; }
        private Context context;

        public DlgShortCut()
        {
            InitializeComponent();

            var paths = GlobalPath.GetShortCutPaths();
            this.cmbTargetDir.Items.AddRange(paths);
            if (this.cmbTargetDir.Items.Count > 0) this.cmbTargetDir.SelectedIndex = 0;

            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
        }

        public void Init(Context context)
        {
            if (this.context == null)
            {
                this.context = context;
                this.cmbTarget.Items.Clear();
                this.cmbTarget.Items.AddRange(context.GetExecutables());
                if (this.cmbTarget.Items.Count > 0) this.cmbTarget.SelectedIndex = 0;
            }
            else
            {
                this.cmbTarget.TryChangeComboItems( context.GetExecutables());
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                MessageBox.Show("请填写快捷方式的名称。", "提示", MessageBoxButtons.OK);
                txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(cmbTargetDir.Text.Trim()))
            {
                MessageBox.Show("请选择快捷方式的目录。", "提示", MessageBoxButtons.OK);
                cmbTargetDir.Focus();
                return;
            }
            if (string.IsNullOrEmpty(cmbTarget.Text.Trim()))
            {
                MessageBox.Show("请选择快捷方式的目标。", "提示", MessageBoxButtons.OK);
                cmbTarget.Focus();
                return;
            }
            this.ShortCutInfo.Name = txtName.Text.Trim();
            this.ShortCutInfo.TargetDir = cmbTargetDir.Text.Trim();
            this.ShortCutInfo.Args = txtArgs.Text.Trim();
            this.ShortCutInfo.Target = cmbTarget.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public DialogResult ShowDialog(ShortCutInfo item, IWin32Window owner = null)
        {
            this.ShortCutInfo = item ?? new ShortCutInfo { Name = context.AppInfo.AppName };
            this.txtName.Text = this.ShortCutInfo.Name;
            if (this.ShortCutInfo.TargetDir != null)
            {
                this.cmbTargetDir.SelectedItem = this.ShortCutInfo.TargetDir;
            }
            else
            {
                if (this.cmbTargetDir.Items.Count > 0) this.cmbTargetDir.SelectedIndex = 0;
            }
            if (this.ShortCutInfo.Target != null)
            {
                this.cmbTarget.SelectedItem = this.ShortCutInfo.Target;
            }
            this.txtArgs.Text = this.ShortCutInfo.Args;

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
