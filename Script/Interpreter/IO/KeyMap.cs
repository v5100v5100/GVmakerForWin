using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    /// <summary>
    /// 一个得到可用系统按键信息已经提供系统按键到GVM按键转换方法的接口
    /// </summary>
    public interface KeyMap
    {

        /// <summary>
        /// 被使用的按键,也就是GVM中只对这些按键有响应
        /// GVM中使用到的系统按键值,外部操作该数组不会影响到其内部状态
        /// </summary>
        /// <returns></returns>
        int[] keyValues();


        /// <summary>
        ///  将系统原始键值映射到GVM使用的键值
        /// </summary>
        /// <param name="rawKeyCode">rawKeyCode 原始键值</param>
        /// <returns>keyCode GVM中使用的键值</returns>
        UInt16 translate(int rawKeyCode);
    }
}
