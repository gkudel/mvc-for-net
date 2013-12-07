using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    internal class Relation
    {
        #region Properties
        internal string ParentTable { get; set; }
        internal string ChildTable { get; set; }
        internal string ParentKey { get; set; }
        internal Func<object, object> ParentValue { get; set; }
        internal string ChildKey { get; set; }
        internal Func<object, object> ChildValue { get; set; }
        #endregion Properties
    }
}
