using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    /// <summary>
    /// 内存管理模块,默认维持运行时内存与字符堆并提供对栈的支持<p>
    /// 并且通过install方法支持对显存与文本缓冲区的可选支持<p>
    /// VM可以通过RamManager提供的内存读写访问所有内在Ram,而不需要知道里面的具体结构<p>
    /// </summary>
    public class RamManager : Accessable
    {
        public static readonly int SIZE_OF_ADDR = 3;
        public static readonly int SIZE_OF_CHAR = 1;
        public static readonly int SIZE_OF_INT = 2;
        public static readonly int SIZE_OF_LONG = 4;
        /**
         * 运行时内存的开始地址
         */
        public static readonly int START_ADDR = 0x2000;
        private RelativeRam textRam,  graphRam,  bufferRam;
        private ScreenModel screen;
        private RuntimeRam runRam;
        private StringRam strRam;
        private Stack stack;
        private Ram[] rams = new Ram[3];
        private int ramCount;

        public RamManager(RuntimeRam runRam, StringRam strRam, Stack stack) {
            if (runRam == null || strRam == null || stack == null) {
                throw new IllegalArgumentException("param can't be null");
            }
            this.stack = stack;
            install(runRam);
            install(strRam);
        }

        /**
         * 等同于getByte(addr)
         * @param addr 地址
         * @return 一个无符号char值表示一个byte值
         * @see #getByte(int)
         */
        public char getChar(int addr) {
            return (char) (getByte(addr) & 0xff);
        }

        /**
         * 从指定地址读取两字节组成一个lava中的int数据
         * @param addr 地址
         * @return int
         */
        public short getInt(int addr) {
            return (short) getBytes(addr, SIZE_OF_INT);
        }

        /**
         * 从指定地址读取三字节组成一个lava中的文件指针数据
         * @param addr 地址
         * @return 文件指针
         */
        public int getAddr(int addr) {
            return getBytes(addr, SIZE_OF_ADDR);
        }

        /**
         * 从指定地址读取四字节组成一个lava中的long数据
         * @param addr 地址
         * @return long
         */
        public int getLong(int addr) {
            return getBytes(addr, SIZE_OF_LONG);
        }

        /**
         * 设置一个lava中的char数据,等同于setByte((byte)c)
         * @param addr 地址
         * @param c char值,第八位有效
         * @see #setByte(int,byte)
         */
        public void setChar(int addr, char c) {
            setByte(addr, (byte) c);
        }

        /**
         * 设置一个lava中的int数据
         */
        public void setInt(int addr, short i) {
            setBytes(addr, SIZE_OF_INT, i);
        }

        /**
         * 设置一个lava中的文件指针数据
         */
        public void setAddr(int addr, int a) {
            setBytes(addr, SIZE_OF_ADDR, a);
        }

        /**
         * 设置一个lava中的long数据
         */
        public void setLong(int addr, int l) {
            setBytes(addr, SIZE_OF_LONG, l);
        }

        /**
         * 从指定内存读取连续count个字节拼成一个整数
         * @param addr 开始地址
         * @param count 字节数
         * @return 整值
         */
        public int getBytes(int addr, int count) {
            int data = 0;
            while (--count >= 0) {
                data <<= 8;
                data |= (getByte(addr + count) & 0xff);
            }
            return data;
        }

        /**
         * 用一个整值来设置内存中连续count个字节值<p>
         * 注意:这些内存地址中可能涉及到显存或屏幕缓冲区,该方法不负责通知相应组件<p>
         *      应该自己调用intersectWithGraph()方法判断并做出相应反应
         * @see #intersectWithGraph(int,int)
         */
        public void setBytes(int addr, int count, int data) {
            while (--count >= 0) {
                setByte(addr++, (byte) data);
                data >>>= 8;
            }
        }

        /**
         * 得到stringRam
         */
        public StringRam getStringRam() {
            return strRam;
        }

        /**
         * 得到该JLVM使用的Stack
         * @return stack
         */
        public Stack getStack() {
            return stack;
        }

        /**
         * 得到该JLVM使用的RuntimeRam
         * @return runtimeRam
         */
        public RuntimeRam getRuntimeRam() {
            return runRam;
        }

        /**
         * 得到以start开始end结束的内存地址在屏幕上的显示区域<p>
         * @param start 开始地址,包括
         * @param end 结束地址,不包括
         * @return 该内存区域在屏幕显示区域的最小覆盖矩形;如果不存在现存或该内存块不与显存相交,则返回一个空的Area
         */
        public Area intersectWithGraph(int start, int end) {
            if (graphRam == null) {
                return Area.EMPTY_AREA;
            }
            if (start >= graphRam.getStartAddr() + graphRam.size() || end <= graphRam.getStartAddr()) {
                return Area.EMPTY_AREA;
            }
            if (start < graphRam.getStartAddr()) {
                start = graphRam.getStartAddr();
            }
            if (end > graphRam.getStartAddr() + graphRam.size()) {
                end = graphRam.getStartAddr() + graphRam.size();
            }
            start <<= 3;
            end = (end << 3) - 1;
            int y1 = start / screen.getWidth();
            int x1 = start % screen.getWidth();
            int y2 = end / screen.getWidth();
            int x2 = end % screen.getWidth();
            if (y1 == y2) {
                return new Area(x1, y1, x2 - x1 + 1, 1);
            }
            else {
                return new Area(0, y1, screen.getWidth(), y2 - y1 + 1);
            }
        }

        /**
         * 读取指定内存地址一字节数据,并以byte返回
         * @param addr 地址
         * @return byte数据
         */
        public byte getByte(int addr) {
            //Notice: 该实现与resetRamAddress的实现方式有关
            if (addr >= runRam.getStartAddr()) {
                return runRam.getByte(addr);
            }
            if (addr >= strRam.getStartAddr()) {
                return strRam.getByte(addr);
            }
            for (int index = ramCount - 1; index >= 0; index--) {
                Ram ram = rams[index];
                if (addr >= ram.getStartAddr()) {
                    return ram.getByte(addr);
                }
            }
            throw new IndexOutOfBoundsException("内存读越界:" + addr);
        }

        /**
         * 将地址为addr的数据设为b
         * 注意:这些内存地址中可能涉及到显存或屏幕缓冲区,该方法不负责通知相应组件<p>
         *      应该自己调用intersectWithGraph()方法判断并做出相应反应
         * @param addr 地址
         * @param b 数据
         * @throws IndexOutOfBoundsException 内存写越界
         */
        public void setByte(int addr, byte b) {
            //Notice: 该实现与resetRamAddress的实现方式有关
            if (addr >= runRam.getStartAddr()) {
                runRam.setByte(addr, b);
                return;
            }
            if (addr >= strRam.getStartAddr()) {
                strRam.setByte(addr, b);
                return;
            }
            for (int index = ramCount - 1; index >= 0; index--) {
                Ram ram = rams[index];
                if (addr >= ram.getStartAddr()) {
                    ram.setByte(addr, b);
                    return;
                }
            }
            throw new IndexOutOfBoundsException("内存写越界:" + addr);
        }

        /**
         * 将所有内存模块清零
         */
        public void clear() {
            runRam.clear();
            strRam.clear();
            stack.clear();
            for (int index = 0; index < ramCount; index++) {
                rams[index].clear();
            }

        }

        /**
         * 往里面添加内存模块,每种类型的内存最多只能安装一个.<p>
         * ram为null不会抛出异常,且不会改变RamManager的任何状态
         * @param ram 需要安装的内存
         * @throws IllegalStateException 已经安装了这种类型的内存
         * @see #uninstall
         */
        public void install(Ram ram) {
            if (ram == null) {
                return;
            }
            switch (ram.getRamType()) {
                case Ram.RAM_RUNTIME_TYPE:
                    if (runRam != null) {
                        throw new IllegalStateException("Runtime Ram was installed!");
                    }
                    runRam = (RuntimeRam) ram;
                    break;

                case Ram.RAM_GRAPH_TYPE:
                    if (graphRam != null) {
                        throw new IllegalStateException("Graph Ram was installed!");
                    }
                    graphRam = (RelativeRam) ram;
                    screen = graphRam.getScreenModel();
                    break;

                case Ram.RAM_BUFFER_TYPE:
                    if (bufferRam != null) {
                        throw new IllegalStateException("Buffer Ram was installed!");
                    }
                    bufferRam = (RelativeRam) ram;
                    break;

                case Ram.RAM_STRING_TYPE:
                    if (strRam != null) {
                        throw new IllegalStateException("String Ram was installed!");
                    }
                    strRam = (StringRam) ram;
                    break;

                case Ram.RAM_TEXT_TYPE:
                    if (textRam != null) {
                        throw new IllegalStateException("Text Ram was installed!");
                    }
                    textRam = (RelativeRam) ram;
                    break;

            }
            resetRamAddress();

        }

        /**
         * 卸载ram,如果并没有安装过ram,则什么也不会发生
         * @param type 需要卸载的内存类型
         */
        public void uninstall(int type) {
            Ram ram = null;
            switch (type) {
                case Ram.RAM_RUNTIME_TYPE:
                    ram = runRam;
                    runRam = null;
                    break;

                case Ram.RAM_GRAPH_TYPE:
                    ram = graphRam;
                    graphRam = null;
                    break;

                case Ram.RAM_BUFFER_TYPE:
                    ram = bufferRam;
                    bufferRam = null;
                    break;

                case Ram.RAM_STRING_TYPE:
                    ram = strRam;
                    strRam = null;
                    break;

                case Ram.RAM_TEXT_TYPE:
                    ram = textRam;
                    textRam = null;
                    break;

            }
            if (ram != null) {
                resetRamAddress();
            }

        }

        private void resetRamAddress() {

            // 内存模块布局: 0x2000开始为RuntimeRam
            //                      0x0000开始依次为graphRam,bufferRam,textRam,stringRam
            // 应该按被使用概率从小到大排序,在rams中的次序应该按开始地址从小到大排列
            // Notice: 如果修改,可能同时需要修改setByte与getByte方法

            //防止无效引用
            for (int index = 0; index < rams.length; index++) {
                rams[index] = null;
            }

            ramCount = 0;
            if (runRam != null) {
                //不添加到rams中
                runRam.setStartAddr(START_ADDR);
            }

            int startAddr = 0;
            if (graphRam != null) {
                graphRam.setStartAddr(startAddr);
                startAddr += graphRam.size();
                rams[ramCount++] = graphRam;
            }

            if (bufferRam != null) {
                bufferRam.setStartAddr(startAddr);
                startAddr += bufferRam.size();
                rams[ramCount++] = bufferRam;
            }

            if (textRam != null) {
                textRam.setStartAddr(startAddr);
                startAddr += textRam.size();
                rams[ramCount++] = textRam;
            }

            if (strRam != null) {
                strRam.setStartAddr(startAddr);
                startAddr += strRam.size();
            //strRam与runtimeRam不在rams之中
            //rams[ramCount++] = strRam;
            }

            if (startAddr > START_ADDR) {
                throw new IllegalStateException("靠,内存模块这么大!");
            }
        }
    }
}
