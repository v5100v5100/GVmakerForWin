using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.Event
{
     interface ScreenChangeListener
    {
        void screenChanged(ScreenModel screenModel, Area area);
    }
}
