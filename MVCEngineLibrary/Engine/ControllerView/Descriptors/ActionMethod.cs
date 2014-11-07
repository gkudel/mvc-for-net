using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.ControllerView.ViewEngine;
using System.Reflection;

namespace MVCEngine.ControllerView.Descriptors
{
    class ActionMethod
    {
        #region Properties
        internal string ActionName { get; set; }
        internal string MethodName { get; set; }
        internal MethodInfo MethodInfo { get; set; }
        internal Method Method { get; set; }
        #endregion Properties
    }
}
