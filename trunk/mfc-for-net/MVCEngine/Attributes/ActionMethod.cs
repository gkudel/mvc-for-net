using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Validation;

namespace MVCEngine.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method) ]
    public class ActionMethod : System.Attribute
    {
        #region Members
        private string actionName;
        #endregion Members

        #region Constructor
        public ActionMethod(string actionName)
        {
            Validator.GetInstnace().
                IsNotNull(actionName, "actionName");

            this.actionName = actionName;
        }
        #endregion Constructor

        #region Properties
        public string ActionName
        {
            get { return actionName; }
        }
        #endregion Properties
    }
}
