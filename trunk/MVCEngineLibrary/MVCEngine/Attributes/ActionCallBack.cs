using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Attributes
{
    public class ActionCallBack : ActionMethod
    {
        #region Members
        private string controllerName;
        #endregion Members

        #region Constructor
        public ActionCallBack(string controllerName, string actionName)
            : base(actionName)
        {
            this.controllerName = controllerName;
        }
        #endregion Constructor

        #region Properties
        public string ControllerName
        {
            get
            {
                return controllerName;
            }
        }

        public override bool IsAsynchronousInvoke 
        {
            get 
            { 
                return false; 
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        #endregion Properties
    }
}
