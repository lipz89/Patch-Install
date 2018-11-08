namespace QuickShare.Installer.SubForm
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
            this.txtInstallPath = new System.Windows.Forms.TextBox();
            this.btnChangePath = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblNeedSize = new System.Windows.Forms.Label();
            this.lblFreeSize = new System.Windows.Forms.Label();
            this.gbChooseUser = new System.Windows.Forms.GroupBox();
            this.chkOnlyMe = new System.Windows.Forms.RadioButton();
            this.chkAllUsers = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.gbChooseUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtInstallPath
            // 
            this.txtInstallPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInstallPath.Location = new System.Drawing.Point(21, 26);
            this.txtInstallPath.Name = "txtInstallPath";
            this.txtInstallPath.Size = new System.Drawing.Size(479, 25);
            this.txtInstallPath.TabIndex = 1;
            // 
            // btnChangePath
            // 
            this.btnChangePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangePath.Location = new System.Drawing.Point(506, 27);
            this.btnChangePath.Name = "btnChangePath";
            this.btnChangePath.Size = new System.Drawing.Size(103, 23);
            this.btnChangePath.TabIndex = 2;
            this.btnChangePath.Text = "浏览(&B)...";
            this.btnChangePath.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtInstallPath);
            this.groupBox1.Controls.Add(this.btnChangePath);
            this.groupBox1.Location = new System.Drawing.Point(27, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(626, 67);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "目标文件夹";
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.Location = new System.Drawing.Point(24, 21);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(630, 30);
            this.lblInfo.TabIndex = 9;
            this.lblInfo.Text = "Setup 将安装 $[Name] 在下列文件夹。要安装到不同文件夹，单击 [浏览(B)...] 并选择其他的文件夹。单击 [下一步(N)] 继续。";
            // 
            // lblNeedSize
            // 
            this.lblNeedSize.AutoSize = true;
            this.lblNeedSize.Location = new System.Drawing.Point(24, 161);
            this.lblNeedSize.Name = "lblNeedSize";
            this.lblNeedSize.Size = new System.Drawing.Size(82, 15);
            this.lblNeedSize.TabIndex = 10;
            this.lblNeedSize.Text = "所需空间：";
            // 
            // lblFreeSize
            // 
            this.lblFreeSize.AutoSize = true;
            this.lblFreeSize.Location = new System.Drawing.Point(24, 178);
            this.lblFreeSize.Name = "lblFreeSize";
            this.lblFreeSize.Size = new System.Drawing.Size(82, 15);
            this.lblFreeSize.TabIndex = 11;
            this.lblFreeSize.Text = "可用空间：";
            // 
            // gbChooseUser
            // 
            this.gbChooseUser.Controls.Add(this.chkAllUsers);
            this.gbChooseUser.Controls.Add(this.chkOnlyMe);
            this.gbChooseUser.Location = new System.Drawing.Point(27, 237);
            this.gbChooseUser.Name = "gbChooseUser";
            this.gbChooseUser.Size = new System.Drawing.Size(626, 70);
            this.gbChooseUser.TabIndex = 12;
            this.gbChooseUser.TabStop = false;
            this.gbChooseUser.Text = "谁可以使用 $[Name]";
            // 
            // chkOnlyMe
            // 
            this.chkOnlyMe.AutoSize = true;
            this.chkOnlyMe.Location = new System.Drawing.Point(21, 34);
            this.chkOnlyMe.Name = "chkOnlyMe";
            this.chkOnlyMe.Size = new System.Drawing.Size(98, 19);
            this.chkOnlyMe.TabIndex = 0;
            this.chkOnlyMe.Text = "只有我(&O)";
            this.chkOnlyMe.UseVisualStyleBackColor = true;
            // 
            // chkAllUsers
            // 
            this.chkAllUsers.AutoSize = true;
            this.chkAllUsers.Location = new System.Drawing.Point(159, 34);
            this.chkAllUsers.Name = "chkAllUsers";
            this.chkAllUsers.Size = new System.Drawing.Size(98, 19);
            this.chkAllUsers.TabIndex = 1;
            this.chkAllUsers.Text = "所有人(&A)";
            this.chkAllUsers.UseVisualStyleBackColor = true;
            // 
            // FrmDirectory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 333);
            this.Controls.Add(this.gbChooseUser);
            this.Controls.Add(this.lblFreeSize);
            this.Controls.Add(this.lblNeedSize);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmDirectory";
            this.Text = "FrmDirectory";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbChooseUser.ResumeLayout(false);
            this.gbChooseUser.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtInstallPath;
        private System.Windows.Forms.Button btnChangePath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblNeedSize;
        private System.Windows.Forms.Label lblFreeSize;
        private System.Windows.Forms.GroupBox gbChooseUser;
        private System.Windows.Forms.RadioButton chkAllUsers;
        private System.Windows.Forms.RadioButton chkOnlyMe;
    }
}