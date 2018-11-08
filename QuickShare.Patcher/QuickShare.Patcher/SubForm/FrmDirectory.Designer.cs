namespace QuickShare.Patcher.SubForm
{
    partial class FrmDirectory
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
            this.cmbInstallPath = new System.Windows.Forms.ComboBox();
            this.chkAllowChange = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelectLicence = new System.Windows.Forms.Button();
            this.txtLicence = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkForAll = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkAllowChangeUsers = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbInstallPath
            // 
            this.cmbInstallPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbInstallPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstallPath.FormattingEnabled = true;
            this.cmbInstallPath.Location = new System.Drawing.Point(15, 24);
            this.cmbInstallPath.Name = "cmbInstallPath";
            this.cmbInstallPath.Size = new System.Drawing.Size(631, 23);
            this.cmbInstallPath.TabIndex = 0;
            // 
            // chkAllowChange
            // 
            this.chkAllowChange.AutoSize = true;
            this.chkAllowChange.Location = new System.Drawing.Point(15, 54);
            this.chkAllowChange.Name = "chkAllowChange";
            this.chkAllowChange.Size = new System.Drawing.Size(233, 19);
            this.chkAllowChange.TabIndex = 1;
            this.chkAllowChange.Text = "允许用户更改应用程序目录(&H)";
            this.chkAllowChange.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnSelectLicence);
            this.groupBox1.Controls.Add(this.txtLicence);
            this.groupBox1.Location = new System.Drawing.Point(12, 118);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(658, 65);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "许可证协议文件(&L)";
            // 
            // btnSelectLicence
            // 
            this.btnSelectLicence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectLicence.Location = new System.Drawing.Point(607, 24);
            this.btnSelectLicence.Name = "btnSelectLicence";
            this.btnSelectLicence.Size = new System.Drawing.Size(39, 25);
            this.btnSelectLicence.TabIndex = 1;
            this.btnSelectLicence.Text = "...";
            this.btnSelectLicence.UseVisualStyleBackColor = true;
            // 
            // txtLicence
            // 
            this.txtLicence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicence.Location = new System.Drawing.Point(15, 24);
            this.txtLicence.Name = "txtLicence";
            this.txtLicence.ReadOnly = true;
            this.txtLicence.Size = new System.Drawing.Size(586, 25);
            this.txtLicence.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cmbInstallPath);
            this.groupBox2.Controls.Add(this.chkAllowChange);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(658, 85);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "应用程序默认目录(&A)";
            // 
            // chkForAll
            // 
            this.chkForAll.AutoSize = true;
            this.chkForAll.Location = new System.Drawing.Point(15, 24);
            this.chkForAll.Name = "chkForAll";
            this.chkForAll.Size = new System.Drawing.Size(203, 19);
            this.chkForAll.TabIndex = 2;
            this.chkForAll.Text = "默认为“所有人”安装(&F)";
            this.chkForAll.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chkAllowChangeUsers);
            this.groupBox3.Controls.Add(this.chkForAll);
            this.groupBox3.Location = new System.Drawing.Point(12, 204);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(658, 94);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "使用用户(&U)";
            // 
            // chkAllowChangeUsers
            // 
            this.chkAllowChangeUsers.AutoSize = true;
            this.chkAllowChangeUsers.Location = new System.Drawing.Point(15, 54);
            this.chkAllowChangeUsers.Name = "chkAllowChangeUsers";
            this.chkAllowChangeUsers.Size = new System.Drawing.Size(218, 19);
            this.chkAllowChangeUsers.TabIndex = 3;
            this.chkAllowChangeUsers.Text = "允许安装时选择使用用户(&C)";
            this.chkAllowChangeUsers.UseVisualStyleBackColor = true;
            // 
            // FrmDirectory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 333);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmDirectory";
            this.Text = "FrmDirectory";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbInstallPath;
        private System.Windows.Forms.CheckBox chkAllowChange;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSelectLicence;
        private System.Windows.Forms.TextBox txtLicence;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkForAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkAllowChangeUsers;
    }
}