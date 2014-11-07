using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.ControllerView.Descriptors
{
    class View
    {
        #region Constructor
        internal View()
        {
            CallBack = new List<ActionCallBack>();
        }
        #endregion Constructor

        #region Properties
        internal string Name { get; set; }
        internal List<ActionCallBack> CallBack { get; set; }       
        #endregion Properties

    }
}
