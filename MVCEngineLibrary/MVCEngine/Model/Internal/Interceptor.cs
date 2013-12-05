using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;

namespace MVCEngine.Model.Internal
{
    internal class Interceptor
    {
        #region Prioperties
        internal string Name { get; set; }
        internal string Namespace { get; set; }
        internal string Assembly { get; set; }
        internal List<string> Methods { get; set; }
        internal string RegEx { get; set; }
        #endregion Prioperties
    }
}
