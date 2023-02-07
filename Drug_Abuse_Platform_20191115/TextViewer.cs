using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intelligent_Conversation_Platform
{
    public partial class TextViewer : Form
    {
        private string file_path;
        private string file_text;
        private string secret = null;

        public TextViewer()
        {
            InitializeComponent();
        }

        public TextViewer(string path)
        {
            InitializeComponent();
            file_path = path;
            FileStream fs = new FileStream(file_path, FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs);
            file_text = sr.ReadToEnd();
            fs.Close();
            sr.Close();
            file_text = file_text.ToString();
            textBox1.Text = file_text;
        }

        private void TextViewer_Load(object sender, EventArgs e)
        {
            textBox1.Left = 0;
            textBox1.Width = this.ClientSize.Width;
            textBox1.Top = 0;
            textBox1.Height = this.ClientSize.Height;
        }

        private void TextViewer_Resize(object sender, EventArgs e)
        {
            textBox1.Left = 0;
            textBox1.Width = this.ClientSize.Width;
            textBox1.Top = 0;
            textBox1.Height = this.ClientSize.Height;
        }

        private void TextViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (file_text != textBox1.Text)
            {
                if (MessageBox.Show("文本内容已改变，是否保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(file_path);
                    char[] text_char = TextEncrypt(textBox1.Text, secret);
                    sw.Write(new string(text_char));
                    sw.Close();
                }
            }
        }

        private char[] TextEncrypt(string content, string secretKey)
        {
            char[] data = content.ToCharArray();
            char[] key = secretKey.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            return data;
        }

        private string TextDecrypt(char[] data, string secretKey)
        {
            char[] key = secretKey.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }
            return new string(data);
        }

    }
}
