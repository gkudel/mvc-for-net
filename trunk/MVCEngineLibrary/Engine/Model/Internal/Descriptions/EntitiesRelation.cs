using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public enum OnDelete { Nothing, Cascade, SetNull }
    public class EntitiesRelation
    {
        #region Members
        private static int _ordninal = 0;
        #endregion Members

        #region Constructor
        internal EntitiesRelation()
        {
            Ordinal = _ordninal++;
        }
        #endregion Constructor

        #region Properties
        internal int Ordinal { get; set; }
        public string Name { get; internal set; }
        public string ParentEntityName { get; internal set; }
        public EntityClass ParentEntity { get; internal set; }
        public string ChildEntityName { get; internal set; }
        public EntityClass ChildEntity { get; internal set; }

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
