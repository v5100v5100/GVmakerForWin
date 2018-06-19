using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    /// <summary>
    /// 联内存模块<p>
    /// 这种内存直接或间接与ScreenModel相关联,其内容也与ScreenModel相对应
    /// </summary>
    public interface RelativeRam : Ram
    {
        ScreenModel getScreenModel();
    }
}
