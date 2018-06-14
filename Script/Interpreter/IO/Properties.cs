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
        
        public const string BLACK_COLOR = "BLACK_COLOR";
        public const string WHITE_COLOR = "WHITE_COLOR";
        public const string BACKGROUND = "BACKGROUND";
        public const string CIRC_ANGLE = "CIRC_ANGLE";
        public const string SCREEN_RATE = "SCREEN_RATE";
        public const string GVM_ROOT = "GVM_ROOT";
        public const string KEY_ENTER = "KEY_ENTER";
        public const string KEY_ESC = "KEY_ESC";
        public const string KEY_UP = "KEY_UP";
        public const string KEY_DOWN = "KEY_DOWN";
        public const string KEY_LEFT = "KEY_LEFT";
        public const string KEY_RIGHT = "KEY_RIGHT";
        public const string NUMBER_KEY_SUPPORTED = "NUMBER_KEY_SUPPORTED";
        public const string QUICK_EXIT = "QUICK_EXIT";
        public const string KEY_NUMBER0 = "KEY_NUMBER0";
        public const string KEY_NUMBER1 = "KEY_NUMBER1";
        public const string KEY_NUMBER2 = "KEY_NUMBER2";
        public const string KEY_NUMBER3 = "KEY_NUMBER3";
        public const string KEY_NUMBER4 = "KEY_NUMBER4";
        public const string KEY_NUMBER5 = "KEY_NUMBER5";
        public const string KEY_NUMBER6 = "KEY_NUMBER6";
        public const string KEY_NUMBER7 = "KEY_NUMBER7";
        public const string KEY_NUMBER8 = "KEY_NUMBER8";
        public const string KEY_NUMBER9 = "KEY_NUMBER9";
        private Hashtable ht;

        public Properties(FileStream inputStream){
            ht = parseStream(inputStream);
        }

        /// <summary>
        /// 得到指定属性的值
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>对应的值;如果该属性不存在,返回null</returns>
        public string getProperty(string name) {
            //return (string) ht.get(name);
            return (string)ht[name];
        }

        private static Hashtable parseStream(FileStream inputStream) {
            Hashtable ht = new Hashtable();
            byte[] buffer = new byte[128];
            string name, property;
            int b, index;
            while (true) {
                b = skipCommentAndSpace(inputStream);
                if (b == -1) {
                    break;
                }
                index = 0;
                do {
                    buffer[index++] = (byte) b;
                    //b = inputStream.read();
                    b = inputStream.ReadByte();
                } while (b != -1 && b != '=');
                name = new string(buffer, 0, index).Trim();
                b = skipCommentAndSpace(inputStream);
                index = 0;
                do {
                    buffer[index++] = (byte) b;

                    //b = inputStream.read();
                    b = inputStream.ReadByte();

                } while (b != -1 && b != 0x0a);
                property = new string(buffer, 0, index).trim();


                //ht.put(name, property);
                ht[name] = property;

            }
            inputStream.Close();
            return ht;
        }

        private static int skipCommentAndSpace(FileStream inputStream)
        {
            int b = 0;
            while (true)
            {
                do 
                {

                    //b = inputStream.read();
                    b = inputStream.ReadByte();

                } while (isSpace(b));
                if (b == -1) 
                {
                    return -1;
                }
                else if (b == '#')
                {
                    do 
                    {
                        //b = inputStream.read();
                        b = inputStream.ReadByte();

                    } while (b != -1 && b != 0x0a);

                    if (b == -1)
                    {
                        return -1;
                    }
                }
                else
                {
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
