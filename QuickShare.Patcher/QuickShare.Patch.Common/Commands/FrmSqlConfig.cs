using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuickShare.Patch.Common.Commands
{
    public partial class FrmSqlConfig : Form
    {
        private static FrmSqlConfig instance;

        private static FrmSqlConfig Instance
        {
            get { return instance ?? (instance = new FrmSqlConfig()); }
        }

        public FrmSqlConfig()
        {
            InitializeComponent();

            //this.TopMost = true;

            this.Closing += FrmSqlConfig_Closing;

            this.btnCancel.Click += BtnCancel_Click;
            this.btnOk.Click += BtnOk_Click;
            this.btnTest.Click += BtnTest_Click;
        }

        private void FrmSqlConfig_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                var dc = MessageBox.Show(this, "取消数据库配置将会取消后续操作，\r\n确定要取消吗？", "提示", MessageBoxButtons.YesNo);
                if (dc != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void BtnTest_Click(object sender, System.EventArgs e)
        {
            if (Test())
            {
                MessageBox.Show(this, "连接成功");
            }
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            if (Test())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public string Server
        {
            get { return this.txtServer.Text.Trim(); }
        }
        public string Database
        {
            get { return this.txtDbname.Text.Trim(); }
        }
        public string UserId
        {
            get { return this.txtUserID.Text.Trim(); }
        }
        public string Password
        {
            get { return this.txtPassword.Text; }
        }

        private bool Test()
        {
            if (string.IsNullOrEmpty(Server))
            {
                MessageBox.Show(this, "服务器名不能为空。");
                this.txtServer.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(Database))
            {
                MessageBox.Show(this, "数据库名不能为空。");
                this.txtDbname.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(UserId))
            {
                MessageBox.Show(this, "服务器名不能为空。");
                this.txtUserID.Focus();
                return false;
            }
            try
            {
                var connstr =
                    $"data source={Server};initial catalog={Database};user id={UserId};password={Password};MultipleActiveResultSets=True";
                var sqlConn = new SqlConnection(connstr);
                sqlConn.Open();
                sqlConn.Close();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "数据库配置错误：" + e.Message);
                return false;
            }
        }

        public static bool GetConfig(IWin32Window owner, out string server, out string dbName, out string userId, out string password)
        {
            var da = owner == null ? Instance.ShowDialog() : Instance.ShowDialog(owner);
            if (da == DialogResult.OK)
            {
                server = Instance.Server;
                dbName = Instance.Database;
                userId = Instance.UserId;
                password = Instance.Password;
                return true;
            }

            server = null;
            dbName = null;
            userId = null;
            password = null;
            return false;
        }
    }
}
