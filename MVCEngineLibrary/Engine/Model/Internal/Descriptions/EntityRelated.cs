using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    
    public class EntityRelated
    {
        #region Constructor
        internal EntityRelated()
        { }
        #endregion Constructor

        #region Properties
        public string EntityName { get; internal set; }
        public EntityClass Entity { get; internal set; }
        public string Key { get; internal set; }
        internal Type Type { get; set; }
        internal Func<object, object> Value { get; set; }
        #endregion Properties
    }
}
