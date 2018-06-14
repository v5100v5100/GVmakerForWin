using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    public class Util
    {
        
        private Util() {
            //
        }

        /**
         * 得到src从addr开始的length个字节的crc16码
         */
        public static char getCrc16Value(Getable src, int addr, int length) {
            char crc = 0, tmp;
            while (--length >= 0) {
                tmp = (char) ((crc >> 8) & 0xff);
                crc <<= 4;
                crc ^= CRC16_TAB[(tmp >> 4) ^ ((src.getByte(addr) & 0xff) >> 4)];
                tmp = (char) ((crc >> 8) & 0xff);
                crc <<= 4;
                crc ^= CRC16_TAB[(tmp >> 4) ^ (src.getByte(addr) & 0x0f)];
                addr++;
            }
            return crc;
        }
        private static char[] CRC16_TAB = {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef
        };

        /**
         * 从Properties中得到GVM系统按键信息
         */
        public static KeyModelSysInfo loadFromProperties(Properties pp) {
            return new KeyModelSysInfoImpl(pp);
        }

        /**
         * 从按键配置文件中解析出KeyMap<p>
         * 配置文件格式:<p>
         *     1.基本格式: [注释] 系统Key值=GVM对应Key值 [注释]\n
         *       表示将系统上的按键值为{系统Key值}的对应到GVM上按键值{GVM对应Key值}
         *     2.关于注释: 配置文件中允许有注释,解析时会跳过注释部分,注释格式与C++中的注释格式相同
         *     3.注意事项: 每行最多只能有一个配置项,(即{系统Key值=GVM对应Key值}),配置项前后可以带有注释,但注释项中间不能混有注释
         * @param in 用于解析的输入流
         * @return 解析出的KeyMap
         * @throws IOException 发生IO错误
         */
        public static KeyMap parseKeyMap(InputStream in) throws IOException {
            ByteArrayOutputStream bos = new ByteArrayOutputStream();
            byte[] tmpBuffer = new byte[512];
            int length;
            while ((length = in.read(tmpBuffer)) != -1) {
                bos.write(tmpBuffer, 0, length);
            }
            in.close();
            byte[] buffer = bos.toByteArray();
            int count = 0;
            for (int index = 0; index < buffer.length;) {
                if (buffer[index] == '=') {
                    count++;
                }
                int comment = skipComment(buffer, index);
                if (comment > 0) {
                    index += comment;
                }
                else {
                    index++;
                }
            }
            int[] keyValues = new int[count];
            char[] keyCodes = new char[count];
            int start = 0, end = 0;
            while (--count >= 0) {
                while (buffer[end] != '=') {
                    int comment = skipComment(buffer, end);
                    if (comment == 0) {
                        end++;
                    }
                    else {
                        end += comment;
                        start = end;
                    }
                }
                keyValues[count] = parseInt(buffer, start, end);
                start = end;
                while (end < buffer.length && buffer[end] != 0x0a) {
                    int command = skipComment(buffer, end);
                    if (command == 0) {
                        end++;
                    }
                    else {
                        break;
                    }
                }
                keyCodes[count] = (char) parseInt(buffer, start, end);
                start = end;
            }

            return new DefaultKeyMap(keyValues, keyCodes);
        }

        /**
         * 跳过注释部分
         * @param data
         * @param offset
         * @return 如果有注释,返回注释总长度;否则返回0
         */
        private static int skipComment(byte[] data, int offset) {
            int start = offset;
            if (data[offset++] != '/') {
                return 0;
            }
            if (data[offset] != '/' && data[offset] != '*') {
                return 0;
            }
            if (data[offset++] == '/') {
                while (offset < data.length && data[offset] != 0x0a) {
                    offset++;
                }
            }
            else {
                while (!(data[offset] == '*' && data[offset + 1] == '/')) {
                    offset++;
                }
                offset += 2;
            }
            return offset - start;
        }

        /**
         * 解析一个byte形式的字符串为int,这个byte数组开始与结尾可能存在多余的字符
         * @return 解析结果
         */
        private static int parseInt(byte[] data, int start, int end) {
            int mark = -1, count = 0;
            for (int index = start; index < end; index++) {
                if (data[index] == '\'') {
                    count++;
                    if (count > 2 || (count == 2 && index != mark + 2)) {
                        throw new IllegalArgumentException("按键配置文件错误!" + count + "," + index + "," + mark);
                    }
                    if (count == 1) {
                        mark = index;
                    }
                }
            }
            if (count > 0) {
                return data[mark + 1];
            }
            while (!((data[start] >= '0' && data[start] <= '9') || data[start] == '-')) {
                start++;
            }
            int num = 0;
            if (data[start] == '0' && (data[start + 1] == 'x' || data[start + 1] == 'X')) {
                start += 2;
                while (start < end) {
                    byte b = data[start];
                    if (b >= '0' && b <= '9') {
                        num <<= 4;
                        num |= b - '0';
                    }
                    else if (b >= 'a' && b <= 'f') {
                        num <<= 4;
                        num |= b - 'a' + 10;
                    }
                    else if (b >= 'A' && b <= 'F') {
                        num <<= 4;
                        num |= b = 'A' + 10;
                    }
                    else {
                        break;
                    }
                    start++;
                }
            }
            else {
                boolean flags = (data[start] == '-');
                if (flags) {
                    start++;
                }
                while (start < end) {
                    byte b = data[start];
                    if (b >= '0' && b <= '9') {
                        num = num * 10 + b - '0';
                    }
                    else {
                        break;
                    }
                    start++;
                }
                if (flags) {
                    num = -num;
                }
            }
            return num;
        }

        /**
         * 将一个整数化作gb2312编码的字符串数据
         * @param i 一个整数值
         * @return 一个表示gb2312编码的byte数组
         */
        public static byte[] intToGB(int i) {
            int sign = i < 0 ? 1 : 0;
            if (i < 0) {
                i = -i;
            }
            int length = 1;
            int tmp = i;
            while ((tmp /= 10) > 0) {
                length++;
            }
            byte[] data = new byte[sign + length];
            while (--length >= 0) {
                data[length + sign] = (byte) (i % 10 + 0x30);
                i /= 10;
            }
            if (sign > 0) {
                data[0] = 0x2d;
            }
            return data;
        }

        /**
         * 功能同GVmaker中的cos函数
         */
        public static int cos(int deg) {
            deg &= 0x7fff;
            deg = 90 - deg;
            deg = deg % 360 + 360;
            return sin(deg);
        }

        /**
         * 功能同GVmaker中的cos函数
         */
        public static int sin(int deg) {
            deg &= 0x7fff;
            deg %= 360;
            switch (deg / 90) {
                case 0:
                    return sinTab[deg];
                case 1:
                    return sinTab[180 - deg];
                case 2:
                    return -sinTab[deg - 180];
                default:
                    return -sinTab[360 - deg];
            }
        }

        /**
         * 将一个byte数组包装为一个Accessable
         * @param array byte数组
         * @return 一个Accessable
         */
        public static Accessable asAccessable(final byte[] array) {
            return new Accessable() {

                public byte getByte(
                        int addr) throws IndexOutOfBoundsException {
                    return array[addr];
                }

                public void setByte(int addr, byte b) throws IndexOutOfBoundsException {
                    array[addr] = b;
                }
            };
        }

        /**
         * 得到gb2312编码字符c的点阵数据<p>
         * 如果字符不是一个正常的gb2312字符,则用0填充数组
         * @param c 字符
         * @param data 用来保存点阵数据,其大小应不小于12或24
         * @return count 数据字节数
         */
        public static int getGB12Data(char c, byte[] data) {
            int offset, count;
            byte[] buffer;
            if (c <= 0xff) {
                count = 12;
                offset = c * 12;
                buffer = ascii12Data;
            }
            else {
                count = 24;
                buffer = gb12Data;
                int high = (c & 0xff) - 0xa1;
                if (high > 8) {
                    high -= 6;
                }
                offset = (high * 94 + (c >> 8) - 0xa1) * 24;
            }
            if (offset < 0 || offset + count > buffer.length) {
                fillZero(data);
            }
            else {
                for (int index = count - 1; index >= 0; index--) {
                    data[index] = buffer[offset + index];
                }
            }
            return count;
        }

        private static void fillZero(byte[] array) {
            for (int index = 0; index < array.length; index++) {
                array[index] = 0;
            }
        }

        /**
         * 得到gb2312编码字符c的点阵数据
         * @param c 字符
         * @param data 用来保存点阵数据,其大小应不小于16或32
         * @return count 数据字节数
         */
        public static int getGB16Data(char c, byte[] data) {
            int offset, count;
            byte[] buffer;
            if (c <= 0xff) {
                count = 16;
                offset = c << 4;
                buffer = ascii16Data;
            }
            else {
                count = 32;
                buffer = gb16Data;
                int high = (c & 0xff) - 0xa1;
                if (high > 8) {
                    high -= 6;
                }
                offset = (high * 94 + (c >> 8) - 0xa1) << 5;
            }
            if (offset < 0 || offset + count > buffer.length) {
                fillZero(data);
            }
            else {
                for (int index = count - 1; index >= 0; index--) {
                    data[index] = buffer[offset + index];
                }
            }
            return count;
        }
        private static byte[] gb16Data;
        private static byte[] gb12Data;
        private static byte[] ascii16Data;
        private static byte[] ascii12Data;
        private static final int[] sinTab = {
            0, 18, 36, 54, 71, 89, 107, 125, 143, 160, 178, 195, 213, 230, 248, 265, 282,
            299, 316, 333, 350, 367, 384, 400, 416, 433, 449, 465, 481, 496, 512, 527,
            543, 558, 573, 587, 602, 616, 630, 644, 658, 672, 685, 698, 711, 724, 737,
            749, 761, 773, 784, 796, 807, 818, 828, 839, 849, 859, 868, 878, 887, 896,
            904, 912, 920, 928, 935, 943, 949, 956, 962, 968, 974, 979, 984, 989, 994,
            998, 1002, 1005, 1008, 1011, 1014, 1016, 1018, 1020, 1022, 1023, 1023, 1024, 1024
        };

        private static void readData(InputStream in, byte[] buffer) throws IOException {
            int offset = 0;
            int len = buffer.length;
            do {
                int size = in.read(buffer, offset, len - offset);
                if (size == -1) {
                    break;
                }
                offset += size;
            } while (offset < len);
            in.close();
        }
    

        static {
            try {
                InputStream in = null;
            
                android.content.Context con = eastsun.jgvm.plaf.android.MainView.getCurrentView().getContext();
            
                in = con.getResources().openRawResource(eastsun.jgvm.plaf.android.R.raw.gbfont);
                //in = Util.class.getResourceAsStream("/eastsun/jgvm/module/io/res/gbfont.bin");
                gb12Data = new byte[in.available()];
                readData(in, gb12Data);

                in = con.getResources().openRawResource(eastsun.jgvm.plaf.android.R.raw.gbfont16);
                //in = Util.class.getResourceAsStream("/eastsun/jgvm/module/io/res/gbfont16.bin");
                gb16Data = new byte[in.available()];
                readData(in, gb16Data);

                in = con.getResources().openRawResource(eastsun.jgvm.plaf.android.R.raw.ascii);
                //in = Util.class.getResourceAsStream("/eastsun/jgvm/module/io/res/ascii.bin");
                ascii12Data = new byte[in.available()];
                readData(in, ascii12Data);

                in = con.getResources().openRawResource(eastsun.jgvm.plaf.android.R.raw.ascii8);
                //in = Util.class.getResourceAsStream("/eastsun/jgvm/module/io/res/ascii8.bin");
                ascii16Data = new byte[in.available()];
                readData(in, ascii16Data);
            } catch (IOException ex) {
                throw new IllegalStateException(ex.toString());
            }
        }
    }
}
