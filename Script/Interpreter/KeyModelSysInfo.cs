using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    /// <summary>
    /// 提供GVM必须的几个系统按键值,这些按键在GVM中用于文件列表以及输入法中使用<p>
    /// 方向键,输入键,跳出键是必须;数字键是可选的
    /// </summary>
    public interface KeyModelSysInfo
    {
        int getLeft();

        int getRight();

        int getUp();

        int getDown();

        int getEnter();

        int getEsc();


        /// <summary>
        /// 是否支持'0'-'9'这10个数字按键,当且仅当系统支持数字键时支持完整的输入法
        /// </summary>
        /// <returns>是否支持</returns>
        bool hasNumberKey();

        /// <summary>
        /// 得到'0'-'9'这10个数字键的键值
        /// </summary>
        /// <param name="num">数字</param>
        /// <returns>键值;如haveNumberKey返回false,该返回值未定义</returns>
        int getNumberKey(int num);
    }
}
