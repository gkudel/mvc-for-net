using MVCEngine.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.View
{
    public sealed class ForwardView
    {
        #region Constructor
        public ForwardView(string actionMethod)
        {
            Validator.GetInstnace().
            IsNotEmpty(actionMethod, "actionMethod");

            ActionMethod = actionMethod;
        }
        #endregion Constructor

        #region Error Parameters
        public string ControllerName { get; set; }
        public string ActionMethod { get; set; }
        public object Params { get; set; }
        #endregion Error Parameters
    }
}
