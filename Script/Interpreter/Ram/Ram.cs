using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    public class RamConst 
    {

        /**
         * 运行时内存类型
         */
        public static int RAM_RUNTIME_TYPE = 0x01;
        /**
         * 显存内存类型
         */
        public static int RAM_GRAPH_TYPE = 0x02;
        /**
         * 屏幕缓存类型
         */
        public static int RAM_BUFFER_TYPE = 0x04;
        /**
         * 文本缓冲区类型
         */
        public static int RAM_TEXT_TYPE = 0x08;
        /**
         * 字符堆内存类型
         */
        public static int RAM_string_TYPE = 0x10;
    }

    /// <summary>
    ///  GVM各种内存模块的父接口.
    ///  注意,其某些子接口可能会限制setByte()的使用
    /// </summary>
    public interface Ram : Accessable
    {


        /**
         * 这块内存的大小,以字节数计算
         * @return size
         */
        int size();

        /**
         * 得到内存的类型,只能是RAM_RUNTIME_TYPE,RAM_GRAPH_TYPE,RAM_BUFFER_TYPE,RAM_TEXT_TYPE,RAM_string_TYPE五者者之一
         * @return type
         */
        int getRamType();

        /**
         * 这块内存的开始地址
         * @return addr
         */
        int getStartAddr();

        /**
         * 设置内存的开始地址,由RamManager调用
         * @param addr 开始地址
         */
        void setStartAddr(int addr);

        /**
         * 读取指定地址的一个字节
         * @param addr 地址
         * @return data
         * @throws IndexOutOfBoundsException 内存读越界
         */
        byte getByte(int addr);

        /**
         * 设置指定地址的数据
         * @param addr 地址
         * @param data 值
         * @throws IndexOutOfBoundsException 内存写越界
         */
        void setByte(int addr, sbyte data);

        /**
         * 全部内存清零
         */
        void clear();
    }
}
