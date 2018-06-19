using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    public class RuntimeRam : Ram
    {

        private int startAddr, regionStartAddr, regionEndAddr;
        private sbyte[] buffer;

        public RuntimeRam(int size)
        {
            buffer = new sbyte[size];
        }

        /**
         * 得到当前所在函数使用内存区域的起始地址
         * @return startAddr
         */
        public int getRegionStartAddr()
        {
            return regionStartAddr;
        }

        /**
         * 设置当前函数使用内存的起始地址
         * @param addr 起始地址
         */
        public void setRegionStartAddr(int addr)
        {
            regionStartAddr = addr;
        }

        /**
         * 得到当前所在函数使用内存区域的结束地址(不包括)
         * @return startAddr
         */
        public int getRegionEndAddr()
        {
            return regionEndAddr;
        }

        /**
         * 设置当前函数使用内存的结束地址
         * @param addr 起始地址
         */
        public void setRegionEndAddr(int addr)
        {
            regionEndAddr = addr;
        }

        public int size()
        {
            return buffer.Length;
        }

        public int getRamType()
        {
            return RamConst.RAM_RUNTIME_TYPE;
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
