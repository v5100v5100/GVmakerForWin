﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    /// <summary>
    /// 读取地址addr处的数据
    /// 
    /// </summary>
     interface Getable
    {
        byte getByte(int addr);
    }
}