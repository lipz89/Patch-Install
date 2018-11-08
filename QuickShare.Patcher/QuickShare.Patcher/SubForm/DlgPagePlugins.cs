using System.Collections.Generic;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class DlgPagePlugins : Form
    {
        public DlgPagePlugins()
        {
            InitializeComponent();

            this.lvCmds.MouseDoubleClick += LvCmds_MouseDoubleClick;
            this.lvCmds.SelectedIndexChanged += LvCmds_SelectedIndexChanged;
            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnPerview.Click += BtnPerview_Click;

            this.lvCmds.EnableSort();
        }

        private void LvCmds_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.btnPerview.Enabled = this.lvCmds.SelectedItems.Count == 1;
        }

        private void BtnPerview_Click(object sender, System.EventArgs e)
        {
            if (this.lvCmds.SelectedItems.Count == 1)
            {
                var item = this.lvCmds.SelectedItems[0];
                if (item.Tag is PageInfo page)
                {
                    var form = new FrmMain(page);
                    form.ShowDialog(this);
                }
            }
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            this.Change();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public List<PageInfo> Infos { get; private set; }

        public void SetPlugins(List<IPagePlugin> plugins)
        {
            if (plugins != null)
            {
                this.lvCmds.Items.Clear();
                foreach (var plugin in plugins)
                {
                    var pageinfo = PageHelper.GetPageInfo(plugin);
                    ListViewItem item = new ListViewItem(pageinfo.Title);
                    item.SubItems.Add(pageinfo.Keys);
                    item.SubItems.Add(pageinfo.Privoder);
                    item.Tag = pageinfo;
                    this.lvCmds.Items.Add(item);
                }
            }
        }

        private void Change()
        {
            Infos = new List<PageInfo>();
            if (this.lvCmds.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in this.lvCmds.SelectedItems)
                {
                    var info = item.Tag as PageInfo;
                    Infos.Add(info);
                }
            }
        }

        private void LvCmds_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Change();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
