namespace Intelligent_Conversation_Platform
{
    partial class ChangeCode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeCode));
            this.textBox_oldcode = new System.Windows.Forms.TextBox();
            this.textBox_newcode = new System.Windows.Forms.TextBox();
            this.button_login = new System.Windows.Forms.Button();
            this.button_admin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_oldcode
            // 
            this.textBox_oldcode.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.textBox_oldcode.Location = new System.Drawing.Point(136, 9);
            this.textBox_oldcode.Name = "textBox_oldcode";
            this.textBox_oldcode.PasswordChar = '●';
            this.textBox_oldcode.Size = new System.Drawing.Size(100, 25);
            this.textBox_oldcode.TabIndex = 0;
            // 
            // textBox_newcode
            // 
            this.textBox_newcode.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.textBox_newcode.Location = new System.Drawing.Point(136, 44);
            this.textBox_newcode.Name = "textBox_newcode";
            this.textBox_newcode.Size = new System.Drawing.Size(100, 25);
            this.textBox_newcode.TabIndex = 1;
            // 
            // button_login
            // 
            this.button_login.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_login.Location = new System.Drawing.Point(68, 119);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(168, 30);
            this.button_login.TabIndex = 2;
            this.button_login.Text = "修改登录密码";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // button_admin
            // 
            this.button_admin.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_admin.Location = new System.Drawing.Point(68, 79);
            this.button_admin.Name = "button_admin";
            this.button_admin.Size = new System.Drawing.Size(168, 30);
            this.button_admin.TabIndex = 3;
            this.button_admin.Text = "修改管理员密码";
            this.button_admin.UseVisualStyleBackColor = true;
            this.button_admin.Click += new System.EventHandler(this.button_admin_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(64, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 25);
            this.label2.TabIndex = 126;
            this.label2.Text = "旧密码：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(64, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 25);
            this.label1.TabIndex = 127;
            this.label1.Text = "新密码：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChangeCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 161);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_admin);
            this.Controls.Add(this.button_login);
            this.Controls.Add(this.textBox_newcode);
            this.Controls.Add(this.textBox_oldcode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeCode";
            this.Text = "修改密码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_oldcode;
        private System.Windows.Forms.TextBox textBox_newcode;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.Button button_admin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}