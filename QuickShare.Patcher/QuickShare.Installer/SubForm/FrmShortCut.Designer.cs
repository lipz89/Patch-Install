namespace QuickShare.Installer.SubForm
{
    partial class FrmShortCut
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
            this.label1 = new System.Windows.Forms.Label();
            this.chkDesktopShortcut = new System.Windows.Forms.CheckBox();
            this.chkIconGroup = new System.Windows.Forms.CheckBox();
            this.txtIconGroup = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 15);
            this.label1.TabIndex = 0;
            // 
            // chkDesktopShortcut
            // 
            this.chkDesktopShortcut.AutoSize = true;
            this.chkDesktopShortcut.Location = new System.Drawing.Point(29, 31);
            this.chkDesktopShortcut.Name = "chkDesktopShortcut";
            this.chkDesktopShortcut.Size = new System.Drawing.Size(143, 19);
            this.chkDesktopShortcut.TabIndex = 7;
            this.chkDesktopShortcut.Text = "创建快捷方式(&M)";
            this.chkDesktopShortcut.UseVisualStyleBackColor = true;
            // 
            // chkIconGroup
            // 
            this.chkIconGroup.AutoSize = true;
            this.chkIconGroup.Location = new System.Drawing.Point(18, 75);
            this.chkIconGroup.Name = "chkIconGroup";
            this.chkIconGroup.Size = new System.Drawing.Size(188, 19);
            this.chkIconGroup.TabIndex = 8;
            this.chkIconGroup.Text = "创建“开始菜单组”(&S)";
            this.chkIconGroup.UseVisualStyleBackColor = true;
            // 
            // txtIconGroup
            // 
            this.txtIconGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIconGroup.Location = new System.Drawing.Point(18, 101);
            this.txtIconGroup.Name = "txtIconGroup";
            this.txtIconGroup.Size = new System.Drawing.Size(588, 25);
            this.txtIconGroup.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(15, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(591, 30);
            this.label3.TabIndex = 10;
            this.label3.Text = "选择“开始菜单组”文件夹，以便创建程序的快捷方式。你也可以输入名称，创建新文件夹。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 15);
            this.label2.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtIconGroup);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.chkIconGroup);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(29, 75);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(626, 141);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "开始菜单组";
            // 
            // FrmShortCut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 333);
            this.Controls.Add(this.chkDesktopShortcut);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Name = "FrmShortCut";
            this.Text = "FrmDirectory";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkDesktopShortcut;
        private System.Windows.Forms.CheckBox chkIconGroup;
        private System.Windows.Forms.TextBox txtIconGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}