using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Internal.Descriptor
{
    internal class ActionMethod
    {
        #region Constructor
        internal ActionMethod()
        {
            Listernes = new List<Listener>();
        }
        #endregion Constructor

        #region Properties
        internal string ActionName { get; set; }
        internal Method Action { get; set; }
        internal Func<object> ControllerActivator { get; set; }
        internal List<Listener> Listernes { get; set; }
        internal bool IsAsynchronousInvoke { get; set; }
        #endregion Properties
    }
}
