using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.View;

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
        internal List<Listener> Listernes { get; set; }
        #endregion Properties
    }
}
