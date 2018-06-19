using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    /// <summary>
    /// 
    /// </summary>
    public class StringRam : Ram
    {
        private sbyte[] buffer;
        private int offset, startAddr;

        public StringRam(int size)
        {
            buffer = new sbyte[size];
            offset = 0;
        }


        /// <summary>
        /// 从lav文件中读取一个以0结尾的字符串数组
        /// </summary>
        /// <param name="source">数据源</param>
        /// <returns>这个数据保存在stringRam中的地址</returns>
        public int addstring(LavApp source)
        {
            int addr = offset + startAddr;
            sbyte b = 0;
            do
            {
                b = (sbyte)source.getChar();
                buffer[offset++] = b;
            } while (b != 0);
            if (offset >= buffer.Length * 3 / 4)
            {
                offset = 0;
            }
            return addr;
        }

        /// <summary>
        /// 该Ram不允许直接写内存,只能通过addstring()方法想里面写数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        public void setByte(int addr, sbyte data)
        {
            //throw new IndexOutOfBoundsException("常字符串不能修改: " + addr);
            throw new IndexOutOfRangeException("常字符串不能修改: " + addr);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int size()
        {
            return buffer.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getRamType()
        {
            return RamConst.RAM_string_TYPE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getStartAddr()
        {
            return startAddr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        public void setStartAddr(int addr)
        {
            startAddr = addr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public sbyte getByte(int addr)
        {
            return buffer[addr - startAddr];
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            offset = 0;
        }
    }
}
