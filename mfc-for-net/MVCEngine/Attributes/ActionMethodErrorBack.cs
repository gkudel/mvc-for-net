using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Attributes
{
    public class ActionMethodErrorBack : ActionCallBack
    {
        #region Constructor
        public ActionMethodErrorBack(string controllerName, string actionName)
            : base(controllerName, actionName)
        {
        }
        #endregion Constructor
    }
}
