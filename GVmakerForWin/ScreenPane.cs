using Script.Interpreter;
using Script.Interpreter.Event;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GVmakerForWin
{
    public class ScreenPane
    {
        int[] mBuffer = new int[ScreenModel.WIDTH * ScreenModel.HEIGHT];
        int[] mLastBuffer=new int[ScreenModel.WIDTH * ScreenModel.HEIGHT];
        private Bitmap mBitmap;
        private int mUpdataState=0;
        private bool mOptimize=false;
        public ScreenPane(VirtualMachine gvm)
        {
    	    mBitmap =new Bitmap(ScreenModel.WIDTH, ScreenModel.HEIGHT);
            //gvm.setColor(0xff000000, 0xffdfffdf);
            int black = Convert.ToInt32(0xff000000);
            int white = Convert.ToInt32(0xffdfffdf);
            gvm.setColor(black, white);
            mBufferRect = new Rectangle(0, 0, ScreenModel.WIDTH, ScreenModel.HEIGHT);
            setSize(ScreenModel.WIDTH, ScreenModel.HEIGHT);
        }
    
   
        //private Rect mBufferRect;
        //private Rect mScreenRect;
        private Rectangle mBufferRect;
        private Rectangle mScreenRect;
    
        //private float mScale = 3.5f;
        //private float mScaleCurrent = 3.5f;
        private float mScale = 4.0f;
        private float mScaleCurrent = 4.0f;
    
        public void setSize(int width, int height) {
            //Log.w("wangyu,w:",width+"");
            //Log.w("wangyu,h:",height+"");
            height/=2;
            float maxScaleW = width / (float)mBufferRect.Right;
    	    float maxScaleH = height / (float)mBufferRect.Bottom;
            //Log.w("wangyu,mbfr:",mBufferRect.right+"");

    	    // use the minimum value
    	    mScaleCurrent = maxScaleW < mScale ? maxScaleW:mScale; 
    	    mScaleCurrent = maxScaleH < mScaleCurrent ? maxScaleH:mScaleCurrent;
    	
    	    // set to center
    	    mScreenRect = new Rectangle( 0, 0, 
    						        (int)(mScaleCurrent * mBufferRect.Right), 
    						        (int)(mScaleCurrent * mBufferRect.Bottom));
            //mScreenRect.Left = (width - mScreenRect.Right) / 2;
            //mScreenRect.Right += mScreenRect.Left;
            //mScreenRect.Top = (height - mScreenRect.Bottom) / 2;
            //mScreenRect.Bottom += mScreenRect.Top;
            mScreenRect.X = (width - mScreenRect.Right) / 2;
            mScreenRect.Y = (height - mScreenRect.Bottom) / 2;
            mScreenRect.Width = mScreenRect.X;
            mScreenRect.Height = mScreenRect.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenModel"></param>
        /// <param name="area"></param>
        public void screenChanged(ScreenModel screenModel, Area area)
        {
    	    screenModel.getRGB(mBuffer, area, 1, 0);
            //synchronized (this)
            //{
            //        mBitmap.setPixels(mBuffer, 0, ScreenModel.WIDTH, 0, 0,
            //                ScreenModel.WIDTH, ScreenModel.HEIGHT);
            // }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="area"></param>
        public void refresh(Graphics graphics, Area area) 
        {
            //canvas.drawBitmap(mBitmap, mBufferRect, mScreenRect, null);
            graphics.DrawImage(mBitmap, mScreenRect, mBufferRect.X, mBufferRect.Y,mBufferRect.Width,mBufferRect.Height, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GVMToScreent(int n)
        {
    	    return (int)( n * mScaleCurrent);
        }
    }
}
