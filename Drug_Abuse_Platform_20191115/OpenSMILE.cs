using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace VoiceEmotionAnalyser
{
    /// <summary>
    /// 调用openSMILE_live.dll，进行特征提取
    /// </summary>
    public class OpenSMILE
    {
        private static IntPtr extractor = IntPtr.Zero;                              // openSMILE_live.dll中的extractor类对象指针
        private static string configFile;
        private static string featureFile;

        /// <summary>
        /// 初始化函数
        /// 注：OpenSMILE只能初始化一次，因此configFile和featureFile在创建后不可更改
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="featureFile"></param>
        public static void Init(string configFile, string featureFile)
        {
            try
            {
                OpenSMILE.configFile = configFile;
                OpenSMILE.featureFile = featureFile;
                if (extractor == IntPtr.Zero)
                    extractor = SetExtractor(configFile, featureFile);
            }
            catch (IOException)
            {
                MessageBox.Show("配置文件缺失");
            }
        }

        /// <summary>
        /// 用于修改输出文件路径，未完成，需要进一步修改openSMILE源码，导出一个接口
        /// </summary>
        /// <param name="path"></param>
        public static void SetConfigPath(string path)
        {

        }

        /// <summary>
        /// 用于重新选择配置文件，未完成，需要进一步修改openSMILE源码，导出一个接口
        /// </summary>
        /// <param name="path"></param>
        public static void SetOutputPath(string path)
        {

        }

        /// <summary>
        /// 提取输入数据的特征
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sampleNum"></param>
        /// <param name="sampleRate"></param>
        /// <returns></returns>
        public static List<double> GetFeatures(short[] data, int sampleNum, int sampleRate)
        {
            int retryTime = 0;
            CallOpenSMILE: int isError = Extract(extractor, data, sampleNum, sampleRate);
            if (isError == 1)
            {
                MessageBox.Show("openSMILE出现错误");
                return null;
            }
            List<double> featureVector = ParseArff(featureFile);

            // openSMILE有时候会出现错误，导致提取的特征为空，此时需要再次尝试
            if (featureVector.Count == 0 && retryTime < 10)
            {
                retryTime++;
                goto CallOpenSMILE;
            }

            File.Delete(featureFile);
            return featureVector;
        }

        /// <summary>
        /// 解析openSMILE产生的arff文件指定位置的特征向量
        /// </summary>
        /// <param name="index">所需特征向量在arff文件中的序号</param>
        /// <param name="arffPath"></param>
        /// <returns></returns>
        public static List<double> ParseArff(int index, string arffPath)
        {
            List<double> featureVector = null;

            using (StreamReader r = new StreamReader(arffPath))
            {
                while (!r.EndOfStream)
                {
                    if (r.ReadLine() == "@data")
                    {
                        for (int i = 0; i < index; i++)
                            r.ReadLine();

                        featureVector = new List<double>();
                        string[] elements = r.ReadLine().Split(new char[] { ',' });
                        for (int i = 1; i < elements.Length - 2; i++)                   // ???
                            featureVector.Add(Convert.ToDouble(elements[i]));

                        break;
                    }
                }
            }

            return featureVector;
        }

        /// <summary>
        /// 解析openSMILE产生的arff文件最后一个特征向量
        /// </summary>
        /// <param name="arffPath"></param>
        /// <returns></returns>
        public static List<double> ParseArff(string arffPath)
        {
            List<double> featureVector = new List<double>();

            using (StreamReader r = new StreamReader(arffPath))
            {
                string line = string.Empty;
                while (!r.EndOfStream)
                    line = r.ReadLine();
                
                string[] elements = line.Split(new char[] { ',' });
                for (int i = 1; i < elements.Length - 2; i++)                   // ???
                    featureVector.Add(Convert.ToDouble(elements[i]));
            }

            return featureVector;
        }

        #region 从openSMILE_live.dll中导入的C++函数

        /// <summary>
        /// 创建一个extractor对象
        /// </summary>
        /// <param name="configFile">配置文件路径</param>
        /// <param name="featureFile">结果保存路径</param>
        /// <returns>extractor对象指针</returns>
        /// 注：
        /// 1. dll文件应置于exe所在目录，置于system32目录下无效，原因暂时不明
        /// 2. 如不指定CallingConvention = CallingConvention.Cdecl，会引发堆栈不平衡异常
        /// 3. 由于openSMILE的问题，在整个程序的生命周期内，本函数只能执行一次
        [DllImport("SMILExtract_live.dll", EntryPoint = "setExtractor", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SetExtractor(string configFile, string featureFile);

        /// <summary>
        /// 提取语音信号的特征
        /// </summary>
        /// <param name="extractor">extractor对象指针</param>
        /// <param name="data">存放语音数据</param>
        /// <param name="sampleNum">数组data的长度</param>
        /// <param name="sampleRate">输入语音数据的采样率</param>
        /// <returns>程序运行状态 0：正确运行；1：运行出现错误</returns>
        /// 注：
        /// 1. dll文件应置于exe所在目录，置于system32目录下无效，原因暂时不明
        /// 2. 如不指定CallingConvention = CallingConvention.Cdecl，会引发堆栈不平衡异常
        [DllImport("SMILExtract_live.dll", EntryPoint = "extract", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Extract(IntPtr extractor, short[] data, int sampleNum, int sampleRate);

        #endregion

    }
}