using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    public sealed class stringRam : Ram
    {
        private byte[] buffer;
        private int offset, startAddr;

        public stringRam(int size)
        {
            buffer = new byte[size];
            offset = 0;
        }

        /**
         * 从lav文件中读取一个以0结尾的字符串数组
         * @param source 数据源
         * @return addr 这个数据保存在stringRam中的地址
         */
        public int addstring(LavApp source)
        {
            int addr = offset + startAddr;
            byte b =0;
            do
            {
                b = (byte)source.getChar();
                buffer[offset++] = b;
            } while (b != 0);
            if (offset >= buffer.Length * 3 / 4)
            {
                offset = 0;
            }
            return addr;
        }

        /**
         * 该Ram不允许直接写内存,只能通过addstring()方法想里面写数据
         * @throws IndexOutOfBoundsException 调用此方法总是抛出该异常
         * @see #addstring(LavApp)
         */
        public void setByte(int addr, byte data)
        {
            //throw new IndexOutOfBoundsException("常字符串不能修改: " + addr);
            throw new IndexOutOfRangeException("常字符串不能修改: " + addr);
        }

        /**
         * {@inheritDoc}
         */
        public int size()
        {
            return buffer.Length;
        }

        /**
         * {@inheritDoc}
         */
        public int getRamType()
        {
            return RamConst.RAM_string_TYPE;
        }

        /**
         * {@inheritDoc}
         */
        public int getStartAddr()
        {
            return startAddr;
        }

        /**
         * {@inheritDoc}
         */
        public void setStartAddr(int addr)
        {
            startAddr = addr;
        }

        /**
         * {@inheritDoc}
         */
        public byte getByte(int addr)
        {
            return buffer[addr - startAddr];
        }

        /**
         * {@inheritDoc}
         */
        public void clear()
        {
            offset = 0;
        }
    }
}
