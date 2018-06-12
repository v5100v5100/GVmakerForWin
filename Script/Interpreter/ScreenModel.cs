using Script.Interpreter.Event;
using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    public abstract class ScreenModel
    {
        /// <summary>
        /// 屏幕的宽度
        /// </summary>
         public const int WIDTH = 160;

        /// <summary>
        /// 屏幕高度
        /// </summary>
         public const int HEIGHT = 80;

    /**
     * 创建一个ScreenModel实例
     */
     static ScreenModel newScreenModel() {
        return new ScreenModelImp();
    }
    private ScreenChangeListener[] lis;

    protected ScreenModel() {
        lis = new ScreenChangeListener[0];
    }

    /**
     * 为Screen添加监听器,当Screen的Graph状态发生变化时发生变化时将激发事件
     * @param listener 事件监听器
     */
      void addScreenChangeListener(ScreenChangeListener listener) {
        ScreenChangeListener[] oldValue = lis;
        lis = new ScreenChangeListener[oldValue.Length + 1];
        int index = 0;
        for (; index < oldValue.Length; index++) {
            lis[index] = oldValue[index];
        }
        lis[index] = listener;
    }

    /**
     * 通知屏幕监听器
     */
      void fireScreenChanged() {
        for (int index = 0; index < lis.Length; index++)
        {
            lis[index].screenChanged(this, getChangedArea());
        }
        refreshArea();
    }

    /**
     * 得到屏幕的宽度
     * @return width
     */
      int getWidth() {
        return WIDTH;
    }

    /**
     * 得到屏幕的高度
     * @return height
     */
      int getHeight() {
        return HEIGHT;
    }

    /**
     * 设置用于表示黑白的颜色的RGB值
     * @param black 黑
     * @param white 白
     */
     abstract void setColor(int black, int white);

    /**
     * 得到屏幕rgb数据<p>
     * 约定:<p>
     *     1. buffer的长度应不小于getWidth()*getHeight()*rate*rate<p>
     *     2. 往buffer填充数据的时候会将buffer看成一个(getWidth()*rate)*(getHeight()*rate)(这只没有旋转的情况,旋转的情况类似)<p>
     *        然后将发生变化的区域的数据填充到buffer的相应位置,而不是从数组开始填充,也不一定是连续填充<p>
     *  2008.2.24修改
     * @param buffer 用于保存屏幕rgb数据的int数组,其长度应不小于 offset+rate*getWidth()*getHeight()
     * @param area   需要的到图像数据在屏幕的范围
     * @param rate   图像的放大比例
     * @param circ   可以为0,1,2,3.分别表示不旋转,逆时针旋转90度,180度,270度
     * @return buffer本身
     */
     abstract int[] getRGB(int[] buffer, Area area, int rate, int circ);

    /**
     * 是否有相关联的显存以及显存缓冲区Ram
     * @return 当且仅当屏幕大小为160*80时返回true
     */
     abstract bool hasRelativeRam();

    /**
     * 得到与该屏幕显存相关联的Ram,可以将其安装到RamManager中,以使得LAVA程序能够直接访问显存
     * @return ram 得到关联的Ram,该Ram的内容与Screen内容保持同步变化
     * @throws IllegalStateException 如果hasRelativeRam()返回false
     * @see #hasRelativeRam()
     * @see RamManager#install(Ram)
     */
     abstract RelativeRam getGraphRam();

    /**
     * 得到与屏幕缓冲区相关联的Ram,可以将其安装到RamManager中,以使得LAVA程序能够直接访问屏幕缓冲区
     * @return ram 得到关联的Ram,该Ram的内容与Screen缓冲区内容保持同步变化
     * @throws IllegalStateException 如果hasRelativeRam()返回false
     * @see #hasRelativeRam()
     * @see RamManager#install(Ram)
     */
     abstract RelativeRam getBufferRam();

    /**
     * 获得屏幕绘图接口
     * @return render
     */
     abstract Renderable getRender();

    /**
     * 重置描述变化范围的area,使其从新记录变化范围<p>
     * 这几个关于changedArea的方法只应该由JGVM内部的组件使用
     * @see #getChangedArea()
     * @see #getRGB(int[],Area,int,int)
     */
    abstract void refreshArea();

    /**
     * 得到发生改变的屏幕范围<p>
     * 2008.2.24添加
     * @return area
     */
    abstract Area getChangedArea();

    /**
     * 往改变区域添加add
     * @param add 新的改动的区域
     */
    abstract void addToChangedArea(Area add);
    }
}
