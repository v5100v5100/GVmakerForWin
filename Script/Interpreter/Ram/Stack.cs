using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    /// <summary>
    /// GVM的栈模块
    /// </summary>
    public readonly class Stack
    {

        private int[] buffer;
        private int pointer;
        private int last;

        public Stack(int size)
        {
            buffer = new int[size];
            pointer = 0;
        }

        /**
         * 往栈压入数据,栈指针加一
         * @param data 数据
         */
        public void push(int data)
        {
            last = data;
            buffer[pointer++] = data;
        }

        /**
         * 弹出一个数据,指针减一
         * @return data
         */
        public int pop()
        {
            last = buffer[--pointer];
            return last;
        }

        /**
         * 读取距栈顶offset的数据,但不改变栈指针.
         * peek(-1) ==pop(),而peek(-2)的值与pop()后的pop()相同,余类推.
         * @param offset 欲读数据位置相对栈顶的偏移量
         * @return data
         */
        public int peek(int offset)
        {
            return buffer[pointer + offset];
        }

        /**
         * 最近一次弹出|压入的值
         * @return lastValue
         */
        public int lastValue()
        {
            return last;
        }

        /**
         * 得到栈指针,其值与当前栈中数据个数相同
         * @return pointer
         */
        public int getPointer()
        {
            return pointer;
        }

        /**
         * 得到此stack的大小(单位int)
         * @return size
         */
        public int size()
        {
            return buffer.Length;
        }

        /**
         * 修改栈指针,修改后的指针值等于getPointer()+offset
         * @param offset 偏移值
         * @return 修改后的pointer值
         */
        public int movePointer(int offset)
        {
            pointer += offset;
            return pointer;
        }

        /**
         * 栈指针归零
         */
        public void clear()
        {
            pointer = 0;
        }
    }
}
