using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
     interface Setable
    {
        void setByte(int addr, byte b);
    }
}
