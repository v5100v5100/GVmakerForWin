using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Script.Interpreter
{
    public class LavApp
    {
        private sbyte[] appData;
        private int offset;

        /// <summary>
        /// 通过一个输入流创建一个LavApp对象
        /// </summary>
        /// <param name="inputStream">一个输入流</param>
        /// <returns>一个LavApp对象</returns>
        public static LavApp createLavApp(FileStream inputStream)
        {
            return new DefaultLavApp(inputStream);
        }

        /// <summary>
        /// 通过一个lav程序数据来创建一个LavApp<p>
        /// 注意,LavApp内部使用的就是该数组,类创建后不能从外部修改这个数组
        /// </summary>
        /// <param name="data"></param>
        protected LavApp(sbyte[] data)
        {
            this.appData = data;
            verifyData();
        }

        /// <summary>
        /// lav程序数据大小(字节数)
        /// </summary>
        /// <returns>size 这个lav程序数据的总大小,含文件头</returns>
        public int size() 
        {
            return appData.Length;
        }


        /// <summary>
        /// 在pointer处读取一字节数据,并使pointer加一<p>
        /// 注意,这里返回值是char类型,对应lav的char类型,因为lav的char类型是无符号的.
        /// </summary>
        /// <returns></returns>
        public char getChar()
        {
            return (char) (appData[offset++] & 0xff);
        }

        /// <summary>
        /// 从app中读取两字节数据,对应lav中的int类型
        /// </summary>
        /// <returns></returns>
        public UInt16 getInt()
        {
            UInt16 s;
            s = (UInt16)(appData[offset++] & 0xff);
            s |= (appData[offset++] & 0xff) << 8;
            return s;
        }

        /// <summary>
        /// 从app中读取三字节数据(无符号),对应lav中文件指针数据
        /// </summary>
        /// <returns></returns>
        public int getAddr()
        {
            int addr;
            addr = appData[offset++] & 0xff;
            addr |= (appData[offset++] & 0xff) << 8;
            addr |= (appData[offset++] & 0xff) << 16;
            return addr;
        }

        /// <summary>
        /// 从app中读取四字节数据,对应lav中的long类型
        /// </summary>
        /// <returns></returns>
        public int getLong()
        {
            int i;
            i = appData[offset++] & 0xff;
            i |= (appData[offset++] & 0xff) << 8;
            i |= (appData[offset++] & 0xff) << 16;
            i |= (appData[offset++] & 0xff) << 24;
            return i;
        }

        /// <summary>
        /// 得到当前数据偏移量
        /// </summary>
        /// <returns>下次读取时的位置</returns>
        public int getOffset() 
        {
            return offset;
        }


        /// <summary>
        /// 设置读取偏移量
        /// </summary>
        /// <param name="pos">偏移量,下次读取数据时开始位置</param>
        public void setOffset(int pos)
        {
            offset = pos;
        }

        /// <summary>
        /// 检查数据格式并设置相应参数
        /// data 一个lavApp数据
        /// </summary>
        private void verifyData()
        {
            if (appData.Length <= 16)
            {
                //throw new IllegalArgumentException("不是有效的LAV文件!");
            }
            if (appData[0] != 0x4c || appData[1] != 0x41 || appData[2] != 0x56) 
            {
                //throw new IllegalArgumentException("不是有效的LAV文件!");
            }
            offset = 16;
        }
    }
}
