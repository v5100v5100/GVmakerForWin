using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Ram
{
    public class ScreenRam : RelativeRam
    {

        private sbyte[] buffer;
        private int type;
        private int startAddr;
        private ScreenModel screen;

        public ScreenRam(ScreenModel screen, sbyte[] buffer, int type)
        {
            this.screen = screen;
            this.buffer = buffer;
            this.type = type;
        }

        public ScreenModel getScreenModel() {
            return screen;
        }

        public int size() {
            return buffer.Length;
        }

        public int getRamType() {
            return type;
        }

        public int getStartAddr() {
            return startAddr;
        }

        public void setStartAddr(int addr) {
            startAddr = addr;
        }

        public sbyte getByte(int addr)
        {
            return buffer[addr - startAddr];
        }

        public void setByte(int addr, sbyte data)
        {
            buffer[addr - startAddr] = data;
        }

        public void clear() {
            for (int index = buffer.Length - 1; index >= 0; index--) {
                buffer[index] = 0;
            }
        }
    }
}
