using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using description = MVCEngine.Model.Internal.Descriptions;

namespace MVCEngine.Model.Attributes.Default
{
    public class PrimaryKeyDefaultValue : DefaultValue
    {
        #region Value
        public override object Value(Entity e, description.Column c)
        {
            object ret = null;
            if (e.Table.IsNotNull())
            {
                ret = e.Table.Entities.Select(entity => entity[c.Name]).Max();
                if(ret.IsNotNull())
                {
                    decimal d;
                    if(decimal.TryParse(ret.ToString(), out d))
                    {
                        ret = Convert.ChangeType(d + 1, c.ColumnType);
                    }
                }
            }
            return ret;
        }
        #endregion Value
    }
}

