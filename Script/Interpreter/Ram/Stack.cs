using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    /// <summary>
    /// GVM的栈模块
    /// </summary>
    public sealed class Stack
    {

        private int[] buffer;
        private int pointer;
        private int last;

        public Stack(int size)
        {
            buffer = new int[size];
            pointer = 0;
        }

        /// <summary>
        /// 往栈压入数据,栈指针加一
        /// </summary>
        /// <param name="data">数据</param>
        public void push(int data)
        {
            last = data;
            buffer[pointer++] = data;
        }

        /// <summary>
        /// 弹出一个数据,指针减一
        /// </summary>
        /// <returns></returns>
        public int pop()
        {
            last = buffer[--pointer];
            return last;
        }

        /// <summary>
        /// 读取距栈顶offset的数据,但不改变栈指针.
        /// peek(-1) ==pop(),而peek(-2)的值与pop()后的pop()相同,余类推.
        /// </summary>
        /// <param name="offset">欲读数据位置相对栈顶的偏移量</param>
        /// <returns></returns>
        public int peek(int offset)
        {
            return buffer[pointer + offset];
        }

        /// <summary>
        /// 最近一次弹出|压入的值
        /// </summary>
        /// <returns></returns>
        public int lastValue()
        {
            return last;
        }

        /// <summary>
        /// 得到栈指针,其值与当前栈中数据个数相同
        /// </summary>
        /// <returns></returns>
        public int getPointer()
        {
            return pointer;
        }

        /// <summary>
        /// 得到此stack的大小(单位int)
        /// </summary>
        /// <returns></returns>
        public int size()
        {
            return buffer.Length;
        }

        /// <summary>
        /// 修改栈指针,修改后的指针值等于getPointer()+offset
        /// </summary>
        /// <param name="offset">偏移值</param>
        /// <returns>修改后的pointer值</returns>
        public int movePointer(int offset)
        {
            pointer += offset;
            return pointer;
        }

        /// <summary>
        /// 栈指针归零
        /// </summary>
        public void clear()
        {
            pointer = 0;
        }
    }
}
