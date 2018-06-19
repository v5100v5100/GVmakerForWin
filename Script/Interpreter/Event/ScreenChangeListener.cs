using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Event
{
    /// <summary>
    /// 
    /// </summary>
    public interface ScreenChangeListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenModel"></param>
        /// <param name="area"></param>
        void screenChanged(ScreenModel screenModel, Area area);
    }
}
