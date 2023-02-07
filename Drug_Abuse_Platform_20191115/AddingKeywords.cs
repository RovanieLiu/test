using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace Intelligent_Conversation_Platform
{
    public partial class AddingKeywords : Form
    {
        private string akeyword;
        public string allkeywordsstring;
        private string keywordtext_path = Path.Combine(Application.StartupPath, "告警语句.txt");
        public AddingKeywords()
        {
            InitializeComponent();
            for (int i = 0; i < ProcessVideo.allkeywords.Count; i++)
            {
                listBox1.Items.Add(ProcessVideo.allkeywords[i]);
            }
        }

        public void addbutton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入需要添加的告警语句！");
            }
            else
            {
                akeyword = textBox1.Text;
                textBox1.Text = null;
                ProcessVideo.allkeywords.Add(akeyword);
                allkeywordsstring = null;
                for (int i = 0; i < ProcessVideo.allkeywords.Count; i++)
                {
                    listBox1.Items.Add(ProcessVideo.allkeywords[i]);
                    allkeywordsstring = allkeywordsstring + ProcessVideo.allkeywords[i] + "\r\n";
                }
                if (File.Exists(keywordtext_path))
                {
                    File.Delete(keywordtext_path);
                }
                using (StreamWriter sw = new StreamWriter(keywordtext_path, false, Encoding.UTF8))
                {
                    sw.Write(allkeywordsstring);
                    sw.Close();
                }
                listBox1.Items.Clear();
                for (int i = 0; i < ProcessVideo.allkeywords.Count; i++)
                {
                    listBox1.Items.Add(ProcessVideo.allkeywords[i]);
                }
                MessageBox.Show("告警语句添加成功！");
            }
        }

        private void deletebutton_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请选中需要删除的告警语句！");
            }
            else
            {
                ProcessVideo.allkeywords.RemoveAt(index);
                listBox1.Items.Clear();
                allkeywordsstring = null;
                for (int i = 0; i < ProcessVideo.allkeywords.Count; i++)
                {
                    listBox1.Items.Add(ProcessVideo.allkeywords[i]);
                    allkeywordsstring = allkeywordsstring + ProcessVideo.allkeywords[i] + "\r\n";
                }

                if (File.Exists(keywordtext_path))
                {
                    File.Delete(keywordtext_path);
                }
                using (StreamWriter sw = new StreamWriter(keywordtext_path, false, Encoding.UTF8))
                {
                    sw.Write(allkeywordsstring);
                    sw.Close();
                }
                MessageBox.Show("告警语句删除成功！");
            }
        }

    }
}
