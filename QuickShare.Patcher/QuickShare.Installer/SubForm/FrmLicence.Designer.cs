namespace QuickShare.Installer.SubForm
{
    partial class FrmLicence
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtLicence = new System.Windows.Forms.RichTextBox();
            this.chkAgree = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtLicence
            // 
            this.txtLicence.BackColor = System.Drawing.SystemColors.Control;
            this.txtLicence.Location = new System.Drawing.Point(27, 48);
            this.txtLicence.Name = "txtLicence";
            this.txtLicence.ReadOnly = true;
            this.txtLicence.Size = new System.Drawing.Size(626, 199);
            this.txtLicence.TabIndex = 0;
            this.txtLicence.Text = "...";
            // 
            // chkAgree
            // 
            this.chkAgree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAgree.AutoSize = true;
            this.chkAgree.Location = new System.Drawing.Point(27, 302);
            this.chkAgree.Name = "chkAgree";
            this.chkAgree.Size = new System.Drawing.Size(263, 19);
            this.chkAgree.TabIndex = 1;
            this.chkAgree.Text = "我接受“许可证协议”中的条款(&A)";
            this.chkAgree.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(341, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "检阅协议的其余部分，按 [PgDn] 往下卷动页面。";
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.Location = new System.Drawing.Point(24, 259);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(630, 30);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "如果你接受协议中的条款，单击下方的勾选框。必须要接受协议才能安装 $[Name]。单击 [下一步(N)] 继续。";
            // 
            // FrmLicence
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 333);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.txtLicence);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkAgree);
            this.Name = "FrmLicence";
            this.Text = "FrmLicence";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkAgree;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtLicence;
        private System.Windows.Forms.Label lblInfo;
    }
}