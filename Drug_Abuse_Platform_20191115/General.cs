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
}