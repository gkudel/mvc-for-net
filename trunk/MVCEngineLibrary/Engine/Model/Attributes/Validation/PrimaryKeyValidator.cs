using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using description = MVCEngine.Model.Internal.Descriptions;

namespace MVCEngine.Model.Attributes.Validation
{
    public class PrimaryKeyValidator : EntityValidator
    {
        #region Constructor
        public PrimaryKeyValidator()
            : base(null)
        {
        }
        #endregion Constructor

        #region Validate
        public override bool Validate(Entity entity, string column, object value)
        {
            bool ret = true;
            if (entity.Table.IsNotNull() && entity.Table.PrimaryKey.IsNotNull())
            {
                return entity.Table.Entities.FirstOrDefault(e => e.State != EntityState.Deleted && !e.Equals(entity)
                        && entity.Table.PrimaryKey(e).Equals(value)).IsNull();
            }
            return ret;
        }

        public override bool Validate(Entity entity)
        {
            bool ret = true;
            if (entity.Table.IsNotNull() && entity.Table.PrimaryKey.IsNotNull())
            {
                return Validate(entity, null, entity.Table.PrimaryKey(entity));
            }
            return ret;
        }
        #endregion Validate
    }
}
