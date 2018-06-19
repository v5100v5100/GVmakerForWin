using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    /// <summary>
    /// KeyModel的默认实现
    /// 该实现提供了两个回调方法:keyReleased与KeyPressed,当系统按键发生变化时应当确保调用这些方法
    /// </summary>
    public class DefaultKeyModel : KeyModel
    {
        
        private int[] keyValues;
        private bool[] keyStatus;
        private bool hasKey;
        private int keyCode;
        private KeyMap keyMap;
        private KeyModelSysInfo keyInf;

        public DefaultKeyModel(KeyModelSysInfo keyInf)
        {
            this.keyInf = keyInf;
            hasKey = false;
        }

        public void setKeyMap(KeyMap map) {
            this.keyMap = map;
            keyValues = keyMap.keyValues();
            keyStatus = new bool[keyValues.Length];
        }

        public void keyPreesed(int rawKeyCode)
        {
            hasKey = true;
            keyCode = rawKeyCode;
            for (int index = 0; index < keyValues.Length; index++) {
                if (rawKeyCode == keyValues[index]) {
                    keyStatus[index] = true;
                    break;
                }
            }
        }

        public void keyReleased(int rawKeyCode)
        {
            if (keyCode == rawKeyCode) {
                hasKey = false;
            }
            for (int index = 0; index < keyValues.Length; index++) {
                if (rawKeyCode == keyValues[index]) {
                    keyStatus[index] = false;
                    break;
                }
            }
        }

        public void releaseKey(UInt16 key)
        {
            if ((key & 0x80) != 0)
            {
                for (int index = 0; index < keyStatus.Length; index++)
                {
                    if (keyStatus[index])
                    {
                        hasKey = true;
                        keyCode = keyValues[index];
                        break;
                    }
                }
            }
            else 
            {
                for (int index = 0; index < keyStatus.Length; index++)
                {
                    if (keyStatus[index] && key == keyMap.translate(keyValues[index]))
                    {
                        hasKey = true;
                        keyCode = keyValues[index];
                        break;
                    }
                }
            }
        }

        public UInt16 checkKey(UInt16 key)
        {
            //System.out.printf("CheckKey :%2x%n",(int)key);
            if ((key & 0x80) != 0)
            {
                for (int index = 0; index < keyStatus.Length; index++)
                {
                    if (keyStatus[index])
                    {
                        return keyMap.translate(keyValues[index]);
                    }
                }
                return 0;
            }
            else
            {
                for (int index = 0; index < keyStatus.Length; index++)
                {
                    if (keyStatus[index] && key == keyMap.translate(keyValues[index]))
                    {
                        return key;
                    }
                }
                return 0;
            }
        }

        public UInt16 getchar()
        {
            return keyMap.translate(getRawKey());
        }

        public UInt16 inkey()
        {
            if (hasKey)
            {
                hasKey = false;
                return keyMap.translate(keyCode);
            }
            else
            {
                return 0;
            }
        }

        public int getRawKey()
        {
            while (true) {
                if (hasKey) {
                    hasKey = false;
                    return keyCode;
                } else {
                    //wait();
                }
            }
        }

        public KeyModelSysInfo getSysInfo()
        {
            return keyInf;
        }
    }
}
