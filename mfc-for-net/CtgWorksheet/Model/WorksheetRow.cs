using MVCEngine.Model;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Default;
using MVCEngine.Model.Interceptors;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using attributes = MVCEngine.Model.Attributes;
using MVCEngine;
using System.Data;
using CtgWorksheet.Model.Attributes;

namespace CtgWorksheet.Model
{
    [Table("GP_WORKSHEETROW")]
    public class WorksheetRow : EntityRow
    {
        public WorksheetRow()
            : base()
        {}

        public WorksheetRow(DataRow row)
            : base(row)
        { }

        [attributes.PrimaryKey()]
        [Column("GP_WROW_RECID")]
        public virtual long Id { get; set; }

        [Column("GP_WROW_CODE")]
        public virtual string Code { get; set; }

        [Column("GP_WROW_NAME")]
        public virtual string Name { get; set; }

        [Column("GP_WROW_SCRID")]
        public virtual long ScreeningId { get; set; }

        [attributes.Relation("WorksheetRow_Screening", "Screening", "Id", "WorksheetRow", "ScreeningId", OnDelete = OnDelete.Cascade)]
        public virtual Screening Screening { get; private set; }

        [DynamicProperties("Code", "Value")]
        [attributes.RelationName("WorksheetRow_UserDefined")]
        public virtual EntitiesCollection<UserDefined> UserDefineds { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            if (UserDefineds.IsNotNull())
            {
                UserDefineds.Clear();
            }
            Screening = null;
        }
    }
}
