using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Internal.Validation;

namespace MVCEngine.View
{
    public sealed class ForwardView : View
    {
        #region Constructor
        public ForwardView(string actionMethod)
        {
            ArgumentValidator.GetInstnace().
            IsNotEmpty(actionMethod, "actionMethod");

            ActionMethod = actionMethod;
        }
        #endregion Constructor

        #region Error Parameters
        public string ActionMethod { get; set; }
        #endregion Error Parameters
    }
}
