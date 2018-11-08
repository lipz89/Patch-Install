namespace QuickShare.Patcher.SubForm
{
    partial class FrmShortCuts
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkCreateUninstallLnk = new System.Windows.Forms.CheckBox();
            this.chkCreateWebLnk = new System.Windows.Forms.CheckBox();
            this.chkAllowChangIconGroup = new System.Windows.Forms.CheckBox();
            this.txtIconGroupName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lvShortCuts = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkCreateUninstallLnk);
            this.groupBox1.Controls.Add(this.chkCreateWebLnk);
            this.groupBox1.Controls.Add(this.chkAllowChangIconGroup);
            this.groupBox1.Controls.Add(this.txtIconGroupName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(658, 131);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "应用程序“开始菜单组”(&M)";
            // 
            // chkCreateUninstallLnk
            // 
            this.chkCreateUninstallLnk.AutoSize = true;
            this.chkCreateUninstallLnk.Checked = true;
            this.chkCreateUninstallLnk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreateUninstallLnk.Location = new System.Drawing.Point(9, 105);
            this.chkCreateUninstallLnk.Name = "chkCreateUninstallLnk";
            this.chkCreateUninstallLnk.Size = new System.Drawing.Size(308, 19);
            this.chkCreateUninstallLnk.TabIndex = 5;
            this.chkCreateUninstallLnk.Text = "在“开始菜单组”文件夹创建卸载图标(&U)";
            this.chkCreateUninstallLnk.UseVisualStyleBackColor = true;
            // 
            // chkCreateWebLnk
            // 
            this.chkCreateWebLnk.AutoSize = true;
            this.chkCreateWebLnk.Checked = true;
            this.chkCreateWebLnk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreateWebLnk.Location = new System.Drawing.Point(9, 80);
            this.chkCreateWebLnk.Name = "chkCreateWebLnk";
            this.chkCreateWebLnk.Size = new System.Drawing.Size(353, 19);
            this.chkCreateWebLnk.TabIndex = 4;
            this.chkCreateWebLnk.Text = "在“开始菜单组”文件夹创建互联网快捷方式(&I)";
            this.chkCreateWebLnk.UseVisualStyleBackColor = true;
            // 
            // chkAllowChangIconGroup
            // 
            this.chkAllowChangIconGroup.AutoSize = true;
            this.chkAllowChangIconGroup.Location = new System.Drawing.Point(9, 55);
            this.chkAllowChangIconGroup.Name = "chkAllowChangIconGroup";
            this.chkAllowChangIconGroup.Size = new System.Drawing.Size(323, 19);
            this.chkAllowChangIconGroup.TabIndex = 3;
            this.chkAllowChangIconGroup.Text = "允许用户更改“开始菜单组”文件夹名称(&C)";
            this.chkAllowChangIconGroup.UseVisualStyleBackColor = true;
            // 
            // txtIconGroupName
            // 
            this.txtIconGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIconGroupName.Location = new System.Drawing.Point(109, 24);
            this.txtIconGroupName.Name = "txtIconGroupName";
            this.txtIconGroupName.Size = new System.Drawing.Size(543, 25);
            this.txtIconGroupName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "文件夹名称：";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lvShortCuts);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.btnEdit);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Location = new System.Drawing.Point(12, 149);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(658, 183);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "快捷方式";
            // 
            // lvShortCuts
            // 
            this.lvShortCuts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvShortCuts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvShortCuts.FullRowSelect = true;
            this.lvShortCuts.HideSelection = false;
            this.lvShortCuts.Location = new System.Drawing.Point(9, 47);
            this.lvShortCuts.MultiSelect = false;
            this.lvShortCuts.Name = "lvShortCuts";
            this.lvShortCuts.ShowItemToolTips = true;
            this.lvShortCuts.Size = new System.Drawing.Size(643, 130);
            this.lvShortCuts.TabIndex = 9;
            this.lvShortCuts.UseCompatibleStateImageBehavior = false;
            this.lvShortCuts.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "快捷方式";
            this.columnHeader1.Width = 231;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "目标文件";
            this.columnHeader2.Width = 282;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(145, 24);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(69, 23);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "移除(&R)";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(77, 24);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(69, 23);
            this.btnEdit.TabIndex = 7;
            this.btnEdit.Text = "编辑(&E)";
            this.btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(9, 24);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(69, 23);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "添加(&A)";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // FrmShortCuts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 333);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmShortCuts";
            this.Text = "FrmShortCuts";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkCreateUninstallLnk;
        private System.Windows.Forms.CheckBox chkCreateWebLnk;
        private System.Windows.Forms.CheckBox chkAllowChangIconGroup;
        private System.Windows.Forms.TextBox txtIconGroupName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListView lvShortCuts;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}