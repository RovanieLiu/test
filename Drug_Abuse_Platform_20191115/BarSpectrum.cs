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
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace BarSpectrumcs
{
    /// <summary>
    /// 由小方块组成的柱状频谱
    /// </summary>
    class BarSpectrum
    {
        /// <summary>
        /// 画布
        /// </summary>
        Graphics mGraphics;
        /// <summary>
        /// 图形的尺寸
        /// </summary>
        Size mChartSize;
        /// <summary>
        /// 储存各柱形
        /// </summary>
        BlockBar[] mBars;
        /// <summary>
        /// 上限值对应的方块数
        /// </summary>
        int mMaxBlockPerBar;
        /// <summary>
        /// 柱形的宽度
        /// </summary>
        int mBarWidth = 30;
        /// <summary>
        /// 柱形间距
        /// </summary>
        int mBarInterval;
        /// <summary>
        /// 一个方块的高度
        /// </summary>
        int mBlockHeight = 6;
        /// <summary>
        /// 上下两方块间距
        /// </summary>
        int mBlockInterval = 1;

        /// <summary>
        /// 绘制柱状频谱
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="chartSize">图形大小</param>
        /// <param name="spectrum">要绘制的频谱数据</param>
        public void Paint(Graphics g, Size chartSize, double[] spectrum)
        {
            mGraphics = g;
            mChartSize = chartSize;

            int barNum = mChartSize.Width / mBarWidth;
            mBars = new BlockBar[barNum];
            mBarInterval = (int)Math.Round((double)(mChartSize.Width - barNum * mBarWidth) / (barNum + 1));
            mMaxBlockPerBar = mChartSize.Height / (mBlockHeight + mBlockInterval);

            /// 将频谱值映射为各柱状图的高度
            double[] value = new double[barNum];
            int samplePerBar = (int)(spectrum.Length / barNum);     // 每个柱形对应的频谱点数
            for (int i = 0; i < barNum; i++)
            {
                value[i] = 0;
                if (samplePerBar != 0)
                {
                    for (int j = 0; j < samplePerBar; j++)
                    {
                        value[i] += spectrum[i * samplePerBar + j];
                    }
                    value[i] /= samplePerBar;
                }
            }
            
            int[] blockNum = new int[barNum];
            for (int i = 0; i < barNum; i++)
            {
                blockNum[i] = (int)(value[i] * mChartSize.Height / mBlockHeight);
            }

            /// 如果过高则整体放缩
            if(blockNum.Max() > mMaxBlockPerBar)
            {
                double scaleFactor = blockNum.Max() / mMaxBlockPerBar;
                for (int i = 0; i < barNum; i++)
                {
                    blockNum[i] = (int)(blockNum[i] / scaleFactor);
                }
            }

            for (int i = 0; i < barNum; i++)
            {
                mBars[i].blocks = new Block[blockNum[i]];

                /// 计算各方块的位置和颜色
                int x = i * (mBarWidth + mBarInterval) + mBarInterval;
                for (int j = 0; j < blockNum[i]; j++)
                {
                    mBars[i].blocks[j].left = x;
                    mBars[i].blocks[j].right = mBars[i].blocks[j].left + mBarWidth;
                    mBars[i].blocks[j].top = mChartSize.Height - (j + 1) * (mBlockHeight + mBlockInterval);
                    mBars[i].blocks[j].bottom = mBars[i].blocks[j].top + mBlockHeight;

                    mBars[i].blocks[j].backColor = GetColor(mBars[i].blocks[j].top);
                }

                PaintBar(mBars[i]);
            }
        }

        /// <summary>
        /// 绘制条形
        /// </summary>
        /// <param name="bar">要绘制的条形</param>
        void PaintBar(BlockBar bar)
        {
            for(int i = 0; i < bar.blocks.Length; i++)
            {
                PaintBlock(bar.blocks[i]);
            }
        }

        /// <summary>
        /// 绘制方块
        /// </summary>
        /// <param name="block">要绘制的方块</param>
        void PaintBlock(Block block)
        {
            Brush b = new SolidBrush(block.backColor);
            mGraphics.FillPath(b, GetGraphicPath(block.left, block.right, block.top, block.bottom));
        }

        /// <summary>
        /// 根据坐标给出颜色
        /// </summary>
        /// <param name="y">某点的坐标</param>
        /// <returns>该点的颜色</returns>
        Color GetColor(int y)
        {
            if(y < 0)
            {
                y = 0;
            }
            else if(y > mChartSize.Height)
            {
                y = mChartSize.Height;
            }

            int red;
            int green;

            green = (int)(511 * (double)y / mChartSize.Height);
            red = 511 - green;

            if(red > 255)
            {
                red = 255;
            }
            if (green > 255)
            {
                green = 255;
            }

            return Color.FromArgb(200, red, green, 50);
        }

        /// <summary>
        /// 根据矩形四条边的位置返回矩形的边框
        /// </summary>
        /// <param name="left">矩形左边缘的x坐标</param>
        /// <param name="right">矩形右边缘的x坐标</param>
        /// <param name="top">矩形上边缘的y坐标</param>
        /// <param name="bottom">矩形下边缘的y坐标</param>
        /// <returns>矩形的边框</returns>
        static GraphicsPath GetGraphicPath(int left, int right, int top, int bottom)
        {
            Point topLeft = new Point(left, top);
            Point topRight = new Point(right, top);
            Point bottomLeft = new Point(left, bottom);
            Point bottomRight = new Point(right, bottom);

            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(topLeft, topRight);
            gp.AddLine(topRight, bottomRight);
            gp.AddLine(bottomRight, bottomLeft);
            gp.AddLine(bottomLeft, topLeft);

            return gp;
        }
    }

    /// <summary>
    /// 由方块构成的柱形
    /// </summary>
    struct BlockBar
    {
        /// <summary>
        /// 储存构成柱形的所有方块
        /// </summary>
        public Block[] blocks;
    }

    /// <summary>
    /// 表示一个矩形方块
    /// </summary>
    struct Block
    {
        /// <summary>
        /// 方块左边框x坐标
        /// </summary>
        public int left;
        /// <summary>
        /// 方块右边框x坐标
        /// </summary>
        public int right;
        /// <summary>
        /// 方块上边框y坐标
        /// </summary>
        public int top;
        /// <summary>
        /// 方块下边框y坐标
        /// </summary>
        public int bottom;
        /// <summary>
        /// 方块颜色
        /// </summary>
        public Color backColor;
    }
}
