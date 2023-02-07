using System;
using System.Windows.Forms;

namespace Intelligent_Conversation_Platform
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Oraycn.MFile.GlobalUtil.SetAuthorizedUser("huiguanzs", "hgzs0612");
            Oraycn.MCapture.GlobalUtil.SetAuthorizedUser("huiguanzs", "hgzs0612");
            Oraycn.MPlayer.GlobalUtil.SetAuthorizedUser("huiguanzs", "hgzs0612");
            RegisterForm registerform = new RegisterForm();
            registerform.ShowDialog();
            if (registerform.DialogResult == DialogResult.OK)
            {
                Application.Run(new ProcessVideo());
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
