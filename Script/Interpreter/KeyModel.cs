using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    public interface KeyModel
    {
        
        /// <summary>
        /// 释放按键key,即使该键正被按下
        /// </summary>
        /// <param name="key"></param>
        void releaseKey(char key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        char checkKey(char key);


        /// <summary>
        /// 阻塞直到有键按下，如果阻塞期间线程被中断
        /// </summary>
        /// <returns></returns>
        //char getchar() throws InterruptedException;
        char getchar();

        /// <summary>
        /// 如果当前有键按下,则获得该键值并标记按键标志;否则返回0
        /// </summary>
        /// <returns></returns>
        char inkey();


        /// <summary>
        /// 与getChar功能类似,但返回的是未经处理的原始key值<p>
        /// 阻塞期间线程被中断
        /// </summary>
        /// <returns></returns>
        // getRawKey() throws InterruptedException;
        int getRawKey();

        /// <summary>
        /// 得到一些必须的系统按键值,这些值用于fileList与输入法之用
        /// </summary>
        /// <returns></returns>
        KeyModelSysInfo getSysInfo();
    }
}
