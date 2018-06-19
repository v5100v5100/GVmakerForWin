using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Script.Interpreter.IO
{

    /// <summary>
    /// 虚拟文件,使用内存模拟文件操作<p>
    /// 每个虚拟文件除了读写数据外,还有三个属性:position,limit,capacity<p>
    /// 其capacity描述了这个虚拟文件当前能容纳的最大数据量,这个值不能从外部修改,但在向其写入数据时根据需要内部会适当扩充其capacity<p>
    /// limit描述的是虚拟文件当前存储的数据总量,外部可以读取或修改或增加数据到虚拟文件.这个值在调用readFromStream时自动初始化,并且内部自动维护<p>
    /// position表示下一个读/写数据的地址,相当于普通文件操作中的文件指针.其初始值应该由调用者在调用readFromStream方法后正确设置<p>
    /// 对于一个含有数据,并正确初始化的VirtualFile,应有以下关系成立:<p>
    /// 0<=position<=limit<=capacity
    /// </summary>
    public class VirtualFile
    {

        //每次增量的最小值:128K
        private static readonly int MIN_ADD_CAPS = 0x20000;
        private static readonly int MAX_COUNT = 30;

        //内存块,最多支持10块,也就是1280K,对GVmaker来说足够了
        private byte[][] bufs = new byte[MAX_COUNT][];
        //caps[k]表示第0,..k-1块内存的总容量
        private int[] caps = new int[MAX_COUNT + 1];
        //内存块数量
        private int count_;
        //当前所在内存块下标
        private int index_;
        //文件长度
        private int limit_;
        //the index of the next element to be read or written
        private int position_;

        /// <summary>
        /// 使用一个初始容量构造VirtualFile
        /// </summary>
        /// <param name="size"></param>
        public VirtualFile(int size)
        {
            bufs[0] = new byte[size];
            for (int n = 1; n <= MAX_COUNT; n++) {
                caps[n] = size;
            }
            count_ = 1;
        }


        /// <summary>
        /// 得到该VirtualFile总容量
        /// </summary>
        /// <returns></returns>
        public int capacity()
        {
            return caps[count_];
        }

        /// <summary>
        /// 得到VirtualFile中实际存储数据的长度,也就是文件的长度
        /// </summary>
        /// <returns></returns>
        public int limit() 
        {
            return limit_;
        }

        /// <summary>
        /// 得到VirtualFile中的读写指针位置,也就是文件指针
        /// </summary>
        /// <returns></returns>
        public int position() 
        {
            return position_;
        }

        /// <summary>
        /// 设置文件指针
        /// </summary>
        /// <param name="newPos">新的指针位置</param>
        /// <returns>设置后的指针,若出错返回-1</returns>
        public int position(int newPos)
        {
            if (newPos < 0 || newPos > limit_)
            {
                return -1;
            }
            position_ = newPos;
            //修改index,使其满足caps[index]<=position<caps[index+1]
            while (index_ < MAX_COUNT && caps[index_] < caps[index_ + 1] && caps[index_ + 1] <= position_)
            {
                index_++;
            }
            while (caps[index_] > position_)
            {
                index_--;
            }
            return position_;
        }

        /// <summary>
        /// 读取文件数据,并且position加1
        /// </summary>
        /// <returns>当前position出的文件内容;若已到文件位(>=limit()),返回-1</returns>
        public int getc()
        {
            if (position_ >= limit_) {
                return -1;
            }
            int c = bufs[index_][position_ - caps[index_]] & 0xff;
            position_++;
            if (position_ >= caps[index_ + 1]) {
                index_++;
            }
            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public int putc(int ch) 
        {
            if (position_ > limit_) {
                return -1;
            }
            ensureCapacity(position_ + 1);
            bufs[index_][position_ - caps[index_]] = (byte) ch;
            position_++;
            if (position_ > limit_) {
                limit_ = position_;
            }
            if (position_ >= caps[index_ + 1]) {
                index_++;
            }
            return ch;
        }

        /// <summary>
        /// 将position,limit清零
        /// </summary>
        public void refresh() 
        {
            index_ = position_ = limit_ = 0;
        }

        /// <summary>
        /// 从in读取数据到VirtualFile,初始limit的值为从in读取的数量长度<p>
        /// 操作完成后关闭in
        /// </summary>
        /// <param name="inputStream">数据来源</param>
        public void readFromStream(FileStream inputStream)
        {
            limit_ = 0;
            int length;
            int n = 0;
            for (;;) {
                //需要增加容量
                if (n == count_) {
                    bufs[n] = new byte[MIN_ADD_CAPS];
                    for (int k = n + 1; k <= MAX_COUNT; k++) {
                        caps[k] = caps[n] + MIN_ADD_CAPS;
                    }
                    count_++;
                }
                length = inputStream.Read(bufs[n], 0, bufs[n].Length);
                if (length == -1) {
                    break;
                }
                else {
                    limit_ += length;
                    if (length < bufs[n].Length) {
                        break;
                    }
                }
                n++;
            }
        }

        /// <summary>
        /// 将VirtualFile中的内容写入到out<p>
        /// 操作完成后关闭out
        /// </summary>
        /// <param name="outputStream"></param>
        public void writeToStream(FileStream outputStream)
        {
            int n = 0;
            while (limit_ > caps[n + 1]) {
                //outputStream.Write(bufs[n++]);
                outputStream.Write(bufs[n], 0, bufs[n].Length);
                n++;
            }
            if (limit_ > caps[n]) {
                outputStream.Write(bufs[n], 0, limit_ - caps[n]);
            }
            outputStream.Close();
        }

        /// <summary>
        /// 确保至少有minCap大小的内存可用
        /// </summary>
        /// <param name="minCap"></param>
        private void ensureCapacity(int minCap) {
            if (caps[count_] >= minCap) {
                return;
            }
            //每次至少增加128K
            int addCap = Math.Max(MIN_ADD_CAPS, minCap - caps[count_]);
            bufs[count_] = new byte[addCap];
            for (int n = count_ + 1; n <= MAX_COUNT; n++) {
                caps[n] = caps[count_] + addCap;
            }
            count_++;
        }

    }
}
