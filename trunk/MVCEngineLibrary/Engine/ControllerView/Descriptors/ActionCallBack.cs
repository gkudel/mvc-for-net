using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.ControllerView.Descriptors
{
    class ActionCallBack
    {
        #region Constructor
        internal ActionCallBack()
        {
            Listeners = new List<Listener>();
        }
        #endregion Constructor

        #region Properties
        internal string ActionName { get; set; }
        internal List<Listener> Listeners { get; set; }
        #endregion Properties
    }
}
