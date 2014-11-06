using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.ControllerView.Descriptors
{
    class Method
    {
        #region Constructor
        internal Method()
        {
            Parameters = new List<Parameter>();
            Anonymous = new List<AnonymousType>();
        }
        #endregion Constructor

        #region Properties
        internal Func<object, object[], object> MethodTriger { get; set; }
        internal List<Parameter> Parameters { get; set; }
        internal List<AnonymousType> Anonymous { get; set; }
        #endregion Properties
    }
}
