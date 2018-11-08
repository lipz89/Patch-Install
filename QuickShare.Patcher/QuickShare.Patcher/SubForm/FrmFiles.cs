using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.SubForm
{
    public partial class FrmFiles : Form, IStep
    {
        private readonly DlgFile dlgFile;
        private readonly DlgFolder dlgFolder;
        private bool isItemAdded;
        private bool isInitChanged;
        private bool isFull;

        public FrmFiles()
        {
            InitializeComponent();

            this.btnFile.Click += BtnFile_Click;
            this.btnFolder.Click += BtnFolder_Click;
            this.btnEdit.Click += BtnEdit_Click;
            this.btnDelete.Click += BtnDelete_Click;
            this.cmbMain.SelectedIndexChanged += CmbMain_SelectedIndexChanged;
            this.lvFiles.SelectedIndexChanged += LvFiles_SelectedIndexChanged;
            this.lvFiles.MouseDoubleClick += LvFiles_MouseDoubleClick;
            this.dlgFile = new DlgFile();
            this.dlgFolder = new DlgFolder();
            this.btnDelete.Enabled = false;
            this.btnEdit.Enabled = false;

            this.lvFiles.EnableSort();
        }

        private void CmbMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            var slt = cmbMain.SelectedItem as FileItem;
            var origin = Context.Files?.FirstOrDefault(x => x.IsMain);
            if (origin == null)
            {
                if (slt != null)
                {
                    OnChanged?.Invoke(sender, e);
                }
            }
            else
            {
                if (slt == null || slt.Path != origin.Path)
                {
                    OnChanged?.Invoke(sender, e);
                }
            }
        }

        private void LvFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.BtnEdit_Click(sender, e);
        }

        private void LvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnDelete.Enabled = lvFiles.SelectedItems.Count > 0;
            this.btnEdit.Enabled = lvFiles.SelectedItems.Count > 0;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var item = lvFiles.SelectedItems[0];
            var index = lvFiles.Items.IndexOf(item);
            lvFiles.SelectedItems[0].Remove();
            this.OnChanged?.Invoke(sender, e);
            if (lvFiles.Items.Count > index)
            {
                lvFiles.Items[index].Selected = true;
            }
            else if (lvFiles.Items.Count > 0)
            {
                lvFiles.Items[index - 1].Selected = true;
            }
            ChangeMainOptions();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var listViewItem = lvFiles.SelectedItems[0];
            var file = listViewItem.Tag as FileItem;

            if (dlgFile.ShowDialog(file, this) == DialogResult.OK)
            {
                var index = lvFiles.Items.IndexOf(listViewItem);
                listViewItem.Remove();
                file = dlgFile.FileItem;
                file.Key = Path.Combine(file.TargetDir, file.Name);

                ListViewItem item = lvFiles.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Key);
                if (item != null)
                {
                    index = lvFiles.Items.IndexOf(item);
                    item.Remove();
                }

                item = new ListViewItem(file.Key);
                item.SubItems.Add(file.Path);
                item.Tag = file;
                this.lvFiles.Items.Insert(index, item);
                this.OnChanged?.Invoke(sender, e);
                ChangeMainOptions();
            }
        }

        private void BtnFolder_Click(object sender, EventArgs e)
        {
            if (dlgFolder.ShowDialog(this) == DialogResult.OK)
            {
                var files = dlgFolder.FileItems;
                foreach (var file in files)
                {
                    file.Key = Path.Combine(file.TargetDir, file.Name);

                    ListViewItem item = lvFiles.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Key);
                    item?.Remove();

                    item = new ListViewItem(file.Key);
                    item.SubItems.Add(file.Path);
                    item.Tag = file;
                    this.lvFiles.Items.Add(item);
                }
                this.OnChanged?.Invoke(sender, e);
                ChangeMainOptions();
            }
        }

        private void BtnFile_Click(object sender, EventArgs e)
        {
            if (dlgFile.ShowDialog(null, this) == DialogResult.OK)
            {
                var file = dlgFile.FileItem;
                file.Key = Path.Combine(file.TargetDir, file.Name);
                ListViewItem item = lvFiles.Items.OfType<ListViewItem>().FirstOrDefault(x => x.Text == file.Key);
                var index = lvFiles.Items.Count;
                if (item != null)
                {
                    index = lvFiles.Items.IndexOf(item);
                    item.Remove();
                }

                item = new ListViewItem(file.Key);
                item.SubItems.Add(file.Path);
                item.Tag = file;
                this.lvFiles.Items.Insert(index, item);
                this.OnChanged?.Invoke(sender, e);
                ChangeMainOptions();
            }
        }

        private void SetMainOptions()
        {
            if (!isFull) return;
            var list = lvFiles.Items.OfType<ListViewItem>().Select(x => x.Tag).OfType<FileItem>().ToList();
            list.ForEach(x => x.Key = Path.Combine(x.TargetDir, x.Name));
            var executables = list.Where(x => x.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)).ToList();

            if (executables.Count > 0)
            {
                cmbMain.DataSource = executables;
                cmbMain.DisplayMember = "Key";
                cmbMain.ValueMember = "Path";

                var main = executables.FirstOrDefault(x => x.IsMain);
                if (main != null)
                    cmbMain.SelectedItem = main;
            }
            else
            {
                cmbMain.DataSource = null;
            }
        }
        private void ChangeMainOptions()
        {
            if (!isFull) return;
            var last = cmbMain.SelectedValue?.ToString();

            SetMainOptions();

            foreach (var item in cmbMain.Items)
            {
                var fi = item as FileItem;
                if (fi != null && fi.Path == last)
                {
                    cmbMain.SelectedItem = item;
                    break;
                }
            }
        }

        public string Title { get; } = "应用程序文件";
        public string SubTitle { get; } = "请定义应用程序安装时的文件和文件夹结构。";
        private Context Context { get; set; }

        public void SetChangeHandler(EventHandler handler)
        {
            if (!isInitChanged)
            {
                this.OnChanged += handler;
                isInitChanged = true;
            }
        }

        public void SetContext(Context context, bool? isFromPrev = null)
        {
            this.Context = context;
            this.isFull = string.IsNullOrEmpty(Context.AppInfo.LastVersion);
            if (isFromPrev != true) { return; }
            if (!isItemAdded)
            {
                if (this.Context.Files != null)
                {
                    foreach (var file in this.Context.Files)
                    {
                        ListViewItem item = new ListViewItem(file.Key);
                        item.SubItems.Add(file.Path);
                        item.Tag = file;
                        this.lvFiles.Items.Add(item);
                    }

                    SetMainOptions();
                }

                isItemAdded = true;
            }

            this.label1.Visible = this.cmbMain.Visible = isFull;
        }

        public void ChangeContext()
        {
            var list = lvFiles.Items.OfType<ListViewItem>().Select(x => x.Tag).OfType<FileItem>()
                .ToList();
            list.ForEach(x => x.Key = Path.Combine(x.TargetDir, x.Name));
            list.ForEach(x => x.IsMain = false);
            Context.Files = list;
            var slt = cmbMain.SelectedItem as FileItem;
            if (slt != null) slt.IsMain = true;
        }

        public bool Check()
        {
            if (lvFiles.Items.Count == 0)
            {
                MessageBox.Show("请选择保内要包含的文件。", "提示", MessageBoxButtons.OK);
                btnFolder.Focus();
                return false;
            }

            return true;
        }

        private event EventHandler OnChanged;
    }
}
