namespace Intelligent_Conversation_Platform
{
    partial class PictureViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PictureViewer));
            this.ImageBox_0 = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_0)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox_0
            // 
            this.ImageBox_0.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ImageBox_0.Location = new System.Drawing.Point(22, 12);
            this.ImageBox_0.Name = "ImageBox_0";
            this.ImageBox_0.Size = new System.Drawing.Size(690, 519);
            this.ImageBox_0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_0.TabIndex = 45;
            this.ImageBox_0.TabStop = false;
            // 
            // PictureViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 543);
            this.Controls.Add(this.ImageBox_0);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PictureViewer";
            this.Text = "PictureViewer";
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_0)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBox_0;
    }
}