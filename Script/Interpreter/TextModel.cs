using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    /// <summary>
    /// 文本输出模式的操作接口 <p>
    /// 一个textmodel负责维持文本缓冲区的内容以及文本缓冲区的输出指针<p>
    /// 其内容的绘制需要调用相应的ScreenModel<p>
    /// 对于该类的大部分方法，应该在使用setScreenModel方法设置好ScreenModel后调用<p>
    /// 该接口保留了对文本缓冲区关联RAM的可选实现
    /// </summary>
    public class TextModel
    {
        private ScreenModel screen;
        private Renderable render;
        private int maxRow,  maxCol;
        private int curRow,  curCol;
        private bool isBigMode;
        private byte[] buffer;
        private Getable getter;
        private RelativeRam ram;

        public TextModel() {
            //..
        }


        /// <summary>
        /// 设置用于显示的ScreenModel,并作适当的初始化
        /// </summary>
        /// <param name="screen"></param>
        public void setScreenModel(ScreenModel screen) {
            if (screen == null) {
                //throw new IllegalArgumentException("Screen must't be null!");
            }
            if (this.screen != screen) {
                this.screen = screen;
                this.buffer = new byte[(screen.getWidth() / 6) * (screen.getHeight() / 13)];
                this.getter = new ByteArrayGetter(buffer);
                this.ram = new ByteArrayRam(buffer, screen);
                this.render = screen.getRender();
            }
            else {
                ram.clear();
            }
            this.curCol = this.curRow = 0;
            this.isBigMode = true;
            this.maxCol = screen.getWidth() / 8;
            this.maxRow = screen.getHeight() / 16;
        }

        /// <summary>
        /// 得到用于显示的ScreenModel
        /// </summary>
        /// <returns></returns>
        public ScreenModel getScreenModel() {
            return screen;
        }

        /// <summary>
        /// 文本缓冲区是否有关联的Ram
        /// </summary>
        /// <returns>总是true</returns>
        public bool hasRelativeRam() {
            return true;
        }

        /// <summary>
        /// 得到与该文本缓冲区相关联的Ram,可以将其安装到RamManager中,以使得LAVA程序能够直接访问到文本缓冲区
        /// </summary>
        /// <returns>关联的Ram,该Ram内容与文本缓冲区保持同步变化</returns>
        /// IllegalStateException 如果hasRelativeRam()方法返回false
        public RelativeRam getTextRam() {
            return ram;
        }

        /// <summary>
        /// 往文本缓冲区添加一个gb2312编码的字符,不刷新到屏幕
        /// </summary>
        /// <param name="c"></param>
        public void addChar(sbyte c) {
            if (curRow >= maxRow) {
                //如果已经超出屏幕,将文本缓冲区内容上移一行
                textMoveUp();
            }
            if (c > 0xff) {
                //如果是一个gb2312字符
                if (curCol + 1 >= maxCol) {
                    //空位不足,转下一行
                    buffer[maxCol * curRow + curCol] = (byte) 0x20;
                    curCol = 0;
                    curRow++;
                    if (curRow >= maxRow) {
                        textMoveUp();
                    }
                }
                buffer[maxCol * curRow + curCol] = (byte) c;
                curCol++;
                buffer[maxCol * curRow + curCol] = (byte) (c >>> 8);
                curCol++;
                if (curCol >= maxCol) {
                    curCol = 0;
                    curRow++;
                }
                return;
            }
            //是一个单字节字符
            switch (c) {
                case 0x0a:
                    curCol = 0;
                    curRow++;
                    if (curRow >= maxRow) {
                        textMoveUp();
                    }
                    break;
                case 0x0d:
                    break;
                default:
                    buffer[maxCol * curRow + curCol] = (byte) c;
                    curCol++;
                    if (curCol >= maxCol) {
                        curCol = 0;
                        curRow++;
                    }
                    break;
            }

        }

        /**
         * 刷新屏幕,只考虑低八位<p>
         * 从高到低控制屏幕的每一行，0表示该行更新，1表示该行不更新<p>
         * 当m为0时刷新全部文本缓冲区到屏幕<p>
         * @param m 刷新模式
         */
        public void updateLCD(int m) {
            m &= 0xff;
            //不需要刷新
            if (m == 0xff) {
                return;
            }
            int ox = isBigMode ? 0 : 1;
            int oy = isBigMode ? 0 : 1;
            int dy = isBigMode ? 16 : 13;
            int drawMode = render.DRAW_COPY_TYPE | render.RENDER_GRAPH_TYPE;
            if (isBigMode) {
                drawMode |= render.TEXT_BIG_TYPE;
            }
            if (m == 0) {
                //刷新所有
                render.setDrawMode(render.DRAW_CLEAR_TYPE | render.RENDER_GRAPH_TYPE | render.RENDER_FILL_TYPE);
                render.drawRect(0, 0, screen.getWidth(), screen.getHeight());
                render.setDrawMode(drawMode);
                for (int row = 0; row < maxRow; row++) {
                    render.drawString(ox, oy + row * dy, getter, row * maxCol, maxCol);
                }
            }
            else {
                render.setDrawMode(drawMode);
                for (int row = 0; row < maxRow; row++) {
                    if ((m & 0x80) == 0) {
                        //刷新该行
                        render.drawString(ox, oy + row * dy, getter, row * maxCol, maxCol);
                    }
                    m <<= 1;
                }
            }
            screen.fireScreenChanged();
        }

        public void setLocation(int row, int col) {
            if (row >= 0 && row < maxRow) {
                curRow = row;
            }
            if (col >= 0 && col < maxCol) {
                curCol = col;
            }
        }

        /**
         * 清除文本缓冲区并设置字体,即SetScreen
         * @param mode mode==0为大字体,否则为小字体
         */
        public void setTextMode(int mode) {
            ram.clear();

            curRow = curCol = 0;
            isBigMode = (mode == 0);
            if (isBigMode) {
                maxCol = screen.getWidth() / 8;
                maxRow = screen.getHeight() / 16;
            }
            else {
                maxCol = screen.getWidth() / 6;
                maxRow = screen.getHeight() / 13;
            }
        }

        /**
         * 将文本缓冲区内容整体上移一行,curRow--
         */
        private void textMoveUp() {
            if (curRow <= 0) {
                return;
            }
            int index = 0;
            while (index < maxCol * maxRow - maxCol) {
                buffer[index] = buffer[index + maxCol];
                index++;
            }
            while (index < maxCol * maxRow) {
                buffer[index++] = (byte) 0;
            }
            curRow--;
        }
    }
}
