using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Intelligent_Conversation_Platform
{
    public partial class ChangeCode : Form
    {
        public ChangeCode()
        {
            InitializeComponent();
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            StreamReader sr = File.OpenText("code1.txt");
            string login_code = sr.ReadLine();//读取密码文本中的密码
            sr.Close();
            if (textBox_oldcode.Text == login_code)
            {
                File.Delete("code1.txt");
                using (StreamWriter sw = new StreamWriter("code1.txt", false, Encoding.UTF8))
                {
                    if (textBox_newcode.Text != null)
                    {
                        sw.Write(textBox_newcode.Text);
                        sw.Close();
                        MessageBox.Show("登录密码已成功修改！");
                    }
                    else
                    {
                        MessageBox.Show("未输入新密码！");
                        this.textBox_oldcode.Text = null;
                    }
                }
            }
            else
            {
                MessageBox.Show("密码错误！");
                this.textBox_oldcode.Text = null;
            }
        }

        private void button_admin_Click(object sender, EventArgs e)
        {
            StreamReader sr = File.OpenText("code2.txt");
            string admin_code = sr.ReadLine();//读取密码文本中的密码
            sr.Close();
            if (textBox_oldcode.Text == admin_code)
            {
                File.Delete("code2.txt");
                using (StreamWriter sw = new StreamWriter("code2.txt", false, Encoding.UTF8))
                {
                    if (textBox_newcode.Text != null)
                    {
                        sw.Write(textBox_newcode.Text);
                        sw.Close();
                        MessageBox.Show("管理员密码已成功修改！");
                    }
                    else
                    {
                        MessageBox.Show("未输入新密码！");
                        this.textBox_oldcode.Text = null;
                    }
                }
            }
            else
            {
                MessageBox.Show("密码错误！");
                this.textBox_oldcode.Text = null;
            }
        }
    }
}
