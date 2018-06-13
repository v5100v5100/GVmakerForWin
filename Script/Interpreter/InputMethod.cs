using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    public class InputMethodConst
    {
        /// <summary>
        /// 默认输入英文字母
        /// </summary>
        public static readonly int ENGLISH_MODE = 0;

        /// <summary>
        /// 默认输入为数字
        /// </summary>
        public static readonly int NUMBER_MODE = 1;

        /// <summary>
        /// 默认输入为汉字
        /// </summary>
        public static readonly int GB2312_MODE = 2;

        /// <summary>
        /// 保持之前的输入模式
        /// </summary>
        public static readonly int DEFAULT_MODE = 3;
    }

    /// <summary>
    /// 输入法接口
    /// 注意: 不同的JGVM实例应该使用不同的InputMethod实例
    /// </summary>
    public interface InputMethod
    {
        /// <summary>
        /// 通过接受键盘操作并适当反馈信息到屏幕而得到用户输入的信息<p>
        /// 该方法使用屏幕底部12行的内容用于反馈信息
        /// </summary>
        /// <param name="key">用于获得用户按键</param>
        /// <param name="screen">用于反馈操作过程中的信息</param>
        /// <returns>一个gb2312编码的字符</returns>
        /// 期间线程被中断
        public char getWord(KeyModel key, ScreenModel screen);
    
        /// <summary>
        /// 设置该输入法的默认输入模式
        /// </summary>
        /// <param name="mode">输入模式</param>
        /// <returns>之前使用的输入模式</returns>
        public int setMode(int mode);
    }
}
