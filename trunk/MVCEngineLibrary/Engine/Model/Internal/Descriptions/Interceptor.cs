using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using attribuet = MVCEngine.Model.Attributes;

namespace MVCEngine.Model.Internal.Descriptions
{
    internal class Interceptor
    {
        #region Prioperties
        internal string InterceptorFullName { get; set; }
        internal List<string> Methods { get; set; }
        internal string RegEx { get; set; }
        #endregion Prioperties
    }
}
