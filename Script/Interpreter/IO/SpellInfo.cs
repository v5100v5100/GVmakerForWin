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
        
        private SpellInfo() {
        }

        /**
         * 得到所有SpellNode的枚举器,其中包含按拼音字典顺序排列的count个SpellNode
         */
        public static Enumeration list() {
            return new SpellNodeEnum();
        }

        /**
         * 得到不同拼音的数目
         * @return count
         */
        public static int count() {
            return SPELL_NODES.Length;
        }
        private static readonly string SPELL_DATA = "/eastsun/jgvm/module/io/res/spell.dat";
        private static readonly SpellNode[] SPELL_NODES;
    

        static {
            //装载拼音数据
            //数据格式:
            //第一个字节0xAB表示版本号为A.B
            //第二,三个字节表示总共有多少个拼音
            //接下来为拼音数据,每个拼音数据的格式为"对应汉字数(一字节) 拼音字符串 |"
            //后面是汉字数据,按拼音出现的次序顺序排列
    	    android.content.Context con = eastsun.jgvm.plaf.android.MainView.getCurrentView().getContext();
    	    InputStream in = con.getResources().openRawResource(eastsun.jgvm.plaf.android.R.raw.spell);
            //InputStream in = SpellInfo.class.getResourceAsStream(SPELL_DATA);
            SpellNode[] tmp = null;
            try {
                int version = in.read();
                if (version != 0x10) {
                    throw new IllegalStateException("不适合的数据版本:" + Integer.toHexString(version));
                }
                int count = in.read();
                count = count + in.read() * 256;
                tmp = new SpellNode[count];
                byte[] buffer = new byte[20];
                for (int index = 0; index < count; index++) {
                    int size = in.read();
                    int n = 0, b;
                    while ((b = in.read()) != '|') {
                        buffer[n++] = (byte) b;
                    }
                    String spell = new String(buffer, 0, n);
                    tmp[index] = new SpellNode(spell, size);
                }
                for (int index = 0; index < count; index++) {
                    SpellNode node = tmp[index];
                    byte[] data = new byte[2 * node.size];
                    in.read(data);
                    node.setData(data);
                }
            } catch (IOException ex) {
                System.exit(-1);
            } finally {
                try {
                    in.close();
                } catch (Exception ex) {
                    //do nothing
                }
            }
            SPELL_NODES = tmp;
        }

        private static class SpellNodeEnum implements Enumeration {

            private int index = 0;

            public boolean hasMoreElements() {
                return index < SPELL_NODES.length;
            }

            public Object nextElement() {
                return SPELL_NODES[index++];
            }
        }

        public static class SpellNode {

            private String spell;
            private byte[] data;
            private int size;

            /**
             * 封装单个拼音的相关信息
             * @param spell 拼音
             * @param data spell对应的汉字的gb2312编码组成的数组,内部将直接使用这个数组
             */
            SpellNode(String spell, int size) {
                this.spell = spell;
                this.size = size;
            }

            /**
             * 得到该node的拼音字符串
             */
            public String spell() {
                return spell;
            }

            /**
             * 该拼音对应汉字个数
             */
            public int size() {
                return size;
            }

            /**
             * 取得该拼音从id开始的len个汉字的gb2312编码数据
             * @param dst 用于保存数据
             * @param id       开始的汉字编号,从0开始
             * @param len     欲获取的汉字个数
             * @return           实际获取的汉字个数
             */
            public int getGB(Setable dst, int offset, int id, int len) {
                id <<= 1;
                len <<= 1;
                int index = 0;
                while (index < len && id + index < data.length) {
                    dst.setByte(offset + index, data[id + index]);
                    index++;
                    dst.setByte(offset + index, data[id + index]);
                    index++;
                }
                return index >>> 1;
            }

            public String toString() {
                String gbStr = null;
                try {
                    gbStr = new String(data, 0, data.length, "gb2312");
                } catch (UnsupportedEncodingException ex) {
                    gbStr ="Don't Unsupport GB2312";
                }
                return spell + ": " + gbStr;
            }

            /**
             *  设置该node的汉字数据,内部直接使用该byte数组
             */
            void setData(byte[] data) {
                if (data.length != size * 2) {
                    throw new IllegalStateException();
                }
                this.data = data;
            }
        }

    }
}
