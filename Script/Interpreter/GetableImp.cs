using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    public class GetableImp : Getable
    {
        private sbyte[] buf_;

        public void setBuffer(sbyte[] buf)
        {
            this.buf_ = buf;
        }

        public sbyte getByte(int addr)
        {
            return buf_[addr];
        }
    }
}
