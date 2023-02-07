using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;


namespace Intelligent_Conversation_Platform
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            textbox_code.Text = " 请输入密码：";
        }

        private void button_okay_Click(object sender, EventArgs e)
        {
            string input_code = this.textbox_code.Text;
            StreamReader sr = File.OpenText("code1.txt");
            string correct_code = sr.ReadLine();//读取密码文本中的密码
            sr.Close();
            if (input_code == correct_code)
            {
                this.DialogResult = DialogResult.OK;    //返回一个登录成功的对话框状态
                this.Close();
            }
            else
            {
                MessageBox.Show("密码不正确！请重新输入！");
                this.textbox_code.Text = null;
            }
        }       // 点击登陆按钮

        private void button_exit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }       // 点击退出按钮    
        private void Exit_MouseEnter(object sender, EventArgs e)
        {
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.BackColor = Color.FromArgb(85, 210, 246);
            p.SetToolTip(this.button_exit, "退出程序");
        }
        private void textbox_code_Enter(object sender, EventArgs e)
        {
            textbox_code.Text = "";
            textbox_code.PasswordChar = '●';
        }
    }
}
