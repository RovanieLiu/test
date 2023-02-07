using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VoiceEmotionAnalyser
{
    /// <summary>
    /// 管理目录
    /// </summary>
    public class DirectoryManager
    {
        /// <summary>
        /// 清空目录下所有内容
        /// </summary>
        /// <param name="srcPath"></param>
        public static void CleanDirectory(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                        File.Delete(i.FullName);      //删除指定文件
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 将当前目录下所有内容复制到另一个目录下
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="dstPath"></param>
        public static void CopyDirectory(string srcPath, string dstPath)
        {
            if (!Directory.Exists(dstPath))
                Directory.CreateDirectory(dstPath);
            else
                CleanDirectory(dstPath);

            try
            {
                DirectoryInfo srcDir = new DirectoryInfo(srcPath);
                FileSystemInfo[] dirlist = srcDir.GetFileSystemInfos();
                foreach (FileSystemInfo path in dirlist)
                    if (path is DirectoryInfo)
                        CopyDirectory(path.FullName, dstPath + @"\" + path.Name);
                    else
                        File.Copy(path.FullName, dstPath + @"\" + path.Name);
            }
            catch { }
        }

    }
}
