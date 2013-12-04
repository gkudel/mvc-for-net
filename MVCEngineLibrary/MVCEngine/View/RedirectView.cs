﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Internal.Validation;

namespace MVCEngine.View
{
    public class RedirectView
    {
        #region Constructor
        public RedirectView(string actionMethod)
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
        public object RedirectParams { get; set; }
        public object ControllerProperties { get; set; }
        #endregion Error Parameters
    }
}