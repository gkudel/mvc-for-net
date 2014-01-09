using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model;
using attributes = MVCEngine.Model.Attributes;
using MVCEngine.Model.Interceptors;
using MVCEngine.Model.Attributes.Validation;
using MVCEngine.Model.Attributes.Default;
using System.ComponentModel;
using MVCEngine.Model.Attributes.Discriminators;
using MVCEngine.Model.Internal.Descriptions;
using MVCEngine;
using MVCEngine.Model.Attributes.Formatter;
using CtgWorksheet.Model.Attributes;
using System.Data;

namespace CtgWorksheet.Model
{
    [Table("GP_SCREENING")]
    public class Screening : EntityRow
    {
        public Screening()
            : base()
        {}

        public Screening(DataRow row)
            : base(row)
        { }

        [attributes.PrimaryKey()]
        [Column("GP_SCR_RECID")]
        public virtual long Id { get; set; }

        [Column("GP_SCR_WKSID")]
        public virtual long WorksheetId { get; set; }

        [Column("GP_SCR_PROBEID")]
        public virtual long ProbeId { get; set; }

        [Column("GP_SCR_PARENTID")]
        public virtual long? ParentId { get; set; }

        [StringDefaultValue(StringValue="10")]
        [Column("GP_SCR_VALUEA")]
        public virtual string ValueA { get; set; }

        [RangeValidator(Min=10, Max=20, RealTimeValidation=true)]
        [Column("GP_SCR_VALUEB")]
        [StringDefaultValue(StringValue = "15")]
        public virtual string ValueB { get; set; }

        [NumberFormatter(PropertyName="ValueDecimal")]
        [Column("GP_SCR_RESULT")]
        public virtual string ValueResult { get; set; }

        [Column("GP_SCR_RESULTNUMBER")]
        public virtual decimal? ValueDecimal { get; set; }

        [Column("GP_SCR_COMMENT")]
        public virtual string Comment { get; set; }

        [StringDefaultValue(StringValue="N")]
        [UniqStringValidator(UniqValues = new string[] { "N", "Y" }, RealTimeValidation=true)]
        [Column("GP_SCR_COMP")]
        public virtual string Completed { get; set; }

        [CurentDateTimeDefaultValue()]
        [Column("GP_SCR_DATE")]        
        public virtual DateTime? Date { get; set; }

        [attributes.Relation("Worksheet_Screening", "Worksheet", "Id", "Screening", "WorksheetId", OnDelete = OnDelete.Cascade)]
        public virtual Worksheet Worksheet { get; private set; }

        [attributes.Relation("Probe_Screening", "Probe", "Id", "Screening", "ProbeId", OnDelete = OnDelete.Cascade)]
        public virtual Probe Probe { get; private set; }

        [attributes.RelationName("WorksheetRow_Screening")]
        [attributes.Synchronized()]
        public virtual EntitiesCollection<WorksheetRow> WorksheetRows { get; private set; }

        [attributes.Relation("Screening_Screening", "Screening", "Id", "Screening", "ParentId", OnDelete = OnDelete.Cascade)]
        public virtual Screening Parent { get; private set; }

        [attributes.RelationName("Screening_Screening")]
        public virtual EntitiesCollection<Screening> Childs { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            Worksheet = null;
            Probe = null;
            Parent = null;
            if (WorksheetRows.IsNotNull())
            {
                WorksheetRows.Clear();
            }
            if (Childs.IsNotNull())
            {
                Childs.Clear();
            }
        }
    }
}
