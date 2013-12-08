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
        public PrimaryKeyValidator(params string[] columnsName)
            : base(columnsName)
        {
        }
        #endregion Constructor

        #region Validate
        public override bool Validate(Entity entity, string column, object value)
        {
            bool ret = true;
            description.Table table = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().Name);
            if (table.IsNull() && entity.GetType().BaseType.IsNotNull())
            {
                table = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().BaseType.Name);
            }
            if (table.IsNotNull())
            {
                description.Column primaryColumn = table.Columns.FirstOrDefault(c => c.PrimaryKey);
                if (primaryColumn.IsNotNull())
                {
                    return table.Entities.FirstOrDefault(e => !e.Equals(entity)
                           && e.GetValue<object>(primaryColumn.Property).Equals(value)).IsNull();
                }
            }
            return ret;
        }

        public override bool Validate(Entity entity)
        {
            bool ret = true;
            description.Table table = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().Name);
            if (table.IsNull() && entity.GetType().BaseType.IsNotNull())
            {
                table = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().BaseType.Name);
            }
            if (table.IsNotNull())
            {
                description.Column primaryColumn = table.Columns.FirstOrDefault(c => c.PrimaryKey);
                if (primaryColumn.IsNotNull())
                {
                    return Validate(entity, primaryColumn.Property, entity.GetValue<object>(primaryColumn.Property));
                }
            }
            return ret;
        }
        #endregion Validate
    }
}
