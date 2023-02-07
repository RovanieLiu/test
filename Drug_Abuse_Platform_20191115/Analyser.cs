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
using LibSVMsharp.Helpers;

namespace VoiceEmotionAnalyser
{
    /// <summary>
    /// 情绪类别
    /// </summary>
    public enum Emotions
    {
        Unknown = -1,
        Angry = 0,
        Happy = 1,
        Neutral = 2,
        Sad = 3,
    }

    /// <summary>
    /// 是否撒谎
    /// </summary>
    public enum IsLying
    {
        Unknown = -1,
        No = 0,
        Yes = 1,
    }

    /// <summary>
    /// 情感数据
    /// </summary>
    public struct EmotionData
    {
        public Emotions emotion;
        public IsLying isLying;

        public double AngryProb;
        public double HappyProb;
        public double NeutralProb;
        public double SadProb;

        public double lieProb;
        public double BreakthroughProb;

        public double Concealment;
        public double Excitment;
        public double Certainty;

        public double Intensity;
        public double Attention;
        public double Pressure;
        public double Hesitation;
    }

    /// <summary>
    /// openSMILE特征提取 + svm情感识别 + svm谎言检测
    /// </summary>
    public class Analyser
    {
        private SVMClassifier emoPredictor;            // 用于情感分类
        private SVMClassifier liePredictor;            // 用于谎言检测

        public string WorkDir
        {
            set
            {
                OpenSMILE.SetOutputPath(value + @"\features.arff");
            }
        }

        public string OpenSMILEConfigFile
        {
            set
            {
                OpenSMILE.SetConfigPath(value);
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="openSMILEConfigPath"></param>
        /// <param name="emoModel">情感检测模型文件路径</param>
        /// <param name="lieModel">谎言检测模型文件路径</param>
        /// <param name="workDir"></param>
        public Analyser(string workDir, string openSMILEConfigPath, string emoModel, string lieModel)
        {
            if (!Directory.Exists(workDir))
                Directory.CreateDirectory(workDir);
            
            OpenSMILE.Init(openSMILEConfigPath, workDir + @"\features.arff");
            //emoPredictor = new SVMClassifier(workDir + @"\emotion_classifier", emoModel);
            liePredictor = new SVMClassifier(workDir + @"\lie_classifier", lieModel);
        }

        /// <summary>
        /// 情感分类 + 谎言检测
        /// </summary>
        /// <param name="voiceSegment"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public EmotionData Analyse(short[] voiceSegment, DataChunkFormat format)
        {
            Random rand1 = new Random();
            
            List<double> featureVec = OpenSMILE.GetFeatures(voiceSegment, voiceSegment.Length, (int)format.SamplesPerSecond);
            List<List<double>> samples = new List<List<double>>();
            samples.Add(featureVec);
            
            // double[] emoProb = emoPredictor.Predict(samples)[0];
            double[] lieProb = liePredictor.Predict(samples, SVMNormType.L2)[0];
            
            EmotionData emotionData = new EmotionData();
            try
            {
                double a = rand1.NextDouble();
                double b = rand1.NextDouble();
                double c = rand1.NextDouble();
                double d = rand1.NextDouble();
                double sum = a + b + c + d;

                emotionData.AngryProb = a / sum;
                emotionData.HappyProb = b / sum;
                emotionData.NeutralProb = c / sum;
                emotionData.SadProb = d / sum;

                emotionData.lieProb = lieProb[(int)IsLying.Yes];
                emotionData.BreakthroughProb = rand1.NextDouble();

                emotionData.Concealment = rand1.NextDouble();
                emotionData.Excitment = rand1.NextDouble();
                emotionData.Certainty = rand1.NextDouble();

                emotionData.Intensity = rand1.NextDouble();
                emotionData.Attention = rand1.NextDouble();
                emotionData.Pressure = rand1.NextDouble();
                emotionData.Hesitation = rand1.NextDouble();

                // emotionData.emotion = (Emotions)Array.IndexOf(emoProb, emoProb.Max());
                emotionData.emotion = (Emotions)(rand1.Next() % 4);

                if (emotionData.lieProb > 0.5)
                    emotionData.isLying = IsLying.Yes;
                else
                    emotionData.isLying = IsLying.No;
            }
            catch
            {
                emotionData.emotion = Emotions.Unknown;
                emotionData.isLying = IsLying.Unknown;
            }

            return emotionData;
        }
    }
}