using MVCEngine.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct) ]
    public class Controller : System.Attribute
    {
        #region Members
        private string controllerName;
        #endregion Members

        #region Constructor
        public Controller(string controllerName)
        {
            Validator.GetInstnace().
                       IsNotEmpty(controllerName, "controllerName");

            this.controllerName = controllerName;
        }
        #endregion Constructor

        #region Properties
        public string ControllerName
        {
            get { return controllerName; }
        }
        #endregion Properties
    }
}
