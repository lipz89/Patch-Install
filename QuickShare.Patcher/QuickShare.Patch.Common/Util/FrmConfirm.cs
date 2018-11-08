using System;
using System.Windows.Forms;

namespace QuickShare.Patch.Common.Util
{
    public partial class FrmConfirm : Form
    {
        public bool NotShowNext { get; private set; }
        private readonly int padh = 42;
        private readonly int padv = 175;
        public FrmConfirm()
        {
            InitializeComponent();
            this.button1.Click += Button1_Click;
            this.button2.Click += Button2_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.NotShowNext = this.chkNotShow.Checked;
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public DialogResult ShowDialog(string text, string title = null, IWin32Window owner = null)
        {
            this.label1.Text = text;

            this.Width = this.label1.Width + padh;
            this.Height = this.label1.Height + padv;
            if (!string.IsNullOrEmpty(title))
            {
                this.Text = title;
            }

            return owner != null ? this.ShowDialog(owner) : this.ShowDialog();
        }
    }
}
