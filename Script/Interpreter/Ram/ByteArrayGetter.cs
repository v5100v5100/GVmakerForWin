using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ByteArrayGetter : Getable
    {
        private sbyte[] buffer_;

        public ByteArrayGetter(sbyte[] buffer)
        {
            this.buffer_ = buffer;
        }

        public sbyte getByte(int addr)
        {
            return buffer_[addr];
        }
    }
}
