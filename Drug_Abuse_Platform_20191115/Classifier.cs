using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using LibSVMsharp;
using LibSVMsharp.Helpers;
using LibSVMsharp.Extensions;

namespace VoiceEmotionAnalyser
{
    class SVMClassifier
    {
        private string txtPath;
        private SVMModel model;

        /// <summary>
        /// 改变工作目录
        /// </summary>
        public string WorkDir
        {
            set
            {
                txtPath = value + @"\features.txt";
            }
        }

        /// <summary>
        /// 加载指定模型
        /// </summary>
        public string Model
        {
            set
            {
                LoadModel(value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workDir">存放中间文件的目录</param>
        /// <param name="modelFile"></param>
        public SVMClassifier(string workDir, string modelFile)
        {
            WorkDir = workDir;
            Model = modelFile;
        }

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="modelFile"></param>
        private void LoadModel(string modelFile)
        {
            try
            {
                model = SVM.LoadModel(modelFile);
            }
            catch (IOException)
            {
                MessageBox.Show("模型文件缺失");
            }
        }

        /// <summary>
        /// 分类
        /// </summary>
        /// <param name="featureFile"></param>
        /// <returns></returns>
        public List<double[]> Predict(List<List<double>> samples)
        {
            if (File.Exists(txtPath))
                File.Delete(txtPath);
            GenerateSVMInputFile(samples, txtPath);
            SVMProblem testSet = SVMProblemHelper.Load(txtPath);
            List<double[]> lieProbMat = new List<double[]>();
            testSet.PredictProbability(model, out lieProbMat);
            return lieProbMat;
        }

        /// <summary>
        /// 分类
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="normType">归一化方式</param>
        /// <returns></returns>
        public List<double[]> Predict(List<List<double>> samples, SVMNormType normType)
        {
            if (File.Exists(txtPath))
                File.Delete(txtPath);
            GenerateSVMInputFile(samples, txtPath);
            SVMProblem testSet = SVMProblemHelper.Load(txtPath);
            testSet = testSet.Normalize(normType);
            List<double[]> lieProbMat = new List<double[]>();
            testSet.PredictProbability(model, out lieProbMat);
            return lieProbMat;
        }

        /// <summary>
        /// 生成SVM的输入文件
        /// </summary>
        /// <param name="featureVectors"></param>
        /// <param name="path"></param>
        private void GenerateSVMInputFile(List<List<double>> featureVectors, string path)
        {
            string outputDir = path.Substring(0, path.LastIndexOf(@"\"));
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            for (int i = 0; i < featureVectors.Count; i++)
            {
                featureVectors[i].Insert(0, i + 1);                 // 在开头插入样本编号

                using (StreamWriter outputStream = new StreamWriter(path, true, Encoding.UTF8))
                {
                    outputStream.Write(featureVectors[i][0]);
                    outputStream.Write(" ");
                    for (int j = 1; j < featureVectors[i].Count - 1; j++)
                    {
                        outputStream.Write(j);
                        outputStream.Write(":");
                        outputStream.Write(featureVectors[i][j]);
                        outputStream.Write(" ");
                    }
                    outputStream.Write("\r\n");
                }
            }
        }

    }
}
