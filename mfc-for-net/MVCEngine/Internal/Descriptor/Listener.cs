using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MVCEngine.Internal.Descriptor
{
    internal class Listener
    {
        #region Properties
        internal object ThisObject { get; set; }
        internal PropertyInfo IdProperty { get; set; }
        internal string IdParameterName { get; set; }
        internal Method ActionCallBack { get; set; }
        internal Method ActionErrorBack { get; set; }
        #endregion Properties
    }
}
