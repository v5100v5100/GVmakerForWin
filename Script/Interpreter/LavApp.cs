using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Script.Interpreter
{
    public class LavApp
    {
        private byte[] appData;
        private int offset;

    /**
     * 通过一个输入流创建一个LavApp对象
     * @param in 一个输入流
     * @return 一个LavApp对象
     * @throws IllegalArgumentException 发生IO错误或数据格式不正确
     */
    public static LavApp createLavApp(InputStream in) throws IllegalArgumentException {
        return new DefaultLavApp(in);
    }

    /**
     * 通过一个lav程序数据来创建一个LavApp<p>
     * 注意,LavApp内部使用的就是该数组,类创建后不能从外部修改这个数组
     * @param data
     * @throws IllegalArgumentException
     */
    protected LavApp(byte[] data) throws IllegalArgumentException {
        this.appData = data;
        verifyData();
    }

    /**
     * lav程序数据大小(字节数)
     * @return size 这个lav程序数据的总大小,含文件头
     */
    public readonly int size() {
        return appData.Length;
    }

    /**
     * 在pointer处读取一字节数据,并使pointer加一<p>
     * 注意,这里返回值是char类型,对应lav的char类型,因为lav的char类型是无符号的.
     */
    public readonly char getChar() {
        return (char) (appData[offset++] & 0xff);
    }

    /**
     * 从app中读取两字节数据,对应lav中的int类型
     * @return int
     */
    public readonly short getInt() {
        short s;
        s = (short) (appData[offset++] & 0xff);
        s |= (appData[offset++] & 0xff) << 8;
        return s;
    }

    /**
     * 从app中读取三字节数据(无符号),对应lav中文件指针数据
     */
    public readonly int getAddr() {
        int addr;
        addr = appData[offset++] & 0xff;
        addr |= (appData[offset++] & 0xff) << 8;
        addr |= (appData[offset++] & 0xff) << 16;
        return addr;
    }

    /**
     * 从app中读取四字节数据,对应lav中的long类型
     */
    public readonly int getLong() {
        int i;
        i = appData[offset++] & 0xff;
        i |= (appData[offset++] & 0xff) << 8;
        i |= (appData[offset++] & 0xff) << 16;
        i |= (appData[offset++] & 0xff) << 24;
        return i;
    }

    /**
     * 得到当前数据偏移量
     * @return pointer 下次读取时的位置
     */
    public readonly int getOffset() {
        return offset;
    }

    /** 设置读取偏移量
     * @param pos 偏移量,下次读取数据时开始位置
     */
    public readonly void setOffset(int pos) {
        offset = pos;
    }

    /**
     * 检查数据格式并设置相应参数
     * @param data 一个lavApp数据
     * @throws IllegalArgumentException 不正确的lava格式
     */
    private void verifyData() throws IllegalArgumentException {
        if (appData.length <= 16) {
            throw new IllegalArgumentException("不是有效的LAV文件!");
        }
        if (appData[0] != 0x4c || appData[1] != 0x41 || appData[2] != 0x56) {
            throw new IllegalArgumentException("不是有效的LAV文件!");
        }
        offset = 16;
    }
    }
}
