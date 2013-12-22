using MVCEngine.Model;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using attributes = MVCEngine.Model.Attributes;

namespace CtgWorksheet.Model
{
    [Table("GP_USERDEFINED")]
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation = true, ErrrorMessage = "Integrity Constraint")]
    [attributes.EntityInterceptor("WorksheetRow", "CtgWorksheet.Model.WorksheetRow, mfc-for-net", RelationName = "UserDefined_WorksheetRow")]
    public class UserDefined : Entity
    {
        [Column("GP_UDF_RECID", IsPrimaryKey = true)]
        [PrimaryKeyDefaultValue()]
        public virtual long Id { get; set; }

        [Column("GP_UDF_CODE")]
        public virtual string Code { get; set; }

        [Column("GP_UDF_VALUE")]
        public virtual string Value { get; set; }

        [Column("GP_UDF_WORKSHEETROWID", IsForeignKey = true, ForeignTable = "GP_WORKSHEETROW", RelationName = "UserDefined_WorksheetRow", OnDelete = OnDelete.Cascade)]
        public virtual long WorksheetRowId { get; set; }

        public virtual WorksheetRow WorksheetRow { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            WorksheetRow = null;
        }
    }
}
