using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    public class DefaultKeyMap : KeyMap
    {
        int[] keyValues_;
        char[] keyCodes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="keyCodes"></param>
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
            Array.Copy(keyValues, 0, keyValues_, 0, keyValues.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] keyValues()
        {
            int[] newValues = new int[keyValues_.Length];
            //System.arraycopy(keyValues, 0, newValues, 0, keyValues.Length);
            Array.Copy(keyValues_, 0, newValues, 0, keyValues_.Length);
            return newValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawKeyCode"></param>
        /// <returns></returns>
        public UInt16 translate(int rawKeyCode)
        {
            for (int index = 0; index < keyValues_.Length; index++)
            {
                if (keyValues_[index] == rawKeyCode)
                {
                    return keyCodes[index];
                }
            }
            return '0';
        }
    }
}
