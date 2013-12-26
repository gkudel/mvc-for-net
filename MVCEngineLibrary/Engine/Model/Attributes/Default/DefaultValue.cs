using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using description = MVCEngine.Model.Internal.Descriptions;

namespace MVCEngine.Model.Attributes.Default
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public abstract class DefaultValue : System.Attribute
    {
        #region Value
        public abstract object Value(Entity e, description.EntityProperty c);
        #endregion Value
    }
}
