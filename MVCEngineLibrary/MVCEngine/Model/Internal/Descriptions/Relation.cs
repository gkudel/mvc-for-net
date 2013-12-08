using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class Relation
    {
        #region Properties
        public string Name { get; set; }
        public string ParentTable { get; set; }
        public string ChildTable { get; set; }
        public string ParentKey { get; set; }
        internal Func<object, object> ParentValue { get; set; }
        public string ChildKey { get; set; }
        internal Func<object, object> ChildValue { get; set; }
        #endregion Properties
    }
}
