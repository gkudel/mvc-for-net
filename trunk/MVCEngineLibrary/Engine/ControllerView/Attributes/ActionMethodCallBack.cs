using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.ControllerView.Attributes
{
    public sealed class ActionMethodCallBack : ActionCallBack
    {
        #region Constructor
        public ActionMethodCallBack(string controllerName, string actionName)
            : base(controllerName, actionName)
        {
        }
        #endregion Constructor
    }
}
