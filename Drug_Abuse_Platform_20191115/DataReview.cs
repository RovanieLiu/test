using System;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace Intelligent_Conversation_Platform
{
    public partial class DataReview : Form
    {
        public DataReview()
        {
            InitializeComponent();
        }
        private void LoadData(string path)
        {
            Console.WriteLine(Path.Combine(path, "result.txt"));
            StreamReader sr = new StreamReader(Path.Combine(path, "result.txt"), Encoding.Default);
            String line = sr.ReadLine();
            sr.Close();
            int index = this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[index].Cells[0].Value = count;
            this.dataGridView1.Rows[index].Cells[1].Value = line.Split(',')[0];
            this.dataGridView1.Rows[index].Cells[2].Value = line.Split(',')[1];
            this.dataGridView1.Rows[index].Cells[3].Value = line.Split(',')[2];
            this.dataGridView1.Rows[index].Cells[4].Value = line.Split(',')[3];
            this.dataGridView1.Rows[index].Cells[5].Value = line.Split(',')[4];
            this.dataGridView1.Rows[index].Cells[6].Value = line.Split(',')[5];
            this.dataGridView1.Rows[index].Cells[7].Value = line.Split(',')[6];
            this.dataGridView1.Rows[index].Cells[8].Value = line.Split(',')[7];
            this.dataGridView1.Rows[index].Cells[9].Value = line.Split(',')[8];
            this.dataGridView1.Rows[index].Cells[10].Value = line.Split(',')[9];
            this.dataGridView1.Rows[index].Cells[11].Value = line.Split(',')[10];
            this.dataGridView1.Rows[index].Cells[12].Value = line.Split(',')[11];
        }
        private int count;
        private string textname;
        private string textpath;
        private List<string> folderList = new List<string>();
        private void button_search_Click(object sender, EventArgs e)
        {
            count = 0;
            folderList.Clear();
            string input_code = this.textBox_code.Text;
            StreamReader sr = File.OpenText("code2.txt");
            string correct_code = sr.ReadLine();//读取密码文本中的密码
            sr.Close();
            if (input_code == correct_code)
            {
                textpath = textBox_path.Text;
                textname = textBox_name.Text;
                DirectoryInfo folder = new DirectoryInfo(textpath);
                DirectoryInfo[] di = folder.GetDirectories();
                for (int i = 0; i < di.Length; i++)
                {
                    if (di[i].FullName.ToString().Split('_')[0] == (textpath + "\\" + textname))
                    {
                        count += 1;
                        folderList.Add(di[i].FullName);
                        LoadData(di[i].FullName.ToString());
                    }
                }
                if (count == 0)
                {
                    MessageBox.Show("未能查找到此人的任何记录！");
                }
            }
            else
            {
                MessageBox.Show("密码不正确！请重新输入！");
                this.textBox_code.Text = null;
            }
        }
        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewButtonCell btnCell = dataGridView1.CurrentCell as DataGridViewButtonCell;
            if (btnCell.ColumnIndex == 13 && e.RowIndex < folderList.ToArray().Length && count > 0)
            {
                Directory.Delete(@folderList[btnCell.RowIndex], true);
                MessageBox.Show("已删除" + folderList[btnCell.RowIndex]);
                this.dataGridView1.Rows.Clear();
                folderList.Clear();
                count = 0;
                DirectoryInfo folder = new DirectoryInfo(textpath);
                DirectoryInfo[] di = folder.GetDirectories();
                for (int i = 0; i < di.Length; i++)
                {
                    if (di[i].FullName.ToString().Split('_')[0] == (textpath + "\\" + textname))
                    {
                        count += 1;
                        folderList.Add(di[i].FullName);
                        LoadData(di[i].FullName.ToString());
                    }
                }
            }
            if (btnCell.ColumnIndex == 14 && e.RowIndex < folderList.ToArray().Length && count > 0)
            {
                System.Diagnostics.Process.Start("Explorer.exe", folderList[btnCell.RowIndex]);
            }
        }
    }
}
