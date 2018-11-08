using System;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patcher.SubForm
{
    public partial class FrmCommands : Form, IStep
    {
        private readonly DlgCommand dlgCommand;
        private bool isItemAdded;
        private bool isInitChanged;

        public FrmCommands()
        {
            InitializeComponent();

            dlgCommand = new DlgCommand();

            this.btnAdd.Click += BtnAdd_Click;
            this.btnEdit.Click += BtnEdit_Click;
            this.btnDelete.Click += BtnDelete_Click;
            this.btnUp.Click += BtnUp_Click;
            this.btnDown.Click += BtnDown_Click;
            this.lvCmds.MouseDoubleClick += LvCmds_MouseDoubleClick;
            this.lvCmds.SelectedIndexChanged += LvCmds_SelectedIndexChanged;
            this.btnDelete.Enabled = false;
            this.btnEdit.Enabled = false;
            this.btnUp.Enabled = false;
            this.btnDown.Enabled = false;
        }

        private void LvCmds_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnDelete.Enabled = lvCmds.SelectedItems.Count > 0;
            this.btnEdit.Enabled = lvCmds.SelectedItems.Count > 0;
            this.btnUp.Enabled = lvCmds.SelectedItems.Count > 0 && lvCmds.SelectedIndices[0] > 0;
            this.btnDown.Enabled = lvCmds.SelectedItems.Count > 0 && lvCmds.SelectedIndices[0] < lvCmds.Items.Count - 1;
        }

        private void LvCmds_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.BtnEdit_Click(sender, e);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (dlgCommand.ShowDialog(null, this) == DialogResult.OK)
            {
                var file = dlgCommand.CmdInfo;
                ListViewItem item = lvCmds.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Name);
                var index = lvCmds.Items.Count;
                if (item != null)
                {
                    index = lvCmds.Items.IndexOf(item);
                    item?.Remove();
                }

                item = new ListViewItem(file.Name)
                {
                    Tag = file
                };
                item.SubItems.Add(file.Display);
                this.lvCmds.Items.Insert(index, item);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var listViewItem = lvCmds.SelectedItems[0];
            var file = listViewItem.Tag as CmdInfo;
            if (dlgCommand.ShowDialog(file, this) == DialogResult.OK)
            {
                var index = lvCmds.Items.IndexOf(listViewItem);
                listViewItem.Remove();
                file = dlgCommand.CmdInfo;

                ListViewItem item = lvCmds.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Name);
                if (item != null)
                {
                    index = lvCmds.Items.IndexOf(item);
                    item.Remove();
                }

                item = new ListViewItem(file.Name)
                {
                    Tag = file
                };
                item.SubItems.Add(file.Display);
                this.lvCmds.Items.Insert(index, item);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var item = lvCmds.SelectedItems[0];
            var index = lvCmds.Items.IndexOf(item);
            lvCmds.SelectedItems[0].Remove();
            this.OnChanged?.Invoke(sender, e);
            if (lvCmds.Items.Count > index)
            {
                lvCmds.Items[index].Selected = true;
            }
            else if (lvCmds.Items.Count > 0)
            {
                lvCmds.Items[index - 1].Selected = true;
            }
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            var item = lvCmds.SelectedItems[0];
            var index = lvCmds.Items.IndexOf(item);
            if (index > 0)
            {
                lvCmds.SelectedItems[0].Remove();
                lvCmds.Items.Insert(index - 1, item);
                item.Selected = true;
                lvCmds.Focus();

                LvCmds_SelectedIndexChanged(sender, e);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            var item = lvCmds.SelectedItems[0];
            var index = lvCmds.Items.IndexOf(item);
            if (index < lvCmds.Items.Count - 1)
            {
                lvCmds.SelectedItems[0].Remove();
                lvCmds.Items.Insert(index + 1, item);
                item.Selected = true;
                lvCmds.Focus();

                LvCmds_SelectedIndexChanged(sender, e);
                this.OnChanged?.Invoke(sender, e);
            }
        }

        public string Title { get; } = "创建自定义命令";
        public string SubTitle { get; } = "创建在安装过程中自动执行的命令，程序或脚本。";
        private Context Context { get; set; }
        public void SetChangeHandler(EventHandler handler)
        {
            if (!isInitChanged)
            {
                this.OnChanged += handler;
                isInitChanged = true;
            }
        }

        public void SetContext(Context context, bool? isFromPrev=null)
        {
            this.Context = context;
            if (isFromPrev != true) { return; }
            this.dlgCommand.Init(this.Context);

            if (!isItemAdded)
            {
                if (this.Context.Commands != null)
                {
                    foreach (var shortCut in this.Context.Commands.OrderBy(x => x.Index))
                    {
                        ListViewItem item = new ListViewItem(shortCut.Name);
                        item.SubItems.Add(shortCut.Display);
                        item.Tag = shortCut;
                        this.lvCmds.Items.Add(item);
                    }
                }
                isItemAdded = true;
            }
        }
        public void ChangeContext()
        {
            this.Context.Commands = lvCmds.Items
                .OfType<ListViewItem>()
                .Select(x => x.Tag)
                .OfType<CmdInfo>()
                .Select((x, i) =>
                    {
                        x.Index = i;
                        return x;
                    })
                .ToList();
        }

        public bool Check()
        {
            return true;
        }

        private event EventHandler OnChanged;
    }
}
