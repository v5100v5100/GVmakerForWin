using Script.Interpreter.Event;
using Script.Interpreter.IO;
using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Script.Interpreter
{
    public class VirtualMachine
    {
        private const int FALSE = 0;
        private const int TRUE = -1;
        private GvmConfig config;
        private RamManager ramManager;
        private RuntimeRam runtimeRam;
        private StringRam stringRam;
        private ScreenModel screen;
        private Renderable render;
        private TextModel text;
        private KeyModel key;
        private FileModel file;
        private KeyModelSysInfo keyInf;
        private InputMethod input;
        private LavApp app;
        private Stack stack;
        private int seed;
        private bool end;
        //private Calendar cal = Calendar.getInstance();
        //private Date date = new Date();


        public VirtualMachine(GvmConfig cfg, FileModel fileModel, KeyModel keyModel)
        {
            this.config = cfg;
            runtimeRam = new RuntimeRam(cfg.runtimeRamSize());
            stringRam = new StringRam(cfg.stringRamSize());
            stack = new Stack(cfg.stackSize());
            ramManager = new RamManager(runtimeRam, stringRam, stack);

            text = new TextModel();
            screen = ScreenModel.newScreenModel();
            render = screen.getRender();

            text.setScreenModel(screen);
            if (screen.hasRelativeRam()) {
                ramManager.install(screen.getGraphRam());
                ramManager.install(screen.getBufferRam());
            }
            if (text.hasRelativeRam()) {
                ramManager.install(text.getTextRam());
            }
            key = keyModel;
            keyInf = key.getSysInfo();
            file = fileModel;
        }

        /**
         * 设置此GVM运行的lav程序文件,并对JGVM做适当的初始化
         * @param app GVmaker程序
         * @throws IllegalStateException 如果不支持此app的运行
         */
        public void loadApp(LavApp app)
        {
            if (this.app != null) 
            {
                dispose();
            }
            this.app = app;
            end = false;
        }

        /**
         * 卸去目前执行的app,并释放及清理相应资源
         */
        public void dispose()
        {
            if (this.app == null)
            {
                return;
            }
            file.dispose();
            render.clearBuffer();
            render.refresh();
            ramManager.clear();
            text.setTextMode(0);
            this.app = null;
            this.end = true;
        }

        public bool isEnd()
        {
            return end;
        }

        public InputMethod setInputMethod(InputMethod im) 
        {
            InputMethod oldValue = input;
            input = im;
            return oldValue;
        }

        /**
         * 运行下一个指令
         * @throws IllegalStateException 程序已经结束或不支持的操作
         * @throws InterruptedException 运行期间当前线程被中断
         */
        public void nextStep()
        {
            if (isEnd())
            {
                //throw new IllegalStateException("程序已经终止!");
            }
            int cmd = app.getChar();
            //System.out.println(Integer.toHexString(cmd));
            switch (cmd) {
                case 0x01:
                    stack.push(app.getChar());
                    break;
                case 0x02:
                    stack.push(app.getInt());
                    break;
                case 0x03:
                    stack.push(app.getLong());
                    break;
                case 0x04:
                    stack.push(ramManager.getChar(app.getInt() & 0xffff));
                    break;
                case 0x05:
                    stack.push(ramManager.getInt(app.getInt() & 0xffff));
                    break;
                case 0x06:
                    stack.push(ramManager.getLong(app.getInt() & 0xffff));
                    break;
                case 0x07:
                    stack.push(ramManager.getChar((stack.pop() + app.getInt()) & 0xffff));
                    break;
                case 0x08:
                    stack.push(ramManager.getInt((stack.pop() + app.getInt()) & 0xffff));
                    break;
                case 0x09:
                    stack.push(ramManager.getLong((stack.pop() + app.getInt()) & 0xffff));
                    break;
                case 0x0a:
                    stack.push((app.getInt() + stack.pop()) & 0xffff | 0x00010000);
                    break;
                case 0x0b:
                    stack.push((app.getInt() + stack.pop()) & 0xffff | 0x00020000);
                    break;
                case 0x0c:
                    stack.push((app.getInt() + stack.pop()) & 0xffff | 0x00040000);
                    break;
                case 0x0d:
                    stack.push(stringRam.addString(app) | 0x00100000);
                    break;
                case 0x0e:
                    stack.push(ramManager.getChar((app.getInt() + runtimeRam.getRegionStartAddr()) & 0xffff));
                    break;
                case 0x0f:
                    stack.push(ramManager.getInt((app.getInt() + runtimeRam.getRegionStartAddr()) & 0xffff));
                    break;
                case 0x10:
                    stack.push(ramManager.getLong((app.getInt() + runtimeRam.getRegionStartAddr()) & 0xffff));
                    break;
                case 0x11:
                    stack.push(ramManager.getChar((app.getInt() + stack.pop() + runtimeRam.getRegionStartAddr()) & 0xffff));
                    break;
                case 0x12:
                    stack.push(ramManager.getInt((app.getInt() + stack.pop() + runtimeRam.getRegionStartAddr()) & 0xffff));
                    break;
                case 0x13:
                    stack.push(ramManager.getLong((app.getInt() + stack.pop() + runtimeRam.getRegionStartAddr()) & 0xffff));
                    break;
                case 0x14:
                    stack.push((app.getInt() + stack.pop() + runtimeRam.getRegionStartAddr()) & 0xffff | 0x00010000);
                    break;
                case 0x15:
                    stack.push((app.getInt() + stack.pop() + runtimeRam.getRegionStartAddr()) & 0xffff | 0x00020000);
                    break;
                case 0x16:
                    stack.push((app.getInt() + stack.pop() + runtimeRam.getRegionStartAddr()) & 0xffff | 0x00040000);
                    break;
                case 0x17:
                    stack.push((app.getInt() + stack.pop()) & 0xffff);
                    break;
                case 0x18:
                    stack.push((app.getInt() + stack.pop() + runtimeRam.getRegionStartAddr()) & 0xffff);
                    break;
                case 0x19:
                    stack.push((app.getInt() + runtimeRam.getRegionStartAddr()) & 0xffff);
                    break;
                case 0x1a:
                    stack.push(text.getTextRam().getStartAddr());
                    break;
                case 0x1b:
                    stack.push(screen.getGraphRam().getStartAddr());
                    break;
                case 0x1c:
                    stack.push(-stack.pop());
                    break;
                case 0x1d:
                case 0x1e:
                case 0x1f:
                case 0x20:
                     {
                        int data = stack.pop();
                        int addr = data & 0xffff;
                        if ((data & 0x00800000) != 0) {
                            addr += runtimeRam.getRegionStartAddr();
                        }
                        //int len = (data >>> 16) & 0x7f;
                        int len = (TypeConverter.UnsignedRightMove(data,16)) & 0x7f;
                        int value = ramManager.getBytes(addr, len);
                        if (len == 2) {
                            //lvm的int为有符号两字节数据
                            value = (short) value;
                        }
                        switch (cmd) {
                            case 0x1d:
                                stack.push(++value);
                                break;
                            case 0x1e:
                                stack.push(--value);
                                break;
                            case 0x1f:
                                stack.push(value++);
                                break;
                            case 0x20:
                                stack.push(value--);
                                break;
                        }
                        ramManager.setBytes(addr, len, value);
                        notifyRamChanged(addr, addr + len);
                    }
                    break;
                case 0x21:
                    stack.push(stack.pop() + stack.pop());
                    break;
                case 0x22:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v2 - v1);
                    }
                    break;
                case 0x23:
                    stack.push(stack.pop() & stack.pop());
                    break;
                case 0x24:
                    stack.push(stack.pop() | stack.pop());
                    break;
                case 0x25:
                    stack.push(~stack.pop());
                    break;
                case 0x26:
                    stack.push(stack.pop() ^ stack.pop());
                    break;
                case 0x27:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push((v1 != 0 && v2 != 0) ? TRUE : FALSE);
                    }
                    break;
                case 0x28:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push((v1 != 0 || v2 != 0) ? TRUE : FALSE);
                    }
                    break;
                case 0x29:
                    stack.push(stack.pop() == 0 ? TRUE : FALSE);
                    break;
                case 0x2a:
                    stack.push(stack.pop() * stack.pop());
                    break;
                case 0x2b:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v1 == 0 ? -1 : v2 / v1);
                    }
                    break;
                case 0x2c:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v1 == 0 ? 0 : v2 % v1);
                    }
                    break;
                case 0x2d:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v2 << v1);
                    }
                    break;
                case 0x2e:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v2 >> v1);
                    }
                    break;
                case 0x2f:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v1 == v2 ? TRUE : FALSE);
                    }
                    break;
                case 0x30:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v1 != v2 ? TRUE : FALSE);
                    }
                    break;
                case 0x31:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v2 <= v1 ? TRUE : FALSE);
                    }
                    break;
                case 0x32:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v2 >= v1 ? TRUE : FALSE);
                    }
                    break;
                case 0x33:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v2 > v1 ? TRUE : FALSE);
                    }
                    break;
                case 0x34:
                     {
                        int v1 = stack.pop();
                        int v2 = stack.pop();
                        stack.push(v2 < v1 ? TRUE : FALSE);
                    }
                    break;
                case 0x35:
                     {
                        int data = stack.pop();
                        int offset = stack.pop();
                        int addr = offset & 0xffff;
                        if ((offset & 0x00800000) != 0) {
                            addr += runtimeRam.getRegionStartAddr();
                        }
                        //int len = (offset >>> 16) & 0x7f;
                        int len = (TypeConverter.UnsignedRightMove(offset,16)) & 0x7f;
                        ramManager.setBytes(addr, len, data);
                        stack.push(data);
                        notifyRamChanged(addr, addr + len);
                    }
                    break;
                case 0x36:
                    stack.push(ramManager.getChar(stack.pop() & 0xffff));
                    break;
                case 0x37:
                    stack.push(stack.pop() & 0xffff | 0x00010000);
                    break;
                case 0x38:
                    stack.pop();
                    break;
                case 0x39:
                     {
                        int addr = app.getAddr();
                        if (stack.lastValue() == 0) {
                            app.setOffset(addr);
                        }
                    }
                    break;
                case 0x3a:
                     {
                        int addr = app.getAddr();
                        if (stack.lastValue() != 0) {
                            app.setOffset(addr);
                        }
                    }
                    break;
                case 0x3b:
                    app.setOffset(app.getAddr());
                    break;
                case 0x3c:
                     {
                        int addr = app.getInt() & 0xffff;
                        runtimeRam.setRegionEndAddr(addr);
                        runtimeRam.setRegionStartAddr(addr);
                    }
                    break;
                case 0x3d:
                     {
                        //invoke
                        int nextAddr = app.getAddr();
                        int currAddr = app.getOffset();
                        ramManager.setAddr(runtimeRam.getRegionEndAddr(), currAddr);
                        app.setOffset(nextAddr);
                    }
                    break;
                case 0x3e:
                     {
                        //function entry
                        ramManager.setBytes(runtimeRam.getRegionEndAddr() + 3, 2, runtimeRam.getRegionStartAddr());
                        runtimeRam.setRegionStartAddr(runtimeRam.getRegionEndAddr());
                        runtimeRam.setRegionEndAddr(runtimeRam.getRegionStartAddr() + (app.getInt() & 0xffff));
                        int paramCount = app.getChar();
                        while (--paramCount >= 0) {
                            ramManager.setLong(runtimeRam.getRegionStartAddr() + 5 + 4 * paramCount, stack.pop());
                        }
                    }
                    break;
                case 0x3f:
                     {
                        int addr = ramManager.getAddr(runtimeRam.getRegionStartAddr());
                        runtimeRam.setRegionEndAddr(runtimeRam.getRegionStartAddr());
                        runtimeRam.setRegionStartAddr(ramManager.getInt(runtimeRam.getRegionEndAddr() + 3) & 0xffff);
                        app.setOffset(addr);
                    }
                    break;
                case 0x40:
                    end = true;
                    break;
                case 0x41:
                     {
                        int addr = app.getInt() & 0xffff;
                        int len = app.getInt() & 0xffff;
                        sbyte b;
                        while (--len >= 0) {
                            //ramManager.setChar(addr++, app.getChar());
                            //正常GVmaker中,这些数据是保存在runtimeRam中
                            b = (sbyte) app.getChar();
                            runtimeRam.setByte(addr++, b);
                        }
                    }
                    break;
                case 0x42:
                    stack.push(screen.getBufferRam().getStartAddr());
                    break;
                case 0x43:
                    //throw new IllegalStateException("未知的指令: 0x43");
                case 0x44:
                    //loadall
                    break;
                case 0x45:
                    stack.push(app.getInt() + stack.pop());
                    break;
                case 0x46:
                    stack.push(stack.pop() - app.getInt());
                    break;
                case 0x47:
                    stack.push(stack.pop() * app.getInt());
                    break;
                case 0x48:
                     {
                        int v1 = app.getInt();
                        int v2 = stack.pop();
                        stack.push(v1 == 0 ? -1 : v2 / v1);
                    }
                    break;
                case 0x49:
                     {
                        int v1 = app.getInt();
                        int v2 = stack.pop();
                        stack.push(v1 == 0 ? 0 : v2 % v1);
                    }
                    break;
                case 0x4a:
                    stack.push(stack.pop() << app.getInt());
                    break;
                case 0x4b:
                    stack.push(stack.pop() >> app.getInt());
                    break;
                case 0x4c:
                    stack.push(app.getInt() == stack.pop() ? TRUE : FALSE);
                    break;
                case 0x4d:
                    stack.push(app.getInt() != stack.pop() ? TRUE : FALSE);
                    break;
                case 0x4e:
                    stack.push(app.getInt() < stack.pop() ? TRUE : FALSE);
                    break;
                case 0x4f:
                    stack.push(app.getInt() > stack.pop() ? TRUE : FALSE);
                    break;
                case 0x50:
                    stack.push(app.getInt() <= stack.pop() ? TRUE : FALSE);
                    break;
                case 0x51:
                    stack.push(app.getInt() >= stack.pop() ? TRUE : FALSE);
                    break;

                //system function
                case 0x80:
                    text.addChar((char) (stack.pop() & 0xff));
                    text.updateLCD(0);
                    break;
                case 0x81:
                    stack.push(key.getchar());
                    break;
                case 0x82:
                    printf();
                    break;
                //strcpy
                case 0x83:
                   {
                        int source = stack.pop() & 0xffff;
                        int dest = stack.pop() & 0xffff;
                        sbyte b;
                        do {
                            b = ramManager.getByte(source++);
                            ramManager.setByte(dest++, b);
                        } while (b != 0);
                    //这个应该不会改变显存与屏幕缓冲,但可能修改文本缓冲以及读取字符堆
                    }
                    break;
                case 0x84:
                     {
                        int addr = stack.pop() & 0xffff;
                        int length = 0;
                        while (ramManager.getByte(addr++) != 0) {
                            length++;
                        }
                        stack.push(length);
                    }
                    break;
                case 0x85:
                    text.setTextMode(stack.pop() & 0xff);
                    break;
                case 0x86:
                    text.updateLCD(stack.pop());
                    break;
                case 0x87:
                     {
                        int delayTime = stack.pop() & 0x7fff;
                        if (delayTime * 3 / 4 > 0) {
                            //Thread.sleep(delayTime * 3 / 4);
                            Thread.Sleep(delayTime * 3 / 4);
                        }
                    }
                    break;
                case 0x88:
                    stack.movePointer(-6);
                    render.setDrawMode(stack.peek(4));
                    render.drawRegion((short) stack.peek(0), (short) stack.peek(1),
                            (short) stack.peek(2), (short) stack.peek(3),
                            ramManager, stack.peek(5) & 0xffff);
                    notifyScreenListener(stack.peek(4));
                    break;
                case 0x89:
                    render.refresh();
                    break;
                case 0x8a:
                    stack.movePointer(-4);
                    render.setDrawMode(stack.peek(3));
                    render.drawString((short) stack.peek(0), (short) stack.peek(1),
                            ramManager, stack.peek(2) & 0xffff);
                    notifyScreenListener(stack.peek(3));
                    break;
                case 0x8b:
                    stack.movePointer(-5);
                    render.setDrawMode(stack.peek(4) | RenderableConst.RENDER_FILL_TYPE);
                    render.drawRect((short) stack.peek(0), (short) stack.peek(1),
                            (short) stack.peek(2), (short) stack.peek(3));
                    notifyScreenListener(stack.peek(4));
                    break;
                case 0x8c:
                    stack.movePointer(-5);
                    render.setDrawMode(stack.peek(4));
                    render.drawRect((short) stack.peek(0), (short) stack.peek(1),
                            (short) stack.peek(2), (short) stack.peek(3));
                    notifyScreenListener(stack.peek(4));
                    break;
                case 0x8d:
                    //exit
                    end = true;
                    break;
                case 0x8e:
                    render.clearBuffer();
                    break;
                case 0x8f:
                     {
                        int value = stack.pop();
                        stack.push(value >= 0 ? value : -value);
                    }
                    break;
                case 0x90:
                    seed = seed * 22695477 + 1;
                    stack.push((seed >> 16) & 0x7fff);
                    break;
                case 0x91:
                    seed = stack.pop();
                    break;
                case 0x92:
                     {
                        int col = stack.pop() & 0xff;
                        int row = stack.pop() & 0xff;
                        text.setLocation(row, col);
                    }
                    break;
                case 0x93:
                    stack.push(key.inkey());
                    break;
                case 0x94:
                    stack.movePointer(-3);
                    render.setDrawMode(stack.peek(2) ^ RenderableConst.RENDER_GRAPH_TYPE);
                    render.drawPoint((short) stack.peek(0), (short) stack.peek(1));
                    notifyScreenListener(stack.peek(2) ^ RenderableConst.RENDER_GRAPH_TYPE);
                    break;
                case 0x95:
                     {
                        int y = (short) stack.pop();
                        int x = (short) stack.pop();
                        stack.push(render.getPoint(x, y));
                    }
                    break;
                case 0x96:
                    stack.movePointer(-5);
                    render.setDrawMode(stack.peek(4) ^ RenderableConst.RENDER_GRAPH_TYPE);
                    render.drawLine((short) stack.peek(0), (short) stack.peek(1),
                            (short) stack.peek(2), (short) stack.peek(3));
                    notifyScreenListener(stack.peek(4) ^ RenderableConst.RENDER_GRAPH_TYPE);
                    break;
                case 0x97:
                     {
                        stack.movePointer(-6);
                        int mode = stack.peek(5) ^ RenderableConst.RENDER_GRAPH_TYPE;
                        if ((stack.peek(4) & 0xff) != 0) {
                            mode |= RenderableConst.RENDER_FILL_TYPE;
                        }
                        render.setDrawMode(mode);
                        render.drawRect((short) stack.peek(0), (short) stack.peek(1),
                                (short) stack.peek(2), (short) stack.peek(3));
                        notifyScreenListener(mode);
                    }
                    break;
                case 0x98:
                     {
                        stack.movePointer(-5);
                        int mode = stack.peek(4) ^ RenderableConst.RENDER_GRAPH_TYPE;
                        if ((stack.peek(3) & 0xff) != 0) {
                            mode |= RenderableConst.RENDER_FILL_TYPE;
                        }
                        render.setDrawMode(mode);
                        render.drawOval((short) stack.peek(0), (short) stack.peek(1),
                                (short) stack.peek(2), (short) stack.peek(2));
                        notifyScreenListener(mode);
                    }
                    break;
                case 0x99:
                     {
                        stack.movePointer(-6);
                        int mode = stack.peek(5) ^ RenderableConst.RENDER_GRAPH_TYPE;
                        if ((stack.peek(4) & 0xff) != 0) {
                            mode |= RenderableConst.RENDER_FILL_TYPE;
                        }
                        render.setDrawMode(mode);
                        render.drawOval((short) stack.peek(0), (short) stack.peek(1),
                                (short) stack.peek(2), (short) stack.peek(3));
                        notifyScreenListener(mode);
                    }
                    break;
                case 0x9a:
                    //beep,do nothing
                    break;
                case 0x9b:
                     {
                        int c = stack.pop() & 0xff;
                        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')) {
                            stack.push(TRUE);
                        }
                        else {
                            stack.push(FALSE);
                        }
                    }
                    break;
                case 0x9c:
                     {
                        int c = stack.pop() & 0xff;
                        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                            stack.push(TRUE);
                        }
                        else {
                            stack.push(FALSE);
                        }
                    }
                    break;
                case 0x9d:
                     {
                        int c = stack.pop() & 0xff;
                        if ((c >= 0 && c <= 0x1f) || c == 0x7f) {
                            stack.push(TRUE);
                        }
                        else {
                            stack.push(FALSE);
                        }
                    }
                    break;
                case 0x9e:
                     {
                        int c = stack.pop() & 0xff;
                        stack.push((c >= '0' && c <= '9') ? TRUE : FALSE);
                    }
                    break;
                case 0x9f:
                     {
                        int c = stack.pop() & 0xff;
                        stack.push((c >= 0x21 && c <= 0x7e) ? TRUE : FALSE);
                    }
                    break;
                case 0xa0:
                     {
                        int c = stack.pop() & 0xff;
                        stack.push((c >= 'a' && c <= 'z') ? TRUE : FALSE);
                    }
                    break;
                case 0xa1:
                     {
                        int c = stack.pop() & 0xff;
                        stack.push((c >= 0x20 && c <= 0x7e) ? TRUE : FALSE);
                    }
                    break;
                //ispunct
                case 0xa2:
                     {
                        int c = stack.pop() & 0xff;
                        if (c < 0x20 || c > 0x7e) {
                            stack.push(FALSE);
                        }
                        else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
                            stack.push(FALSE);
                        }
                        else if ((c >= '0' && c <= '9') || c == 0x20) {
                            stack.push(FALSE);
                        }
                        else {
                            stack.push(TRUE);
                        }
                    }
                    break;
                case 0xa3:
                     {
                        int c = stack.pop() & 0xff;
                        if (c == 0x09 || c == 0x0a || c == 0x0b || c == 0x0c || c == 0x0d || c == 0x20) {
                            stack.push(TRUE);
                        }
                        else {
                            stack.push(FALSE);
                        }
                    }
                    break;
                case 0xa4:
                     {
                        int c = stack.pop() & 0xff;
                        stack.push((c <= 'Z' && c >= 'A') ? TRUE : FALSE);
                    }
                    break;
                case 0xa5:
                     {
                        int c = stack.pop() & 0xff;
                        if (c <= 'F' && c >= 'A') {
                            stack.push(TRUE);
                        }
                        else if (c <= 'f' && c >= 'a') {
                            stack.push(TRUE);
                        }
                        else if (c <= '9' && c >= '0') {
                            stack.push(TRUE);
                        }
                        else {
                            stack.push(FALSE);
                        }
                    }
                    break;
                //strcat
                case 0xa6:
                     {
                        //不会修改显存相关
                        int src = stack.pop() & 0xffff;
                        int dst = stack.pop() & 0xffff;
                        while (ramManager.getByte(dst) != 0) {
                            dst++;
                        }
                        byte b;
                        do {
                            b = ramManager.getByte(src++);
                            ramManager.setByte(dst++, b);
                        } while (b != 0);
                    }
                    break;
                //strchr
                case 0xa7:
                     {
                        byte c = (byte) stack.pop();
                        int addr = stack.pop() & 0xffff;
                        while (true) {
                            byte b = ramManager.getByte(addr);
                            if (b == c) {
                                break;
                            }
                            if (b == 0) {
                                addr = 0;
                                break;
                            }
                            addr++;
                        }
                        stack.push(addr);
                    }
                    break;
                case 0xa8:
                     {
                        int str2 = stack.pop() & 0xffff;
                        int str1 = stack.pop() & 0xffff;
                        int cmp = 0;
                        while (true) {
                            int c1 = ramManager.getChar(str1++);
                            int c2 = ramManager.getChar(str2++);
                            cmp = c1 - c2;
                            if (cmp != 0 || c1 == 0) {
                                break;
                            }
                        }
                        stack.push(cmp);
                    }
                    break;
                //strstr
                case 0xa9:
                     {
                        int str2 = stack.pop() & 0xffff;
                        int str1 = stack.pop() & 0xffff;
                        int addr = 0;
                        caseA9Loop:
                        while (ramManager.getByte(str1) != 0) {
                            int s1 = str1;
                            int s2 = str2;
                            while (true) {
                                if (ramManager.getByte(s2) == 0) {
                                    addr = str1;
                                    break caseA9Loop;
                                }
                                if (ramManager.getByte(s1) == 0) {
                                    break caseA9Loop;
                                }
                                if (ramManager.getByte(s1) != ramManager.getByte(s2)) {
                                    break;
                                }
                                s1++;
                                s2++;
                            }
                            str1++;
                        }
                        stack.push(addr);
                    }
                    break;
                case 0xaa:
                     {
                        int c = stack.pop() & 0xff;
                        if (c >= 'A' && c <= 'Z') {
                            c += 'a' - 'A';
                        }
                        stack.push(c);
                    }
                    break;
                case 0xab:
                     {
                        int c = stack.pop() & 0xff;
                        if (c >= 'a' && c <= 'z') {
                            c += 'A' - 'a';
                        }
                        stack.push(c);
                    }
                    break;
                case 0xac:
                     {
                        int len = (short) stack.pop();
                        byte b = (byte) stack.pop();
                        int addr = stack.pop() & 0xffff;
                        //System.out.println(addr+","+len+","+b);
                        int start = addr;
                        while (--len >= 0) 
                        {
                            ramManager.setByte(addr++, b);
                        }
                        notifyRamChanged(start, addr);
                    }
                    break;
                case 0xad:
                     {
                        int len = (short) stack.pop();
                        int str2 = stack.pop() & 0xffff;
                        int str1 = stack.pop() & 0xffff;
                        int start = str1;
                        while (--len >= 0) {
                            ramManager.setByte(str1++, ramManager.getByte(str2++));
                        }
                        notifyRamChanged(start, str1);
                    }
                    break;
                //fopen
                case 0xae:
                    stack.movePointer(-2);
                    stack.push(file.fopen(ramManager, stack.peek(0) & 0xffff, stack.peek(1) & 0xffff));
                    break;
                //fclose
                case 0xaf:
                    file.fclose(stack.pop());
                    break;
                //fread
                case 0xb0:
                     {
                        stack.movePointer(-4);
                        int v = file.fread(ramManager, stack.peek(0) & 0xffff,
                                (short) stack.peek(2), stack.peek(3));
                        stack.push(v);
                    }
                    break;
                //fwrite
                case 0xb1:
                     {
                        stack.movePointer(-4);
                        int v = file.fwrite(ramManager, stack.peek(0) & 0xffff,
                                (short) stack.peek(2), stack.peek(3));
                        stack.push(v);
                    }
                    break;
                //fseek
                case 0xb2:
                     {
                        stack.movePointer(-3);
                        int v = file.fseek(stack.peek(0), stack.peek(1), stack.peek(2));
                        stack.push(v);
                    }
                    break;
                //ftell
                case 0xb3:
                    stack.push(file.ftell(stack.pop()));
                    break;
                //feof
                case 0xb4:
                    stack.push(file.feof(stack.pop()) ? TRUE : FALSE);
                    break;
                //rewind
                case 0xb5:
                    file.rewind(stack.pop());
                    break;
                //fgetc
                case 0xb6:
                    stack.push(file.getc(stack.pop()));
                    break;
                //fputc
                case 0xb7:
                     {
                        int fp = stack.pop();
                        int ch = stack.pop();
                        stack.push(file.putc(ch, fp));
                    }
                    break;
                //sprintf
                case 0xb8:
                    sprintf();
                    break;
                //makeDir
                case 0xb9:
                    stack.push(file.makeDir(ramManager, stack.pop() & 0xffff) ? TRUE : FALSE);
                    break;
                //deletefILE
                case 0xba:
                    stack.push(file.deleteFile(ramManager, stack.pop() & 0xffff) ? TRUE : FALSE);
                    break;
                //getms
                case 0xbb:
                     {
                        //int ms = (int) (System.currentTimeMillis() % 1000);
                         int ms = (int) Environment.TickCount % 1000;
                        ms = ms * 256 / 1000;
                        stack.push(ms);
                    }
                    break;
                //checkKey
                case 0xbc:
                     {
                        char c = (char) stack.pop();
                        stack.push(key.checkKey(c));
                    }
                    break;
                //memmove
                case 0xbd:
                     {
                        int len = (short) stack.pop();
                        int src = stack.pop() & 0xffff;
                        int dst = stack.pop() & 0xffff;
                        if (src > dst) {
                            for (int index = 0; index < len; index++) {
                                ramManager.setByte(dst + index, ramManager.getByte(src + index));
                            }
                        }
                        else {
                            for (int index = len - 1; index >= 0; index--) {
                                ramManager.setByte(dst + index, ramManager.getByte(src + index));
                            }
                        }
                        notifyRamChanged(dst, dst + len);
                    }
                    break;
                //crc16
                case 0xbe:
                     {
                        int length = (short) stack.pop();
                        int addr = stack.pop() & 0xffff;
                        stack.push(Util.getCrc16Value(ramManager, addr, length));
                    }
                    break;
                //secret
                case 0xbf:
                     {
                        int strAddr = stack.pop() & 0xffff;
                        int length = (short) stack.pop();
                        int memAddr = stack.pop() & 0xffff;
                        int index = 0;
                        while (--length >= 0) {
                            sbyte mask = ramManager.getByte(strAddr + index);
                            if (mask == 0) {
                                index = 0;
                                mask = ramManager.getByte(strAddr + index);
                            }
                            sbyte value = ramManager.getByte(memAddr);
                            ramManager.setByte(memAddr, (sbyte) (value ^ mask));
                            index++;
                            memAddr++;
                        }
                    }
                    break;
                //chDir
                case 0xc0:
                    stack.push(file.changeDir(ramManager, stack.pop() & 0xffff) ? TRUE : FALSE);
                    break;
                //fileList
                case 0xc1:
                    stack.push(fileList());
                    break;
                //getTime
                case 0xc2:
                   {
                        date.setTime(System.currentTimeMillis());
                        cal.setTime(date);
                        int addr = stack.pop() & 0xffff;
                        ramManager.setBytes(addr, 2, cal.get(Calendar.YEAR));
                        ramManager.setBytes(addr + 2, 1, cal.get(Calendar.MONTH));
                        ramManager.setBytes(addr + 3, 1, cal.get(Calendar.DAY_OF_MONTH));
                        ramManager.setBytes(addr + 4, 1, cal.get(Calendar.HOUR_OF_DAY));
                        ramManager.setBytes(addr + 5, 1, cal.get(Calendar.MINUTE));
                        ramManager.setBytes(addr + 6, 1, cal.get(Calendar.SECOND));
                        ramManager.setBytes(addr + 7, 1, cal.get(Calendar.DAY_OF_WEEK));
                    }
                    break;
                //setTime
                case 0xc3:
                    //忽略之
                    stack.pop();
                    break;
    //                throw new IllegalStateException("不支持的函数: SetTime");
                //getWord
                case 0xc4:
                     {
                        int mode = stack.pop();
                        char c;
                        if (input == null) {
                            c = key.getchar();
                        }
                        else {
                            input.setMode(mode);
                            c = input.getWord(key, screen);
                        }
                        stack.push(c);
                    }
                    break;
                //xDraw
                case 0xc5:
                    render.xdraw(stack.pop());
                    break;
                //releaseKey
                case 0xc6:
                    key.releaseKey((char) stack.pop());
                    break;
                //getBlock
                case 0xc7:
                     {
                        stack.movePointer(-6);
                        render.setDrawMode(stack.peek(4));
                        int addr = stack.peek(5) & 0xffff;
                        int length = render.getRegion((short) stack.peek(0), (short) stack.peek(1),
                                (short) stack.peek(2), (short) stack.peek(3),
                                ramManager, addr);
                        notifyRamChanged(addr, addr + length);
                    }
                    break;
                case 0xc8:
                     {
                        int arc = (short) stack.pop();
                        stack.push(Util.cos(arc));
                    }
                    break;
                case 0xc9:
                     {
                        int arc = (short) stack.pop();
                        stack.push(Util.sin(arc));
                    }
                    break;
                case 0xca:
                    //throw new IllegalStateException("不支持的函数: FillArea");
            }
        }


        public GvmConfig getConfig() {
            return config;
        }

        private int fileList()
        {
            int addr = stack.pop() & 0xffff;
            int count = file.getFileNum();
            byte[][] encodes = new byte[count + 1][];
            string[] dirName = new string[1];
            encodes[0] = new byte[]{'.', '.'};
            for (int index = 0; index < count; index++) {
                file.listFiles(dirName, index, 1);
                try {
                    encodes[index + 1] = dirName[0].getBytes("gb2312");
                } catch (UnsupportedEncodingException uee) {
                    encodes[index + 1] = dirName[0].getBytes();
                }
            }

            GetableImp getter = new GetableImp();
            int maxRow = screen.getHeight() / 13;
            int first = 0, current = 0;
            for (;;) {
                //绘制文件名与反显条
                render.setDrawMode(RenderableConst.RENDER_FILL_TYPE |
                        RenderableConst.DRAW_CLEAR_TYPE |
                        RenderableConst.RENDER_GRAPH_TYPE);
                render.drawRect(0, 0, screen.getWidth(), screen.getHeight());
                render.setDrawMode(RenderableConst.DRAW_COPY_TYPE | RenderableConst.RENDER_GRAPH_TYPE);
                for (int row = 0; row < maxRow && row + first <= count; row++) {
                    getter.setBuffer(encodes[row + first]);
                    render.drawString(0, row * 13, getter, 0, encodes[row + first].Length);
                }
                render.setDrawMode(RenderableConst.DRAW_NOT_TYPE |
                        RenderableConst.RENDER_FILL_TYPE |
                        RenderableConst.RENDER_GRAPH_TYPE);
                render.drawRect(0, 13 * current, screen.getWidth(), 13 * current + 12);
                screen.fireScreenChanged();
                //接受按键
                for (;;)
                {
                    int keyValue = key.getRawKey();
                    if (keyValue == keyInf.getEnter())
                    {
                        int index = 0;
                        while (index < encodes[first + current].Length)
                        {
                            ramManager.setByte(addr++, encodes[first + current][index++]);
                        }
                        ramManager.setByte(addr, (sbyte) 0);
                        return TRUE;
                    }
                    if (keyValue == keyInf.getEsc()) {
                        return FALSE;
                    }
                    if (keyValue == keyInf.getDown() || keyValue == keyInf.getRight()) {
                        if (first + current >= count) {
                            continue;
                        }
                        else {
                            if (current < maxRow - 1) {
                                current++;
                            }
                            else {
                                first++;
                            }
                            break;
                        }
                    }
                    if (keyValue == keyInf.getUp() || keyValue == keyInf.getLeft()) {
                        if (first + current == 0) {
                            continue;
                        }
                        else {
                            if (current > 0) {
                                current--;
                            }
                            else {
                                first--;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void notifyRamChanged(int start, int end) {
            Area area = ramManager.intersectWithGraph(start, end);
            if (!area.isEmpty()) {
                screen.addToChangedArea(area);
                screen.fireScreenChanged();
            }
        }

        private void notifyScreenListener(int type) {
            if ((type & RenderableConst.RENDER_GRAPH_TYPE) != 0) {
                screen.fireScreenChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void sprintf() {
            int paramCount = stack.pop() & 0xff;
            //弹出参数
            stack.movePointer(-paramCount);
            int index = 0;
            //保存字符串的地址
            int data = stack.peek(index++) & 0xffff;
            //格式化字符串起始地址
            int addr = stack.peek(index++) & 0xffff;
            sbyte fstr, t, b;
            while ((fstr = ramManager.getByte(addr++)) != 0) {
                if (fstr == 0x25) {
                    //%
                    t = ramManager.getByte(addr++);
                    if (t == 0) {
                        break;
                    }
                    switch (t) {
                        //%d
                        case 0x64:
                           {
                                sbyte[] array = Util.intToGB(stack.peek(index++));
                                for (int k = 0; k < array.Length; k++)
                                {
                                    ramManager.setByte(data++, array[k]);
                                }
                            }
                            break;
                        //%c
                        case 0x63:
                            ramManager.setByte(data++, (sbyte) stack.peek(index++));
                            break;
                        //%s
                        case 0x73:
                            {
                                int strAddr = stack.peek(index++) & 0xffff;
                                while ((b = ramManager.getByte(strAddr++)) != 0) {
                                    ramManager.setByte(data++, b);
                                }
                            }
                            break;
                        default:
                            ramManager.setByte(data++, t);
                            break;
                    }//switch

                }
                else {
                    ramManager.setByte(data++, fstr);
                }
            }
            ramManager.setByte(data, (sbyte) 0);
        }


        /// <summary>
        /// 
        /// </summary>
        private void printf() {
            int paramCount = stack.pop() & 0xff;
            //弹出参数
            stack.movePointer(-paramCount);
            int index = 0;
            //格式化字符串起始地址
            int addr = stack.peek(index++) & 0xffff;
            sbyte fstr, data, b;
            char c;
            while ((fstr = ramManager.getByte(addr++)) != 0) {
                if (fstr == 0x25) {
                    //%
                    data = ramManager.getByte(addr++);
                    if (data == 0) {
                        break;
                    }
                    switch (data) {
                        //%d
                        case 0x64:
                             {
                                sbyte[] array = Util.intToGB(stack.peek(index++));
                                for (int k = 0; k < array.Length; k++) {
                                    text.addChar((char) array[k]);
                                }
                            }
                            break;
                        //%c
                        case 0x63:
                            text.addChar((char) (stack.peek(index++) & 0xff));
                            break;
                        //%s
                        case 0x73:
                             {
                                int strAddr = stack.peek(index++) & 0xffff;
                                while ((b = ramManager.getByte(strAddr++)) != 0) {
                                    if (b >= 0) {
                                        text.addChar((char) b);
                                    }
                                    else {
                                        c = (char) (b & 0xff);
                                        b = ramManager.getByte(strAddr++);
                                        if (b == 0) {
                                            text.addChar(c);
                                            break;
                                        }
                                        c |= b << 8;
                                        text.addChar(c);
                                    }
                                }
                            }
                            break;
                        default:
                            text.addChar((char) (data & 0xff));
                            break;
                    }//switch

                }
                else if (fstr > 0)
                {
                    text.addChar((char) fstr);
                }
                else {
                    b = ramManager.getByte(addr++);
                    if (b == 0) {
                        text.addChar((char) (fstr & 0xff));
                        break;
                    }
                    c = (char) ((fstr & 0xff) | (b << 8));
                    text.addChar(c);
                }
            }
            text.updateLCD(0);
        }

        public void setColor(int black, int white) {
            screen.setColor(black, white);
        }

        public void addScreenChangeListener(ScreenChangeListener listener) {
            screen.addScreenChangeListener(listener);
        }
    }
}
