using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intelligent_Conversation_Platform
{
    public partial class VideoPlayer : Form
    {
        public VideoPlayer()
        {
            InitializeComponent();
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.URL = ProcessVideo.local_desktop_path;
        }

    }
}
