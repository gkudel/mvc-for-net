using CtgWorksheet.Model.Attributes;
using MVCEngine.Model;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Default;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using attributes = MVCEngine.Model.Attributes;

namespace CtgWorksheet.Model
{
    [Table("GP_ANSUM")]
    public class UserDefined : EntityRow
    {
        public UserDefined()
            : base()
        {}

        public UserDefined(DataRow row)
            : base(row)
        { }

        [attributes.PrimaryKey()]
        [Column("GP_ANS_RECID")]
        public virtual long Id { get; set; }

        [Column("GP_ANS_CODE")]
        public virtual string Code { get; set; }

        [Column("GP_ANS_VALUE")]
        public virtual string Value { get; set; }

        [Column("GP_ANS_WROWID")]
        public virtual long WorksheetRowId { get; set; }

        [Relation("WorksheetRow_UserDefined", "WorksheetRow", "Id", "UserDefined", "WorksheetRowId", OnDelete = OnDelete.Cascade)]
        public virtual WorksheetRow WorksheetRow { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            WorksheetRow = null;
        }
    }
}
