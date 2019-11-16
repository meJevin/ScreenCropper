namespace ScreenCropper
{
    partial class AboutF
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
            this.CurrentVersionLabel = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.UpdateInfoRichTextBox = new System.Windows.Forms.RichTextBox();
            this.LoadingIndicator = new System.Windows.Forms.PictureBox();
            this.WebsiteLinkLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.LoadingIndicator)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current version:";
            // 
            // CurrentVersionLabel
            // 
            this.CurrentVersionLabel.AutoSize = true;
            this.CurrentVersionLabel.Location = new System.Drawing.Point(96, 18);
            this.CurrentVersionLabel.Name = "CurrentVersionLabel";
            this.CurrentVersionLabel.Size = new System.Drawing.Size(22, 13);
            this.CurrentVersionLabel.TabIndex = 1;
            this.CurrentVersionLabel.Text = "0.0";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(9, 49);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(96, 13);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Check for Updates";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CheckForUpdatesClicked);
            // 
            // UpdateInfoRichTextBox
            // 
            this.UpdateInfoRichTextBox.Location = new System.Drawing.Point(12, 75);
            this.UpdateInfoRichTextBox.Name = "UpdateInfoRichTextBox";
            this.UpdateInfoRichTextBox.ReadOnly = true;
            this.UpdateInfoRichTextBox.Size = new System.Drawing.Size(323, 96);
            this.UpdateInfoRichTextBox.TabIndex = 3;
            this.UpdateInfoRichTextBox.Text = "";
            // 
            // LoadingIndicator
            // 
            this.LoadingIndicator.ImageLocation = "Loading.gif";
            this.LoadingIndicator.Location = new System.Drawing.Point(300, 34);
            this.LoadingIndicator.Name = "LoadingIndicator";
            this.LoadingIndicator.Size = new System.Drawing.Size(35, 35);
            this.LoadingIndicator.TabIndex = 4;
            this.LoadingIndicator.TabStop = false;
            this.LoadingIndicator.Visible = false;
            // 
            // linkLabel2
            // 
            this.WebsiteLinkLabel.AutoSize = true;
            this.WebsiteLinkLabel.Location = new System.Drawing.Point(289, 18);
            this.WebsiteLinkLabel.Name = "linkLabel2";
            this.WebsiteLinkLabel.Size = new System.Drawing.Size(46, 13);
            this.WebsiteLinkLabel.TabIndex = 5;
            this.WebsiteLinkLabel.TabStop = true;
            this.WebsiteLinkLabel.Text = "Website";
            this.WebsiteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebsiteClicked);
            // 
            // AboutF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 183);
            this.Controls.Add(this.WebsiteLinkLabel);
            this.Controls.Add(this.LoadingIndicator);
            this.Controls.Add(this.UpdateInfoRichTextBox);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.CurrentVersionLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AboutF";
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.LoadingIndicator)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label CurrentVersionLabel;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.RichTextBox UpdateInfoRichTextBox;
        private System.Windows.Forms.PictureBox LoadingIndicator;
        private System.Windows.Forms.LinkLabel WebsiteLinkLabel;
    }
}