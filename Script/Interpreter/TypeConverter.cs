using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    public static class TypeConverter
    {
        static TypeConverter() 
        {

        }

        /// <summary>
        /// 将byte类型转换成sbyte类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static sbyte ByteToSByte(byte value)
        {
            if (value > 127)
            {
                return (sbyte)(value - 256);
            }
            else 
            {
                return (sbyte)value;
            }
        }

        /// <summary>
        /// 将sbyte类型转换成byte类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte SByteToByte(sbyte value)
        {
            if (value < 0)
            {
                return (byte) (127 - value);
            }
            else 
            {
                return (byte)value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] SByteArrayToByteArray(sbyte []values)
        {
            byte[] result = new byte[values.Length];
            for (int i = 0;i < values.Length ;i++ )
            {
                result[i] = SByteToByte(values[i]);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static sbyte[] ByteArrayToSByteArray(byte[] values)
        {
            sbyte[] result = new sbyte[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = ByteToSByte(values[i]);
            }
            return result;
        }

        /// <summary>
        /// 无符号右移, 相当于java里的 value>>>pos
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        public static int UnsignedRightMove(this int value, int pos)
        {
            //移动 0 位时直接返回原值
            if (pos != 0)
            {
                // int.MaxValue = 0x7FFFFFFF 整数最大值
                int mask = int.MaxValue;
                //无符号整数最高位不表示正负但操作数还是有符号的，有符号数右移1位，正数时高位补0，负数时高位补1
                value = value >> 1;
                //和整数最大值进行逻辑与运算，运算后的结果为忽略表示正负值的最高位
                value = value & mask;
                //逻辑运算后的值无符号，对无符号的值直接做右移运算，计算剩下的位
                value = value >> pos - 1;
            }
            return value;
        }

        /// <summary>
        /// 无符号左移, 相当于java里的 value<<<pos
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int UnsignedLeftMove(this int value, int pos)
        {
            //移动 0 位时直接返回原值
            if (pos != 0)
            {
                // int.MaxValue = 0x7FFFFFFF 整数最大值
                int mask = int.MaxValue;
                //无符号整数最高位不表示正负但操作数还是有符号的，有符号数左移1位，正数时高位补0，负数时高位补1
                value = value << 1;
                //和整数最大值进行逻辑与运算，运算后的结果为忽略表示正负值的最高位
                value = value & mask;
                //逻辑运算后的值无符号，对无符号的值直接做左移运算，计算剩下的位
                value = value << pos - 1;
            }
            return value;
        }
    }
}
