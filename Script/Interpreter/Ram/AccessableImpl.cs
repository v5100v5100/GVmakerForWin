using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    public class AccessableImpl : Accessable
    {
        private byte[] array;

        public AccessableImpl(byte[] array)
        {
            this.array = array;
        }

        public byte getByte(int addr) {
            return array[addr];
        }

        public void setByte(int addr, byte b) 
        {
            array[addr] = b;
        }
    }
}
