using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    /// <summary>
    /// 使用一个外部byte数组作为数据存储的Ram,注意:外部修改此byte数组会影响到此Ram
    /// </summary>
    public sealed class ByteArrayRam : RelativeRam
    {

        private sbyte[] buffer;
        private ScreenModel screen;
        private int startAddr;

        public ByteArrayRam(sbyte[] buffer, ScreenModel screen)
        {
            this.buffer = buffer;
            this.screen = screen;
        }

        public ScreenModel getScreenModel()
        {
            return screen;
        }

        public int size()
        {
            return buffer.Length;
        }

        public int getRamType()
        {
            return RamConst.RAM_TEXT_TYPE;
        }

        public int getStartAddr()
        {
            return startAddr;
        }

        public void setStartAddr(int addr)
        {
            startAddr = addr;
        }

        public sbyte getByte(int addr)
        {
            return buffer[addr - startAddr];
        }

        public void setByte(int addr, sbyte data)
        {
            buffer[addr - startAddr] = data;
        }

        public void clear()
        {
            for (int index = buffer.Length - 1; index >= 0; index--)
            {
                buffer[index] = 0;
            }
        }
    }
}
