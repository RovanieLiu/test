using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using CCWin;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AxSXClientActiveXControlLib;
using BarSpectrumcs;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using ESBasic;
using OMCS.Tools;
using Oraycn.MCapture;
using Oraycn.MFile;
using VoiceEmotionAnalyser;
using System.Speech.Synthesis;
using System.Drawing.Drawing2D;

namespace Intelligent_Conversation_Platform
{
    public partial class ProcessVideo : Form, Affdex.ImageListener
    {
        #region 界面初始化
        public ProcessVideo()
        {
            InitializeComponent();
            Init_CameraDic();
            Init_MicphoneDic();
            Init_EmotionDic();
            Init_EmotionArray();
            Init_Background_Worker();
            Init_iFlytek();
            Init_Speech();
            Init_QnaireList();
            this.Width = 1920;
            this.Height = 1080;
            this.person = new Person();
            this.timer1.Interval = timer1_interval;
            this.timer2.Interval = timer2_interval;
            this.face_size = Affdex.FaceDetectorMode.LARGE_FACES;
            this.textbox_name.Text = person.name;
            this.textbox_age_range.Text = person.age_range;
            this.textbox_gender.SelectedIndex = 0;
            this.combox_talkmode.SelectedIndex = 0;
            clear_videoFile();
            save_root_is_done = check_save_root();
            add_toolStripMenuItem();
            get_data_from_config();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            timer2.Enabled = true;
            allmode = combox_talkmode.Items.Count;  // last index + 1
            
        }//Process类的初始化函数
        [ComVisibleAttribute(true)]

        #region _baseChatHtml
        private string _baseChatHtml = @"<html><head>
            <script type=""text/javascript"">window.location.hash = ""#ok"";</script>
            <style type=""text/css"">
            body{
            font-family:微软雅黑;
            font-size:14px;
            background: rgb(11, 12, 39);
            }
            /*滚动条宽度*/  
            ::-webkit-scrollbar {  
            width: 8px;  
            }  
   
            /* 轨道样式 */  
            ::-webkit-scrollbar-track {  
            }  
   
            /* Handle样式 */  
            ::-webkit-scrollbar-thumb {  
            border-radius: 10px;  
            background: rgba(0,0,0,0.2);   
            }  
  
            /*当前窗口未激活的情况下*/  
            ::-webkit-scrollbar-thumb:window-inactive {  
            background: rgba(0,0,0,0.1);   
            }  
  
            /*hover到滚动条上*/  
            ::-webkit-scrollbar-thumb:vertical:hover{  
            background-color: rgba(0,0,0,0.3);  
            }  
            /*滚动条按下*/  
            ::-webkit-scrollbar-thumb:vertical:active{  
            background-color: rgba(0,0,0,0.7);  
            }  
            textarea{width: 500px;height: 300px;border: none;padding: 5px;}  

	        .chat_content_group.self {
            text-align: right;
            }
            .chat_content_group {
            margin: 10px;
            }
            .chat_content_group.self>.chat_content {
            text-align: left;
            }
            .chat_content_group.self>.chat_content {
            background: #46b9f1;
            color:#FFFFFF;
            }
            .chat_content {
            display: inline-block;
            min-height: 16px;
            max-width: 50%;
            color:#292929;
            background: #FFFFFF;
            font-family:微软雅黑;
            font-size:15px;
            -webkit-border-radius: 5px;
            -moz-border-radius: 5px;
            border-radius: 5px;
            padding: 10px 15px;
            margin: 5px 0;
            word-break: break-all;
            line-height: 1.4;
            }

            .chat_content_group.self>.chat_nick {
            text-align: right;
            }
            .chat_nick {
            font-size: 13px;
            margin: 1px 1px;
            color:#8b8b8b;
            }

            .chat_content_group.buddy {
            text-align: left;
            }
            .chat_content_group {
            margin: 10px;
            }
	        </style>
            </head><body>";
        #endregion
        private void ProcessVideo_Load(object sender, EventArgs e)
        {
            webKitBrowser1.IsWebBrowserContextMenuEnabled = false;
            webKitBrowser1.DocumentText = _baseChatHtml;
            menuStrip1.BackColor = Color.FromArgb(11, 12, 39);
            
            X = this.Width;
            Y = this.Height;
            this.FormBorderStyle = FormBorderStyle.None;
            setTag(this);
        }//加载界面
        private void ProcessVideo_Resize(object sender, EventArgs e)
        {
            float newx = this.Width / X;
            float newy = this.Height / Y;
            try
            {
                setControls(newx, newy, this);
            }
            catch
            {

            }
        }//缩放界面
        private void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                if (con.Name == "textbox_recording_interval" || con.Name == "horizontalScrollBar" || con.Name == "verticalScrollBar")
                {
                    continue;
                }
                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });
                float a = Convert.ToSingle(mytag[0]) * newx;
                con.Width = (int)a;
                a = Convert.ToSingle(mytag[1]) * newy;
                con.Height = (int)(a);
                a = Convert.ToSingle(mytag[2]) * newx;
                con.Left = (int)(a);
                a = Convert.ToSingle(mytag[3]) * newy;
                con.Top = (int)(a);
                Single currentSize = Convert.ToSingle(mytag[4]) * newy;
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }
        }//实现控件以及字体的缩放
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;//获取或设置包含有关控件的数据的对象
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }//Control类，定义控件的基类
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                ImageBox_0.Refresh();
                pictureBox1.Refresh();
                pictureBox2.Refresh();
                textbox_recording_interval.Refresh();
                if (!paint_fresh_flag)
                {
                    return;
                }

                if (warningmessage_flag)
                {
                    warningmessage();
                    warningmessage_flag = false;
                    if (faceRectangle.Height > 0)
                    {
                        SB3.Append(DateTime.Now.ToLocalTime() + "," + progressbar_谎言.Value + "," + progressbar_抑郁.Value + "," + progressbar_焦虑.Value + "," + progressbar_攻击.Value + "," + progressbar_关注.Value + "," + progressbar_压力.Value + "," + progressbar_兴奋.Value + "," + progressbar_犹豫.Value + "\r\n");
                    }
                }

                if (camera_main_record_flag)
                {
                    update_record_interval();
                }
                if (detector_id == camera_detetcor_id || camera_0_fresh_flag)
                {
                    if (!small_imagebox_flag)
                    {
                        if (camera_0_fresh_flag)
                        {
                            ImageBox_0.Image = frame_0;
                        }
                        if (main_camera_fresh_flag && matImage != null)
                        {
                            ImageBox_0.Image = matImage;
                            if (faces != null)
                            {
                                showResults(e.Graphics, faces);
                            }
                        }
                    }
                    else
                    {
                        if (main_camera_fresh_flag && matImage != null)
                        {
                            ImageBox_0.Image = matImage;
                            if (faces != null)
                            {
                                showResults(e.Graphics, faces);
                            }
                        }
                    }
                }
                if (StartButton_flag)
                {
                    if (qindex < questionList.Count - 1)
                    {
                        NextButton.Text = "下一题";
                    }
                    else
                    {
                        NextButton.Text = "完成并提交";
                    }
                }
                e.Graphics.Flush();
            }
            catch
            {

            }
        }//回调函数，绘制窗体
        private void paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;   //实例化Graphics 对象g
            Color FColor = Color.White; //颜色1
            Color TColor = Color.Blue;  //颜色2 
            Brush b = new LinearGradientBrush(this.ClientRectangle, FColor, TColor, LinearGradientMode.Horizontal);  //实例化刷子，第一个参数指示上色区域，第二个和第三个参数分别渐变颜色的开始和结束，第四个参数表示颜色的方向。
            g.FillRectangle(b, this.ClientRectangle);  //进行上色
            g.Dispose();
        }
        #endregion
        #region 程序初始化
        private void add_toolStripMenuItem()
        {
            camera_number = 0;
            micphone_number = 0;
            IList<CameraInformation> cameras = Camera.GetCameras();
            foreach (var item in cameras)
            {
                string line = "Camera_" + item.Index.ToString() + " : " + item.Name;
                main_camera_combox.Items.AddRange(new object[] { line });
                cameras_dic[item.Index].Text = line;
                cameras_dic[item.Index].Name = line;
                cameras_dic[item.Index].Size = new Size(224, 26);
                camera_number += 1;
            }
            //获取麦克风列表
            IList<MicrophoneInformation> microphones = SoundDevice.GetMicrophones();
            foreach (var item in microphones)
            {
                string line = "Micphone_" + item.Index.ToString() + " : " + item.Name;
                //麦克风.DropDownItems.AddRange(new ToolStripItem[] { micphones_dic[item.Index] });
                main_micphone_combox.Items.AddRange(new object[] { line });
                //micphones_dic[item.Index].Text = line;
                //micphones_dic[item.Index].Name = line;
                //micphones_dic[item.Index].Size = new System.Drawing.Size(224, 26);
                //micphones_dic[item.Index].Click += new EventHandler(open_camera);
                micphone_number += 1;
            }

            //获取扬声器列表
            IList<SpeakerInformation> speakers = SoundDevice.GetSpeakers();
            foreach (var item in speakers)
            {
                //Console.WriteLine(item);
            }
        }//根据获得摄像头和麦克风数增加工具栏下拉菜单
        private void clear_videoFile()
        {
            Directory.GetFiles(videofile_basepath).ToList().ForEach(File.Delete);
            foreach (var dir in Directory.GetDirectories(videofile_basepath))
            {
                Directory.GetFiles(videofile_basepath).ToList().ForEach(File.Delete);
            }
        } //清理videofile_basepath目录下缓存的视频文件
        public bool check_save_root()
        {
            save_root = ConfigurationManager.AppSettings["save_root"];
            //filepath = ConfigurationManager.AppSettings["eyemoving_root"];
            show_save_root.Text = save_root;
            //show_eyemoving_root.Text = filepath;
            string driver_root = Path.GetPathRoot(save_root);
            if (!Directory.Exists(driver_root))    ////磁盘不存在
            {
                return false;
            }
            if (driver_is_full(driver_root))      ////磁盘空间已满
            {
                return false;
            }

            if (!Directory.Exists(save_root))        ////磁盘存在，但是目录不存在
            {
                Directory.CreateDirectory(save_root);
            }
            return true;
        }//检查文件保存的根目录是否符合条件
        public bool driver_is_full(string driver_name)
        {
            long ldr = 0;
            long gb = 1024 * 1024 * 1024;

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.Name == driver_name)
                {
                    ldr = drive.TotalFreeSpace / gb;
                    if (ldr < 3)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        } //检查磁盘空间是否不足
        private void get_data_from_config()
        {
            string main_camera_id_string = ConfigurationManager.AppSettings["main_camera_id"];
            string main_micphone_id_string = ConfigurationManager.AppSettings["main_micphone_id"];
            string recording_time_string = ConfigurationManager.AppSettings["recording_time"];
            string face_size_string = ConfigurationManager.AppSettings["face_size"];

            main_camera_id = int.Parse(main_camera_id_string);
            main_micphone_id = int.Parse(main_micphone_id_string);

            if (main_camera_id < camera_number)
            {
                this.main_camera_combox.SelectedIndex = main_camera_id;
            }
            else
            {
                if (camera_number == 0)
                {
                    MessageBox.Show("没有检测到摄像头，请检查是否连接摄像头！");
                }
                if (camera_number > 0)
                {
                    main_camera_id = 0;
                    this.main_camera_combox.SelectedIndex = 0;
                }
            }

            if (main_micphone_id < micphone_number)
            {
                this.main_micphone_combox.SelectedIndex = main_micphone_id;
            }
            else
            {
                if (micphone_number == 0)
                {
                    MessageBox.Show("没有检测到麦克风，请检查是否连接麦克风！");
                }
                if (micphone_number > 0)
                {
                    main_micphone_id = 0;
                    this.main_micphone_combox.SelectedIndex = 0;
                }
            }

            this.recording_time_combox.SelectedIndex = int.Parse(recording_time_string);
            this.timer1.Interval = (15 + this.recording_time_combox.SelectedIndex * 15) * 60 * 1000;

            this.face_size_combox.SelectedIndex = int.Parse(face_size_string);
            choose_face_size(this.face_size_combox.SelectedIndex);
        }//从config文件中获得各种信息
        private void choose_face_size(int i)
        {
            if (i == 1)
            {
                face_size = Affdex.FaceDetectorMode.SMALL_FACES;
            }
            if (i == 0)
            {
                face_size = Affdex.FaceDetectorMode.LARGE_FACES;
            }
        }//根据索引选择人脸大小指标
        private void update_save_root(string dir)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            config.AppSettings.Settings["save_root"].Value = dir;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
            show_save_root.Text = dir;
        }//更新config文件中修改后的"save_root"的值
        //private void update_eyemoving_root(string dir)
        //{
        //    string file = System.Windows.Forms.Application.ExecutablePath;
        //    Configuration config = ConfigurationManager.OpenExeConfiguration(file);
        //    config.AppSettings.Settings["eyemoving_root"].Value = dir;
        //    config.Save();
        //    ConfigurationManager.RefreshSection("appSettings");
        //    show_eyemoving_root.Text = dir;
        //}//更新config文件中修改后的"eyemoving_root"的值
        private void update_face_size(int i)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            config.AppSettings.Settings["face_size"].Value = i.ToString();
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }//更新config文件中修改后的"face_size"的值
        private void update_recording_interval(int i)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            config.AppSettings.Settings["recording_time"].Value = i.ToString();
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }//更新config文件中修改后的"recording_time"的值
        #endregion
        #region 功能按钮
        private void Mini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void Mini_MouseEnter(object sender, EventArgs e)
        {
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.BackColor = Color.FromArgb(85, 210, 246);
            p.SetToolTip(this.MiniWin, "最小化");
        }
        private void Shut_MouseEnter(object sender, EventArgs e)
        {
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.BackColor = Color.FromArgb(85, 210, 246);
            p.SetToolTip(this.ShutWin, "退出程序");
        }
        private void Shut_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void 添加问卷_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择要添加的问卷";
            fileDialog.Filter = "文本文件|*.txt";
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                InputDialog input = new InputDialog();
                input.ShowDialog();
                var newMode = input.Name;
                var newFile = @".\questions\" + newMode + @".txt";
                var file = fileDialog.FileName;
                File.Copy(file, newFile);
                questionnairelist.Add(newFile);
                combox_talkmode.Items.Add(newMode);
            }
        }
        private void 修改问卷_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择要修改的问卷";
            fileDialog.Filter = "文本文件|*.txt";
            fileDialog.InitialDirectory = Application.StartupPath + "\\questions";
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var file = fileDialog.FileName;
                Process.Start("notepad.exe", file);
            }
        }
        private void 删除问卷_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择要删除的问卷";
            fileDialog.Filter = "文本文件|*.txt";
            fileDialog.InitialDirectory = Application.StartupPath + "\\questions";
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var file = fileDialog.FileName;
                File.Delete(file);
                questionnairelist.Remove(file);
                string filename = Path.GetFileNameWithoutExtension(file);
                combox_talkmode.Items.Remove(filename);
            }
        }
        private void 问卷选择_SelectedIndexChanged(object sender, EventArgs e)
        {
            talkmode = combox_talkmode.SelectedIndex;
        }
        private void 开始谈话_Click(object sender, EventArgs e)
        { 
            if(combox_talkmode.SelectedItem == null)
            {
                DialogResult dr = MessageBox.Show("请先选择谈话模式！");
                if(dr == DialogResult.OK)
                {
                    combox_talkmode.SelectedIndex = 0;
                    talkmode = 0;
                }
            }
            else
            {
                if (!StartButton_flag)
                {
                    StartButton_flag = !StartButton_flag;
                    if (this.textbox_name.Text != "")
                    {
                        person.name = this.textbox_name.Text;
                    }
                    else
                    {
                        person.name = default_string;
                    }

                    if (textbox_age_range.Text != "")
                    {
                        person.age_range = this.textbox_age_range.Text;
                    }
                    else
                    {
                        person.age_range = default_string;
                    }
                    person.gender = textbox_gender.SelectedItem.ToString();
                    this.textbox_name.Text = person.name;
                    this.textbox_age_range.Text = person.age_range;
                    this.textbox_name.Refresh();
                    this.textbox_gender.Refresh();
                    textbox_age_range.Refresh();
                    if (!save_root_is_done)
                    {
                        MessageBox.Show("录制视频保存路径所在的磁盘不存在或者磁盘空间已满!\r\n,请点击“功能设置”——>“修改录制视频保存路径”,重新选择录制视频保存路径.");
                        return;
                    }
                    if (camera_main_record_flag)
                    {
                        MessageBox.Show("请先停止录制摄像头视频，再重新打开摄像头");
                        return;
                    }
                    if (person.name == null)
                    {
                        MessageBox.Show("未保存个人信息，无法打开摄像头和麦克风；请填写姓名，性别，年龄信息，并单击“保存个人信息按钮”，然后再打开摄像头和麦克风！！！");
                        return;
                    }
                    small_imagebox_flag = false;
                    paint_fresh_flag = true;
                    if (camera_0 < camera_number)
                    {
                        if (main_camera_id == camera_0)
                        {
                            if (!main_camera_fresh_flag)
                            {
                                try
                                {
                                    background_worker.RunWorkerAsync(camera_detetcor_id);
                                    webKitBrowser1.DocumentText = _baseChatHtml;
                                    webKitBar.SendToBack();
                                    webKitResult.SendToBack();
                                    webKitMode.SendToBack();
                                    StartButton.Text = "停止谈话";
                                    combox_talkmode.Enabled = false;
                                }
                                catch
                                {
                                    StartButton_flag = !StartButton_flag;
                                    MessageBox.Show("后台进程正忙，请稍后再试！");
                                }
                            }
                        }
                        else
                        {
                            if (!camera_0_fresh_flag)
                            {
                                camera_0_fresh_flag = true;
                                open_camera_0();
                            }
                        }
                    }
                    本地文件ToolStripMenuItem.Enabled = false;
                    指定讯问麦克风.Enabled = false;
                    指定讯询问摄像头ToolStripMenuItem.Enabled = false;
                    录制时间ToolStripMenuItem.Enabled = false;
                    人脸大小ToolStripMenuItem.Enabled = false;
                    修改录制视频的保存位置ToolStripMenuItem.Enabled = false;
                    StartButton.Enabled = true;
                }
                else if (OKflag)
                {
                    StartButton_flag = !StartButton_flag;
                    OKflag = false;
                    speechSyn.SpeakAsyncCancelAll();
                    if (last == false)
                    {
                        showRecResults_notfinish();
                    }
                    if (detector_id == camera_detetcor_id && Record_Camera_Flag)
                    {
                        timer1.Enabled = false;
                        temp_counter = save_counter;
                        save_counter = 1;
                        recording_audio_flag = false;
                        recording_camera_flag = false;
                        paint_fresh_flag = false;
                        is_saving_flag = true;
                        invalidate_imagebox();
                        invalidate_record_controls();
                        validate_message_textbox(message_manu_save);
                        background_worker.RunWorkerAsync(manu_save_id);
                        axSXClientActiveXControl1.StopRecord();
                        本地文件ToolStripMenuItem.Enabled = true;
                        指定讯问麦克风.Enabled = true;
                        指定讯询问摄像头ToolStripMenuItem.Enabled = true;
                        录制时间ToolStripMenuItem.Enabled = true;
                        人脸大小ToolStripMenuItem.Enabled = true;
                        修改录制视频的保存位置ToolStripMenuItem.Enabled = true;
                    }
                    StartButton.Text = "开始谈话";
                    NextButton.Text = "下一题";
                    NextButton.Visible = false;
                    combox_talkmode.Enabled = true;
                    StartButton.Enabled = true;
                }
            }
            
        }
        private void 下一题_Click(object sender, EventArgs e)
        {
            fscore += qscore;
            if (qindex < questionList.Count - 1)
            {
                qindex++;
                AskQuestion();
            }
            else
            {
                if (qindex == questionList.Count - 1)
                {
                    qindex++;
                    findex = qindex;
                    last = true;
                }
                if(qindex == questionList.Count)
                {
                    // show results
                    showRecResults();
                    NextButton.Hide();
                }
            }
        }
        private void 打开本地文档_Click(object sender, EventArgs e)
        {
            if (is_saving_flag)
            {
                MessageBox.Show("正在保存视频，请在视频保存完成后，再进行操作！");
                return;
            }
            if (camera_main_record_flag)
            {
                MessageBox.Show("正在录制视频，请先停止录制视频，再进行操作！");
                return;
            }
            if (micphone_enable_flag)
            {
                microphoneCapturer.Stop();
                micphone_enable_flag = false;
            }
            close_all_camera();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "文本文件|*.txt|所有文件|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ValidateNames = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextViewer text_viewer = new TextViewer(openFileDialog.FileName);
                text_viewer.StartPosition = FormStartPosition.CenterParent;
                text_viewer.ShowDialog();
            }
        }
        private void 打开本地视频_Click(object sender, EventArgs e)
        {
            DialogResult openvideo_result;
            OpenFileDialog openvideo = new OpenFileDialog();
            openvideo.Title = "选择录屏所在文件夹";
            openvideo.Filter = "媒体文件（所有类型）|*.mp4;*.xun;*.wma;*.wmv;*.wav;*.avi";
            openvideo_result = openvideo.ShowDialog();//运行通用对话框
            if (openvideo_result == DialogResult.OK)
            {
                local_desktop_path = openvideo.FileName;
                VideoPlayer vp = new VideoPlayer();
                vp.StartPosition = FormStartPosition.CenterParent;
                vp.Show();//显示用户的控件
            }

        }
        private void 指定摄像头_SelectedIndexChanged(object sender, EventArgs e)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            main_camera_id = main_camera_combox.SelectedIndex;
            config.AppSettings.Settings["main_camera_id"].Value = main_camera_id.ToString();
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void 指定麦克风_SelectedIndexChanged(object sender, EventArgs e)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            main_micphone_id = main_micphone_combox.SelectedIndex;
            config.AppSettings.Settings["main_micphone_id"].Value = main_micphone_id.ToString();
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void 录制时间_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer1.Interval = (recording_time_combox.SelectedIndex * 15 + 15) * 60 * 1000;
            update_recording_interval(recording_time_combox.SelectedIndex);
        }
        private void 人脸大小_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = face_size_combox.SelectedIndex;
            choose_face_size(i);
            update_face_size(i);
        }
        private void 修改录制视频保存位置_Click(object sender, EventArgs e)
        {
            DialogResult saveResult = new DialogResult();
            string temp_file_dir = null;
            using (FolderBrowserDialog saveDialog = new FolderBrowserDialog())
            {
                saveDialog.Description = "选择保存录制视频的文件夹";
                saveResult = saveDialog.ShowDialog();
                temp_file_dir = saveDialog.SelectedPath;
            }
            if (saveResult == DialogResult.OK)
            {
                save_root = temp_file_dir;
                save_root_is_done = true;
                update_save_root(save_root);
            }
            else
            {
                MessageBox.Show("未选择保存录制视频的文件夹！");
            }
        }
        private void 历史谈话记录_Click(object sender, EventArgs e)
        {
            if (StartButton_flag)
            {
                MessageBox.Show("正在谈话，请在停止谈话后进行操作！");
                return;
            }
            if (is_saving_flag)
            {
                MessageBox.Show("正在保存视频，请在视频保存完成后再进行操作！");
                return;
            }
            if (camera_main_record_flag)      ////停止录制
            {
                MessageBox.Show("正在录制视频，请先停止录制视频，再进行视频标注！");
                return;
            }
            DataReview dr = new DataReview();
            dr.StartPosition = FormStartPosition.CenterParent;
            dr.ShowDialog();
        }
        private void 修改密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (StartButton_flag)
            {
                MessageBox.Show("正在谈话，请在停止谈话后进行操作！");
                return;
            }
            if (is_saving_flag)
            {
                MessageBox.Show("正在保存视频，请在视频保存完成后再进行操作！");
                return;
            }
            if (camera_main_record_flag)      ////停止录制
            {
                MessageBox.Show("正在录制视频，请先停止录制视频，再进行视频标注！");
                return;
            }
            ChangeCode cc = new ChangeCode();
            cc.StartPosition = FormStartPosition.CenterParent;
            cc.ShowDialog();
        }
        private void ProcessVideo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (is_saving_flag)
            {
                MessageBox.Show("正在保存视频，请在视频保存完成后再进行操作！");
                e.Cancel = true;
            }
            if (camera_main_record_flag)
            {
                DialogResult result = MessageBox.Show(this, "请先点击“停止谈话”按钮，然后再关闭窗口！！！", "关闭提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
            }
            else
            {
                if (micphone_enable_flag)
                {
                    microphoneCapturer.Stop();
                    thVAD.Abort();
                }

                if (detector_enable_flag)
                {
                    if (this.detector != null)
                    {
                        this.detector.stop();
                    }

                    if (this.detector != null)
                    {
                        this.detector.Dispose();
                    }
                }

            }
            //axSXClientActiveXControl1.StopUdp();//关闭UDP
            axSXClientActiveXControl1.StopRecord();//科大讯飞插件停止录音
            axSXClientActiveXControl1.CloseOCX();//关闭科大讯飞插件
        }
        #endregion
        #region 多线程
        private void Background_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            switch ((int)e.Result)
            {
                case camera_detetcor_id:
                    main_camera_fresh_flag = true;
                    open_micphone();
                    desktop_capture();
                    invalidate_message_textbox();
                    if (StartButton_flag)
                    {
                        start_recording();
                        Record_Camera_Flag = true;
                        background_worker.RunWorkerAsync(open_iFlytek_id);
                    }
                    break;
                case open_iFlytek_id:
                    string nowtime = DateTime.Now.ToLocalTime().ToString();
                    string StartTime = nowtime + "\t" + "开始谈话" + "\r\n";
                    SB1.Append(StartTime);
                    SB3.Append("姓名:," + person.name + "," + "年龄:," + person.age_range + "," + "性别:," + person.gender +  "\r\n");
                    SB3.Append("时间" + "," + "谎言概率/%" + "," + "抑郁程度/%" + "," + "焦虑程度/%" + "," + "攻击度/%" + "," + "关注度/%" + "," + "压力值/%" + "," + "兴奋度/%" + "," + "犹豫度/%" + "\r\n");
                    DialogResult dr = MessageBox.Show("系统准备完毕！");
                    OKflag = true;
                    if (dr == DialogResult.OK && talkmode != 0)
                    {
                        background_worker.RunWorkerAsync(ready_id);
                    }
                    break;
                case close_main_camera_id:
                    invalidate_message_textbox();
                    paint_fresh_flag = true;
                    StartButton.Enabled = true;
                    break;
                case auto_save_id:
                    #region
                    if (save_counter > 2)
                    {
                        if (save_counter == 3)
                        {
                            CombineTXT(Path.Combine(save_path_main, save_name + "_临时历史笔录_1.txt"),
                                Path.Combine(save_path_main, save_name + "_临时历史笔录_2.txt"),
                                Path.Combine(save_path_main, save_name + "_历史笔录.txt"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时历史笔录_1.txt"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时历史笔录_2.txt"));

                            CombineTXT(Path.Combine(save_path_main, save_name + "_临时数据报表_1.txt"),
                                Path.Combine(save_path_main, save_name + "_临时数据报表_2.txt"),
                                Path.Combine(save_path_main, save_name + "_数据报表.txt"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时数据报表_1.txt"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时数据报表_2.txt"));

                            CombineMp4(Path.Combine(save_path_main, save_name + "_临时视频文件_1"),
                                Path.Combine(save_path_main, save_name + "_临时视频文件_2"),
                                Path.Combine(save_path_main, save_name + "_视频文件.mp4"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时视频文件_1.mp4"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时视频文件_2.mp4"));


                            CombineMp4(Path.Combine(save_path_desktop, save_name + "_临时录屏文件_1"),
                                Path.Combine(save_path_desktop, save_name + "_临时录屏文件_2"),
                                Path.Combine(save_path_desktop, save_name + "_录屏文件.mp4"));
                            File.Delete(@Path.Combine(save_path_desktop, save_name + "_临时录屏文件_1.mp4"));
                            File.Delete(@Path.Combine(save_path_desktop, save_name + "_临时录屏文件_2.mp4"));

                            CombineMp3(Path.Combine(save_path_main, save_name + "_临时音频文件_1"),
                                Path.Combine(save_path_main, save_name + "_临时音频文件_2"),
                                Path.Combine(save_path_main, save_name + "_音频文件.mp3"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时音频文件_1.mp3"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时音频文件_2.mp3"));
                        }
                        else
                        {
                            CombineTXT(Path.Combine(save_path_main, save_name + "_历史笔录.txt"),
                                Path.Combine(save_path_main, save_name + "_临时历史笔录_" + (save_counter - 1).ToString() + ".txt"),
                                Path.Combine(save_path_main, save_name + "_临时历史笔录.txt"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_临时历史笔录_" + (save_counter - 1).ToString() + ".txt"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_历史笔录.txt"));
                            FileInfo fi = new FileInfo(Path.Combine(save_path_main, save_name + "_临时历史笔录.txt"));
                            fi.MoveTo(Path.Combine(save_path_main, save_name + "_历史笔录.txt"));

                            CombineTXT(Path.Combine(save_path_main, save_name + "_数据报表.txt"),
                                Path.Combine(save_path_main, save_name + "_临时数据报表_" + (save_counter - 1).ToString() + ".txt"),
                                Path.Combine(save_path_main, save_name + "_临时数据报表.txt"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_临时数据报表_" + (save_counter - 1).ToString() + ".txt"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_数据报表.txt"));
                            FileInfo fi3 = new FileInfo(Path.Combine(save_path_main, save_name + "_临时数据报表.txt"));
                            fi3.MoveTo(Path.Combine(save_path_main, save_name + "_数据报表.txt"));

                            CombineMp4(Path.Combine(save_path_main, save_name + "_视频文件"),
                                Path.Combine(save_path_main, save_name + "_临时视频文件_" + (save_counter - 1).ToString()),
                                Path.Combine(save_path_main, save_name + "_视频文件.mp4"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_临时视频文件_" + (save_counter - 1).ToString() + ".mp4"));

                            CombineMp4(Path.Combine(save_path_desktop, save_name + "_录屏文件"),
                                Path.Combine(save_path_desktop, save_name + "_临时录屏文件_" + (save_counter - 1).ToString()),
                                Path.Combine(save_path_desktop, save_name + "_录屏文件.mp4"));
                            File.Delete(Path.Combine(save_path_desktop, save_name + "_临时录屏文件_" + (save_counter - 1).ToString() + ".mp4"));

                            CombineMp3(Path.Combine(save_path_main, save_name + "_音频文件"),
                                Path.Combine(save_path_main, save_name + "_临时音频文件_" + (save_counter - 1).ToString()),
                                Path.Combine(save_path_main, save_name + "_音频文件.mp3"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_临时音频文件_" + (save_counter - 1).ToString() + ".mp3"));
                        }
                    }
                    break;
                #endregion
                case manu_save_id:
                    #region
                    if (temp_counter > 1)
                    {
                        if (temp_counter == 2)
                        {
                            CombineTXT(Path.Combine(save_path_main, save_name + "_临时历史笔录_1.txt"),
                                Path.Combine(save_path_main, save_name + "_临时历史笔录_2.txt"),
                                Path.Combine(save_path_main, save_name + "_历史笔录.txt"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时历史笔录_1.txt"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时历史笔录_2.txt"));

                            CombineTXT(Path.Combine(save_path_main, save_name + "_临时数据报表_1.txt"),
                                Path.Combine(save_path_main, save_name + "_临时数据报表_2.txt"),
                                Path.Combine(save_path_main, save_name + "_数据报表.txt"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时数据报表_1.txt"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时数据报表_2.txt"));

                            CombineMp4(Path.Combine(save_path_main, save_name + "_临时视频文件_1"),
                                Path.Combine(save_path_main, save_name + "_临时视频文件_2"),
                                Path.Combine(save_path_main, save_name + "_视频文件.mp4"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时视频文件_1.mp4"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时视频文件_2.mp4"));

                            CombineMp4(Path.Combine(save_path_desktop, save_name + "_临时录屏文件_1"),
                                Path.Combine(save_path_desktop, save_name + "_临时录屏文件_2"),
                                Path.Combine(save_path_desktop, save_name + "_录屏文件.mp4"), true);
                            File.Delete(@Path.Combine(save_path_desktop, save_name + "_临时录屏文件_1.mp4"));
                            File.Delete(@Path.Combine(save_path_desktop, save_name + "_临时录屏文件_2.mp4"));

                            CombineMp3(Path.Combine(save_path_main, save_name + "_临时音频文件_1"),
                                Path.Combine(save_path_main, save_name + "_临时音频文件_2"),
                                Path.Combine(save_path_main, save_name + "_音频文件.mp3"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时音频文件_1.mp3"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_临时音频文件_2.mp3"));

                            Txt2Csv(Path.Combine(save_path_main, save_name + "_数据报表.txt"), Path.Combine(save_path_main, save_name + "_数据报表.csv"));
                            File.Delete(@Path.Combine(save_path_main, save_name + "_数据报表.txt"));

                        }
                        else
                        {
                            CombineTXT(Path.Combine(save_path_main, save_name + "_历史笔录.txt"),
                                Path.Combine(save_path_main, save_name + "_临时历史笔录_" + (temp_counter).ToString() + ".txt"),
                                Path.Combine(save_path_main, save_name + "_临时历史笔录.txt"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_临时历史笔录_" + (temp_counter).ToString() + ".txt"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_历史笔录.txt"));
                            FileInfo fi3 = new FileInfo(Path.Combine(save_path_main, save_name + "_临时历史笔录.txt"));
                            fi3.MoveTo(Path.Combine(save_path_main, save_name + "_历史笔录.txt"));

                            CombineTXT(Path.Combine(save_path_main, save_name + "_数据报表.txt"),
                                Path.Combine(save_path_main, save_name + "_临时数据报表_" + (temp_counter).ToString() + ".txt"),
                                Path.Combine(save_path_main, save_name + "_临时数据报表.txt"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_临时数据报表_" + (temp_counter).ToString() + ".txt"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_数据报表.txt"));
                            FileInfo fi6 = new FileInfo(Path.Combine(save_path_main, save_name + "_临时数据报表.txt"));
                            fi6.MoveTo(Path.Combine(save_path_main, save_name + "_数据报表.txt"));

                            CombineMp4(Path.Combine(save_path_main, save_name + "_视频文件"),
                                Path.Combine(save_path_main, save_name + "_临时视频文件_" + (temp_counter).ToString()),
                                Path.Combine(save_path_main, save_name + "_视频文件.mp4"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_临时视频文件_" + (temp_counter).ToString() + ".mp4"));

                            CombineMp4(Path.Combine(save_path_desktop, save_name + "_录屏文件"),
                                Path.Combine(save_path_desktop, save_name + "_临时录屏文件_" + (temp_counter).ToString()),
                                Path.Combine(save_path_desktop, save_name + "_录屏文件.mp4"), true);
                            File.Delete(Path.Combine(save_path_desktop, save_name + "_临时录屏文件_" + (temp_counter).ToString() + ".mp4"));

                            CombineMp3(Path.Combine(save_path_main, save_name + "_音频文件"),
                                Path.Combine(save_path_main, save_name + "_临时音频文件_" + (temp_counter).ToString()),
                                Path.Combine(save_path_main, save_name + "_音频文件.mp3"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_临时音频文件_" + (temp_counter).ToString() + ".mp3"));

                            Txt2Csv(Path.Combine(save_path_main, save_name + "_数据报表.txt"), Path.Combine(save_path_main, save_name + "_数据报表.csv"));
                            File.Delete(Path.Combine(save_path_main, save_name + "_数据报表.txt"));
                        }
                    }
                    else
                    {
                        FileInfo fi_mp4 = new FileInfo(Path.Combine(save_path_main, save_name + "_临时视频文件_1.mp4"));
                        fi_mp4.MoveTo(Path.Combine(save_path_main, save_name + "_视频文件.mp4"));

                        FileInfo fi_mp41 = new FileInfo(Path.Combine(save_path_desktop, save_name + "_临时录屏文件_1.mp4"));
                        fi_mp41.MoveTo(Path.Combine(save_path_desktop, save_name + "_录屏文件.mp4"));

                        FileInfo fi_txt = new FileInfo(Path.Combine(save_path_main, save_name + "_临时历史笔录_1.txt"));
                        fi_txt.MoveTo(Path.Combine(save_path_main, save_name + "_历史笔录.txt"));

                        FileInfo fi_txt3 = new FileInfo(Path.Combine(save_path_main, save_name + "_临时数据报表_1.txt"));
                        fi_txt3.MoveTo(Path.Combine(save_path_main, save_name + "_数据报表.txt"));

                        FileInfo fi_mp3 = new FileInfo(Path.Combine(save_path_main, save_name + "_临时音频文件_1.mp3"));
                        fi_mp3.MoveTo(Path.Combine(save_path_main, save_name + "_音频文件.mp3"));

                        Txt2Csv(Path.Combine(save_path_main, save_name + "_数据报表.txt"), Path.Combine(save_path_main, save_name + "_数据报表.csv"));
                        File.Delete(Path.Combine(save_path_main, save_name + "_数据报表.txt"));

                        invalidate_message_textbox();
                        close_micphone();
                        close_all_camera();
                        MessageBox.Show("录制信息保存成功！");
                        is_saving_flag = false;
                        camera_main_record_flag = false;
                        StartButton.Enabled = true;
                    }
                    break;
                #endregion
                case ready_id:
                    ready_flag = true;
                    StartButton.Enabled = false;
                    break;
                case ask_id:
                    StartButton.Enabled = false;
                    background_worker.RunWorkerAsync(answer_id);
                    break;
                case answer_id:

                    break;
                default:
                    break;
                
            }

        }
        private void Background_worker_DoWork(object sender, DoWorkEventArgs e)
        {

            switch ((int)e.Argument)
            {
                case camera_detetcor_id:
                    set_detector(camera_detetcor_id);
                    person.Clear();
                    break;
                case close_main_camera_id:
                    invalidate_detetcor();
                    break;
                case manu_save_id:
                    save_iFlytek();
                    save_datareport();
                    close_record_filemaker();
                    person.Clear();
                    break;
                case auto_save_id:
                    save_iFlytek();
                    save_datareport();
                    close_record_filemaker();
                    person.Clear();
                    continue_recording();
                    break;
                case open_iFlytek_id:
                    count = 0;
                    depression = 0;
                    anxiety = 0; 
                    lie = 0; 
                    aggression = 0;
                    suicide = 0;
                    last = false;
                    open_iFlytek();
                    break;
                case ready_id:
                    ReadQuestion(questionnairelist[talkmode]);
                    qindex = 0;
                    qscore = 0;
                    findex = 0;
                    fscore = 0;
                    speechSyn.SpeakAsync("接下来开始问卷谈话，请根据实际情况如实作答。");
                    break;
                case ask_id:
                    NextButton.Enabled = false;
                    speechSyn.SpeakAsync(word2);
                    break;
                case answer_id:
                    
                    break;
                default:
                    break;
            }
            e.Result = e.Argument;
        }
        #endregion
        #region 录制视频
        private void ProcessFrame_0(object sender, EventArgs arg)
        {
            if (!camera_0_fresh_flag)
            {
                return;
            }
            if (capture_0 != null && capture_0.Ptr != IntPtr.Zero)
            {
                capture_0.Retrieve(frame_0, 0);
                if (!main_camera_fresh_flag)
                {
                    this.Invalidate();
                }
            }
        }
        private void open_camera_0()
        {
            try
            {
                capture_0 = new Capture(0);
                capture_0.SetCaptureProperty(CapProp.FrameHeight, 480);//设置捕获摄像头图像的高
                capture_0.SetCaptureProperty(CapProp.FrameWidth, 640);//设置捕获摄像头图像的宽

                capture_0.ImageGrabbed += ProcessFrame_0;

            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }

            capture_0.Start();
        }//打开摄像头
        private void timer1_Tick(object sender, EventArgs e)
        {
            save_counter += 1;
            is_saving_flag = true;
            this.recording_audio_flag = false;
            this.recording_camera_flag = false;
            background_worker.RunWorkerAsync(auto_save_id);
        }
        private void update_record_interval()
        {
            var now_time = System.DateTime.Now;
            var timeInterval = now_time - recordStart;
            var hour = timeInterval.ToString("hh");
            var minute = timeInterval.ToString("mm");
            var second = timeInterval.ToString("ss");
            textbox_recording_interval.Text = hour + ":" + minute + ":" + second;
        }//更新录制时间
        private void ImageCaptured()
        {
            if (bitmapImage.Width == record_width && bitmapImage.Height == record_height)
            {
                this.videoFileMaker_main.AddVideoFrame(bitmapImage);
            }
        }//将获得图像保存到videoFileMaker中
        private void desktop_ImageCaptured(Bitmap img)
        {
            Bitmap imgRecorded = img;
            if (this.sizeRevised) // 对图像进行裁剪，  MFile要求录制的视频帧的长和宽必须是4的整数倍。
            {
                imgRecorded = ESBasic.Helpers.ImageHelper.RoundSizeByNumber(img, 4);
                img.Dispose();
            }
            this.silenceVideoFileMaker.AddVideoFrame(imgRecorded);
        }
        private void desktop_capture()
        {
            this.desktopCapturer = CapturerFactory.CreateDesktopCapturer(record_desktop_fps, false);
            this.desktopCapturer.ImageCaptured += this.desktop_ImageCaptured;
            videoSized = this.desktopCapturer.VideoSize;
            this.desktopCapturer.Start();//开始采集桌面
        }
        public void start_recording()
        {
            save_name = person.name + "_" + person.gender + "_" + person.age_range + "_";
            save_dir = save_root + "\\" + save_name + DateTime.Now.ToString("yyyyMMdd_HHmmss");//保存文件夹（默认save_root为E:\\data）
            save_path_desktop = save_dir + "\\" + "录制屏幕";//录屏保存文件夹
            if (!Directory.Exists(save_path_desktop))
            {
                Directory.CreateDirectory(save_path_desktop);
            }
            temp_video_path_desktop = Path.Combine(save_path_desktop, save_name + "_临时录屏文件_" + save_counter.ToString() + ".mp4");//录屏文件夹下的文件命名
            Init_record_video(desktop);//设置录制组件
            if (main_camera_fresh_flag)
            {
                save_path_main = save_dir + "\\" + "讯(询)问摄像头_" + main_camera_id.ToString() + "号摄像头";
                if (!Directory.Exists(save_path_main))
                {
                    Directory.CreateDirectory(save_path_main);
                }
                temp_video_path_main = Path.Combine(save_path_main, save_name + "_临时视频文件_" + save_counter.ToString() + ".mp4");
                iFlytek_save_path_main = Path.Combine(save_path_main, save_name + "_临时历史笔录_" + save_counter.ToString() + ".txt");
                datareport_save_path_main = Path.Combine(save_path_main, save_name + "_临时数据报表_" + save_counter.ToString() + ".txt");
                result_save_path_main = Path.Combine(save_dir, "result.txt");
                temp_audio_path_main = Path.Combine(save_path_main, save_name + "_临时音频文件_" + save_counter.ToString() + ".mp3");
                Init_record_audio();
                recording_audio_flag = true;
                Init_record_video(main_camera_id);
                camera_main_record_flag = true;
            }
            this.textbox_recording_interval.Text = null;
            this.textbox_recording_interval.Visible = true;
            menuStrip1.Refresh();
            textbox_recording_interval.Refresh();
            this.recordStart = System.DateTime.Now;  // 用于记录开始录制时的时间
            this.timer1.Enabled = true;              // 启动timer1
            person.Clear();                          // 录制开始时，重置person
            recording_camera_flag = true;
        }//开始录制时调用
        public void continue_recording()
        {
            if (!Directory.Exists(save_path_desktop))
            {
                Directory.CreateDirectory(save_path_desktop);
            }
            temp_video_path_desktop = Path.Combine(save_path_desktop, save_name + "_临时录屏文件_" + save_counter.ToString() + ".mp4");
            Init_record_video(desktop);//设置录制组件
            if (main_camera_fresh_flag)
            {
                if (!Directory.Exists(save_path_main))
                {
                    Directory.CreateDirectory(save_path_main);
                }
                temp_video_path_main = Path.Combine(save_path_main, save_name + "_临时视频文件_" + save_counter.ToString() + ".mp4");
                iFlytek_save_path_main = Path.Combine(save_path_main, save_name + "_临时历史笔录_" + save_counter.ToString() + ".txt");
                datareport_save_path_main = Path.Combine(save_path_main, save_name + "_临时数据报表_" + save_counter.ToString() + ".txt");
                temp_audio_path_main = Path.Combine(save_path_main, save_name + "_临时音频文件_" + save_counter.ToString() + ".mp3");
                Init_record_audio();
                recording_audio_flag = true;
                Init_record_video(main_camera_id);
                camera_main_record_flag = true;
            }
            recording_camera_flag = true;
        }//自动保存后调用
        #endregion
        #region 录制音频
        private void audioMixter_AudioMixed(byte[] audioData)
        {
            if (recording_audio_flag)
            {
                audioFileMaker_main.AddAudioFrame(audioData);
            }
            if (recording_camera_flag)
            {
                if (camera_main_record_flag)
                {
                    videoFileMaker_main.AddAudioFrame(audioData);
                }
            }
            audio_Enqueue(audioData);
        }//将获得的语音保存到videoFileMaker,audioFileMaker,audioData_Queue中
        private void open_micphone()
        {
            if (!micphone_enable_flag)
            {
                this.microphoneCapturer = CapturerFactory.CreateMicrophoneCapturer(main_micphone_id);
                this.microphoneCapturer.AudioCaptured += audioMixter_AudioMixed;
                this.microphoneCapturer.CaptureError += new CbGeneric<Exception>(this.CaptureError);
                this.microphoneCapturer.Start();
                wavFormat = new DataChunkFormat();
                wavFormat.EncodingFormat = EncodingFormat.Pcm;
                wavFormat.ChannelNum = 1;
                wavFormat.BitsPerSample = 16;
                wavFormat.BytesPerSample = 2;
                wavFormat.SamplesPerSecond = 16000;
                wavFormat.BytesPerSecond = 32000;
                voiceDetector = new VoiceDetector(workDir, wavFormat);
                thVAD = new Thread(new ThreadStart(VAD));
                thVAD.Start();
                micphone_enable_flag = true;
            }
        }//打开选定的询问麦克风
        private void audio_Enqueue(byte[] audioData)
        {
            short[] audio_short = new short[320];

            int c = 0;
            for (int i = 0; i < audioData.Length; i += 2)
            {
                short_audioData = BitConverter.ToInt16(audioData, i);//转为short类型
                audio_short[c] = short_audioData;
                if (++c == 320)
                {
                    lock (audioData1)
                    {
                        Monitor.Pulse(audioData1);
                    }
                    audioData1.Enqueue(audio_short);
                    c = 0;
                }
                float float_audioData = short_audioData / 32768.0f;
                audiodata_Queue.Enqueue(float_audioData);
            }

            while (audiodata_Queue.Count > audioData_queueCapacity)
            {
                audiodata_Queue.Dequeue();
            }
        }//语音数据归一化，入队
        #endregion
        #region 显示结果
        private void showResults(Graphics g, Dictionary<int, Affdex.Face> faces)
        {
            this.faceRectangle = new Rectangle(0, 0, 0, 0);
            this.foreheadRectangle = new Rectangle(0, 0, 0, 0);
            int number = 0;
            Init_EmotionArray();

            string feature_points_string = null;
            string feature_point_x = null;
            string feature_point_y = null;

            foreach (KeyValuePair<int, Affdex.Face> pair in faces)
            {
                Affdex.Face face = pair.Value;
                Affdex.FeaturePoint tl = minPoint(face.FeaturePoints);
                Affdex.FeaturePoint br = maxPoint(face.FeaturePoints);

                float mean_x = 0;
                float mean_y = 0;
                this.mouthPoints.Clear();

                foreach (Affdex.FeaturePoint fp in face.FeaturePoints)  ////遍历特征点
                {
                    number++;
                    if (number > 20 && number < 31)
                    {
                        this.mouthPoints.Add(new PointF(fp.X, fp.Y));
                        mean_x += fp.X;
                        mean_y += fp.Y;
                    }
                    feature_point_x = ((int)fp.X).ToString();
                    feature_point_y = ((int)fp.Y).ToString();

                    feature_points_string = feature_points_string + feature_point_x + "_" + feature_point_y + ";";
                }

                mean_x /= 10.0f;
                mean_y /= 10.0f;
                double variance = 0;
                foreach (var point in mouthPoints)
                {
                    variance += Math.Pow((double)(point.X - mean_x), 2) + Math.Pow((double)(point.Y - mean_y), 2);
                }
                if (mouth_variance != -1000)
                {
                    if (Math.Abs(mouth_variance - variance) > threshold_mouth)
                    {
                        number_tupo++;
                    }
                }
                this.mouth_variance = variance;
                this.faceRectangle = new Rectangle((Int32)tl.X, (Int32)tl.Y, (Int32)(br.X - tl.X), (Int32)(br.Y - tl.Y));
                this.foreheadRectangle = getforeheadRectangle(this.faceRectangle, 0.5, 0.2, 0.5, 0.2);
                CvInvoke.Rectangle(matImage, faceRectangle, new MCvScalar(0, 255, 0), 2);
                CvInvoke.Rectangle(matImage, foreheadRectangle, new MCvScalar(0, 69, 255), 2);

                #region  情感信息计算和显示
                foreach (PropertyInfo prop in typeof(Affdex.Emotions).GetProperties())
                {
                    double value_emotion = (float)prop.GetValue(face.Emotions, null);
                    switch (prop.Name)
                    {
                        case "Anger":
                            {
                                emotionDic["生气"] = Math.Round(value_emotion, 3);
                            }
                            break;
                        case "Disgust":
                            {
                                emotionDic["厌恶"] = Math.Round(value_emotion, 3);
                            }
                            break;
                        case "Fear":
                            {
                                emotionDic["恐惧"] = Math.Round(value_emotion, 3) / 5;
                            }
                            break;
                        case "Joy":
                            {
                                emotionDic["喜悦"] = Math.Round(value_emotion, 3);
                            }
                            break;
                        case "Sadness":
                            {
                                emotionDic["悲伤"] = Math.Round(value_emotion, 3);
                            }
                            break;

                        case "Surprise":
                            {
                                emotionDic["吃惊"] = Math.Round(value_emotion, 3);
                            }
                            break;
                        default:
                            break;
                    }

                    var dicSorted = from objDic in emotionDic orderby objDic.Value select objDic;
                    var emotiondic_key = dicSorted.Last().Key;
                    var emotiondic_value = dicSorted.Last().Value;
                    person.emotion_key = emotiondic_key;
                    person.emotion_value = emotiondic_value;

                    switch (emotiondic_key)
                    {
                        case "生气":
                            {
                                emotionArray[0] = emotiondic_value;
                            }
                            break;
                        case "厌恶":
                            {
                                emotionArray[1] = emotiondic_value;
                            }
                            break;
                        case "恐惧":
                            {
                                emotionArray[2] = emotiondic_value;
                            }
                            break;
                        case "喜悦":
                            {
                                emotionArray[3] = emotiondic_value;
                            }
                            break;
                        case "悲伤":
                            {
                                emotionArray[4] = emotiondic_value;
                            }
                            break;

                        case "吃惊":
                            {
                                emotionArray[5] = emotiondic_value;
                            }
                            break;
                        default:
                            {
                                emotionArray[6] = 100;
                            }
                            break;
                    }
                    depression_value = (face.Expressions.ChinRaise + face.Expressions.Dimpler + face.Expressions.InnerBrowRaise + face.Expressions.LipCornerDepressor) / 4;
                    anxiety_value = (face.Expressions.BrowFurrow + face.Expressions.InnerBrowRaise + face.Expressions.LipPress) / 3;
                    attack_value = (face.Expressions.LidTighten + face.Expressions.NoseWrinkle + face.Expressions.UpperLipRaise) / 3;
                    attention_value = face.Expressions.Attention;
                    tension_value = (face.Expressions.BrowFurrow + face.Expressions.Dimpler + face.Expressions.LipPress) / 3;
                    hesitate_value = face.Expressions.LipSuck;
                }
                #endregion

            }
            if (open_iFlytek_flag == true)
            {
                progressbar_谎言.Value = lie_probability;
            }

            if (faceRectangle.Height > 0)
            {
                textbox_name.Text = person.name;
                textbox_age_range.Text = person.age_range;
                if (camera_main_record_flag)
                {
                    person.precessed_frame_id.Add(person.frame_number);
                    person.emotionKeys.Add(person.emotion_key);
                    person.facefeaturePoints.Add(feature_points_string);
                    if (person.emotion_key == "平静")
                    {
                        person.emotionValues.Add(100);
                    }
                    else
                    {
                        person.emotionValues.Add(person.emotion_value);
                    }

                }
                #region  审问信息显示
                Random ra0 = new Random(0);
                Random ra1 = new Random(1);
                Random ra2 = new Random(2);
                Random ra3 = new Random(3);
                Random ra4 = new Random(4);
                if (current_frame_id % 27 == 0)
                {
                    this.progressbar_抑郁.Value = (int)(emotionArray[2] * 0.2 + emotionArray[4] * 0.2 + depression_value * 0.8) + ra0.Next(0, 10);
                }
                if (current_frame_id % 27 == 0)
                {
                    this.progressbar_焦虑.Value = (int)(anxiety_value) + ra2.Next(0, 10);
                }
                if (current_frame_id % 27 == 0)
                {
                    this.progressbar_攻击.Value = (int)(emotionArray[0] * 0.4 + emotionArray[1] * 0.4 + attack_value * 0.6);
                }
                if (current_frame_id % 27 == 0)
                {
                    this.progressbar_关注.Value = (int)(attention_value * 10) - ra4.Next(900, 920);
                }
                if (current_frame_id % 27 == 0)
                {
                    this.progressbar_压力.Value = (int)(tension_value) + ra1.Next(0, 20);
                }
                if (current_frame_id % 27 == 0)
                {
                    this.progressbar_兴奋.Value = (int)(emotionArray[3] * 0.6 + emotionArray[5] * 0.4);
                }
                if (current_frame_id % 27 == 0)
                {
                    this.progressbar_犹豫.Value = (int)(hesitate_value) > 0 ? (int)(hesitate_value * 0.6) + ra3.Next(0, 20) : ra3.Next(0, 10);
                    count += 1;
                    depression = (depression * (count - 1) + this.progressbar_抑郁.Value) / count;
                    anxiety = (anxiety * (count - 1) + this.progressbar_焦虑.Value) / count;
                    aggression = (aggression * (count - 1) + this.progressbar_攻击.Value) / count;
                    lie = (lie * (count - 1) + this.progressbar_谎言.Value) / count;
                    suicide = depression * 0.4 + anxiety * 0.4 + aggression * 0.2;
                }

                #endregion
            }
            plot_audio_wave();
            PaintBarSpectrum(pictureBox2);//画语音波形图（目前不用）
        }//获取detector检测的结果，并显示界面上的各种信息
        private Affdex.FeaturePoint minPoint(Affdex.FeaturePoint[] points)
        {
            Affdex.FeaturePoint ret = points[0];
            foreach (Affdex.FeaturePoint point in points)
            {
                if (point.X < ret.X) ret.X = point.X;
                if (point.Y < ret.Y) ret.Y = point.Y;
            }
            return ret;
        }//获取人脸特征点最小的x,y
        private Affdex.FeaturePoint maxPoint(Affdex.FeaturePoint[] points)
        {
            Affdex.FeaturePoint ret = points[0];
            foreach (Affdex.FeaturePoint point in points)
            {
                if (point.X > ret.X) ret.X = point.X;
                if (point.Y > ret.Y) ret.Y = point.Y;
            }
            return ret;
        }//获取人脸特征点最大的x,y
        private Rectangle getforeheadRectangle(Rectangle face, double fh_x, double fh_y, double fh_w, double fh_h)
        {
            int x = face.Left;
            int y = face.Top;
            int w = face.Width;
            int h = face.Height;

            Rectangle forehead = new Rectangle((int)(x + w * fh_x - (w * fh_w / 2.0)),
                                          (int)(y - h * fh_y),
                                          (int)(w * fh_w),
                                          (int)(h * fh_h));

            if (forehead.X < 0 || forehead.Y < 0)
            {
                forehead = new Rectangle(0, 0, 0, 0);
            }
            if (forehead.X > matImage.Width || forehead.Y > matImage.Height)
            {
                forehead = new Rectangle(0, 0, 0, 0);
            }
            if (forehead.Height < 0 || forehead.Width < 0)
            {
                forehead = new Rectangle(0, 0, 0, 0);
            }
            if (forehead.Height > matImage.Width || forehead.Width > matImage.Height)
            {
                forehead = new Rectangle(0, 0, 0, 0);
            }
            return forehead;
        }//获取前额矩形框
        private void plot_audio_wave()
        {
            float[] tempdata;
            lock (_syncRoot)
            {
                tempdata = new float[audiodata_Queue.Count + 1];
                audiodata_Queue.CopyTo(tempdata, 0);
            }

            float div = (float)tempdata.Length / (float)(pictureBox1.Width - 10);
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            for (int i = 0; i < tempdata.Length - 1; i = i + 1)
            {
                g.DrawLine(new Pen(Color.YellowGreen, 1), new PointF((i / div + 5), (float)(pictureBox1.Height / 2 - tempdata[i] * pictureBox1.Height / 2)), new PointF(((i + 1) / div + 5), (float)(pictureBox1.Height / 2 - tempdata[i + 1] * pictureBox1.Height / 2)));

            }
            this.pictureBox1.Image = bitmap;
        }//绘制语音波形图（没用到）
        private void PaintBarSpectrum(PictureBox picturebox)
        {
            float[] tempdata;
            lock (_syncRoot)
            {
                tempdata = new float[audiodata_Queue.Count];
                audiodata_Queue.CopyTo(tempdata, 0);
            }

            CFFTss f = new CFFTss();
            f.dData = new Complex[tempdata.Length];
            for (int j = 0; j < f.dData.Length; j++)
            {
                f.dData[j] = new Complex(tempdata[j], 0);
            }
            f.fft();

            double[] magnitude = new double[f.dData.Length / 2];
            for (int j = 0; j < magnitude.Length; j++)
            {
                magnitude[j] = f.dData[j].Magnitude;
            }

            Bitmap bitmap = new Bitmap(picturebox.Width, picturebox.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            (new BarSpectrum()).Paint(g, bitmap.Size, magnitude);
            picturebox.Image = bitmap;
        }//绘制语音柱状图（没用到）
        #endregion
        #region Affdex函数
        private void set_detector(int new_detectorID, string videofilepath = null)
        {
            if (!detector_enable_flag)
            {
                this.detector = null;
            }
            else
            {
                if (new_detectorID == camera_detetcor_id && detector_id == camera_detetcor_id)
                {
                    return;
                }
                else
                {
                    this.detector_id = init_detector_id;
                    this.detector_enable_flag = false;
                    this.detector.stop();
                    this.detector.Dispose();
                }
            }
            this.detector = new Affdex.CameraDetector(main_camera_id, capture_fps, process_fps, 1, face_size);
            if (this.detector != null)
            {
                String classifierpath = "data";
                this.detector.setClassifierPath(classifierpath);
                this.detector.setDetectAllEmotions(true);
                this.detector.setDetectAllExpressions(true);
                this.detector.setDetectAllEmojis(true);
                this.detector.setDetectAllAppearances(true);
                this.detector.start();//初始化一个detector,用来捕获视频帧，然后处理他们。
                this.detector.setImageListener(this);
                if (this.faces != null)
                {
                    this.faces.Clear();
                }
                this.detector_enable_flag = true;
                this.detector_id = new_detectorID;
                this.number_tupo = 1;
            }
        }//设置detetcor
        public void onImageCapture(Affdex.Frame frame)
        {
            if (recording_camera_flag && camera_main_record_flag)
            {
                frame2Bitmap(frame);
                ImageCaptured();
                person.frame_number++;
            }
            frame.Dispose();
        }//回调函数，每当detetcor获取一帧图像，触发该方法（保存）
        public void onImageResults(Dictionary<int, Affdex.Face> faces, Affdex.Frame frame)
        {
            this.faces = faces;
            if (main_camera_fresh_flag && detector_id == camera_detetcor_id)
            {
                frame2Mat(frame);
                this.Invalidate();
            }
            frame.Dispose();
            current_frame_id++;
        }//回调函数，每当detetcor处理完一帧图像，触发该方法（处理）
        #endregion
        #region 告警信息
        private void timer2_Tick(object sender, EventArgs e)
        {
            warningmessage_flag = !warningmessage_flag;
        }
        private ArrayList getIndexArray(string inputStr, string findStr)
        {
            ArrayList list = new ArrayList();
            int start = 0;
            while (start < inputStr.Length)
            {
                int index = inputStr.IndexOf(findStr, start);
                if (index >= 0)
                {
                    list.Add(index);
                    start = index + findStr.Length;
                }
                else
                {
                    break;
                }
            }
            return list;
        }
        private void changeColorRed(string str)//"str"为需要寻找的字符串
        {
            ArrayList list = getIndexArray(textBox_warning.Text, str);
            for (int i = 0; i < list.Count; i++)
            {
                int index = (int)list[i];
                textBox_warning.Select(index, str.Length);
                textBox_warning.SelectionColor = Color.Red;
            }
        }
        private void changeColorYellow(string str)//"str"为需要寻找的字符串
        {
            ArrayList list = getIndexArray(textBox_warning.Text, str);
            for (int i = 0; i < list.Count; i++)
            {
                int index = (int)list[i];
                textBox_warning.Select(index, str.Length);
                textBox_warning.SelectionColor = Color.Yellow;
            }
        }
        private void changeColorGreen(string str)//"str"为需要寻找的字符串
        {
            ArrayList list = getIndexArray(textBox_warning.Text, str);
            for (int i = 0; i < list.Count; i++)
            {
                int index = (int)list[i];
                textBox_warning.Select(index, str.Length);
                textBox_warning.SelectionColor = Color.Lime;
            }
        }
        private void warningmessage()
        {
            StringBuilder SB = new StringBuilder();//表示显示在richBox中的可变字符串（局部变量）
            string nowtime = DateTime.Now.ToLocalTime().ToString();
            string tupomessage = "    " + nowtime + "\r\n";
            SB.Append(tupomessage);
            tupomessage = "    " + "谎言概率   " + progressbar_谎言.Value + "%" + "\r\n";
            SB.Append(tupomessage);
            tupomessage = "    " + "抑郁程度   " + progressbar_抑郁.Value + "%" + "\r\n";
            SB.Append(tupomessage);
            tupomessage = "    " + "焦虑程度   " + progressbar_焦虑.Value + "%" + "\r\n";
            SB.Append(tupomessage);
            tupomessage = "    " + "攻击性     " + progressbar_攻击.Value + "%" + "\r\n";
            SB.Append(tupomessage);
            tupomessage = "    " + "关注度     " + progressbar_关注.Value + "%" + "\r\n";
            SB.Append(tupomessage);
            tupomessage = "    " + "压力值     " + progressbar_压力.Value + "%" + "\r\n";
            SB.Append(tupomessage);
            tupomessage = "    " + "兴奋度     " + progressbar_兴奋.Value + "%" + "\r\n";
            SB.Append(tupomessage);
            tupomessage = "    " + "犹豫度     " + progressbar_犹豫.Value + "%" + "\r\n";
            SB.Append(tupomessage);

            textBox_warning.Text = SB.ToString();//显示在textGBox_warning中

            if (progressbar_谎言.Value <= 70)
            {
                if (progressbar_谎言.Value <= 30)
                {
                    changeColorGreen("谎言概率   " + progressbar_谎言.Value + "%");
                }
                else
                {
                    changeColorYellow("谎言概率   " + progressbar_谎言.Value + "%");
                }
            }
            else
            {
                changeColorRed("谎言概率   " + progressbar_谎言.Value + "%");
            }
            if (progressbar_抑郁.Value <= 70)
            {
                if (progressbar_抑郁.Value <= 30)
                {
                    changeColorGreen("抑郁程度   " + progressbar_抑郁.Value + "%");
                }
                else
                {
                    changeColorYellow("抑郁程度   " + progressbar_抑郁.Value + "%");
                }
            }
            else
            {
                changeColorRed("抑郁程度   " + progressbar_抑郁.Value + "%");
            }
            if (progressbar_焦虑.Value <= 70)
            {
                if (progressbar_焦虑.Value <= 30)
                {
                    changeColorGreen("焦虑程度   " + progressbar_焦虑.Value + "%");
                }
                else
                {
                    changeColorYellow("焦虑程度   " + progressbar_焦虑.Value + "%");
                }
            }
            else
            {
                changeColorRed("焦虑程度   " + progressbar_焦虑.Value + "%");
            }
            if (progressbar_攻击.Value <= 70)
            {
                if (progressbar_攻击.Value <= 30)
                {
                    changeColorGreen("攻击性     " + progressbar_攻击.Value + "%");
                }
                else
                {
                    changeColorYellow("攻击性     " + progressbar_攻击.Value + "%");
                }
            }
            else
            {
                changeColorRed("攻击性     " + progressbar_攻击.Value + "%");
            }
            if (progressbar_关注.Value <= 70)
            {
                if (progressbar_关注.Value <= 30)
                {
                    changeColorGreen("关注度     " + progressbar_关注.Value + "%");
                }
                else
                {
                    changeColorYellow("关注度     " + progressbar_关注.Value + "%");
                }
            }
            else
            {
                changeColorRed("关注度     " + progressbar_关注.Value + "%");
            }
            if (progressbar_压力.Value <= 70)
            {
                if (progressbar_压力.Value <= 30)
                {
                    changeColorGreen("压力值     " + progressbar_压力.Value + "%");
                }
                else
                {
                    changeColorYellow("压力值     " + progressbar_压力.Value + "%");
                }
            }
            else
            {
                changeColorRed("压力值     " + progressbar_压力.Value + "%");
            }
            if (progressbar_兴奋.Value <= 70)
            {
                if (progressbar_兴奋.Value <= 30)
                {
                    changeColorGreen("兴奋度     " + progressbar_兴奋.Value + "%");
                }
                else
                {
                    changeColorYellow("兴奋度     " + progressbar_兴奋.Value + "%");
                }
            }
            else
            {
                changeColorRed("兴奋度     " + progressbar_兴奋.Value + "%");
            }
            if (progressbar_犹豫.Value <= 70)
            {
                if (progressbar_犹豫.Value <= 30)
                {
                    changeColorGreen("犹豫度     " + progressbar_犹豫.Value + "%");
                }
                else
                {
                    changeColorYellow("犹豫度     " + progressbar_犹豫.Value + "%");
                }
            }
            else
            {
                changeColorRed("犹豫度     " + progressbar_犹豫.Value + "%");
            }
        }
        #endregion
        #region 语音识别
        private void open_iFlytek()
        {
            open_iFlytek_flag = true;
            axSXClientActiveXControl1.OnCEFLoadEnd += ctrl_OnCEFLoadEnd;//OnCEFLoadEnd事件：插件完成初始化触发
            axSXClientActiveXControl1.StartRecord();
            //axSXClientActiveXControl1.StartUdp("121.248.54.21", "54321");
        }
        private void axSXClientActiveXControl1_OnOutPutSentence(object sender, _DSXClientActiveXControlEvents_OnOutPutSentenceEvent e)
        {
            string nowtime = DateTime.Now.ToLocalTime().ToString();
            switch (talkmode)
            {
                case 0:
                    #region 使用两路麦克风
                    if (e.id == "0")
                    {
                        lie_probability = 0;
                        word2 = e.content;
                        string wen = nowtime + "\t" + "问话人：" + word2 + "\r\n";
                        SB1.Append(wen);
                        for (int i = 0; i < allkeywords.Count; i++)
                        {
                            if (word2.Contains(allkeywords[i]))
                            {
                                word2 = word2 + "（违规语句！）";
                            }
                        }
                        string str = @"
                                    <script type=""text/javascript"">window.location.hash = ""#ok"";
                                    </script>
 
                                    <div class=""chat_content_group buddy"">


                                            <p class=""chat_nick"">问话人</p>
                                            <p class=""chat_content"">" + word2 + @"</p>
                                    </div>


                                    <a id='ok'></a>

                                    ";
                        webKitBrowser1.DocumentText = webKitBrowser1.DocumentText.Replace("<a id='ok'></a>", "") + str;//左侧
                    }
                    if (e.id == "1")
                    {
                        word1 = e.content;
                        string da = nowtime + "\t" + person.name + "：" + word1 + "\r\n";
                        SB1.Append(da);
                        //SB3.Append(DateTime.Now.ToLocalTime() + "," + person.name + "," + word1 + "," + progressbar_谎言.Value + "," + progressbar_抑郁.Value + "," + progressbar_焦虑.Value + "," + progressbar_攻击.Value + "," + progressbar_关注.Value + "," + progressbar_压力.Value + "," + progressbar_兴奋.Value + "," + progressbar_犹豫.Value + "\r\n");
                        string str = @"
                                    <script type=""text/javascript"">window.location.hash = ""#ok"";
                                    </script>
    
                                    <div class=""chat_content_group self"">
                                            <p class=""chat_nick"">答话人</p>       
                                            <p class=""chat_content"">" + word1 + @"</p>
                                    </div>


                                    <a id='ok'></a>

                                    ";
                        webKitBrowser1.DocumentText = webKitBrowser1.DocumentText.Replace("<a id='ok'></a>", "") + str;//右侧
                    }
                    #endregion
                    break;
                default:
                    #region 使用一路麦克风
                    if (e.id == "1")
                    {
                        word1 = e.content;
                        string da = nowtime + "\t" + person.name + "：" + word1 + "\r\n";
                        SB1.Append(da);
                        string str = @"
                                    <script type=""text/javascript"">window.location.hash = ""#ok"";
                                    </script>
    
                                    <div class=""chat_content_group self"">
                                            <p class=""chat_nick"">答话人</p>       
                                            <p class=""chat_content"">" + word1 + @"</p>
                                    </div>


                                    <a id='ok'></a>

                                    ";
                        webKitBrowser1.DocumentText = webKitBrowser1.DocumentText.Replace("<a id='ok'></a>", "") + str;//右侧
                    }
                    if (e.id == "0")
                    {

                    }
                    #endregion
                    break;
            }
            
        }
        void ctrl_OnCEFLoadEnd(object sender, EventArgs e)
        {
            axSXClientActiveXControl1.StartRecord();

        }

        private void showRecResults()
        {
            string nowtime = DateTime.Now.ToLocalTime().ToString();
            SB2.Append(nowtime + ",");
            SB2.Append(person.name + "," + person.age_range + "," + person.gender + ",");

            aggression = Math.Round(aggression, 2);
            depression = Math.Round(depression, 2);
            anxiety = Math.Round(anxiety, 2);
            lie = Math.Round(lie, 2);
            suicide = Math.Round(suicide, 2);
            
            string colorag;
            string colorde;
            string coloran;
            string colorlie;
            string colorsu;
            string coloras;
            string assess;
            int n0 = 0;
            int n1 = 0;
            int n2 = 0;
            int n3 = 0;

            if (aggression < 35) { colorag = color0; n0 += 1; }
            else if (aggression < 60) { colorag = color1; n1 += 1; }
            else if (aggression < 85) { colorag = color2; n2 += 1; }
            else { colorag = color3; n3 += 1; }

            if (depression < 35) { colorde = color0; n0 += 1; }
            else if (depression < 60) { colorde = color1; n1 += 1; }
            else if (depression < 85) { colorde = color2; n2 += 1; }
            else { colorde = color3; n3 += 1; }

            if (anxiety < 35) { coloran = color0; n0 += 1; }
            else if (anxiety < 60) { coloran = color1; n1 += 1; }
            else if (anxiety < 85) { coloran = color2; n2 += 1; }
            else { coloran = color3; n3 += 1; }

            if (lie < 35) { colorlie = color0; n0 += 1; }
            else if (lie < 60) { colorlie = color1; n1 += 1; }
            else if (lie < 85) { colorlie = color2; n2 += 1; }
            else { colorlie = color3; n3 += 1; }

            if (suicide < 35) { colorsu = color0; n0 += 1; }
            else if (suicide < 60) { colorsu = color1; n1 += 1; }
            else if (suicide < 85) { colorsu = color2; n2 += 1; }
            else { colorsu = color3; n3 += 1; }

            switch (talkmode)
            {
                case 0:
                    #region mode0Html
                    string mode0Html = @"
<html><head>
<script type=""text/javascript"">window.location.hash = ""#ok"";</script>
<style type=""text/css"">
html, body {
	height: 346px;
	display: flex;
    flex-direction: column;
	align-items: center;
	justify-content: center;
    font-family:黑体;
    font-size:18px;
	background: rgb(11,12,39);
    overflow-x:hidden; 
    overflow-y:hidden; 
}
.bkg {
  background: rgb(11,12,39);
  height: 346px;
  width: 470px;
}
.bkg:after {
    border: 0;
}
.skill {
    height: 56px;
    margin-top: 10px;
    color: #fff;
}
.skill:hover {
  background: rgba(0,140,215,0.1);
}
.skill .title {
  display: flex;
  flex-direction: row;
}
.skill span {
    width: 370px;
    text-align: left;
}
.skill small{
    width: 100px;
    text-align: right;
    font-weight: bold;
}
.skill .bar{
    position: relative;
    z-index: 1;
    background: #ddd;
    padding: 0;
    margin-top: 14px;
    height: 9px;
    border-radius: 10px;
}
.skill .back{
    position: relative;
    z-index: 0;
    bottom: 9px;
    background: rgba(0, 0, 0, 0.34);
    height: 10px;
    border-radius: 10px;
    width: 100%;
}
</style>
</head><body>
<div class=""bkg"">
<div class=""skill"">
    <div class=""title"">
        <span>攻击性</span>
        <small class=""level"" style=""color: " + colorag + @""">" + aggression + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + aggression + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>抑郁程度</span>
        <small class=""level"" style=""color: " + colorde + @""">" + depression + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + depression + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>焦虑程度</span>
        <small class=""level"" style=""color: " + coloran + @""">" + anxiety + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + anxiety + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>谎言概率</span>
        <small class=""level"" style=""color: " + colorlie + @""">" + lie + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + lie + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>自杀概率</span>
        <small class=""level"" style=""color: " + colorsu + @""">" + suicide + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + suicide + @"%""></div>
    <div class=""back""></div>
</div>
</div>
</body>";
                    #endregion
                    webKitMode.DocumentText = mode0Html;
                    webKitMode.BringToFront();
                    SB2.Append("默认谈话,");
                    SB2.Append(" " + "," + " " + ",");
                    SB2.Append(aggression + "%," + depression + "%," + anxiety + "%," + lie + "%," + suicide + "%,");
                    break;
                case 1:
                    switch(n0)
                    {
                        case 5:
                        case 4:
                            assess = "低危险性";
                            break;
                        case 3:
                        case 2:
                            if (n3 > 2) assess = "极高危险";
                            else if (n2 > 2) assess = "高度危险";
                            else assess = "中度危险";
                            break;
                        case 1:
                        case 0:
                            if(n3>1) assess = "极高危险";
                            else assess = "高度危险";
                            break;
                        default:
                            assess = "中度危险";
                            break;
                    }
                    switch(assess)
                    {
                        case "低危险性":
                            coloras = color0;
                            break;
                        case "中度危险":
                            coloras = color1;
                            break;
                        case "高度危险":
                            coloras = color2;
                            break;
                        case "极高危险":
                            coloras = color3;
                            break;
                        default:
                            coloras = color3;
                            break;
                    }
                    #region barHtml
                    string barHtml = @"
<html><head>
<script type=""text/javascript"">window.location.hash = ""#ok"";</script>
<style type=""text/css"">
html, body {
	height: 346px;
	display: flex;
    flex-direction: column;
	align-items: center;
	justify-content: center;
    font-family:黑体;
    font-size:18px;
	background: rgb(11,12,39);
    overflow-x:hidden; 
    overflow-y:hidden; 
}
.bkg {
  background: rgb(11,12,39);
  height: 346px;
  width: 235px;
}
.bkg:after {
    border: 0;
}
.skill {
    height: 56px;
    margin-top: 10px;
    color: #fff;
}
.skill:hover {
  background: rgba(0,140,215,0.1);
}
.skill .title {
  display: flex;
  flex-direction: row;
}
.skill span {
    width: 370px;
    text-align: left;
}
.skill small{
    width: 100px;
    text-align: right;
    font-weight: bold;
}
.skill .bar{
    position: relative;
    z-index: 1;
    background: #ddd;
    padding: 0;
    margin-top: 14px;
    height: 9px;
    border-radius: 10px;
}
.skill .back{
    position: relative;
    z-index: 0;
    bottom: 9px;
    background: rgba(0, 0, 0, 0.34);
    height: 10px;
    border-radius: 10px;
    width: 100%;
}
</style>
</head><body>
<div class=""bkg"">
<!--
<div class=""skill"">
    <div class=""title"">
        <span>问卷得分</span>
        <small class=""level"" style=""color: #eedd88"">78分</small>
    </div>
    <div class=""bar"" style=""width: 74%""></div>
    <div class=""back""></div>
</div>
-->
<div class=""skill"">
    <div class=""title"">
        <span>攻击性</span>
        <small class=""level"" style=""color: " + colorag + @""">" + aggression + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + aggression + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>抑郁程度</span>
        <small class=""level"" style=""color: " + colorde + @""">" + depression + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + depression + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>焦虑程度</span>
        <small class=""level"" style=""color: " + coloran + @""">" + anxiety + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + anxiety + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>谎言概率</span>
        <small class=""level"" style=""color: " + colorlie + @""">" + lie + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + lie + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>自杀概率</span>
        <small class=""level"" style=""color: " + colorsu + @""">" + suicide + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + suicide + @"%""></div>
    <div class=""back""></div>
</div>
</div>

</body>";
                    #endregion
                    #region resultHtml
                    string resultHtml = @"
<html><head>
<script type=""text/javascript"">window.location.hash = ""#ok"";</script>
<style type=""text/css"">
html, body {
	height: 346px;
	display: flex;
	align-items: center;
	justify-content: center;
    font-family:微软雅黑;
    font-size:14px;
	background: rgb(11,12,39);
    overflow-x:hidden; 
    overflow-y:hidden; 
}
.card {
	width: 235px;
  height: 346px;
	background: rgb(11,12,39);
  color: #fff;
}
.card span {
  line-height: 22px;
  font-size: 22px;
  margin-top: 4px;
  font-weight: bold;
}
.card p {
  margin-top: 70px;
  text-align: center;
  line-height: 80px;
  font-size: 46px;
  font-weight: bold;
}

</style>
</head><body>

<div class=""card"">
    <span>入所危险性评估</span>
    <p style=""color: " + coloras + @""">" + assess + @"</p>
</div>

</body>";
                    #endregion
                    webKitResult.DocumentText = resultHtml;
                    webKitBar.DocumentText = barHtml;
                    webKitResult.BringToFront();
                    webKitBar.BringToFront();
                    SB2.Append("入所谈话,");
                    SB2.Append(assess + "," + fscore + "分,");
                    SB2.Append(aggression + "%," + depression + "%," + anxiety + "%," + lie + "%," + suicide + "%,");
                    break;
                default:
                    #region otherHtml
                    string otherHtml = @"
<html><head>
<script type=""text/javascript"">window.location.hash = ""#ok"";</script>
<style type=""text/css"">
html, body {
	height: 346px;
	display: flex;
    flex-direction: column;
	align-items: center;
	justify-content: center;
    font-family:黑体;
    font-size:18px;
	background: rgb(11,12,39);
    overflow-x:hidden; 
    overflow-y:hidden; 
}
.bkg {
  background: rgb(11,12,39);
  height: 346px;
  width: 470px;
}
.bkg:after {
    border: 0;
}
.skill {
    height: 56px;
    margin-top: 10px;
    color: #fff;
}
.skill:hover {
  background: rgba(0,140,215,0.1);
}
.skill .title {
  display: flex;
  flex-direction: row;
}
.skill span {
    width: 370px;
    text-align: left;
}
.skill small{
    width: 100px;
    text-align: right;
    font-weight: bold;
}
.skill .bar{
    position: relative;
    z-index: 1;
    background: #ddd;
    padding: 0;
    margin-top: 14px;
    height: 9px;
    border-radius: 10px;
}
.skill .back{
    position: relative;
    z-index: 0;
    bottom: 9px;
    background: rgba(0, 0, 0, 0.34);
    height: 10px;
    border-radius: 10px;
    width: 100%;
}
</style>
</head><body>
<div class=""bkg"">
<div class=""skill"">
    <div class=""title"">
        <span>攻击性</span>
        <small class=""level"" style=""color: " + colorag + @""">" + aggression + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + aggression + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>抑郁程度</span>
        <small class=""level"" style=""color: " + colorde + @""">" + depression + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + depression + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>焦虑程度</span>
        <small class=""level"" style=""color: " + coloran + @""">" + anxiety + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + anxiety + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>谎言概率</span>
        <small class=""level"" style=""color: " + colorlie + @""">" + lie + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + lie + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>自杀概率</span>
        <small class=""level"" style=""color: " + colorsu + @""">" + suicide + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + suicide + @"%""></div>
    <div class=""back""></div>
</div>
</div>
</body>";
                    #endregion
                    webKitMode.DocumentText = otherHtml;
                    webKitMode.BringToFront();
                    var mode = combox_talkmode.SelectedItem;
                    SB2.Append(mode + ",");
                    SB2.Append(" " + "," + " " + ",");
                    SB2.Append(aggression + "%," + depression + "%," + anxiety + "%," + lie + "%," + suicide + "%,");
                    break;
            }
            
            save_result();
        }
        private void showRecResults_notfinish()
        {
            string nowtime = DateTime.Now.ToLocalTime().ToString();
            SB2.Append(nowtime + ",");
            SB2.Append(person.name + "," + person.age_range + "," + person.gender + ",");

            aggression = Math.Round(aggression, 2);
            depression = Math.Round(depression, 2);
            anxiety = Math.Round(anxiety, 2);
            lie = Math.Round(lie, 2);
            suicide = Math.Round(suicide, 2);

            string colorag;
            string colorde;
            string coloran;
            string colorlie;
            string colorsu;
            int n0 = 0;
            int n1 = 0;
            int n2 = 0;
            int n3 = 0;

            if (aggression < 35) { colorag = color0; n0 += 1; }
            else if (aggression < 60) { colorag = color1; n1 += 1; }
            else if (aggression < 85) { colorag = color2; n2 += 1; }
            else { colorag = color3; n3 += 1; }

            if (depression < 35) { colorde = color0; n0 += 1; }
            else if (depression < 60) { colorde = color1; n1 += 1; }
            else if (depression < 85) { colorde = color2; n2 += 1; }
            else { colorde = color3; n3 += 1; }

            if (anxiety < 35) { coloran = color0; n0 += 1; }
            else if (anxiety < 60) { coloran = color1; n1 += 1; }
            else if (anxiety < 85) { coloran = color2; n2 += 1; }
            else { coloran = color3; n3 += 1; }

            if (lie < 35) { colorlie = color0; n0 += 1; }
            else if (lie < 60) { colorlie = color1; n1 += 1; }
            else if (lie < 85) { colorlie = color2; n2 += 1; }
            else { colorlie = color3; n3 += 1; }

            if (suicide < 35) { colorsu = color0; n0 += 1; }
            else if (suicide < 60) { colorsu = color1; n1 += 1; }
            else if (suicide < 85) { colorsu = color2; n2 += 1; }
            else { colorsu = color3; n3 += 1; }

            #region modeAllHtml
            string modeAllHtml = @"
<html><head>
<script type=""text/javascript"">window.location.hash = ""#ok"";</script>
<style type=""text/css"">
html, body {
	height: 346px;
	display: flex;
    flex-direction: column;
	align-items: center;
	justify-content: center;
    font-family:黑体;
    font-size:18px;
	background: rgb(11,12,39);
    overflow-x:hidden; 
    overflow-y:hidden; 
}
.bkg {
  background: rgb(11,12,39);
  height: 346px;
  width: 470px;
}
.bkg:after {
    border: 0;
}
.skill {
    height: 56px;
    margin-top: 10px;
    color: #fff;
}
.skill:hover {
  background: rgba(0,140,215,0.1);
}
.skill .title {
  display: flex;
  flex-direction: row;
}
.skill span {
    width: 370px;
    text-align: left;
}
.skill small{
    width: 100px;
    text-align: right;
    font-weight: bold;
}
.skill .bar{
    position: relative;
    z-index: 1;
    background: #ddd;
    padding: 0;
    margin-top: 14px;
    height: 9px;
    border-radius: 10px;
}
.skill .back{
    position: relative;
    z-index: 0;
    bottom: 9px;
    background: rgba(0, 0, 0, 0.34);
    height: 10px;
    border-radius: 10px;
    width: 100%;
}
</style>
</head><body>
<div class=""bkg"">
<div class=""skill"">
    <div class=""title"">
        <span>攻击性</span>
        <small class=""level"" style=""color: " + colorag + @""">" + aggression + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + aggression + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>抑郁程度</span>
        <small class=""level"" style=""color: " + colorde + @""">" + depression + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + depression + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>焦虑程度</span>
        <small class=""level"" style=""color: " + coloran + @""">" + anxiety + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + anxiety + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>谎言概率</span>
        <small class=""level"" style=""color: " + colorlie + @""">" + lie + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + lie + @"%""></div>
    <div class=""back""></div>
</div>
<div class=""skill"">
    <div class=""title"">
        <span>自杀概率</span>
        <small class=""level"" style=""color: " + colorsu + @""">" + suicide + @"%</small>
    </div>
    <div class=""bar"" style=""width: " + suicide + @"%""></div>
    <div class=""back""></div>
</div>
</div>
</body>";
            #endregion
            webKitMode.DocumentText = modeAllHtml;
            webKitMode.BringToFront();
            switch(talkmode)
            {
                case 0:
                    SB2.Append("默认谈话,");
                    SB2.Append(" " + "," + " " + ",");
                    SB2.Append(aggression + "%," + depression + "%," + anxiety + "%," + lie + "%," + suicide + "%,");
                    break;
                case 1:
                    SB2.Append("入所谈话,");
                    SB2.Append("未完成" + "," + " " + ",");
                    SB2.Append(aggression + "%," + depression + "%," + anxiety + "%," + lie + "%," + suicide + "%,");
                    break;
                default:
                    var mode = combox_talkmode.SelectedItem;
                    SB2.Append(mode + ",");
                    SB2.Append("未完成" + "," + " " + ",");
                    SB2.Append(aggression + "%," + depression + "%," + anxiety + "%," + lie + "%," + suicide + "%,");
                    break;
            }
            save_result();
        }
        #endregion
        #region 语音合成
        private void _SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            StartButton.Enabled = true;
            NextButton.Enabled = true;
            if (ready_flag)
            {
                ready_flag = false;
                NextButton.Visible = true;
                AskQuestion();
            }
            else
            {

            }

        }
        private void ReadQuestion(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            questionList.Clear();
            string input = null;
            while ((input = sr.ReadLine()) != null)
            {
                if (!String.IsNullOrWhiteSpace(input))
                {
                    questionList.Add(input);
                }
                else
                {
                    break;
                }
            }
        }
        private void AskQuestion()
        {
            string nowtime = DateTime.Now.ToLocalTime().ToString();
            word2 = questionList[qindex];
            string wen = nowtime + "\t" + "问题：" + word2 + "\r\n";
            SB1.Append(wen);
            string str = @"
                        <script type=""text/javascript"">window.location.hash = ""#ok"";
                        </script>
                        <div class=""chat_content_group buddy"">
                            <p class=""chat_nick"">问题</p>
                            <p class=""chat_content"">" + word2 + @"</p>
                        </div>
                        <a id='ok'></a>
                        ";
            webKitBrowser1.DocumentText = webKitBrowser1.DocumentText.Replace("<a id='ok'></a>", "") + str;//左侧
            background_worker.RunWorkerAsync(ask_id);
        }
        #endregion
        #region 谎言检测
        private void VAD()
        {
            //开始情感检测
            emoAnalyser = new Analyser(workDir, smileConfig, emoModel, lieModel);
            thAnalyse = new Thread(Analyse);
            thAnalyse.IsBackground = true;
            thAnalyse.Start();
            Queue<short[]> buffer = new Queue<short[]>();
            while (true)
            {
                //如果上一线程已结束，且其产生的数据已被全部处理完毕，则停止
                lock (audioData1)
                {
                    if (audioData1.Count == 0)
                        if (micphone_enable_flag)
                            Monitor.Wait(audioData1);
                        else
                            break;
                    else
                        while (audioData1.Count > 0)
                            buffer.Enqueue(audioData1.Dequeue());
                }

                while (buffer.Count > 0)
                {
                    voiceDetector.Input(buffer.Dequeue());
                    if (voiceDetector.speeches.Count > 0)
                    {
                        VadData speech = voiceDetector.speeches.Dequeue();
                        DataPackage p = new DataPackage();
                        p.vadData = speech;
                        lock (vadResults)
                        {
                            vadResults.Enqueue(p);
                            Monitor.Pulse(vadResults);
                        }
                    }
                }
            }
            lock (vadResults)
            {
                Monitor.Pulse(vadResults);
            }
        }
        private void Analyse()
        {
            int count = 0;
            Queue<DataPackage> buffer = new Queue<DataPackage>();
            while (true)
            {
                //如果上一线程已结束，且其产生的数据已被全部处理完毕，则停止
                lock (vadResults)
                {
                    if (vadResults.Count == 0)
                        if (thVAD.IsAlive)
                            Monitor.Wait(vadResults);
                        else
                            break;
                    else
                        while (vadResults.Count > 0)
                            buffer.Enqueue(vadResults.Dequeue());
                }

                while (buffer.Count > 0)
                {
                    DataPackage p = buffer.Dequeue();
                    EmotionData emoData = emoAnalyser.Analyse(p.vadData.data, wavFormat);
                    p.emotionData = emoData;
                    foreach (short item in p.vadData.data)
                    {
                        //Console.Write(item);
                        if (item == 0) lie_probability = 0;
                        else lie_probability = (int)(emoData.lieProb * 100);
                    }
                    count++;
                }
            }
        }
        #endregion
        #region 保存文件
        private void save_iFlytek()
        {
            string iFlytek_text = SB1.ToString();
            StreamWriter sw1 = new StreamWriter(iFlytek_save_path_main);
            sw1.Write(iFlytek_text);
            sw1.Close();
            SB1.Clear();
        }//保存历史笔录
        private void save_datareport()
        {
            string datareport_text = SB3.ToString();
            StreamWriter sw1 = new StreamWriter(datareport_save_path_main, true, Encoding.UTF8);
            sw1.Write(datareport_text);
            sw1.Close();
            SB3.Clear();
        }//保存数据报表
        private void save_result()
        {
            string result_text = SB2.ToString();
            StreamWriter sw = new StreamWriter(result_save_path_main, true, Encoding.UTF8);
            sw.Write(result_text);
            sw.Close();
            SB2.Clear();
        }//保存数据报表
        private void CombineMp4(string File1, string File2, string DstFile, bool stop_flag = false)
        {
            string strTmp1 = null;
            string strTmp2 = null;
            string strCmd1 = null;
            string strCmd2 = null;
            string strCmd3 = null;

            strTmp1 = File1 + ".ts";
            strTmp2 = File2 + ".ts";
            strCmd1 = " -i " + File1 + ".mp4 -c copy -bsf:v h264_mp4toannexb -f mpegts " + strTmp1 + " -y ";
            strCmd2 = " -i " + File2 + ".mp4 -c copy -bsf:v h264_mp4toannexb -f mpegts " + strTmp2 + " -y ";


            strCmd3 = " -i \"concat:" + strTmp1 + "|" +
                strTmp2 + "\" -c copy -bsf:a aac_adtstoasc -movflags +faststart " + DstFile + " -y ";

            //转换文件类型，由于不是所有类型的视频文件都支持直接合并，需要先转换格式
            Process p = new Process();
            p.StartInfo.FileName = Application.StartupPath + "//ffmpeg//bin//ffmpeg.exe";//要执行的程序名称  
            p.StartInfo.Arguments = " " + strCmd1;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;//可能接受来自调用程序的输入信息  
            p.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息   
            p.StartInfo.RedirectStandardError = false;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口   
            p.Start();//启动程序   
            p.WaitForExit();//等待程序执行完退出进程

            //转换文件类型，由于不是所有类型的视频文件都支持直接合并，需要先转换格式
            p = new Process();
            p.StartInfo.FileName = Application.StartupPath + "//ffmpeg//bin//ffmpeg.exe";//要执行的程序名称  
            p.StartInfo.Arguments = " " + strCmd2;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;//可能接受来自调用程序的输入信息  
            p.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息   
            p.StartInfo.RedirectStandardError = false;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口   
            p.Start();//启动程序   
            p.WaitForExit();//等待程序执行完退出进程

            //合并视频
            p = new Process();
            p.StartInfo.FileName = Application.StartupPath + "//ffmpeg//bin//ffmpeg.exe";//要执行的程序名称  
            p.StartInfo.Arguments = " " + strCmd3;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;//可能接受来自调用程序的输入信息  
            p.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息   
            p.StartInfo.RedirectStandardError = false;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口   
            p.Start();//启动程序   
            p.WaitForExit();//等待程序执行完退出进程

            File.Delete(@strTmp1);
            File.Delete(@strTmp2);
            if (stop_flag)
            {
                invalidate_message_textbox();
                close_micphone();
                close_all_camera();
                MessageBox.Show("录制信息保存成功！");
                is_saving_flag = false;
                camera_main_record_flag = false;
                StartButton.Enabled = true;
            }
            else
            {
                is_saving_flag = false;
                invalidate_message_textbox();
                paint_fresh_flag = true;
            }
        }//拼合已保存的视频
        private void CombineMp3(string File1, string File2, string DstFile)
        {
            string strTmp1 = File1 + "mp3";
            string strTmp2 = File2 + "mp3";
            string strCmd1 = null;
            strCmd1 = " -i concat:\"" + strTmp1 + "|" +
                strTmp2 + "\" -c copy " + DstFile + " -y ";
            Process p = new Process();
            p.StartInfo.FileName = Application.StartupPath + "//ffmpeg//bin//ffmpeg.exe";//要执行的程序名称  
            p.StartInfo.Arguments = " " + strCmd1;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;//可能接受来自调用程序的输入信息  
            p.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息   
            p.StartInfo.RedirectStandardError = false;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口   
            p.Start();//启动程序   
            p.WaitForExit();//等待程序执行完退出进程

        }//拼合已保存的音频
        private void CombineTXT(string File1, string File2, string DstFile)
        {
            string a = File.ReadAllText(File1);
            string b = File.ReadAllText(File2);
            File.WriteAllText(DstFile, a + b);
        }//拼合已保存的文本
        #endregion
        #region 停止工作
        private void invalidate_imagebox()
        {
            ImageBox_0.Image = null;
            ImageBox_0.BringToFront();
            ImageBox_0.Refresh();
        }
        private void invalidate_record_controls()
        {
            this.textbox_recording_interval.Text = null;
            this.textbox_recording_interval.Visible = false;
            menuStrip1.Refresh();
            textbox_recording_interval.Refresh();
        }
        private void invalidate_detetcor()
        {
            this.detector_id = init_detector_id;
            if (detector_enable_flag)
            {
                detector_enable_flag = false;
                this.detector.stop();
                this.detector.Dispose();
            }
        }
        public void close_all_camera()
        {
            paint_fresh_flag = false;
            invalidate_imagebox();
            validate_message_textbox(message_close_camera);
            if (camera_0_fresh_flag)
            {
                camera_0_fresh_flag = false;
                capture_0.Stop();
                capture_0.Dispose();
            }
            if (main_camera_fresh_flag)
            {
                main_camera_fresh_flag = false;
                background_worker.RunWorkerAsync(close_main_camera_id);
            }
            else
            {
                invalidate_message_textbox();
                paint_fresh_flag = true;
            }

        }
        private void close_micphone()
        {
            if (micphone_enable_flag)       ////关麦
            {
                microphoneCapturer.Stop();
                desktopCapturer.Stop();
                micphone_enable_flag = false;
                thVAD.Abort();
            }
        }//关闭麦克风
        private void close_record_filemaker()
        {
            if (camera_main_record_flag)
            {
                this.videoFileMaker_main.Close(true);
            }

            this.silenceVideoFileMaker.Close(true);

            this.audioFileMaker_main.Close(true);
        }//结束掉所有的VideoFileMaker和audioFileMaker
        #endregion
        #region 提示信息
        private void validate_message_textbox(string message)
        {
            this.textbox_waitingMessage.BringToFront();
            this.textbox_waitingMessage.Text = message;
            this.textbox_waitingMessage.Visible = true;
            this.textbox_waitingMessage.Refresh();
        }//使用textbox_waitingMessage控件显示提示信息
        private void invalidate_message_textbox()
        {
            this.textbox_waitingMessage.SendToBack();
            this.textbox_waitingMessage.Visible = false;
            this.textbox_waitingMessage.Refresh();
        }//隐藏textbox_waitingMessage控件
        private void CaptureError(Exception obj)
        {
            MessageBox.Show("视频录制组件采集过程出现错误！");
        }//软件工作过程中如果出现异常会触发该函数，比如拔掉了摄像头
        #endregion
        #region 数据类型转换
        public void frame2Mat(Affdex.Frame frame)
        {
            matImage = new Mat(frame.getHeight(), frame.getWidth(), DepthType.Cv8U, 3);
            byte[] pixels = frame.getBGRByteArray();  // pixels是一个一维像素数组
            IntPtr matIntptr = matImage.DataPointer;  //DataPointer:Pointer to the beginning of the raw data 第一个像素的地址
            int data_x = 0;
            int row_bytes = frame.getWidth() * 3;
            int imageIntptr_x = 0;
            for (int y = 0; y < frame.getHeight(); y++)
            {
                Marshal.Copy(pixels, data_x, matIntptr + imageIntptr_x, row_bytes);
                data_x += row_bytes;
                imageIntptr_x += row_bytes;
            }
        }//frame2Mat,frame数据类型转变成Mat数据类型
        public void frame2Bitmap(Affdex.Frame frame)
        {
            bitmapImage = new Bitmap(frame.getWidth(), frame.getHeight(), PixelFormat.Format24bppRgb);
            byte[] pixels = frame.getBGRByteArray();
            var bounds = new Rectangle(0, 0, frame.getWidth(), frame.getHeight());
            BitmapData bmpData = bitmapImage.LockBits(bounds, ImageLockMode.WriteOnly, bitmapImage.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int data_x = 0;
            int row_bytes = frame.getWidth() * 3;
            int imageIntptr_x = 0;
            for (int y = 0; y < frame.getHeight(); y++)
            {
                Marshal.Copy(pixels, data_x, ptr + imageIntptr_x, row_bytes);
                data_x += row_bytes;
                imageIntptr_x += row_bytes;
            }
            bitmapImage.UnlockBits(bmpData);
        }//frame数据类型转变成Bitmap数据类型
        public void pixels2Bitmap(Mat matImage, Bitmap bitmapImage)
        {
            var pixels = matImage.Mat2bytesArray();
            var bounds = new Rectangle(0, 0, matImage.Width, matImage.Height);
            BitmapData bmpData = bitmapImage.LockBits(bounds, ImageLockMode.WriteOnly, bitmapImage.PixelFormat);
            IntPtr ptr = bmpData.Scan0;

            int data_x = 0;
            int row_bytes = matImage.Width * 3;
            int imageIntptr_x = 0;

            for (int y = 0; y < matImage.Height; y++)
            {
                Marshal.Copy(pixels, data_x, ptr + imageIntptr_x, row_bytes);
                data_x += row_bytes;
                imageIntptr_x += row_bytes;
            }

            bitmapImage.UnlockBits(bmpData);
        }//Mat——>pixels——>Bitmap过程的第二步
        private static Affdex.Frame Bitmap2frame(Bitmap bitmap)
        {
            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int numBytes = bitmap.Width * bitmap.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            int data_x = 0;
            int ptr_x = 0;
            int row_bytes = bitmap.Width * 3;

            // The bitmap requires bitmap data to be byte aligned.
            // http://stackoverflow.com/questions/20743134/converting-opencv-image-to-gdi-bitmap-doesnt-work-depends-on-image-size

            for (int y = 0; y < bitmap.Height; y++)
            {
                Marshal.Copy(ptr + ptr_x, rgbValues, data_x, row_bytes);//(pixels, data_x, ptr + ptr_x, row_bytes);
                data_x += row_bytes;
                ptr_x += bmpData.Stride;
            }

            bitmap.UnlockBits(bmpData);

            return new Affdex.Frame(bitmap.Width, bitmap.Height, rgbValues, Affdex.Frame.COLOR_FORMAT.BGR);
        }
        private void video2wav(string vedioPath, string wavPath)
        {
            string csbitrate = "8000";//码率
            string csfreq = "16000";//频率
            string cschannel = "1";//通道数
            string exePath = Path.Combine(Application.StartupPath, "ffmpeg\\bin\\ffmpeg.exe");

            string exePara = " -i ";
            exePara += vedioPath;
            exePara += " -ab ";
            exePara += csbitrate;
            exePara += " -ar ";
            exePara += csfreq;
            exePara += " -ac ";
            exePara += cschannel;
            exePara += " ";
            exePara += wavPath;
            String cmdline = exePath + exePara;
            ShellExecute(this.Handle, "open", exePath, exePara, null, ShowWindowCommands.SW_HIDE);//执行转换命令
        }//从视频文件中提取语音
        private void mp32wav(string mp3_path, string wave_path)
        {
            string exePath = Path.Combine(Application.StartupPath, "ffmpeg\\bin\\ffmpeg.exe");
            string exePara = null;
            exePara = " -i ";
            exePara += mp3_path;
            exePara += " -f";
            exePara += " wav ";
            exePara += wave_path;
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";    //要执行的程序名称 
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;//可能接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息 
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口 
            p.Start();    //启动 
            //向CMD窗口发送输入信息： 
            p.StandardInput.WriteLine(exePath + exePara + "&exit");
            p.StandardInput.AutoFlush = true;
            p.WaitForExit();
            p.Close();
        }//调ffmpeg将map3转为wav格式
        private void Txt2Csv(string File1, string DstFile)
        {
            string a = File.ReadAllText(File1);
            StreamWriter sw1 = new StreamWriter(DstFile, true, Encoding.UTF8);
            sw1.Write(a);
            sw1.Close();
        }
        #endregion
        #region 人脸识别，暂时不用
        public void LoadFaceNames()
        {
            faceNames = new List<string>();
            faceNamesPath = Path.Combine(Application.StartupPath, "data\\face_names\\face_names.txt");
            faceNames.txt2List(faceNamesPath);
        }
        private int getFaceLabel(Rectangle faceRectangle)
        {
            if (firstfaceFlag)  //第一次检测到人脸和识别人脸时
            {
                this.trainedFaceRecognizer = new Trained_Face_Recognizer();
                trainedFaceRecognizer.Set_faceRecognizer();  //训练或加载faceRecognizer
                LoadFaceNames();                             //重新加载faceNames列表
            }
            UMat faceImage = new UMat();
            Mat tempImage = matImage.Clone();
            tempImage = new Mat(tempImage, faceRectangle);
            CvInvoke.CvtColor(tempImage, faceImage, ColorConversion.Bgr2Gray);
            CvInvoke.Resize(faceImage, faceImage, new Size(100, 100));
            CvInvoke.EqualizeHist(faceImage, faceImage);
            var pr = trainedFaceRecognizer.faceRecognizer.Predict(faceImage);
            return pr.Label;
        }
        private void getPersonName()
        {

            if (firstfaceFlag)  //第一次检测到人脸时
            {
                int label = getFaceLabel(this.faceRectangle);
                person.name = faceNames[label];
                facerecognizerTimestamp = getTimeStamp();
                firstfaceFlag = false;
            }
            else
            {
                double time_now = getTimeStamp();
                if (time_now - facerecognizerTimestamp > 0.2)  //200ms识别一次人脸
                {
                    int label = getFaceLabel(this.faceRectangle);
                    person.name = faceNames[label];
                    facerecognizerTimestamp = time_now;
                }
            }
        }
        private double getTimeStamp()
        {
            DateTime time = DateTime.Now;
            long temp = (time.Ticks - startTime.Ticks) / 10000;
            double timeStamp = temp / 1000.0d;
            return timeStamp;
        }
        #endregion
        #region Init初始函数
        private void Init_QnaireList()
        {

            questionnairelist.Add("null");
            combox_talkmode.Items.Add("入所谈话");
            questionnairelist.Add(@".\questions\入所谈话.txt");  //入所问卷
        }
        private void Init_Speech()
        {
            qindex = 0;
            findex = 0;
            qscore = 0;
            fscore = 0;
            NextButton.Text = "下一题";
            NextButton.Visible = false;
            speechSyn.SetOutputToDefaultAudioDevice();
            speechSyn.Rate = 2;
            speechSyn.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(_SpeakCompleted);
        }
        public void Init_CameraDic()
        {
            cameras_dic = new Dictionary<int, ToolStripMenuItem>();
            cameras_dic.Add(0, Camera_0_ToolStripMenuItem);
            cameras_dic.Add(1, Camera_1_ToolStripMenuItem);
            cameras_dic.Add(2, Camera_2_ToolStripMenuItem);
            cameras_dic.Add(3, Camera_3_ToolStripMenuItem);
        }             // 初始化摄像头字典cameras_dic
        public void Init_MicphoneDic()
        {
            micphones_dic = new Dictionary<int, ToolStripMenuItem>();
            micphones_dic.Add(0, Micphone_0_ToolStripMenuItem);
            micphones_dic.Add(1, Micphone_1_ToolStripMenuItem);
            micphones_dic.Add(2, Micphone_2_ToolStripMenuItem);
            micphones_dic.Add(3, Micphone_3_ToolStripMenuItem);

        }           // 初始化麦克风字典micphones_dic
        public void Init_EmotionDic()
        {
            emotionDic = new Dictionary<string, double>();
            emotionDic.Add("生气", 0);
            emotionDic.Add("厌恶", 0);
            emotionDic.Add("恐惧", 0);
            emotionDic.Add("喜悦", 0);
            emotionDic.Add("悲伤", 0);
            emotionDic.Add("吃惊", 0);
            emotionDic.Add("平静", 5);
        }            // 初始化情感字典emotionDic
        public void Init_EmotionArray()
        {
            emotionArray = new double[7];
            for (int i = 0; i < 7; i++)
            {
                emotionArray[i] = 0;
            }
        }          // 初始化EmotionArray
        private void Init_Background_Worker()
        {
            background_worker.DoWork += new DoWorkEventHandler(Background_worker_DoWork);
            background_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Background_worker_RunWorkerCompleted);
        }    // 初始化后台线程
        private void Init_iFlytek()
        {
            var s = "{\"enginIP\":\"192.168.7.180\",\"leftName\":\"问\",\"rightName\":\"答\",\"leftport\":\"\",\"rightport\":\"\"}";
            axSXClientActiveXControl1.InitActiveX();
            axSXClientActiveXControl1.InitSettings(s);//初始化控件加载需要的设置参数。比如：引擎地址、问话人、答话人等
            axSXClientActiveXControl1.SetSessionID("biluID");//设置单次笔录的唯一标识。
            axSXClientActiveXControl1.SetUserID("userid");//设置当前登录用户的id
            axSXClientActiveXControl1.SetAudioSaveDirect("E:\\data");
            axSXClientActiveXControl1.SaveLocalFile();
        }              //初始化讯飞控件
        private void Init_record_audio()
        {
            try
            {
                this.audioFileMaker_main = new AudioFileMaker();
                this.audioFileMaker_main.Initialize(temp_audio_path_main, AudioCodecType.AAC, audioSampleRate, channelCount);
            }
            catch (Exception)
            {
                throw;
            }

        }         // 初始化录制音频的各种参数
        private void Init_record_video(int camera_id)
        {
            if (camera_id==main_camera_id)
            {
                try
                {
                    System.Drawing.Size videoSize = new System.Drawing.Size(record_width, record_height);
                    this.videoFileMaker_main = new VideoFileMaker();
                    this.videoFileMaker_main.Initialize(temp_video_path_main, VideoCodecType.H264, videoSize.Width, videoSize.Height, record_fps, VideoQuality.High, AudioCodecType.AAC, audioSampleRate, channelCount, true);
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
                return;
            }
            try
            {
                this.sizeRevised = (videoSized.Width % 4 != 0) || (videoSized.Height % 4 != 0);
                if (this.sizeRevised)
                {
                    videoSized = new System.Drawing.Size(videoSized.Width / 4 * 4, videoSized.Height / 4 * 4);
                }
                this.silenceVideoFileMaker = new SilenceVideoFileMaker();
                this.silenceVideoFileMaker.Initialize(temp_video_path_desktop, VideoCodecType.H264, videoSized.Width, videoSized.Height, record_desktop_fps, VideoQuality.Middle);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }  // 初始化录制视频的各种参数
        #endregion
        #region 类的成员变量
        private bool OKflag = false;
        private bool last = false;  //指示当前是否为最后一题

        private int count = 0;
        private double depression = 0;  //抑郁
        private double anxiety = 0;  //焦虑
        private double lie = 0;  //谎言
        private double aggression = 0;  //攻击
        private double suicide = 0;  //自杀
        private const string color0 = "#317cff"; //low [0, 35)
        private const string color1 = "#5bd824"; //mid [35, 60)
        private const string color2 = "#ff8d13"; //high [60, 85)
        private const string color3 = "#ff1111"; //extreme high [85, 100]
        private int talkmode = 0;  //谈话模式，默认为0
        private int qindex = 0;  //当前题号
        private int findex = 0;  //最后题号
        private int qscore = 0;  //当前题目得分
        private int fscore = 0;  //问卷最后得分
        private const int ready_id = -10;  //系统准备完成后，语音合成入口点
        private const int ask_id = -11;  //提问
        private const int answer_id = -12;  //回答
        private bool ready_flag = false;  // 标志是否执行完ready_id代表的线程
        private SpeechSynthesizer speechSyn = new SpeechSynthesizer();  //用于语音合成

        private List<string> questionList = new List<string>();            // 问题列表
        private List<string> answerList = new List<string>();              // 所有回答
        private List<string> questionnairelist = new List<string>();       // 问卷地址

        private int allmode = 0;  //当前程序共有谈话模式种数

        private double depression_value;
        private double attack_value;
        private double attention_value;
        private double tension_value;
        private double anxiety_value;
        private double hesitate_value;
        public static string local_desktop_path;
        private bool sizeRevised = false;
        private short short_audioData;
        Size videoSized = Screen.PrimaryScreen.Bounds.Size;
        private bool Record_Camera_Flag = false;
        private bool StartButton_flag = false;
        public string local_eye_path;
        private Mat frame_0 = new Mat();                        // 用来存储capture_0返回的帧图像 
        private Mat frame_1 = new Mat();                        // 用来存储capture_1返回的帧图像 
        private Mat frame_2 = new Mat();                        // 用来存储capture_2返回的帧图像 
        private Mat frame_3 = new Mat();                        // 用来存储capture_3返回的帧图像 
        private Mat matImage;                                   // 用来存储detector返回的帧图像         
        private Bitmap bitmapImage;                             // 保存到videoFileMaker中的帧图像
        private Person person;                                  // person类用来保存各种检测信息
        private List<string> faceNames;                         // 人脸识别的相关内容，暂时不用
        private Trained_Face_Recognizer trainedFaceRecognizer;  // 人脸识别的相关内容，暂时不用

        private const string default_string = "未知";           // 当检测不到人脸时，default_string用来表示心率，呼吸率，情感的默认值
        private const string message_open_video ="正在打开视频，请等待. . . . . .";
        private const string message_manu_save = "正在保存视频，请等待. . . . . .";
        private const string message_close_camera = "正在关闭摄像头，请等待. . . . . .";
        private const string message_close_video = "正在关闭视频，请等待. . . . . .";

        private string save_root = null;               // 录制视频保存位置的根目录
        //private string filepath = null;                // 打开眼动仪的根目录
        private string save_dir = null;                // 录制视频保存位置的一级目录
        public string save_name = null;               // 录制视频的保存文件名
        private string save_path_main = null;          // 询问摄像头录制视频保存位置的二级目录
        private string save_path_desktop = null;       // 录制屏幕视频保存位置的二级目录
        private string iFlytek_save_path_main = null;  // 科大讯飞文本路径txt文件
        private string datareport_save_path_main = null;// 数据报表表格路径csv文件
        private string result_save_path_main = null;   // 综合评价结果路径txt文件
        private string temp_audio_path_main;           // 音频文件的初始保存位置
        private string temp_video_path_main;           // 询问摄像头录制视频文件的初始保存位置
        private string temp_video_path_desktop;        // 录制屏幕视频文件的初始保存位置
        private string videofile_basepath = Path.Combine(Application.StartupPath, "data\\videoFile");        // 文件的初始保存目录

        private bool firstfaceFlag = true;             // 第一次检测到人脸时的Flag 
        private bool detector_enable_flag = false;     // 第一次初始化detector的Flag
        private bool micphone_enable_flag = false;     // 麦克风是否打开
        private bool paint_fresh_flag = true;          // 用来控制OnPaint函数是否刷新窗体
        private bool camera_0_fresh_flag = false;      // 打开摄像头0时，为true
        private bool main_camera_fresh_flag = false;   // 打开询问摄像头时，为true
        private bool camera_main_record_flag = false;  // 通过询问摄像头录制视频时，为true
        private bool recording_audio_flag = false;     // 单独录制音频时，为true
        private bool recording_camera_flag = false;    // 录制视频是为true
        private bool is_saving_flag = false;           // 当视频自动保存和手动保存录制视频的过程中为true
        private bool save_root_is_done = false;        // 检查save_root路径符合条件时为true
        private bool small_imagebox_flag = false;      // 在小窗口显示4路视频

        private Rectangle faceRectangle;                         // 用来表示人脸区域的矩形框
        private Rectangle foreheadRectangle;                     // 用来表示前额区域的矩形框
        private List<PointF> mouthPoints = new List<PointF>();   // 
        private List<double> times = new List<double>();         // 时间序列，用来保存计算心率时的各个时间点
        private List<double> data_buffer = new List<double>();   // 


        private int temp_counter = 1;
        private int save_counter = 1;                            //
        private int number_tupo = 0;                             //
        private double mouth_variance = -1000;                   //
        private const int plot_buffer_capacity = 100;            // 
        private const double default_heartrate = -1;             // 默认的心率值  
        private const double default_breathrate = -1;            // 默认的呼吸率值
        private const double threshold_hr = 12;                  // 心率阈值为12bpm
        private const double threshold_ft = 1;
        private const double threshold_rr = 2;                   // 呼吸率阈值为5bpm

        private const double threshold_mouth = 100;              //

        private double facerecognizerTimestamp;
        private string faceNamesPath;
        private int current_frame_id = 0;
        
        private int camera_number = 0;
        private int micphone_number = 0;
        public int main_camera_id = 0;
        private int main_micphone_id = 0;
        private const int camera_0 = 0;
        private const int camera_1 = 1;
        private const int camera_2 = 2;
        private const int camera_3 = 3;
        private const int desktop = 4;
        private const int open_main_camera_id = 0;

        private int detector_id = -4;
        private const int video_detector_id = -1;           // 表示进行本地视频的检测，用于传入Background_worker_DoWork函数和set_detetcor函数
        private const int image_detector_id = -2;           // 表示进行图片的检测，用于传入set_detector函数
        private const int camera_detetcor_id = -3;          // 表示进行摄像头视频的检测,用于传入Background_worker_DoWork函数和set_detetcor函数
        private const int init_detector_id = -4;            // detecor_id的初始值

        private const int manu_save_id = -5;                // 表示手动点击停止录制，保存对应的视频，用于传入Background_worker_DoWork函数
        private const int auto_save_id = -6;                // 表示自动保存视频的标志，用于传入Background_worker_DoWork函数
        private const int close_main_camera_id = -7;        // 表示关闭询问摄像头和询问麦克风的标志，用于传入Background_worker_DoWork函数
        private const int open_iFlytek_id = -8;             // 表示打开科大讯飞笔录系统的标志，用于传入Background_worker_DoWork函数
        
        private Dictionary<int, Affdex.Face> faces { get; set; }
        private Dictionary<string, double> emotionDic;
        private double[] emotionArray;
        private Affdex.Detector detector { get; set; }
        private Affdex.FaceDetectorMode face_size;
        private System.DateTime recordStart;
        private int timer1_interval = 30 * 60 * 1000;
        private int timer2_interval = 1 * 1000;
        private System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
        
        private const int audioSampleRate = 16000;          // 音频采样率
        private const int channelCount = 1;                 // 声道
        private const int record_height = 480;              // 录制高度
        private const int record_width = 640;               // 录制宽度
        private const int record_fps = 15;                  // 插件Oracy录制视频的帧率
        private const int record_desktop_fps = 10;          // 插件Oracy录制桌面的帧率
        private const int capture_fps = 15;                 // Affdex.detector捕获视频的帧率
        private const int process_fps = 15;                 // Affdex.detector处理视频的帧率
        //private VideoWriter videoWriter;                  // videoWriter表示opencv存储无声录制视频的类
        private Capture capture_0;                          // 用于捕获camera_0摄像头图像的opencv类
        private VideoFileMaker videoFileMaker_main;         // 插件Oraycn的录制询问摄像头视频的类
        private SilenceVideoFileMaker silenceVideoFileMaker;      // 插件Oraycn的录制桌面的类
        private AudioFileMaker audioFileMaker_main;         // 插件Oraycn的采集并保存音频文件的类AudioFileMaker
        private IMicrophoneCapturer microphoneCapturer;     // 插件Oraycn录制麦克风的类IMicrophoneCapturer
        private IDesktopCapturer desktopCapturer;           // 插件Oraycn桌面采集器
        private readonly object _syncRoot = new object();   // 这个我也不知道是啥
        private int audioData_queueCapacity = 10 * 1600;    // 语音队列audiodata_Queue最大容量 
        private Queue<float> audiodata_Queue = new Queue<float>(12 * 1600);             // 用来保存实时语音数据，用于画语音波形图
        private BackgroundWorker background_worker = new BackgroundWorker();
        private ToolStripMenuItem Camera_0_ToolStripMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem Camera_1_ToolStripMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem Camera_2_ToolStripMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem Camera_3_ToolStripMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem Micphone_0_ToolStripMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem Micphone_1_ToolStripMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem Micphone_2_ToolStripMenuItem = new ToolStripMenuItem();
        private ToolStripMenuItem Micphone_3_ToolStripMenuItem = new ToolStripMenuItem();
        private Dictionary<int, ToolStripMenuItem> cameras_dic;
        private Dictionary<int, ToolStripMenuItem> micphones_dic;

        private VoiceDetector voiceDetector;                                // 用于VAD的模块
        private Queue<DataPackage> vadResults = new Queue<DataPackage>();   // VAD结果
        private Analyser emoAnalyser;                                       // 用于情感分析的模块
        private Thread thVAD;                                               // 负责VAD的线程
        private Thread thAnalyse;                                           // 负责情感分析的线程
        private readonly object syncRoot = new object();                    // 用于线程同步
        private string workDir = @".\tmpemo";
        private string smileConfig = @".\config\emobase.conf";
        private string emoModel = @".\models\emo_casia.txt";
        private string lieModel = @".\models\lie_55mid.txt";
        private DataChunkFormat wavFormat;
        private Queue<short[]> audioData1 = new Queue<short[]>();
        private int lie_probability;//谎言的值

        private float X;//主界面的宽
        private float Y;//主界面的高

        private string keywordtext_save_path = Path.Combine(Application.StartupPath, "告警语句.txt");

        private bool warningmessage_flag = false;//停止刷新多模态告警模块
        private bool open_iFlytek_flag = false; //打开讯飞语音时为true
        public static List<string> allkeywords = new List<string>();//告警词汇数组
        private string word1 = null;//答话人语句
        private string word2 = null;//问话人语句
        private StringBuilder SB1 = new StringBuilder();//历史笔录的动态字符串
        private StringBuilder SB2 = new StringBuilder();//谈话结果的动态字符串
        private StringBuilder SB3 = new StringBuilder();//数据报表的动态字符串
        #endregion
        public enum ShowWindowCommands : int
        {

            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,    //用最近的大小和位置显示，激活
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_MAX = 10
        }
        [DllImport("shell32.dll")]
        public static extern IntPtr ShellExecute(
            IntPtr hwnd,
            string lpszOp,
            string lpszFile,
            string lpszParams,
            string lpszDir,
            ShowWindowCommands FsShowCmd
            );
    }
    public enum WaveFormatTag   //定义枚举类型语音格式标签
    {
        Pcm = 1,
    }
    public struct WaveFormat    //定义语音格式结构体
    {
        public int AverageBytePerSecond { get; set; }
        public short BitsPerSample { get; set; }
        public short BlockAlign { get; set; }                               //块对齐
        public short Channels { get; set; }
        public WaveFormatTag FormatTag { get; set; }
        public int SamplesPerSecond { get; set; }
    }
    public struct DataPackage
    {
        public VadData vadData;
        public EmotionData emotionData;
        public string remark;               // 用户手动输入的备注
    }
}
public class Person    //Person类用来存放一个人在摄像头检测时的各种信息
{
    public Person()
    {
        this.name = null;
        this.gender = null;
        this.age_range = null;
        this.frame_number = 0;
        this.heart_rate = 0;
        this.face_tp = 0;
        this.breath_rate = 0;
        this.emotion_value = 0;
        this.emotion_key = "未知";
        this.emotion_image = null;

        this.emotionKeys = new List<string>();
        this.emotionValues = new List<double>();
        this.heartRates = new List<double>();
        this.faceTP = new List<double>();
        this.breathRates = new List<double>();
        this.facefeaturePoints = new List<string>();
        this.precessed_frame_id = new List<int>();
    }

    public void Clear()
    {
        frame_number = 0;
        heart_rate = 0;
        face_tp = 0;
        breath_rate = 0;
        emotion_value = 0;
        emotion_key = "未知";

        emotionKeys.Clear();
        heartRates.Clear();
        faceTP.Clear();
        breathRates.Clear();
        facefeaturePoints.Clear();
        emotionValues.Clear();
        precessed_frame_id.Clear();
    }       //// 对person.name,person.age,person.gender不做处理

    public string name { get; set; }
    public string age_range { get; set; }
    public string gender { get; set; }
    public int frame_number { get; set; }
    public double heart_rate { get; set; }
    public double face_tp { get; set; }
    public double breath_rate { get; set; }
    public string emotion_key { get; set; }
    public double emotion_value { get; set; }
    public Image emotion_image { get; set; }

    public List<int> precessed_frame_id { get; set; }
    public List<double> heartRates { get; set; }

    public List<double> faceTP { get; set; }
    public List<double> breathRates { get; set; }
    public List<string> emotionKeys { get; set; }
    public List<double> emotionValues { get; set; }
    public List<string> facefeaturePoints { get; set; }
}
public class Face_Datasets   //人脸数据库构成的类
{
    public Face_Datasets()
    {
        face_images = new VectorOfMat();
        face_IDs = new VectorOfInt();
        face_names = new List<string>();
        face_names_path = Path.Combine(Application.StartupPath, "data\\face_names\\face_names.txt");
    }

    public void Get_first_face_datasets()
    {
        string face_datasets_path = Path.Combine(Application.StartupPath, "data\\face_datasets");
        DirectoryInfo folder = new DirectoryInfo(face_datasets_path);
        int face_counter = 0;
        foreach (FileInfo file in folder.GetFiles("*.jpg"))
        {
            //Console.WriteLine("file.fullname:{0}", file.FullName);
            Mat face_image = CvInvoke.Imread(file.FullName, LoadImageType.Grayscale);
            this.face_images.Push(face_image);
            var image_name = Path.GetFileNameWithoutExtension(file.FullName);
            this.face_names.Add(image_name);
            face_counter++;
        }
        //Console.WriteLine("face_counter:{0}", face_counter);
        try
        {
            int[] ids = new int[face_counter];
            for (int i = 0; i < face_counter; i++)
            {
                ids[i] = i;
            }
            this.face_IDs.Push(ids);
        }
        catch (Exception)
        {
            MessageBox.Show("人脸数据库为空！");
            throw;
        }
        face_names.List2txt(face_names_path);
    }

    public void Get_other_face_datasets()
    {

    }


    //private string face_datasets_path;
    private string face_names_path;
    public VectorOfMat face_images;
    public VectorOfInt face_IDs;
    public List<string> face_names;
}
public class Trained_Face_Recognizer  //存放训练的人脸识别器
{
    public Trained_Face_Recognizer()
    {
        faceRecognizer = new Emgu.CV.Face.LBPHFaceRecognizer(1, 8, 8, 8, 400);
        faceRecognizerFile = Path.Combine(Application.StartupPath, "data\\faceRecognizer\\LBPHFaceRecognizer.dat");
    }

    public void Set_faceRecognizer()
    {
        if (!File.Exists(this.faceRecognizerFile)) //faceRecognizerFile文件不存在时
        {
           //Console.WriteLine("Training faceRecognizer first time...");
            Face_Datasets face_datasets = new Face_Datasets();
            face_datasets.Get_first_face_datasets();
            this.faceRecognizer.Train(face_datasets.face_images, face_datasets.face_IDs);
            this.faceRecognizer.Save(faceRecognizerFile);
        }
        else  //faceRecognizerFile文件存在时，加载训练过的模型
        {
            this.faceRecognizer.Load(this.faceRecognizerFile);
        }
    }


    public Emgu.CV.Face.FaceRecognizer faceRecognizer;  //人脸识别器种类
    private string faceRecognizerFile;  //人脸识别器模板所在的路径

}
public static class GraphicsExtensions
{
    public static void DrawCircle(this Graphics g, Pen pen,
                                  float centerX, float centerY, float radius, int number)
    {
        g.DrawEllipse(pen, centerX - radius, centerY - radius,
                      radius + radius, radius + radius);
        // g.DrawString(number.ToString(),new Font(FontFamily.GenericSerif, 32, FontStyle.Bold),new Pen(Color.DarkRed).Brush,new PointF(centerX, centerY));
    }
}  // Graphics类的方法
public static class MatExtensions
{
    public static double Get_Double_Value(this Mat mat, int row, int col, int channel)
    {
        var value = new double[1];
        Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize + (channel * 8), value, 0, 1);
        return value[0];
    }

    public static byte Get_Byte_Value(this Mat mat, int row, int col)
    {
        var value = new byte[1];
        Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
        return value[0];
    }
    public static byte[] Mat2bytesArray(this Mat mat)
    {
        int pixels_number = mat.Height * mat.Width * mat.NumberOfChannels;
        byte[] pixels = new byte[pixels_number];
        Marshal.Copy(mat.DataPointer, pixels, 0,pixels_number);
        return pixels;
    }

    public static void Set_Double_Value(this Mat mat, int row, int col, int channel, double value)
    {
        var target = new[] { value };
        Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize + (channel * 8), 1);
    }

    public static void ExchangeChannels(this Mat mat)
    {
        VectorOfMat vm = new VectorOfMat();
        CvInvoke.Split(mat, vm);
        Mat c1 = vm[0];
        Mat c2 = vm[1];
        Mat c3 = vm[2];
        Mat temp = new Mat();
        c1.CopyTo(temp);
        c3.CopyTo(c1);
        temp.CopyTo(c3);
        CvInvoke.Merge(vm, mat);

    }

    public static UMat Rgb2Gray(this Mat rgb_image)
    {
        UMat gray_image = new UMat();
        CvInvoke.CvtColor(rgb_image, gray_image, ColorConversion.Rgb2Gray);
        CvInvoke.Resize(gray_image, gray_image, new Size(100, 100));
        CvInvoke.EqualizeHist(gray_image, gray_image);
        return gray_image;
    }



    /**
     * Image2Value(): 对一幅图像的像素值做归一化
     * @param: Mat img: 做归一化的图像
     */

    public static double Image2Value(this Mat img)
    {
        int arrayLength = img.Height * img.Width;
        double sum = 0;
        for (int i = 0; i < img.Height; i++)
        {
            for (int j = 0; j < img.Width; j++)
            {
                sum += Get_Double_Value(img, i, j, 0);
            }
        }
        double value = sum / (arrayLength);
        return value;
    }
}  // Mat类的扩展方法
public static class ListStringExtensions
{
    public static bool List2Csv(this List<string> lst, string filePath)
    {
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            // 生成列
            var type = typeof(string);
            PropertyInfo[] props = type.GetProperties();
            StringBuilder strColumn = new StringBuilder();
            foreach (PropertyInfo item in props)
            {
                strColumn.Append(item.Name);
                strColumn.Append(",");
            }
            sw.WriteLine(strColumn);

            // 写入数据
            StringBuilder strValue = new StringBuilder();
            foreach (var dr in lst)
            {
                strValue.Clear();
                foreach (PropertyInfo item in props)
                {
                    strValue.Append(item.GetValue(dr, null));
                    strValue.Append(",");
                }
                sw.WriteLine(strValue);
            }
            return true;
        }
    }

    public static bool List2txt(this List<string> face_names, string filepath)
    {
        using (StreamWriter sw = new StreamWriter(filepath))
        {
            foreach (var name in face_names)
            {
                sw.WriteLine(name);
            }
        }
        return true;
    }

    public static bool txt2List(this List<string> face_names, string filepath)
    {
        using (StreamReader sr = new StreamReader(filepath))
        {
            string face_name = "Mr.wrong";
            while ((face_name = sr.ReadLine()) != null)
            {
                face_names.Add(face_name);
            }
        }
        return true;
    }
}  // 呃
class CFFTss
{
    double _pi_ = 3.1415926535898;
    public Complex[] dData;
    int Len;

    public void fft()
    {
        dData = fft_order(dData);
        int order = (int)(Math.Log(Len, 2));    // 蝶形运算级数
        for (int i = 1; i <= order; i++)
        {
            int row = (int)(Len / Math.Pow(2, i));
            int num = (int)Math.Pow(2, i);
            int num1 = num;
            for (int ii = 0; ii < (Len - num1); ii = ii + num)
            {
                for (int j = ii; j < ii + num / 2; j++)
                {
                    butterfly(dData[j], dData[j + num / 2], Len, (j - ii) * row, out dData[j], out dData[j + num / 2], true);
                }
            }
        }
    }

    private Complex[] fft_order(Complex[] x)
    {
        Len = x.Length;
        int inv_order;
        Complex[] y = new Complex[Len];
        for (int i = 0; i < Len; i++)
        {
            inv_order = inverse_order(i, (int)(Math.Log(Len, 2)));   //这里有问题
            y[inv_order] = x[i];
        }
        return y;
    }

    private int inverse_order(int x, int N)
    {
        if (x < 0)
        {

        }
        if (x >= Math.Pow(2, N))
        {

        }
        int r = x;
        int s = N;
        int m = s;
        for (; 0 != x; x >>= 1)
        {
            r <<= 1;
            r |= x & 1;
            s--;
        }
        r <<= s;
        r = r % (int)Math.Pow(2, m);
        return r;
    }

    private void butterfly(Complex in1, Complex in2, int N, int k, out Complex out1, out Complex out2, bool flag)
    {
        Complex Wnk;
        //  Complex Out1;
        //  Complex Out2;
        if (flag == true)
            Wnk = new Complex(Math.Cos(2 * _pi_ / N * k), -Math.Sin(2 * _pi_ / N * k));
        else
            Wnk = new Complex(Math.Cos(2 * _pi_ / N * k), Math.Sin(2 * _pi_ / N * k));
        out1 = in1 + Wnk * in2;
        out2 = in1 - Wnk * in2;
    }
}
