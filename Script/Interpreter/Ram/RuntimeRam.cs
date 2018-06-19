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

        /// <summary>
        /// 得到当前所在函数使用内存区域的起始地址
        /// </summary>
        /// <returns></returns>
        public int getRegionStartAddr()
        {
            return regionStartAddr;
        }

        /// <summary>
        /// 设置当前函数使用内存的起始地址
        /// </summary>
        /// <param name="addr">起始地址</param>
        public void setRegionStartAddr(int addr)
        {
            regionStartAddr = addr;
        }

        /// <summary>
        /// 得到当前所在函数使用内存区域的结束地址(不包括)
        /// </summary>
        /// <returns></returns>
        public int getRegionEndAddr()
        {
            return regionEndAddr;
        }

        /// <summary>
        /// 设置当前函数使用内存的结束地址
        /// </summary>
        /// <param name="addr">起始地址</param>
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

        public void Clear()
        {
            for (int index = buffer.Length - 1; index >= 0; index--)
            {
                buffer[index] = 0;
            }
        }
    }
}
