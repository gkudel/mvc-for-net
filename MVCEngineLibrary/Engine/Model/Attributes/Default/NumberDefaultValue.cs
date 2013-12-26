using MVCEngine.Model;
using MVCEngine.Model.Attributes.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using description = MVCEngine.Model.Internal.Descriptions;

namespace MVCEngine.Model.Attributes.Default
{
    public class NumberDefaultValue : DefaultValue
    {
        #region Value
        public override object Value(Entity e, description.EntityProperty c)
        {
            return IntValue.HasValue ? IntValue : LongValue.HasValue ? LongValue : DoubleValue;
        }
        #endregion Value

        #region Properties
        public virtual long? LongValue { get; set; }
        public virtual int? IntValue { get; set; }
        public virtual double? DoubleValue { get; set; }
        #endregion Properties
    }
}
