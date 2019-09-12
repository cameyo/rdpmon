namespace Cameyo.RdpMon
{
    partial class ShadowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShadowForm));
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.elevationImg = new System.Windows.Forms.PictureBox();
            this.withConsentRadio = new System.Windows.Forms.RadioButton();
            this.noConsentRadio = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elevationImg)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(140, 96);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 0;
            this.cancelBtn.Text = "&Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(59, 96);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 1;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.elevationImg);
            this.groupBox1.Controls.Add(this.withConsentRadio);
            this.groupBox1.Controls.Add(this.noConsentRadio);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 78);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // elevationImg
            // 
            this.elevationImg.Image = ((System.Drawing.Image)(resources.GetObject("elevationImg.Image")));
            this.elevationImg.Location = new System.Drawing.Point(98, 42);
            this.elevationImg.Name = "elevationImg";
            this.elevationImg.Size = new System.Drawing.Size(16, 16);
            this.elevationImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.elevationImg.TabIndex = 3;
            this.elevationImg.TabStop = false;
            // 
            // withConsentRadio
            // 
            this.withConsentRadio.AutoSize = true;
            this.withConsentRadio.Checked = true;
            this.withConsentRadio.Location = new System.Drawing.Point(15, 19);
            this.withConsentRadio.Name = "withConsentRadio";
            this.withConsentRadio.Size = new System.Drawing.Size(66, 17);
            this.withConsentRadio.TabIndex = 3;
            this.withConsentRadio.TabStop = true;
            this.withConsentRadio.Text = "&Ask user";
            this.withConsentRadio.UseVisualStyleBackColor = true;
            // 
            // noConsentRadio
            // 
            this.noConsentRadio.AutoSize = true;
            this.noConsentRadio.Location = new System.Drawing.Point(15, 42);
            this.noConsentRadio.Name = "noConsentRadio";
            this.noConsentRadio.Size = new System.Drawing.Size(70, 17);
            this.noConsentRadio.TabIndex = 4;
            this.noConsentRadio.Text = "&Don\'t ask";
            this.noConsentRadio.UseVisualStyleBackColor = true;
            // 
            // ShadowForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(227, 125);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.cancelBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShadowForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Prompt for user confirmation";
            this.Load += new System.EventHandler(this.ShadowForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elevationImg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox elevationImg;
        private System.Windows.Forms.RadioButton withConsentRadio;
        private System.Windows.Forms.RadioButton noConsentRadio;
    }
}