using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Script.Interpreter.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class Properties
    {
        
        public readonly string BLACK_COLOR = "BLACK_COLOR";
        public readonly string WHITE_COLOR = "WHITE_COLOR";
        public readonly string BACKGROUND = "BACKGROUND";
        public readonly string CIRC_ANGLE = "CIRC_ANGLE";
        public readonly string SCREEN_RATE = "SCREEN_RATE";
        public readonly string GVM_ROOT = "GVM_ROOT";
        public readonly string KEY_ENTER = "KEY_ENTER";
        public readonly string KEY_ESC = "KEY_ESC";
        public readonly string KEY_UP = "KEY_UP";
        public readonly string KEY_DOWN = "KEY_DOWN";
        public readonly string KEY_LEFT = "KEY_LEFT";
        public readonly string KEY_RIGHT = "KEY_RIGHT";
        public readonly string NUMBER_KEY_SUPPORTED = "NUMBER_KEY_SUPPORTED";
        public readonly string QUICK_EXIT = "QUICK_EXIT";
        public readonly string KEY_NUMBER0 = "KEY_NUMBER0";
        public readonly string KEY_NUMBER1 = "KEY_NUMBER1";
        public readonly string KEY_NUMBER2 = "KEY_NUMBER2";
        public readonly string KEY_NUMBER3 = "KEY_NUMBER3";
        public readonly string KEY_NUMBER4 = "KEY_NUMBER4";
        public readonly string KEY_NUMBER5 = "KEY_NUMBER5";
        public readonly string KEY_NUMBER6 = "KEY_NUMBER6";
        public readonly string KEY_NUMBER7 = "KEY_NUMBER7";
        public readonly string KEY_NUMBER8 = "KEY_NUMBER8";
        public readonly string KEY_NUMBER9 = "KEY_NUMBER9";
        private Hashtable ht;

        public Properties(FileStream inputStream){
            ht = parseStream(inputStream);
        }

        /**
         * 得到指定属性的值
         * @param name 属性名
         * @return  对应的值;如果该属性不存在,返回null
         */
        public String getProperty(String name) {
            return (String) ht.get(name);
        }

        private static Hashtable parseStream(FileStream inputStream) {
            Hashtable ht = new Hashtable();
            byte[] buffer = new byte[128];
            String name, property;
            int b, index;
            while (true) {
                b = skipCommentAndSpace(inputStream);
                if (b == -1) {
                    break;
                }
                index = 0;
                do {
                    buffer[index++] = (byte) b;
                    b = inputStream.read();
                } while (b != -1 && b != '=');
                name = new String(buffer, 0, index).trim();
                b = skipCommentAndSpace(inputStream);
                index = 0;
                do {
                    buffer[index++] = (byte) b;
                    b = inputStream.read();
                } while (b != -1 && b != 0x0a);
                property = new String(buffer, 0, index).trim();
                ht.put(name, property);
            //System.out.println(name + " = " + property);
            }
            inputStream.Close();
            return ht;
        }

        private static int skipCommentAndSpace(FileStream inputStream)
        {
            int b = 0;
            while (true) {
                do {
                    b = inputStream.read();
                } while (isSpace(b));
                if (b == -1) {
                    return -1;
                }
                else if (b == '#') {
                    do {
                        b = inputStream.read();
                    } while (b != -1 && b != 0x0a);
                    if (b == -1) {
                        return -1;
                    }
                }
                else {
                    return b;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool isSpace(int c) {
            return (c == 0x09 || c == 0x0a || c == 0x0b || c == 0x0c || c == 0x0d || c == 0x20);
        }
    }
}
