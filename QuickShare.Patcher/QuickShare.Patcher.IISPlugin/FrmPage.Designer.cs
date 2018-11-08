namespace QuickShare.Patcher.IISPlugin
{
    partial class FrmPage
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
            this.lblPoolName = new System.Windows.Forms.Label();
            this.txtSiteName = new System.Windows.Forms.TextBox();
            this.lblSiteName = new System.Windows.Forms.Label();
            this.txtPoolName = new System.Windows.Forms.TextBox();
            this.lblLogicPath = new System.Windows.Forms.Label();
            this.txtLogicPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtHostName = new System.Windows.Forms.TextBox();
            this.lblHostName = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblIp = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbIpAddress = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPoolName
            // 
            this.lblPoolName.AutoSize = true;
            this.lblPoolName.Location = new System.Drawing.Point(12, 58);
            this.lblPoolName.Name = "lblPoolName";
            this.lblPoolName.Size = new System.Drawing.Size(121, 15);
            this.lblPoolName.TabIndex = 2;
            this.lblPoolName.Text = "应用程序池(&L)：";
            // 
            // txtSiteName
            // 
            this.txtSiteName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSiteName.Location = new System.Drawing.Point(143, 13);
            this.txtSiteName.Name = "txtSiteName";
            this.txtSiteName.Size = new System.Drawing.Size(350, 25);
            this.txtSiteName.TabIndex = 1;
            // 
            // lblSiteName
            // 
            this.lblSiteName.AutoSize = true;
            this.lblSiteName.Location = new System.Drawing.Point(12, 16);
            this.lblSiteName.Name = "lblSiteName";
            this.lblSiteName.Size = new System.Drawing.Size(106, 15);
            this.lblSiteName.TabIndex = 0;
            this.lblSiteName.Text = "网站名称(&S)：";
            // 
            // txtPoolName
            // 
            this.txtPoolName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPoolName.Location = new System.Drawing.Point(143, 55);
            this.txtPoolName.Name = "txtPoolName";
            this.txtPoolName.Size = new System.Drawing.Size(350, 25);
            this.txtPoolName.TabIndex = 3;
            // 
            // lblLogicPath
            // 
            this.lblLogicPath.AutoSize = true;
            this.lblLogicPath.Location = new System.Drawing.Point(12, 31);
            this.lblLogicPath.Name = "lblLogicPath";
            this.lblLogicPath.Size = new System.Drawing.Size(106, 15);
            this.lblLogicPath.TabIndex = 5;
            this.lblLogicPath.Text = "物理路径(&P)：";
            // 
            // txtLogicPath
            // 
            this.txtLogicPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogicPath.Location = new System.Drawing.Point(128, 26);
            this.txtLogicPath.Name = "txtLogicPath";
            this.txtLogicPath.Size = new System.Drawing.Size(335, 25);
            this.txtLogicPath.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblLogicPath);
            this.groupBox1.Controls.Add(this.txtLogicPath);
            this.groupBox1.Location = new System.Drawing.Point(15, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 68);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "内容目录";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cmbIpAddress);
            this.groupBox2.Controls.Add(this.txtHostName);
            this.groupBox2.Controls.Add(this.lblHostName);
            this.groupBox2.Controls.Add(this.txtPort);
            this.groupBox2.Controls.Add(this.lblPort);
            this.groupBox2.Controls.Add(this.lblIp);
            this.groupBox2.Controls.Add(this.lblType);
            this.groupBox2.Controls.Add(this.cmbType);
            this.groupBox2.Location = new System.Drawing.Point(15, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(478, 144);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "绑定";
            // 
            // txtHostName
            // 
            this.txtHostName.Location = new System.Drawing.Point(15, 105);
            this.txtHostName.Name = "txtHostName";
            this.txtHostName.Size = new System.Drawing.Size(338, 25);
            this.txtHostName.TabIndex = 15;
            // 
            // lblHostName
            // 
            this.lblHostName.AutoSize = true;
            this.lblHostName.Location = new System.Drawing.Point(12, 87);
            this.lblHostName.Name = "lblHostName";
            this.lblHostName.Size = new System.Drawing.Size(91, 15);
            this.lblHostName.TabIndex = 14;
            this.lblHostName.Text = "主机名(&H)：";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(369, 49);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(94, 25);
            this.txtPort.TabIndex = 13;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(366, 31);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(76, 15);
            this.lblPort.TabIndex = 12;
            this.lblPort.Text = "端口(&O)：";
            // 
            // lblIp
            // 
            this.lblIp.AutoSize = true;
            this.lblIp.Location = new System.Drawing.Point(125, 31);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(92, 15);
            this.lblIp.TabIndex = 10;
            this.lblIp.Text = "IP地址(&I)：";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(12, 31);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(76, 15);
            this.lblType.TabIndex = 8;
            this.lblType.Text = "类型(&T)：";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(15, 49);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(88, 23);
            this.cmbType.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.lblSiteName);
            this.panel1.Controls.Add(this.txtPoolName);
            this.panel1.Controls.Add(this.lblPoolName);
            this.panel1.Controls.Add(this.txtSiteName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(511, 324);
            this.panel1.TabIndex = 8;
            // 
            // cmbIpAddress
            // 
            this.cmbIpAddress.FormattingEnabled = true;
            this.cmbIpAddress.Location = new System.Drawing.Point(128, 49);
            this.cmbIpAddress.Name = "cmbIpAddress";
            this.cmbIpAddress.Size = new System.Drawing.Size(225, 23);
            this.cmbIpAddress.TabIndex = 16;
            // 
            // FrmPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 324);
            this.Controls.Add(this.panel1);
            this.Name = "FrmPage";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPoolName;
        private System.Windows.Forms.TextBox txtSiteName;
        private System.Windows.Forms.Label lblSiteName;
        private System.Windows.Forms.TextBox txtPoolName;
        private System.Windows.Forms.Label lblLogicPath;
        private System.Windows.Forms.TextBox txtLogicPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.TextBox txtHostName;
        private System.Windows.Forms.Label lblHostName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cmbIpAddress;
    }
}