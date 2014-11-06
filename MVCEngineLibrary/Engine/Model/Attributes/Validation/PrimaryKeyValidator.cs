using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using description = MVCEngine.Model.Internal.Descriptions;
using MVCEngine.Tools;

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
            if (entity.EntityCtx.IsNotNull() && entity.EntityCtx.PrimaryKey.IsNotNull() 
                && entity.EntityCtx.PrimaryKeyProperty.Name == column)
            {
                return entity.EntityCtx.Entities.FirstOrDefault(e => e.State != EntityState.Deleted && !e.Equals(entity)
                        && entity.EntityCtx.PrimaryKey(e).Equals(value)).IsNull();
            }
            return ret;
        }

        public override bool Validate(Entity entity)
        {
            bool ret = true;
            if (entity.EntityCtx.IsNotNull() && entity.EntityCtx.PrimaryKey.IsNotNull())
            {
                return Validate(entity, null, entity.EntityCtx.PrimaryKey(entity));
            }
            return ret;
        }
        #endregion Validate
    }
}
