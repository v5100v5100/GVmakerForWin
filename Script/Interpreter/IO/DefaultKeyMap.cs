using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    public class DefaultKeyMap : KeyMap
    {
        int[] keyValues;
        char[] keyCodes;

        public DefaultKeyMap(int[] keyValues, char[] keyCodes)
        {
            if (keyValues.Length != keyCodes.Length)
            {
                //throw new IllegalArgumentException("keyValues and keyCodes must have the same length!");
            }
            this.keyCodes = new char[keyCodes.Length];
            //System.arraycopy(keyCodes, 0, this.keyCodes, 0, keyCodes.Length);
            Array.Copy(keyCodes, 0, this.keyCodes, 0, keyCodes.Length);
            //this.keyValues = new int[keyValues.Length];
            keyValues = new int[keyValues.Length];
            //System.arraycopy(keyValues, 0, this.keyValues, 0, keyValues.Length);
            Array.Copy(keyValues, 0, this.keyValues, 0, keyValues.Length);
        }

        public int[] keyValues()
        {
            int[] newValues = new int[keyValues.Length];
            //System.arraycopy(keyValues, 0, newValues, 0, keyValues.Length);
            Array.Copy(keyValues, 0, newValues, 0, keyValues.Length);
            return newValues;
        }

        public char translate(int rawKeyCode)
        {
            for (int index = 0; index < keyValues.Length; index++)
            {
                if (keyValues[index] == rawKeyCode)
                {
                    return keyCodes[index];
                }
            }
            return '0';
        }
    }
}
