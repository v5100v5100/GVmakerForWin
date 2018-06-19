using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    public class AccessableImpl : Accessable
    {
        private sbyte[] array;

        public AccessableImpl(sbyte[] array)
        {
            this.array = array;
        }

        public sbyte getByte(int addr)
        {
            return array[addr];
        }

        public void setByte(int addr, sbyte b) 
        {
            array[addr] = b;
        }
    }
}
