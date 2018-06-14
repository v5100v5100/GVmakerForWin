using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    public class KeyModelSysInfoImpl : KeyModelSysInfo
    {
        int left, right, down, up, enter, esc;
        bool hasNumberKey_;
        int[] numberKey = new int[10];

        private Properties pp;

        public KeyModelSysInfoImpl(Properties pp)
        {
            this.pp = pp;
            left = getKeyValue(Properties.KEY_LEFT);
            right = getKeyValue(Properties.KEY_RIGHT);
            up = getKeyValue(Properties.KEY_UP);
            down = getKeyValue(Properties.KEY_DOWN);
            enter = getKeyValue(Properties.KEY_ENTER);
            esc = getKeyValue(Properties.KEY_ESC);
            hasNumberKey_ = pp.getProperty(Properties.NUMBER_KEY_SUPPORTED) == "true";
            if (hasNumberKey_)
            {
                for (int index = 0; index <= 9; index++) {
                    numberKey[index] = getKeyValue("KEY_NUMBER" + index);
                }
            }
        }


        int getKeyValue(string str)
        {
            str = pp.getProperty(str);
            if (str.StartsWith("'"))
            {
                //return str.charAt(1);
                return str[1];
            }
            else {
                //str = str.toLowerCase();
                str = str.ToLower();

                //return str.StartsWith("0x") ? Integer.parseInt(str.substring(2), 16) : Integer.parseInt(str);
                return str.StartsWith("0x") ? Convert.ToInt32(str.Substring(2), 16) : Convert.ToInt32(str);
            }
        }

        public int getLeft() {
            return left;
        }

        public int getRight() {
            return right;
        }

        public int getUp() {
            return up;
        }

        public int getDown() {
            return down;
        }

        public int getEnter() {
            return enter;
        }

        public int getEsc() {
            return esc;
        }

        public bool hasNumberKey() {
            return hasNumberKey_;
        }

        public int getNumberKey(int num) {
            return numberKey[num];
        }
    }
}
