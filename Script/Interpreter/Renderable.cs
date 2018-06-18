using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    public class RenderableConst
    {
        /// <summary>
        /// 使用大字体模式,对drawChar等有效
        /// </summary>
        public static int TEXT_BIG_TYPE = 0x80;
        /// <summary>
        /// 直接在屏幕绘图
        /// </summary>
        public static int RENDER_GRAPH_TYPE = 0x40;
        /// <summary>
        /// 针对几何图形,是否填充
        /// </summary>
        public static int RENDER_FILL_TYPE = 0x10;
        /// <summary>
        /// 图像取反
        /// </summary>
        public static int RENDER_XNOT_TYPE = 0x08;

        /// <summary>
        /// 透明copy,0为透明像素<p>
        /// 注意,0~6互斥
        /// </summary>
        public static int DRAW_TCOPY_TYPE = 6;
        /// <summary>
        /// xor
        /// </summary>
        public static int DRAW_XOR_TYPE = 5;
        /// <summary>
        /// and
        /// </summary>
        public static int DRAW_AND_TYPE = 4;
        /// <summary>
        /// or
        /// </summary>
        public static int DRAW_OR_TYPE = 3;
        /// <summary>
        /// not
        /// </summary>
        public static int DRAW_NOT_TYPE = 2;
        /// <summary>
        /// copy,绘制图形时意思为copy,绘制几何图形时采用前景色绘图
        /// </summary>
        public static int DRAW_COPY_TYPE = 1;
        /// <summary>
        /// clear,对绘制几何图形有效,使用背景色绘图
        /// </summary>
        public static int DRAW_CLEAR_TYPE = 0;
    }

    /// <summary>
    /// 绘图接口,通过该接口向屏幕或缓冲区绘制图像
    /// 注意:除了refresh方法会激发fireScreenChanged方法外,其他方法均不会自动激发fireScreenChanged
    /// </summary>
    interface Renderable
    {
        /// <summary>
        /// 设置绘制属性,可能包括填充/不填充,正常/反色,etc.
        /// </summary>
        /// <param name="m">需要设置的属性</param>
        void setDrawMode(int m);

        /// <summary>
         /// 绘制一个以0结尾的字符串
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="source"></param>
        /// <param name="addr"></param>
        void drawstring(int x, int y, Getable source, int addr);

        /// <summary>
        /// 绘制从地址addr开始的length个字符 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="source"></param>
        /// <param name="addr"></param>
        /// <param name="length"></param>
        public void drawstring(int x, int y, Getable source, int addr, int length);


        /// <summary>
         /// 绘制矩形
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        void drawRect(int x0, int y0, int x1, int y1);

        /// <summary>
        /// 绘制椭圆
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void drawOval(int x, int y, int a, int b);

        /// <summary>
        /// 在屏幕为坐标(x,y)处画点
        /// type=0:2色模式下画白点，16色和256色模式下用背景色画点
        ///      1:2色模式下画黑点，16色和256色模式下用前景色画点
        ///      2:点的颜色取反
        /// 坐标不在屏幕内不会抛出异常
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        void drawPoint(int x, int y);

        /// <summary>
        /// 得到屏幕上(x,y)处点的的状态,该方法不考虑graphMode的值.
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>2色模式下如果是白点返回零，黑点返回非零值</returns>
        int getPoint(int x, int y);

        /// <summary>
        /// 绘制直线
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        void drawLine(int x0, int y0, int x1, int y1);

        /// <summary>
        /// 绘制图像,图像所在区域可以不在屏幕范围内
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="src"></param>
        /// <param name="addr"></param>
        void drawRegion(int x, int y, int width, int height, Getable src, int addr);

        /// <summary>
        /// 得到屏幕或缓冲区的图像数据,忽略x,width的低三位
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dst"></param>
        /// <param name="addr"></param>
        /// <returns>length 图像数据长度</returns>
        /// throws IndexOutOfBoundsException 如果图像超出屏幕范围
        int getRegion(int x, int y, int width, int height, Setable dst, int addr);

        /// <summary>
         /// 特效
        /// </summary>
        /// <param name="mode"></param>
        void xdraw(int mode);

        /// <summary>
        /// 清除屏幕缓冲区
        /// </summary>
        void clearBuffer();

        /// <summary>
        /// 刷新屏幕缓冲区到屏幕<p>
        /// 注意,该方法会自动调用ScreenModel的fireScreenChanged方法
        /// see ScreenModel#fireScreenChanged()
        /// </summary>
        void refresh();
    }
}
