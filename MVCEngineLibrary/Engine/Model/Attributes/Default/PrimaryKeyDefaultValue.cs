using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using description = MVCEngine.Model.Internal.Descriptions;
using MVCEngine.Tools;

namespace MVCEngine.Model.Attributes.Default
{
    public class PrimaryKeyDefaultValue : DefaultValue
    {
        #region Value
        public override object Value(Entity e, description.EntityProperty c)
        {
            object ret = null;
            if (e.EntityCtx.IsNotNull())
            {
                ret = e.EntityCtx.Entities.Select(entity => entity[c.Name]).Max();
                if(ret.IsNotNull())
                {
                    decimal d;
                    if (decimal.TryParse(ret.ToString(), out d))
                    {
                        ret = Convert.ChangeType(d + 1, c.PropertyType);
                    }
                    else
                    {
                        ret = null;
                    }
                }
                else
                {
                    ret = c.PropertyType.GetDefaultValue();
                }
            }
            return ret;
        }
        #endregion Value
    }
}

