namespace QuickShare.Patcher.SubForm
{
    partial class FrmRun
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
            this.label3 = new System.Windows.Forms.Label();
            this.txtArgs = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbRunable = new System.Windows.Forms.ComboBox();
            this.cmbReadme = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "自述(&R):";
            // 
            // txtArgs
            // 
            this.txtArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtArgs.Location = new System.Drawing.Point(117, 59);
            this.txtArgs.Name = "txtArgs";
            this.txtArgs.Size = new System.Drawing.Size(536, 25);
            this.txtArgs.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "参数(&A):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "程序(&M):";
            // 
            // cmbRunable
            // 
            this.cmbRunable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRunable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRunable.FormattingEnabled = true;
            this.cmbRunable.Location = new System.Drawing.Point(117, 17);
            this.cmbRunable.Name = "cmbRunable";
            this.cmbRunable.Size = new System.Drawing.Size(536, 23);
            this.cmbRunable.TabIndex = 11;
            // 
            // cmbReadme
            // 
            this.cmbReadme.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbReadme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReadme.FormattingEnabled = true;
            this.cmbReadme.Location = new System.Drawing.Point(117, 103);
            this.cmbReadme.Name = "cmbReadme";
            this.cmbReadme.Size = new System.Drawing.Size(536, 23);
            this.cmbReadme.TabIndex = 13;
            // 
            // FrmRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 333);
            this.Controls.Add(this.cmbReadme);
            this.Controls.Add(this.cmbRunable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtArgs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FrmRun";
            this.Text = "FrmRun";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtArgs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRunable;
        private System.Windows.Forms.ComboBox cmbReadme;
    }
}