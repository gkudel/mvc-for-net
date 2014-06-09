using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.View;
using System.Reflection;

namespace MVCEngine.Internal.Descriptors
{
    public class ActionMethod
    {
        #region Constructor
        public ActionMethod()
        {
            Listernes = new List<Listener>();
        }
        #endregion Constructor

        #region Properties
        public string ActionName { get; internal set; }
        public string MethodName { get; internal set; }
        internal MethodInfo MethodInfo { get; set; }
        internal Method Method { get; set; }
        internal List<Listener> Listernes { get; set; }
        #endregion Properties
    }
}
