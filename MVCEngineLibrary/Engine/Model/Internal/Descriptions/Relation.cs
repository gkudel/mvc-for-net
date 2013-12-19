using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class Relation
    {
        #region Properties
        public string Name { get; internal set; }
        public string ParentTableName { get; internal set; }
        public Table ParentTable { get; internal set; }
        public string ChildTableName { get; internal set; }
        public Table ChildTable { get; internal set; }

        public string ParentKey { get; internal set; }
        internal Type ParentType { get; set; }
        internal Func<object, object> ParentValue { get; set; }

        public string ChildKey { get; internal set; }
        internal Type ChildType { get; set; }
        internal Func<object, object> ChildValue { get; set; }

        internal OnDelete OnDelete { get; set; }
        #endregion Properties
    }
}
