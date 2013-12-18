using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Internal.Descriptors
{
    internal class AnonymousType
    {
        #region Properties
        internal string Name { get; set; }
        internal Func<object, object[]> MethodArguments { get; set; }
        internal Func<object, object> Id { get; set; }
        #endregion Properties
    }
}
