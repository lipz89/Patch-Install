namespace QuickShare.ScriptPatcher.SubForm
{
    partial class FrmInstall
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
            this.lblStep = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbInfos = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lblStep
            // 
            this.lblStep.AutoSize = true;
            this.lblStep.Location = new System.Drawing.Point(24, 21);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(67, 15);
            this.lblStep.TabIndex = 0;
            this.lblStep.Text = "执行进度";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(27, 39);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(630, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // lbInfos
            // 
            this.lbInfos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbInfos.FormattingEnabled = true;
            this.lbInfos.ItemHeight = 15;
            this.lbInfos.Location = new System.Drawing.Point(27, 68);
            this.lbInfos.Name = "lbInfos";
            this.lbInfos.Size = new System.Drawing.Size(630, 244);
            this.lbInfos.TabIndex = 2;
            // 
            // FrmInstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 333);
            this.Controls.Add(this.lbInfos);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblStep);
            this.Name = "FrmInstall";
            this.Text = "FrmInstall";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox lbInfos;
    }
}