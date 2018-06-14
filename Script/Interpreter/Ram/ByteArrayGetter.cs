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
        private byte[] buffer_;

        public ByteArrayGetter(byte[] buffer)
        {
            this.buffer_ = buffer;
        }

        public byte getByte(int addr)
        {
            return buffer_[addr];
        }
    }
}
