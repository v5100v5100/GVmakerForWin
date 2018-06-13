using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    /// <summary>
    /// 绘图接口,通过该接口向屏幕或缓冲区绘制图像
    /// 注意:除了refresh方法会激发fireScreenChanged方法外,其他方法均不会自动激发fireScreenChanged
    /// </summary>
     interface Renderable
    {
        
    /**
     * 使用大字体模式,对drawChar等有效
     */
     static  int TEXT_BIG_TYPE = 0x80;
    /**
     * 直接在屏幕绘图
     */
     static  int RENDER_GRAPH_TYPE = 0x40;
    /**
     * 针对几何图形,是否填充
     */
     static  int RENDER_FILL_TYPE = 0x10;
    /**
     * 图像取反
     */
     static  int RENDER_XNOT_TYPE = 0x08;
    /**
     * 透明copy,0为透明像素<p>
     * 注意,0~6互斥
     */
     static  int DRAW_TCOPY_TYPE = 6;
    /**
     * xor
     */
     static  int DRAW_XOR_TYPE = 5;
    /**
     * and
     */
     static  int DRAW_AND_TYPE = 4;
    /**
     * or
     */
     static  int DRAW_OR_TYPE = 3;
    /**
     * not
     */
     static  int DRAW_NOT_TYPE = 2;
    /**
     * copy,绘制图形时意思为copy,绘制几何图形时采用前景色绘图
     */
     static  int DRAW_COPY_TYPE = 1;
    /**
     * clear,对绘制几何图形有效,使用背景色绘图
     */
     static  int DRAW_CLEAR_TYPE = 0;

    /**
     * 设置绘制属性,可能包括填充/不填充,正常/反色,etc.
     * @param m 需要设置的属性
     */
     void setDrawMode(int m);

    /**
     * 绘制一个以0结尾的字符串
     */
     void drawstring(int x, int y, Getable source, int addr);

    /**
     * 绘制从地址addr开始的length个字符 
     */
     void drawstring(int x, int y, Getable source, int addr, int length);

    /**
     * 绘制矩形
     */
     void drawRect(int x0, int y0, int x1, int y1);

    /**
     * 绘制椭圆
     */
     void drawOval(int x, int y, int a, int b);

    /**
     * 在屏幕为坐标(x,y)处画点
     * type=0:2色模式下画白点，16色和256色模式下用背景色画点
     *      1:2色模式下画黑点，16色和256色模式下用前景色画点
     *      2:点的颜色取反
     * 坐标不在屏幕内不会抛出异常
     * @param x 横坐标
     * @param y 纵坐标
     */
     void drawPoint(int x, int y);

    /**
     * 得到屏幕上(x,y)处点的的状态,该方法不考虑graphMode的值.
     * @return  2色模式下如果是白点返回零，黑点返回非零值
     */
     int getPoint(int x, int y);

    /**
     * 绘制直线
     */
     void drawLine(int x0, int y0, int x1, int y1);

    /**
     * 绘制图像,图像所在区域可以不在屏幕范围内
     */
     void drawRegion(int x, int y, int width, int height, Getable src, int addr);

    /**
     * 得到屏幕或缓冲区的图像数据,忽略x,width的低三位
     * @throws IndexOutOfBoundsException 如果图像超出屏幕范围
     * @return length 图像数据长度
     */
     int getRegion(int x, int y, int width, int height, Setable dst, int addr);

    /**
     * 特效
     */
     void xdraw(int mode);

    /**
     * 清除屏幕缓冲区
     */
     void clearBuffer();

    /**
     * 刷新屏幕缓冲区到屏幕<p>
     * 注意,该方法会自动调用ScreenModel的fireScreenChanged方法
     * @see ScreenModel#fireScreenChanged()
     */
     void refresh();
    }
}
