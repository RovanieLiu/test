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
using System.Threading;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace VoiceEmotionAnalyser
{
    /// <summary>
    /// 用于写.wav文件的类
    /// </summary>
    public class WavWriter
    {
        public string path;
        public DataChunkFormat format;

        private FileStream fStream;
        private BinaryWriter writer;
        private int dataChunkSize = 0;      // data chunk字节数

        private bool isWriting = false;

        /// <summary>
        /// 创建一个完整的.wav文件
        /// </summary>
        /// <param name="format"></param>
        /// <param name="path"></param>
        /// <param name="samples"></param>
        public static void CreatWavFile(DataChunkFormat format, string path, short[] samples)
        {
            WavWriter writer = new WavWriter(format, path);
            writer.Write(samples);
            writer.FinishWriting();
        }

        /// <summary>
        /// 创建一个完整的.wav文件
        /// </summary>
        /// <param name="format"></param>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        public static void CreatWavFile(DataChunkFormat format, string path, byte[] bytes)
        {
            WavWriter writer = new WavWriter(format, path);
            writer.Write(bytes);
            writer.FinishWriting();
        }

        /// <summary>
        /// WavWriter类构造函数
        /// </summary>
        /// <param name="format">.wav文件的参数</param>
        /// <param name="path">文件保存路径</param>
        public WavWriter(DataChunkFormat format, string path)
        {
            StartWriting(format, path);
        }

        /// <summary>
        /// 创建指定格式.wav文件并写入文件头
        /// </summary>
        /// <param name="format">音频格式</param>
        /// <param name="path">保存路径</param>
        public void StartWriting(DataChunkFormat format, string path)
        {
            this.format = format;
            this.path = path;
            dataChunkSize = 0;

            string outputDir = path.Substring(0, path.LastIndexOf(@"\"));
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            if (File.Exists(path))
                File.Delete(path);

            char[] ChunkRiff = { 'R', 'I', 'F', 'F' };
            char[] ChunkType = { 'W', 'A', 'V', 'E' };
            char[] ChunkFmt = { 'f', 'm', 't', ' ' };
            char[] ChunkData = { 'd', 'a', 't', 'a' };
            short shPad = 1;                // File padding
            int nFormatChunkLength = 0x10;  // Format chunk length
            int nLength = 0;                // File length, minus first 8 bytes of RIFF description. This will be filled in later.
            short BytesPerSample = 0;
            if (8 == format.BitsPerSample && 1 == format.ChannelNum)
            {
                BytesPerSample = 1;
            }
            else if ((8 == format.BitsPerSample && 2 == format.ChannelNum) || (16 == format.BitsPerSample && 1 == format.ChannelNum))
            {
                BytesPerSample = 2;
            }
            else if (16 == format.BitsPerSample && 2 == format.ChannelNum)
            {
                BytesPerSample = 4;
            }
            else
            {
                MessageBox.Show("音频格式错误");
                return;
            }

            fStream = new FileStream(path, FileMode.Create);
            writer = new BinaryWriter(fStream);
            isWriting = true;

            /// RIFF Chunk
            writer.Write(ChunkRiff);
            writer.Write(nLength);
            writer.Write(ChunkType);
            /// Wav Chunk
            writer.Write(ChunkFmt);
            writer.Write(nFormatChunkLength);
            writer.Write(shPad);
            writer.Write(format.ChannelNum);
            writer.Write(format.SamplesPerSecond);
            writer.Write(format.BytesPerSecond);
            writer.Write(BytesPerSample);
            writer.Write(format.BitsPerSample);
            /// Data Chunk
            writer.Write(ChunkData);
            writer.Write(0);
        }

        /// <summary>
        /// 写入一个Int16型采样点
        /// </summary>
        /// <param name="sample">要写入的样本</param>
        public void WriteSample(short sample)
        {
            writer.Write(sample);
            dataChunkSize += 2;
        }

        /// <summary>
        /// 写入一段Int16型数据
        /// </summary>
        /// <param name="samples"></param>
        public void Write(short[] samples)
        {
            if (format.BitsPerSample != 16)
                return;

            for(int i = 0; i < samples.Length; i++)
                writer.Write(samples[i]);
            dataChunkSize += 2 * samples.Length;
        }

        /// <summary>
        /// 写入一段Byte型数据
        /// </summary>
        /// <param name="sample">要写入的样本</param>
        public void Write(byte[] bytes)
        {
            writer.Write(bytes, 0, bytes.Length);
            dataChunkSize += bytes.Length;
        }

        /// <summary>
        /// 结束写wav文件
        /// </summary>
        public void FinishWriting()
        {
            if (isWriting)
            {
                /// 向wav文件中写入文件总大小
                writer.Seek(4, SeekOrigin.Begin);
                writer.Write((dataChunkSize + 36));

                /// 向wav文件中写入data chunk大小  
                writer.Seek(40, SeekOrigin.Begin);
                writer.Write(dataChunkSize);

                /// 释放文件
                writer.Close();
                fStream.Close();

                isWriting = false;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~WavWriter()
        {
            FinishWriting();
        }

    }
}