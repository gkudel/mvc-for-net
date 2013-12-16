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
        #region Properties
        public string ActionName { get; internal set; }
        public string MethodName { get; internal set; }
        internal Action ActionCallBack { get; set; }
        #endregion Properties
    }
}
