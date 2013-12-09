﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;
using attribuet = MVCEngine.Model.Attributes;

namespace MVCEngine.Model.Internal
{
    internal class Interceptor
    {
        #region Prioperties
        internal string InterceptorFullName { get; set; }
        internal List<string> Methods { get; set; }
        internal string RegEx { get; set; }
        internal attribuet.Interceptor InterceptorAttribute { get; set; }
        #endregion Prioperties
    }
}