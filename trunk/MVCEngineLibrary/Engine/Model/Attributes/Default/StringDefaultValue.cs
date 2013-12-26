using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using description = MVCEngine.Model.Internal.Descriptions;

namespace MVCEngine.Model.Attributes.Default
{
    public class StringDefaultValue : DefaultValue
    {
        #region Value
        public override object Value(Entity e, description.EntityProperty c)
        {
            return StringValue;
        }
        #endregion Value

        #region Properties
        public virtual string StringValue { get; set; }
        #endregion Properties
    }
}
