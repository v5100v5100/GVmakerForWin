using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    public class SpellNode
    {

        private string spell_;
        private sbyte[] data;
        private int size_;


        /// <summary>
        /// 封装单个拼音的相关信息
        /// </summary>
        /// <param name="spell">spell 拼音</param>
        /// <param name="size">spell对应的汉字的gb2312编码组成的数组,内部将直接使用这个数组</param>
        public SpellNode(string spell, int size)
        {
            this.spell_ = spell;
            this.size_ = size;
        }

        /// <summary>
        /// 得到该node的拼音字符串
        /// </summary>
        /// <returns></returns>
        public string spell()
        {
            return spell_;
        }

        /// <summary>
        /// 该拼音对应汉字个数
        /// </summary>
        /// <returns></returns>
        public int size()
        {
            return size_;
        }


        /// <summary>
        /// 取得该拼音从id开始的len个汉字的gb2312编码数据
        /// </summary>
        /// <param name="dst">用于保存数据</param>
        /// <param name="offset"></param>
        /// <param name="id">开始的汉字编号,从0开始</param>
        /// <param name="len">欲获取的汉字个数</param>
        /// <returns>实际获取的汉字个数</returns>
        public int getGB(Setable dst, int offset, int id, int len)
        {
            id <<= 1;
            len <<= 1;
            int index = 0;
            while (index < len && id + index < data.Length)
            {
                dst.setByte(offset + index, data[id + index]);
                index++;
                dst.setByte(offset + index, data[id + index]);
                index++;
            }
            //return index >>> 1;
            return index >> 1;//这里有问题，稍后再改
        }

        public override string toString()
        {
            string gbStr = null;
            unsafe
            {
                fixed (sbyte* p_data = data)
                {
                    gbStr = new string(p_data, 0, data.Length, Encoding.GetEncoding("gb2312"));
                }
            }
            return spell_ + ": " + gbStr;
        }

        /// <summary>
        /// 设置该node的汉字数据,内部直接使用该byte数组
        /// </summary>
        /// <param name="data"></param>
        public void setData(sbyte[] data)
        {
            if (data.Length != size_ * 2)
            {
                //throw new IllegalStateException();
            }
            this.data = data;
        }
    }
}
