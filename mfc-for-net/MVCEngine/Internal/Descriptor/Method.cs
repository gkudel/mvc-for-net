using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Internal.Descriptor
{
    internal class Method
    {
        #region Constructor
        internal Method()
        {
            Parameters = new List<Parameter>();
        }
        #endregion Constructor

        #region Properties
        internal MethodInfo MethodInfo { get; set; }
        internal List<Parameter> Parameters { get; set; }
        #endregion Properties
    }
}
