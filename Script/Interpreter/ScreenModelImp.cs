using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
     class ScreenModelImp :ScreenModel,Renderable
    {


        /******************Renderable实现部份开始*********************/
         void setDrawMode(int m)
        {
            throw new NotImplementedException();
        }

         void drawString(int x, int y, Ram.Getable source, int addr)
        {
            throw new NotImplementedException();
        }

         void drawString(int x, int y, Ram.Getable source, int addr, int length)
        {
            throw new NotImplementedException();
        }

         void drawRect(int x0, int y0, int x1, int y1)
        {
            throw new NotImplementedException();
        }

         void drawOval(int x, int y, int a, int b)
        {
            throw new NotImplementedException();
        }

         void drawPoint(int x, int y)
        {
            throw new NotImplementedException();
        }

         int getPoint(int x, int y)
        {
            throw new NotImplementedException();
        }

         void drawLine(int x0, int y0, int x1, int y1)
        {
            throw new NotImplementedException();
        }

         void drawRegion(int x, int y, int width, int height, Ram.Getable src, int addr)
        {
            throw new NotImplementedException();
        }

         int getRegion(int x, int y, int width, int height, Ram.Setable dst, int addr)
        {
            throw new NotImplementedException();
        }

         void xdraw(int mode)
        {
            throw new NotImplementedException();
        }

         void clearBuffer()
        {
            throw new NotImplementedException();
        }

         void refresh()
        {
            throw new NotImplementedException();
        }
        /******************Renderable实现部份结束*********************/
    }
}
