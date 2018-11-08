using System.Windows.Forms;

namespace QuickShare.ScriptPatcher.SubForm
{
    public partial class FrmWelcome : Form
    {
        public FrmWelcome()
        {
            InitializeComponent();

            this.chkShowDetails.Checked = true;
            this.chkShowDetails.Visible = false;
        }

        public void SetLabel(string info = null, string info2 = null)
        {
            lblInfo.Text = info ?? "未配置数据库";
            lblInfo2.Text = info2 ?? "未选择脚本目录";
        }
    }
}
