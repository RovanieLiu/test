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
using System.Runtime.InteropServices;
using webrtc_vad;

namespace VoiceEmotionAnalyser
{
    public struct VadData
    {
        public short[] data;
        public DateTime beginTime;
        public DateTime endTime;
        public int beginSampleIndex;
        public int endSampleIndex;
    }

    /// <summary>
    /// 调用webrtc_vad检测音频中有语音的部分
    /// </summary>
    public class VoiceDetector
    {
        public string workDir;                                                      // 工作目录
        public DataChunkFormat format;                                              // 音频格式
        
        public Queue<VadData> speeches = new Queue<VadData>();                      // 结果队列

        private IntPtr vadModule;                                                   // 指向webrtc组件中的vad模块的指针

        private int position = 0;                                                   // 当前输入数据的位置
        private int voiceFrameNum = 0;                                              // 当前语音段中的语音帧数
        private int nonVoiceFrameNum = 0;                                           // 当前遇到的连续非语音帧数
        private List<short> currentSegment = new List<short>();                     // 当前语音段数据

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workDir"></param>
        public VoiceDetector(string workDir, DataChunkFormat format)
        {
            InitWebrtc();

            this.workDir = workDir;
            this.format = format;
            
            DirectoryManager.CleanDirectory(workDir);
        }

        /// <summary>
        /// 初始化webrtc的vad模块
        /// </summary>
        private void InitWebrtc()
        {
            int vadStatus = vad.WebRtcVad_Create(out vadModule);
            if (vadStatus != 0)
            {
                MessageBox.Show("webrtc模块创建错误，错误代码：" + vadStatus);
            }
            vadStatus = vad.WebRtcVad_Init(vadModule);
            if (vadStatus != 0)
            {
                MessageBox.Show("webrtc模块初始化错误，错误代码：" + vadStatus);
            }
            vadStatus = vad.WebRtcVad_set_mode(vadModule, 2);
            if (vadStatus != 0)
            {
                MessageBox.Show("webrtc模块模式设置错误，错误代码：" + vadStatus);
            }
        }

        /// <summary>
        /// 从持续输入的音频中提取语音段(只支持16K采样率)，每次输入数据长度应相同
        /// </summary>
        /// <param name="format">音频格式</param>
        /// <param name="frame">一帧音频数据,长度必须是frameSize</param>
        /// <param name="position">inputData在本次录音中的位置（以点为单位）</param>
        public void Input(short[] frame)
        {
            int frameSize = frame.Length;
            position += frameSize;

            IntPtr vadInputBuffer = Marshal.AllocHGlobal(frameSize * sizeof(short));
            Marshal.Copy(frame, 0, vadInputBuffer, frameSize);
            int vadStatus = vad.WebRtcVad_Process(vadModule, (int)format.SamplesPerSecond, vadInputBuffer, frameSize);     // 检测信号中是否有语音
            Marshal.FreeHGlobal(vadInputBuffer);

            if (vadStatus == 1)                     // 当前帧为语音帧
            {
                if (nonVoiceFrameNum >= 5)
                    currentSegment.Clear();
                currentSegment.AddRange(frame);

                voiceFrameNum++;
                nonVoiceFrameNum = 0;
            }
            else if (vadStatus == 0)                // 当前帧为非语音帧
            {
                if (voiceFrameNum >= 20)
                {
                    // 若当前片段已经有不少于20个语音帧，且已连续遇到大于5个非语音帧，则认为当前片段结束，且为语音段
                    if (nonVoiceFrameNum >= 5)
                    {
                        /// 保存分析结果
                        VadData result = new VadData();
                        result.data = new short[currentSegment.Count];
                        currentSegment.CopyTo(result.data);
                        result.beginSampleIndex = position + frameSize - currentSegment.Count;
                        result.endSampleIndex = position + frameSize;
                        result.beginTime = new DateTime().AddSeconds(result.beginSampleIndex / format.SamplesPerSecond);
                        result.endTime = new DateTime().AddSeconds(result.endSampleIndex / format.SamplesPerSecond);
                        speeches.Enqueue(result);

                        WavWriter.CreatWavFile(format, workDir + @"\" + result.beginSampleIndex.ToString() + ".wav", result.data);

                        /// 重置变量以开始统计下一个语音段
                        currentSegment.Clear();
                        voiceFrameNum = 0;
                    }
                    // 若当前片段已经有不少于20个语音帧，但只连续遇到少于5个非语音帧，则认为当前片段未结束
                    else
                    {
                        currentSegment.AddRange(frame);
                    }
                }
                else
                {
                    // 若当前片段只有少于20个语音帧，但已连续遇到不少于5个非语音帧，则认为当前片段结束，且为非语音段
                    if (nonVoiceFrameNum >= 5)
                    {
                        currentSegment.Clear();
                        voiceFrameNum = 0;
                    }
                    // 若当前片段只有少于20个语音帧，但只连续遇到少于5个非语音帧，则认为当前片段未结束
                    else
                    {
                        currentSegment.AddRange(frame);
                    }
                }

                nonVoiceFrameNum++;
            }
            else                                // vad运行错误
            {
                MessageBox.Show("webrtc模块错误");
            }
        }
        
        /// <summary>
        /// 重新开始
        /// </summary>
        public void Restart()
        {
            position = 0;
            voiceFrameNum = 0;
            nonVoiceFrameNum = 0;
            speeches.Clear();
            currentSegment.Clear();
        }
        
        /// <summary>
        /// 析构函数
        /// </summary>
        ~VoiceDetector()
        {
            vad.WebRtcVad_Free(vadModule);               //释放webrtc的vad结构的vadModule
        }

    }
}