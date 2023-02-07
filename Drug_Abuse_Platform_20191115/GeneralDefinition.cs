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

namespace VoiceEmotionAnalyser
{
    
    /// <summary>
    /// 音频编码格式
    /// </summary>
    public enum EncodingFormat
    {
        Pcm = 1,
    }

    /// <summary>
    /// .wav文件参数结构体
    /// </summary>
    public struct DataChunkFormat
    {
        /// <summary>
        /// 声道数
        /// </summary>
        public ushort ChannelNum { get; set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public uint SamplesPerSecond { get; set; }

        /// <summary>
        /// 量化位数
        /// </summary>
        public ushort BitsPerSample { get; set; }

        /// <summary>
        /// 每秒字节数
        /// </summary>
        public uint BytesPerSecond { get; set; }

        /// <summary>
        /// 采样帧大小
        /// </summary>
        public ushort BytesPerSample { get; set; }

        /// <summary>
        /// 编码格式
        /// </summary>
        public EncodingFormat EncodingFormat { get; set; }

    }

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
        Yes = 0,
        No = 1,
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
    /// 某个音频段落包含的所有数据
    /// </summary>
    public struct DataPackage
    {
        public DateTime beginTime;
        public DateTime endTime;
        public int beginSampleIndex;
        public int endSampleIndex;
        public EmotionData emotionData;

        public string remark;           // 备注信息
    }

}