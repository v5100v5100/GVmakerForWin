using Script.Interpreter.Event;
using Script.Interpreter.IO;
using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    public class ScreenModelImp :ScreenModel,Renderable
    {
        /// <summary>
        /// 使用大字体模式,对drawChar等有效
        /// </summary>
        public const int TEXT_BIG_TYPE = 0x80;
        /// <summary>
        /// 直接在屏幕绘图
        /// </summary>
        public const int RENDER_GRAPH_TYPE = 0x40;
        /// <summary>
        /// 针对几何图形,是否填充
        /// </summary>
        public const int RENDER_FILL_TYPE = 0x10;
        /// <summary>
        /// 图像取反
        /// </summary>
        public const int RENDER_XNOT_TYPE = 0x08;

        /// <summary>
        /// 透明copy,0为透明像素<p>
        /// 注意,0~6互斥
        /// </summary>
        public const int DRAW_TCOPY_TYPE = 6;
        /// <summary>
        /// xor
        /// </summary>
        public const int DRAW_XOR_TYPE = 5;
        /// <summary>
        /// and
        /// </summary>
        public const int DRAW_AND_TYPE = 4;
        /// <summary>
        /// or
        /// </summary>
        public const int DRAW_OR_TYPE = 3;
        /// <summary>
        /// not
        /// </summary>
        public const int DRAW_NOT_TYPE = 2;
        /// <summary>
        /// copy,绘制图形时意思为copy,绘制几何图形时采用前景色绘图
        /// </summary>
        public const int DRAW_COPY_TYPE = 1;
        /// <summary>
        /// Clear,对绘制几何图形有效,使用背景色绘图
        /// </summary>
        public const int DRAW_CLEAR_TYPE = 0;

        //////////////////////////////////////////////////////////////////////////

        /**
         * 每行所用的字节数
         */
        private const int BYTES_PER_LINE = WIDTH / 8;
        /**
         * 屏幕显存需要的字节数
         * 每行20字节,共80行
         */
        private const int BUFFER_SIZE = WIDTH * HEIGHT / 8;
        //显存与缓存数据
        private sbyte[] graphData,  bufferData;
        private RelativeRam graphRam,  bufferRam;
        private int drawMode;
        //下列变量与drawMode有关
        private bool isFill,  isGraph,  isBig;
        private sbyte[] currData;

        public ScreenModelImp() {
            graphData = new sbyte[BUFFER_SIZE];
            bufferData = new sbyte[BUFFER_SIZE];
            graphRam = new ScreenRam(this, graphData, RamConst.RAM_GRAPH_TYPE);
            bufferRam = new ScreenRam(this, bufferData, RamConst.RAM_BUFFER_TYPE);
        }

        public override bool hasRelativeRam() 
        {
            return true;
        }

        public override RelativeRam getGraphRam() 
        {
            return graphRam;
        }

        public override RelativeRam getBufferRam()
        {
            return bufferRam;
        }

        public void setDrawMode(int m) 
        {
            drawMode = m;
            isFill = (m & RENDER_FILL_TYPE) != 0;
            isGraph = (m & RENDER_GRAPH_TYPE) != 0;
            isBig = (m & TEXT_BIG_TYPE) != 0;
            currData = isGraph ? graphData : bufferData;
        }

        public void drawString(int x, int y, Getable source, int addr)
        {
            int length = 0;
            while (source.getByte(addr + length) != 0) 
            {
                length++;
            }
            drawString(x, y, source, addr, length);
        }

        public void drawString(int x, int y, Getable source, int addr, int length)
        {
            //这个调用drawRegion时会修改区域,所以这儿不需要再修改区域
            sbyte[] data = isBig ? new sbyte[32] : new sbyte[24];
            int h = isBig ? 16 : 12;
            Getable getter = Util.asAccessable(data);
            while (length > 0)
            {
                UInt16 c = (UInt16)(source.getByte(addr++) & 0xff);
                length--;
                if (c >= 0x80 && length > 0) {
                    //c |= source.getByte(addr++) << 8;
                    c |= (UInt16) (source.getByte(addr++) << 8);
                    length--;
                }
                int w = (isBig ? Util.getGB16Data(c, data) : Util.getGB12Data(c, data)) / 2;
                drawRegion(x, y, w, h, getter, 0);
                x += w;
            }
        }

        public void drawRect(int x0, int y0, int x1, int y1)
        {
            if (x0 > x1) {
                int tmp = x1;
                x1 = x0;
                x0 = tmp;
            }
            if (y0 > y1) {
                int tmp = y1;
                y1 = y0;
                y0 = tmp;
            }
            if (x0 >= WIDTH || x1 < 0) {
                return;
            }
            if (y0 >= HEIGHT || y1 < 0) {
                return;
            }

            if (isFill) {
                x0 = x0 < 0 ? 0 : x0;
                x1 = x1 >= WIDTH ? WIDTH - 1 : x1;
                y0 = y0 < 0 ? 0 : y0;
                y1 = y1 >= HEIGHT ? HEIGHT - 1 : y1;
                for (int y = y0; y <= y1; y++) {
                    hLine(x0, x1, y);
                }
                addPoint(x1, y1);
                addPoint(x0, y0);
            } else {
                if (y0 >= 0) {
                    addPoint(x0 >= 0 ? x0 : 0, y0);
                    addPoint(x1 < WIDTH ? x1 : WIDTH - 1, y0);
                    hLine(x0, x1, y0);
                }
                if (y1 > y0 && y1 < HEIGHT) {
                    addPoint(x0 >= 0 ? x0 : 0, y1);
                    addPoint(x1 < WIDTH ? x1 : WIDTH - 1, y1);
                    hLine(x0, x1, y1);
                }
                if (y1 > y0 + 1) {
                    drawLine(x0, y0 + 1, x0, y1 - 1);
                    drawLine(x1, y0 + 1, x1, y1 - 1);
                }
            }
        }

        /**
         * 做水平线
         */
        private void hLine(int x1, int x2, int y) 
        {
            if (y < 0 || y >= HEIGHT) 
            {
                return;
            }
            if (x1 > x2) 
            {
                int tmp = x1;
                x1 = x2;
                x2 = tmp;
            }
            if (x1 >= WIDTH || x2 < 0) 
            {
                return;
            }
            if (x1 < 0)
            {
                x1 = 0;
            }
            if (x2 >= WIDTH)
            {
                x2 = WIDTH - 1;
            }
            int start = BYTES_PER_LINE * y + (x1 >> 3);
            int end = BYTES_PER_LINE * y + (x2 >> 3);
            for (int index = start; index <= end; index++)
            {
                int mask = 0;
                if (index == start) {
                    mask |= maskH[x1 & 0x07];
                }
                if (index == end) {
                    mask |= maskT[7 - (x2 & 0x07)];
                }
                switch (drawMode & 0x03) {
                    case DRAW_CLEAR_TYPE:
                        //currData[index] &= mask; // vliu注释掉的
                        break;
                    case DRAW_COPY_TYPE:
                        //currData[index] |= ~mask;// vliu注释掉的
                        break;
                    case DRAW_NOT_TYPE:
                        //currData[index] ^= ~ mask; // vliu注释掉的
                        break;
                }
            }
        }

        private void ovalPoint(int ox, int oy, int x, int y)
        {
            drawPointImp(ox - x, oy - y);
            drawPointImp(ox - x, oy + y);
            drawPointImp(ox + x, oy - y);
            drawPointImp(ox + x, oy + y);
        }

        public void drawOval(int ox, int oy, int a, int b) 
        {
            if (ox + a < 0 || ox - a >= WIDTH)
            {
                return;
            }
            if (oy + b < 0 || oy - b >= HEIGHT)
            {
                return;
            }
            //添加到修改区域
            if (isGraph)
            {
                addPoint(Math.Max(0, ox - a), Math.Max(0, oy - b));
                addPoint(Math.Min(WIDTH - 1, ox + a), Math.Min(HEIGHT - 1, oy + b));
            }
            int asq = a * a, bsq = b * b;
            int asq2 = asq * 2, bsq2 = bsq * 2;
            int p;
            int x = 0, y = b;
            int px = 0, py = asq2 * y;
            int n = 1;
            p = bsq - asq * b + ((asq + 2) >> 2);
            while (px < py) {
                x++;
                px += bsq2;
                if (p < 0) {
                    p += bsq + px;
                } else {
                    if (isFill) {
                        hLine(ox - x + 1, ox + x - 1, oy + y);
                        hLine(ox - x + 1, ox + x - 1, oy - y);
                    }
                    y--;
                    py -= asq2;
                    p += bsq + px - py;
                }
                if (!isFill) {
                    ovalPoint(ox, oy, x, y);
                }

            }
            if (isFill) {
                hLine(ox - x, ox + x, oy + y);
                hLine(ox - x, ox + x, oy - y);
            }
            p = bsq * x * x + bsq * x + asq * (y - 1) * (y - 1) - asq * bsq + ((bsq + 2) >> 2);
            while (--y > 0) {
                py -= asq2;
                if (p > 0) {
                    p += asq - py;
                } else {
                    x++;
                    px += bsq2;
                    p += asq - py + px;
                }
                if (isFill) {
                    hLine(ox - x, ox + x, oy + y);
                    hLine(ox - x, ox + x, oy - y);
                } else {
                    ovalPoint(ox, oy, x, y);
                }
            }
            if (isFill) {
                hLine(ox - a, ox + a, oy);
            } else {
                drawPointImp(ox, oy + b);
                drawPointImp(ox, oy - b);
                drawPointImp(ox + a, oy);
                drawPointImp(ox - a, oy);
            }
        }

        public void drawPoint(int x, int y) {
            if (drawPointImp(x, y) && isGraph) {
                addPoint(x, y);
            }
        }

        /**
         * 与drawPoint不同的是,这个方法不修改区域
         * @return true,如果绘制了该点
         */
        private bool drawPointImp(int x, int y) 
        {
            if (x < 0 || y < 0 || x >= WIDTH || y >= HEIGHT) 
            {
                return false;
            }
            //int offset = y * 20 + (x >>> 3);
            int offset = y * 20 + (TypeConverter.UnsignedRightMove(x,3));
            int mask = 0x80 >> (x & 0x07);
            switch (drawMode & 0x03) {
                case DRAW_CLEAR_TYPE:
                    //currData[offset] &= ~mask;//vliu注释掉的
                    break;
                case DRAW_COPY_TYPE:
                    //currData[offset] |= mask;//vliu注释掉的
                    break;
                case DRAW_NOT_TYPE:
                    //currData[offset] ^= mask;//vliu注释掉的
                    break;
            }
            return true;
        }

        public int getPoint(int x, int y) 
        {
            //int offset = y * BYTES_PER_LINE + (x >>> 3);
            int offset = y * BYTES_PER_LINE + (TypeConverter.UnsignedRightMove(x,3));
            if (offset < 0 || offset >= BUFFER_SIZE)
            {
                return 0;
            }
            int mask = 0x80 >> (x & 0x07);
            return graphData[offset] & mask;
        }

        public void drawLine(int x0, int y0, int x1, int y1)
        {
            if (x0 > x1)
            {
                int tmp = x1;
                x1 = x0;
                x0 = tmp;
                tmp = y0;
                y0 = y1;
                y1 = tmp;
            }
            int detx = x1 - x0, dety = y1 - y0;
            int sign = 1;
            if (dety < 0) {
                sign = -1;
                dety = -dety;
            }
            int qx = 0, qy = 0, px = 0, py = 0;
            bool flags = false;

            int detm = detx > dety ? detx : dety;
            int step = detm, tx = 0, ty = 0;
            while (step-- >= 0) {
                if (drawPointImp(x0, y0) ^ flags) {
                    if (flags) {
                        break;
                    } else {
                        qx = x0;
                        qy = y0;
                        flags = true;
                    }
                }
                px = x0;
                py = y0;
                tx += detx;
                ty += dety;
                if (tx >= detm) {
                    x0++;
                    tx -= detm;
                }
                if (ty >= detm) {
                    y0 += sign;
                    ty -= detm;
                }
            }

            //添加到修改区域
            if (isGraph && flags) {
                addPoint(qx, py);
                addPoint(px, py);
                addPoint(px, qy);
                addPoint(qx, qy);
            }
        }

        public void drawRegion(int x, int y, int width, int height, Getable source, int addr)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }
            if (x >= WIDTH || y >= HEIGHT || x + width < 0 || y + height < 0)
            {
                return;
            }
            //每行数据占用byte数
            //int bytePerLine = (width + 7) >>> 3;
            int bytePerLine = TypeConverter.UnsignedRightMove((width + 7),3);
            //每行开始第一个数据前无用的bit数
            int unuseDataBits = 0;
            if (x < 0) {
                addr += (-x) / 8;
                unuseDataBits = (-x) % 8;
                width += x;
                x = 0;
            }
            if (y < 0) {
                addr += -bytePerLine * y;
                height += y;
                y = 0;
            }
            if (x + width > WIDTH) {
                width -= x + width - WIDTH;
            }
            if (y + height > HEIGHT) {
                height -= y + height - HEIGHT;
            }

            //如果是屏幕绘图,添加到修改区域
            if (isGraph) {
                addPoint(x, y);
                addPoint(x + width - 1, y + height - 1);
            }

            //绘制处前无用的bit数
            int unuseScreenBits = x % 8;
            //绘制开始地址
            int offset = BYTES_PER_LINE * y + x / 8;
            //实际每行用到数据的byte数
            int count = (unuseDataBits + width + 7) / 8;
            //实际绘制影响到的byte数
            int size = (unuseScreenBits + width + 7) / 8;
            //绘制结尾剩下的bit数
            int remain = size * 8 - unuseScreenBits - width;
            //用于存储图像数据
            sbyte[] mapData = new sbyte[count + 1];
            while (height-- > 0) {
                for (int index = 0; index < count; index++) {
                    mapData[index] = source.getByte(addr + index);
                }
                addr += bytePerLine;
                adjustData(mapData, 0, mapData.Length, unuseDataBits - unuseScreenBits);
                for (int index = 0; index < size; index++) {
                    int s = mapData[index], d = currData[offset + index];
                    int mask = 0;
                    if (index == 0) {
                        mask |= maskH[unuseScreenBits];

                    }
                    if (index == size - 1) {
                        mask |= maskT[remain];
                    }
                    if ((drawMode & 0x08) != 0) {
                        s ^= 0xff;
                        if ((drawMode & 0x07) == 2) {
                            drawMode &= ~0x07;
                        }
                    }
                    s &= ~mask;
                    d &= ~mask;
                    //currData[offset + index] &= mask;//vliu注释掉的
                    switch (drawMode & 0x07) {
                        case 2:
                            s ^= ~mask;
                            break;
                        case 3:
                            s |= d;
                            break;
                        case 4:
                            s &= d;
                            break;
                        case 5:
                            s ^= d;
                            break;

                    }
                    //currData[offset + index] |= s;//vliu注释掉的
                }//for

                offset += BYTES_PER_LINE;
            }
        }
        private  byte[] maskH = { (byte)0x00, (byte)0x80, (byte)0xc0, (byte)0xe0, (byte)0xf0, (byte)0xf8, (byte)0xfc, (byte)0xfe };
        private  byte[] maskT = { (byte)0x00, (byte)0x01, (byte)0x03, (byte)0x07, (byte)0x0f, (byte)0x1f, (byte)0x3f, (byte)0x7f };

        /**
         * 将data中offset起始处的length个byte向左或向右移动det个bit
         * @param data 需要修改的数据
         * @param offset 偏移地址
         * @param length 长度
         * @param det 为正则向左移动,负则向右移动
         */
        private void adjustData(sbyte[] data, int offset, int length, int det) {
            if (det == 0) {
                return;
            }
            bool pre = false, cur;
            while (det < 0) {
                for (int index = 0; index < length; index++) {
                    cur = (data[index + offset] & 1) != 0;
                    data[index + offset] >>= 1;
                    if (pre) {
                        //data[index + offset] |= 0x80;//vliu注释掉的
                    } else {
                        data[index + offset] &= 0x7f;
                    }
                    pre = cur;
                }
                det++;
            }
            pre = false;
            while (det > 0) {
                for (int index = length - 1; index >= 0; index--) {
                    cur = data[index + offset] < 0;
                    data[index + offset] <<= 1;
                    if (pre) {
                        data[index + offset] |= 0x01;
                    }
                    pre = cur;
                }
                det--;
            }
        }

        /**
         * 该方法不对x,y,width,height检查,可能会抛出IndexOutOfBoundsException
         */
        public int getRegion(int x, int y, int width, int height, Setable dest, int addr)
        {
            //每行占用的byte数,忽略低3位
            int bytePerLine = width / 8;
            //图片数据在显存中开始地址,x忽略低三位
            int dataOffset = y * BYTES_PER_LINE + x / 8;
            int size = height * bytePerLine;
            while (height-- > 0) {
                for (int byteCount = 0; byteCount < bytePerLine; byteCount++) {
                    dest.setByte(addr++, currData[dataOffset + byteCount]);
                }
                dataOffset += BYTES_PER_LINE;

            }
            return size;
        }

        public void clearBuffer() {
            bufferRam.Clear();
        }

        public void refresh() {
            //System.arraycopy(bufferData, 0, graphData, 0, BUFFER_SIZE);
            Array.Copy(bufferData, 0, graphData, 0, BUFFER_SIZE);
            sx = sy = 0;
            ex = getWidth() - 1;
            ey = getHeight() - 1;
            isClear = false;
            fireScreenChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public void xdraw(int mode)
        {
            switch (mode & 0x07)
            {
                case 0:
                    {
                        int byteOffset = 0;
                        for (int h = 0; h < HEIGHT; h++)
                        {
                            adjustData(bufferData, byteOffset, BYTES_PER_LINE, 1);
                            byteOffset += BYTES_PER_LINE;
                        }
                    }
                    break;
                case 1:
                    {
                        int byteOffset = 0;
                        for (int h = 0; h < HEIGHT; h++)
                        {
                            adjustData(bufferData, byteOffset, BYTES_PER_LINE, -1);
                            byteOffset += BYTES_PER_LINE;
                        }
                    }
                    break;
                case 4:
                     {
                        int byteOffset = 0;
                        for (int h = 0; h < HEIGHT; h++)
                        {
                            for (int index = 0; index < BYTES_PER_LINE / 2; index++) {
                                int tmp = bufferData[byteOffset + BYTES_PER_LINE - 1 - index] & 0xff;
                                //bufferData[byteOffset + BYTES_PER_LINE - 1 - index] = REVERSE_TAB[bufferData[byteOffset + index] & 0xff];//vliu注释掉的
                                //bufferData[byteOffset + index] = REVERSE_TAB[tmp];//vliu注释掉的
                            }
                            byteOffset += BYTES_PER_LINE;
                        }
                    }
                    break;
                case 5:
                     {
                        for (int h = 0; h < HEIGHT / 2; h++)
                        {
                            int topOffset = h * BYTES_PER_LINE;
                            int bottomOffset = (HEIGHT - 1 - h) * BYTES_PER_LINE;
                            for (int index = 0; index < BYTES_PER_LINE; index++) {
                                sbyte tmp = bufferData[topOffset + index];
                                bufferData[topOffset + index] = bufferData[bottomOffset + index];
                                bufferData[bottomOffset + index] = tmp;
                            }
                        }
                    }
                    break;

            }
        }
        //字节反转表
        private static byte[] REVERSE_TAB = {
        (byte) 0x00, (byte) 0x80, (byte) 0x40, (byte) 0xc0, (byte) 0x20, (byte) 0xa0, (byte) 0x60, (byte) 0xe0,
        (byte) 0x10, (byte) 0x90, (byte) 0x50, (byte) 0xd0, (byte) 0x30, (byte) 0xb0, (byte) 0x70, (byte) 0xf0,
        (byte) 0x08, (byte) 0x88, (byte) 0x48, (byte) 0xc8, (byte) 0x28, (byte) 0xa8, (byte) 0x68, (byte) 0xe8,
        (byte) 0x18, (byte) 0x98, (byte) 0x58, (byte) 0xd8, (byte) 0x38, (byte) 0xb8, (byte) 0x78, (byte) 0xf8,
        (byte) 0x04, (byte) 0x84, (byte) 0x44, (byte) 0xc4, (byte) 0x24, (byte) 0xa4, (byte) 0x64, (byte) 0xe4,
        (byte) 0x14, (byte) 0x94, (byte) 0x54, (byte) 0xd4, (byte) 0x34, (byte) 0xb4, (byte) 0x74, (byte) 0xf4,
        (byte) 0x0c, (byte) 0x8c, (byte) 0x4c, (byte) 0xcc, (byte) 0x2c, (byte) 0xac, (byte) 0x6c, (byte) 0xec,
        (byte) 0x1c, (byte) 0x9c, (byte) 0x5c, (byte) 0xdc, (byte) 0x3c, (byte) 0xbc, (byte) 0x7c, (byte) 0xfc,
        (byte) 0x02, (byte) 0x82, (byte) 0x42, (byte) 0xc2, (byte) 0x22, (byte) 0xa2, (byte) 0x62, (byte) 0xe2,
        (byte) 0x12, (byte) 0x92, (byte) 0x52, (byte) 0xd2, (byte) 0x32, (byte) 0xb2, (byte) 0x72, (byte) 0xf2,
        (byte) 0x0a, (byte) 0x8a, (byte) 0x4a, (byte) 0xca, (byte) 0x2a, (byte) 0xaa, (byte) 0x6a, (byte) 0xea,
        (byte) 0x1a, (byte) 0x9a, (byte) 0x5a, (byte) 0xda, (byte) 0x3a, (byte) 0xba, (byte) 0x7a, (byte) 0xfa,
        (byte) 0x06, (byte) 0x86, (byte) 0x46, (byte) 0xc6, (byte) 0x26, (byte) 0xa6, (byte) 0x66, (byte) 0xe6,
        (byte) 0x16, (byte) 0x96, (byte) 0x56, (byte) 0xd6, (byte) 0x36, (byte) 0xb6, (byte) 0x76, (byte) 0xf6,
        (byte) 0x0e, (byte) 0x8e, (byte) 0x4e, (byte) 0xce, (byte) 0x2e, (byte) 0xae, (byte) 0x6e, (byte) 0xee,
        (byte) 0x1e, (byte) 0x9e, (byte) 0x5e, (byte) 0xde, (byte) 0x3e, (byte) 0xbe, (byte) 0x7e, (byte) 0xfe,
        (byte) 0x01, (byte) 0x81, (byte) 0x41, (byte) 0xc1, (byte) 0x21, (byte) 0xa1, (byte) 0x61, (byte) 0xe1,
        (byte) 0x11, (byte) 0x91, (byte) 0x51, (byte) 0xd1, (byte) 0x31, (byte) 0xb1, (byte) 0x71, (byte) 0xf1,
        (byte) 0x09, (byte) 0x89, (byte) 0x49, (byte) 0xc9, (byte) 0x29, (byte) 0xa9, (byte) 0x69, (byte) 0xe9,
        (byte) 0x19, (byte) 0x99, (byte) 0x59, (byte) 0xd9, (byte) 0x39, (byte) 0xb9, (byte) 0x79, (byte) 0xf9,
        (byte) 0x05, (byte) 0x85, (byte) 0x45, (byte) 0xc5, (byte) 0x25, (byte) 0xa5, (byte) 0x65, (byte) 0xe5,
        (byte) 0x15, (byte) 0x95, (byte) 0x55, (byte) 0xd5, (byte) 0x35, (byte) 0xb5, (byte) 0x75, (byte) 0xf5,
        (byte) 0x0d, (byte) 0x8d, (byte) 0x4d, (byte) 0xcd, (byte) 0x2d, (byte) 0xad, (byte) 0x6d, (byte) 0xed,
        (byte) 0x1d, (byte) 0x9d, (byte) 0x5d, (byte) 0xdd, (byte) 0x3d, (byte) 0xbd, (byte) 0x7d, (byte) 0xfd,
        (byte) 0x03, (byte) 0x83, (byte) 0x43, (byte) 0xc3, (byte) 0x23, (byte) 0xa3, (byte) 0x63, (byte) 0xe3,
        (byte) 0x13, (byte) 0x93, (byte) 0x53, (byte) 0xd3, (byte) 0x33, (byte) 0xb3, (byte) 0x73, (byte) 0xf3,
        (byte) 0x0b, (byte) 0x8b, (byte) 0x4b, (byte) 0xcb, (byte) 0x2b, (byte) 0xab, (byte) 0x6b, (byte) 0xeb,
        (byte) 0x1b, (byte) 0x9b, (byte) 0x5b, (byte) 0xdb, (byte) 0x3b, (byte) 0xbb, (byte) 0x7b, (byte) 0xfb,
        (byte) 0x07, (byte) 0x87, (byte) 0x47, (byte) 0xc7, (byte) 0x27, (byte) 0xa7, (byte) 0x67, (byte) 0xe7,
        (byte) 0x17, (byte) 0x97, (byte) 0x57, (byte) 0xd7, (byte) 0x37, (byte) 0xb7, (byte) 0x77, (byte) 0xf7,
        (byte) 0x0f, (byte) 0x8f, (byte) 0x4f, (byte) 0xcf, (byte) 0x2f, (byte) 0xaf, (byte) 0x6f, (byte) 0xef,
        (byte) 0x1f, (byte) 0x9f, (byte) 0x5f, (byte) 0xdf, (byte) 0x3f, (byte) 0xbf, (byte) 0x7f, (byte) 0xff
    };

        private int whiteColor = 0x40C040,  blackColor = 0;

        public override void setColor(int black, int white) 
        {
            blackColor = black;
            whiteColor = white;
        }

        public override Renderable getRender()
        {
            return this;
        }

        public override void  refreshArea() 
        {
            sx = sy = 0;
            ex = ey = -1;
            isClear = true;
        }

        public override void addToChangedArea(Area add)
        {
            if (add == null || add.isEmpty())
            {
                return;
            }
            addPoint(add.getX(), add.getY());
            addPoint(add.getX() + add.getWidth() - 1, add.getY() + add.getHeight() - 1);
        }

        public override Area getChangedArea()
        {
            return new Area(sx, sy, ex - sx + 1, ey - sy + 1);
        }

        public override int[] getRGB(int[] buffer, Area area, int rate, int circ) 
        {
            if (area.isEmpty()) {
                return buffer;
            }
            int x = area.getX() & (~0x07);
            int w = (area.getX() - x + area.getWidth() + 0x07) & (~0x07);
            int y = area.getY();
            int h = area.getHeight();
            //int bytePreLine = w >>> 3;
            int bytePreLine = TypeConverter.UnsignedRightMove(w,3);

            //正常情况
            if (rate == 1 && circ == 0) {
                //int byteOffset = y * BYTES_PER_LINE + (x >>> 3) + bytePreLine - 1;
                int byteOffset = y * BYTES_PER_LINE + (TypeConverter.UnsignedRightMove(x,3)) + bytePreLine - 1;

                int rgbOffset = y * WIDTH + x + w - 1;
                while (--h >= 0) {
                    int offset = rgbOffset;
                    for (int n = bytePreLine - 1; n >= 0; n--, byteOffset--) {
                        sbyte b = graphData[byteOffset];
                        for (int bits = 7; bits >= 0; bits--, offset--) {
                            buffer[offset] = (b & 0x01) == 0 ? whiteColor : blackColor;
                            b >>= 1;
                        }
                    }
                    rgbOffset += WIDTH;
                    byteOffset += BYTES_PER_LINE + bytePreLine;
                }
            } //放大两倍旋转90度
            else if (rate == 2 && circ == 1) {
                //int byteOffset = y * BYTES_PER_LINE + (x >>> 3) + bytePreLine - 1;
                int byteOffset = y * BYTES_PER_LINE + (TypeConverter.UnsignedRightMove(x,3)) + bytePreLine - 1;
                int rgbOffset = (WIDTH - 1) * (HEIGHT << 2) - (HEIGHT << 2) * x + (y << 1);
                while (h-- > 0) {
                    int offset = rgbOffset - (w - 1) * (HEIGHT << 2);
                    for (int n = bytePreLine; n > 0; n--, byteOffset--) {
                        sbyte b = graphData[byteOffset];
                        for (int bits = 8; bits > 0; bits--, offset += HEIGHT << 2) {
                            if ((b & 0x01) == 0) {
                                buffer[offset] = whiteColor;
                                buffer[offset + 1] = whiteColor;
                                buffer[offset + (HEIGHT << 1)] = whiteColor;
                                buffer[offset + (HEIGHT << 1) + 1] = whiteColor;
                            } else {
                                buffer[offset] = blackColor;
                                buffer[offset + 1] = blackColor;
                                buffer[offset + (HEIGHT << 1)] = blackColor;
                                buffer[offset + (HEIGHT << 1) + 1] = blackColor;
                            }
                            b >>= 1;
                        }
                    }
                    rgbOffset += 2;
                    byteOffset += BYTES_PER_LINE + bytePreLine;
                }
            } //放大两倍旋转270度
            else if (rate == 2 && circ == 3) {
                //int byteOffset = y * BYTES_PER_LINE + (x >>> 3) + bytePreLine - 1;
                int byteOffset = y * BYTES_PER_LINE + (TypeConverter.UnsignedRightMove(x,3)) + bytePreLine - 1;
                int rgbOffset = x * (HEIGHT << 2) + 2 * (HEIGHT - 1) - (y << 1);
                while (h-- > 0) {
                    int offset = rgbOffset + (w - 1) * (HEIGHT << 2);
                    for (int n = bytePreLine; n > 0; n--, byteOffset--) {
                        sbyte b = graphData[byteOffset];
                        for (int bits = 8; bits > 0; bits--, offset -= HEIGHT << 2) {
                            if ((b & 0x01) == 0) {
                                buffer[offset] = whiteColor;
                                buffer[offset + 1] = whiteColor;
                                buffer[offset + (HEIGHT << 1)] = whiteColor;
                                buffer[offset + (HEIGHT << 1) + 1] = whiteColor;
                            } else {
                                buffer[offset] = blackColor;
                                buffer[offset + 1] = blackColor;
                                buffer[offset + (HEIGHT << 1)] = blackColor;
                                buffer[offset + (HEIGHT << 1) + 1] = blackColor;
                            }
                            b >>= 1;
                        }
                    }
                    rgbOffset -= 2;
                    byteOffset += BYTES_PER_LINE + bytePreLine;
                }
            } else if (rate == 2 && circ == 0) {
                //int byteOffset = y * BYTES_PER_LINE + (x >>> 3) + bytePreLine - 1;
                int byteOffset = y * BYTES_PER_LINE + (TypeConverter.UnsignedRightMove(x,3)) + bytePreLine - 1;
                int rgbOffset = y * (WIDTH << 2) + (x + w - 1) * 2;
                while (--h >= 0) {
                    int offset = rgbOffset;
                    for (int n = bytePreLine - 1; n >= 0; n--, byteOffset--) {
                        sbyte b = graphData[byteOffset];
                        for (int bits = 7; bits >= 0; bits--, offset -= 2) {
                            if ((b & 0x01) == 0) {
                                buffer[offset] = whiteColor;
                                buffer[offset + 1] = whiteColor;
                                buffer[offset + (WIDTH << 1)] = whiteColor;
                                buffer[offset + (WIDTH << 1) + 1] = whiteColor;
                            } else {
                                buffer[offset] = blackColor;
                                buffer[offset + 1] = blackColor;
                                buffer[offset + (WIDTH << 1)] = blackColor;
                                buffer[offset + (WIDTH << 1) + 1] = blackColor;
                            }
                            b >>= 1;
                        }
                    }
                    rgbOffset += WIDTH << 2;
                    byteOffset += BYTES_PER_LINE + bytePreLine;
                }
            } else if (rate == 3 && circ == 0) {
                //int byteOffset = y * BYTES_PER_LINE + (x >>> 3) + bytePreLine - 1;
                int byteOffset = y * BYTES_PER_LINE + (TypeConverter.UnsignedRightMove(x,3)) + bytePreLine - 1;

                int rgbOffset = y * WIDTH * 9 + (x + w - 1) * 3;
                while (--h >= 0) {
                    int offset = rgbOffset;
                    for (int n = bytePreLine - 1; n >= 0; n--, byteOffset--) {
                        sbyte b = graphData[byteOffset];
                        for (int bits = 7; bits >= 0; bits--, offset -= 3) {
                            if ((b & 0x01) == 0) {
                                buffer[offset] = whiteColor;
                                buffer[offset + 1] = whiteColor;
                                buffer[offset + 2] = whiteColor;
                                buffer[offset + WIDTH * 3] = whiteColor;
                                buffer[offset + WIDTH * 3 + 1] = whiteColor;
                                buffer[offset + WIDTH * 3 + 2] = whiteColor;
                                buffer[offset + WIDTH * 6] = whiteColor;
                                buffer[offset + WIDTH * 6 + 1] = whiteColor;
                                buffer[offset + WIDTH * 6 + 2] = whiteColor;
                            } else {
                                buffer[offset] = blackColor;
                                buffer[offset + 1] = blackColor;
                                buffer[offset + 2] = blackColor;
                                buffer[offset + WIDTH * 3] = blackColor;
                                buffer[offset + WIDTH * 3 + 1] = blackColor;
                                buffer[offset + WIDTH * 3 + 2] = blackColor;
                                buffer[offset + WIDTH * 6] = blackColor;
                                buffer[offset + WIDTH * 6 + 1] = blackColor;
                                buffer[offset + WIDTH * 6 + 2] = blackColor;
                            }
                            b >>= 1;
                        }
                    }
                    rgbOffset += WIDTH * 9;
                    byteOffset += BYTES_PER_LINE + bytePreLine;
                }
            }
            return buffer;
        }

        /**
         * 往变化区域里添加一个点<p>
         * 注意,该方法不对x,y进行检查,调用者应确保x,y的有效性
         */
        private void addPoint(int x, int y) {
            if (isClear) {
                //第一次加入点
                isClear = false;
                sx = ex = x;
                sy = ey = y;
                return;
            }

            if (x < sx) {
                sx = x;
            } else if (x > ex) {
                ex = x;
            }

            if (y < sy) {
                sy = y;
            } else if (y > ey) {
                ey = y;
            }

        }
        //sx,sy为区域的左上角那个点,ex,ey为右下角那个点,注意都包含在内
        private int sx = 0,  sy = 0,  ex = -1,  ey = -1;
        //如果区域内还没有任何点
        private bool isClear = true;
    }
}
