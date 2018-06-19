using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Script.Interpreter.IO
{
    /// <summary>
    /// 封装拼音输入法所需的信息
    /// </summary>
    public class SpellInfo
    {
        
        private SpellInfo()
        {
        }

        /**
         * 得到所有SpellNode的枚举器,其中包含按拼音字典顺序排列的count个SpellNode
         */
        public static Enumeration list()
        {
            return new SpellNodeEnum();
        }

        /**
         * 得到不同拼音的数目
         * @return count
         */
        public static int count() 
        {
            return SPELL_NODES.Length;
        }
        private static readonly string SPELL_DATA = "/eastsun/jgvm/module/io/res/spell.dat";
        private static readonly SpellNode[] SPELL_NODES;
    
        /// <summary>
        /// 
        /// </summary>
        static SpellInfo()
        {
            //装载拼音数据
            //数据格式:
            //第一个字节0xAB表示版本号为A.B
            //第二,三个字节表示总共有多少个拼音
            //接下来为拼音数据,每个拼音数据的格式为"对应汉字数(一字节) 拼音字符串 |"
            //后面是汉字数据,按拼音出现的次序顺序排列
    	    //android.content.Context con = eastsun.jgvm.plaf.android.MainView.getCurrentView().getContext();
    	    //InputStream in = con.getResources().openRawResource(eastsun.jgvm.plaf.android.R.raw.spell);
            FileStream inputStream = new FileStream("eastsun.jgvm.plaf.android.R.raw.spell",FileMode.Open);
            SpellNode[] tmp = null;
            try {
                //int version = inputStream.read();
                int version = inputStream.ReadByte();
                if (version != 0x10) {
                    //throw new IllegalStateException("不适合的数据版本:" + Integer.toHexString(version));
                }
                //int count = in.read();
                int count = inputStream.ReadByte();
                //count = count + in.read() * 256;
                count = count + inputStream.ReadByte() * 256;
                tmp = new SpellNode[count];
                byte[] buffer = new byte[20];
                for (int index = 0; index < count; index++) {
                    //int size = in.read();
                    int size = inputStream.ReadByte();
                    int n = 0, b;
                    //while ((b = in.read()) != '|') {
                    while ((b = inputStream.ReadByte() != '|')){
                        buffer[n++] = (byte) b;
                    }
                    //String spell = new String(buffer, 0, n);
                    string spell = new string(buffer , 0 ,n);
                    tmp[index] = new SpellNode(spell, size);
                }
                for (int index = 0; index < count; index++) 
                {
                    SpellNode node = tmp[index];
                    byte[] data = new byte[2 * node.size()];
                    //in.read(data);
                    inputStream.Read(data,0,data.Length);
                    node.setData(data);
                }
            } catch (IOException ex) {
                //System.exit(-1);
            } finally {
                try {
                    //in.close();
                    inputStream.Close();
                } catch (Exception ex) {
                    //do nothing
                }
            }
            SPELL_NODES = tmp;
        }

        private static class SpellNodeEnum implements Enumeration
        {

            private int index = 0;

            public boolean hasMoreElements() {
                return index < SPELL_NODES.length;
            }

            public Object nextElement() {
                return SPELL_NODES[index++];
            }
        }
    }
}
