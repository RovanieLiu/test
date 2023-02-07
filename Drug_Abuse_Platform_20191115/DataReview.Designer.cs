namespace Intelligent_Conversation_Platform
{
    partial class DataReview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataReview));
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.textBox_code = new System.Windows.Forms.TextBox();
            this.button_search = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.age = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.attack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.depression = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.anxiety = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lie = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.suicide = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.open = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox_path
            // 
            this.textBox_path.Font = new System.Drawing.Font("Microsoft PhagsPa", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_path.Location = new System.Drawing.Point(116, 9);
            this.textBox_path.Name = "textBox_path";
            this.textBox_path.Size = new System.Drawing.Size(128, 32);
            this.textBox_path.TabIndex = 0;
            this.textBox_path.Text = "E:\\data";
            // 
            // textBox_name
            // 
            this.textBox_name.Font = new System.Drawing.Font("Microsoft PhagsPa", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_name.Location = new System.Drawing.Point(363, 9);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(128, 32);
            this.textBox_name.TabIndex = 1;
            // 
            // textBox_code
            // 
            this.textBox_code.Font = new System.Drawing.Font("Microsoft PhagsPa", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_code.Location = new System.Drawing.Point(624, 10);
            this.textBox_code.Name = "textBox_code";
            this.textBox_code.PasswordChar = '●';
            this.textBox_code.Size = new System.Drawing.Size(128, 32);
            this.textBox_code.TabIndex = 2;
            // 
            // button_search
            // 
            this.button_search.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_search.Location = new System.Drawing.Point(824, 9);
            this.button_search.Name = "button_search";
            this.button_search.Size = new System.Drawing.Size(128, 32);
            this.button_search.TabIndex = 3;
            this.button_search.Text = "查询记录";
            this.button_search.UseVisualStyleBackColor = true;
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.index,
            this.time,
            this.name,
            this.age,
            this.gender,
            this.mode,
            this.result,
            this.score,
            this.attack,
            this.depression,
            this.anxiety,
            this.lie,
            this.suicide,
            this.delete,
            this.open});
            this.dataGridView1.Location = new System.Drawing.Point(0, 52);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1180, 508);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick_1);
            // 
            // index
            // 
            this.index.Frozen = true;
            this.index.HeaderText = "序号";
            this.index.Name = "index";
            this.index.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.index.Width = 56;
            // 
            // time
            // 
            this.time.Frozen = true;
            this.time.HeaderText = "谈话时间";
            this.time.Name = "time";
            this.time.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.time.Width = 160;
            // 
            // name
            // 
            this.name.Frozen = true;
            this.name.HeaderText = "姓名";
            this.name.Name = "name";
            this.name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.name.Width = 56;
            // 
            // age
            // 
            this.age.Frozen = true;
            this.age.HeaderText = "年龄";
            this.age.Name = "age";
            this.age.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.age.Width = 56;
            // 
            // gender
            // 
            this.gender.Frozen = true;
            this.gender.HeaderText = "性别";
            this.gender.Name = "gender";
            this.gender.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.gender.Width = 56;
            // 
            // mode
            // 
            this.mode.Frozen = true;
            this.mode.HeaderText = "谈话类型";
            this.mode.Name = "mode";
            this.mode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.mode.Width = 80;
            // 
            // result
            // 
            this.result.Frozen = true;
            this.result.HeaderText = "评估结果";
            this.result.Name = "result";
            this.result.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.result.Width = 80;
            // 
            // score
            // 
            this.score.Frozen = true;
            this.score.HeaderText = "问卷评分";
            this.score.Name = "score";
            this.score.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.score.Width = 80;
            // 
            // attack
            // 
            this.attack.Frozen = true;
            this.attack.HeaderText = "攻击性";
            this.attack.Name = "attack";
            this.attack.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.attack.Width = 80;
            // 
            // depression
            // 
            this.depression.Frozen = true;
            this.depression.HeaderText = "抑郁程度";
            this.depression.Name = "depression";
            this.depression.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.depression.Width = 80;
            // 
            // anxiety
            // 
            this.anxiety.Frozen = true;
            this.anxiety.HeaderText = "焦虑程度";
            this.anxiety.Name = "anxiety";
            this.anxiety.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.anxiety.Width = 80;
            // 
            // lie
            // 
            this.lie.Frozen = true;
            this.lie.HeaderText = "谎言概率";
            this.lie.Name = "lie";
            this.lie.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.lie.Width = 80;
            // 
            // suicide
            // 
            this.suicide.Frozen = true;
            this.suicide.HeaderText = "自杀概率";
            this.suicide.Name = "suicide";
            this.suicide.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.suicide.Width = 80;
            // 
            // delete
            // 
            this.delete.Frozen = true;
            this.delete.HeaderText = "操作";
            this.delete.Name = "delete";
            this.delete.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.delete.Text = "删除";
            this.delete.UseColumnTextForButtonValue = true;
            this.delete.Width = 56;
            // 
            // open
            // 
            this.open.Frozen = true;
            this.open.HeaderText = "操作";
            this.open.Name = "open";
            this.open.Text = "查看";
            this.open.UseColumnTextForButtonValue = true;
            this.open.Width = 56;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(-4, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 32);
            this.label2.TabIndex = 125;
            this.label2.Text = "存储目录：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(250, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 32);
            this.label1.TabIndex = 126;
            this.label1.Text = "查询姓名：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(497, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 32);
            this.label3.TabIndex = 127;
            this.label3.Text = "管理员密码：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DataReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(1180, 561);
            this.Controls.Add(this.textBox_code);
            this.Controls.Add(this.textBox_name);
            this.Controls.Add(this.textBox_path);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button_search);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DataReview";
            this.Text = "历史谈话记录";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.TextBox textBox_code;
        private System.Windows.Forms.Button button_search;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewTextBoxColumn index;
        private System.Windows.Forms.DataGridViewTextBoxColumn time;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn age;
        private System.Windows.Forms.DataGridViewTextBoxColumn gender;
        private System.Windows.Forms.DataGridViewTextBoxColumn mode;
        private System.Windows.Forms.DataGridViewTextBoxColumn result;
        private System.Windows.Forms.DataGridViewTextBoxColumn score;
        private System.Windows.Forms.DataGridViewTextBoxColumn attack;
        private System.Windows.Forms.DataGridViewTextBoxColumn depression;
        private System.Windows.Forms.DataGridViewTextBoxColumn anxiety;
        private System.Windows.Forms.DataGridViewTextBoxColumn lie;
        private System.Windows.Forms.DataGridViewTextBoxColumn suicide;
        private System.Windows.Forms.DataGridViewButtonColumn delete;
        private System.Windows.Forms.DataGridViewButtonColumn open;
    }
}