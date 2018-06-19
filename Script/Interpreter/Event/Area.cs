using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Event
{
    /// <summary>
    /// 用于描述一个矩形区域的类,该类为immutable<p>
    /// 当getWidth()或getHeight()有一个不大于0时,表示一个空的区域
    /// </summary>
    public class Area
    {

        /// <summary>
        /// 一个Area常量，其x,y,width,height都为0
        /// </summary>
        static Area EMPTY_AREA = new Area(0, 0, 0, 0);
        private int x,  y,  width,  height;
        private bool empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">区域起始x值</param>
        /// <param name="y">区域起始y值</param>
        /// <param name="width">区域的宽度</param>
        /// <param name="height">区域的高度</param>
        public Area(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.empty = (width <= 0 || height <= 0);
        }

        /// <summary>
        /// 判断这个Area是否为空
        /// </summary>
        /// <returns></returns>
        public bool isEmpty() 
        {
            return empty;
        }

        public override string ToString()
        {
            return "[x=" + x + ",y=" + y + ",width=" + width + ",height=" + height + "]";
        }

        public int getHeight() 
        {
            return height;
        }

        public int getWidth()
        {
            return width;
        }

        public int getY() 
        {
            return y;
        }

        public int getX() 
        {
            return x;
        }
    }
}
