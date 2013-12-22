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

namespace CtgWorksheet.Model
{
    [Table("GP_FISHSCREENING")]
    [attributes.Interceptor(DefaultInterceptors.SecurityInterceptor, "", RegEx = "(?=(?!^set_Worksheet$).*)(?=^(set_))")]
    [attributes.Interceptor(DefaultInterceptors.ModificationInterceptor, "", RegEx = "(?=(?!^set_Worksheet$).*)(?=^(set_|get_))")]
    [attributes.EntityInterceptor("Worksheet", "CtgWorksheet.Model.Worksheet, mfc-for-net")]
    [attributes.EntityInterceptor("Probe", "CtgWorksheet.Model.Probe, mfc-for-net")]
    [attributes.CollectionInterceptor("WorksheetRows", "CtgWorksheet.Model.WorksheetRow, mfc-for-net", RelationName = "WorksheetRow_Screening")]
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation= true, ErrrorMessage="Integrity Constraint")]
    public class Screening : Entity
    {
        [Column("GP_FISH_RECID", IsPrimaryKey=true)]
        [PrimaryKeyDefaultValue()]
        public virtual long Id { get; set; }

        [Column("GP_FISH_WORKSHEETID", IsForeignKey = true, ForeignTable = "GP_RESWORKSHEET", RelationName="Worksheet_Screening", OnDelete = OnDelete.Cascade)]        
        public virtual long WorksheetId { get; set; }

        [Column("GP_FISH_PROBEID", IsForeignKey = true, ForeignTable = "GP_PROBE", RelationName = "Probe_Screening", OnDelete = OnDelete.Cascade)]
        public virtual long ProbetId { get; set; }

        [Column("GP_FISH_VALUEA")]        
        [StringDefaultValue(StringValue="10")]
        public virtual string ValueA { get; set; }

        [Column("GP_FISH_VALUEB")]
        [RangeValidator(Min=10, Max=20, RealTimeValidation=true)]
        [StringDefaultValue(StringValue = "15")]
        public virtual string ValueB { get; set; }

        [Column("GP_FISH_VALUERESULT")]
        public virtual string ValueResult { get; set; }

        [Column("GP_FISH_COMMENT")]
        public virtual string Comment { get; set; }

        [Column("GP_FISH_COMPLETED")]
        [StringDefaultValue(StringValue="N")]
        [UniqStringValidator(UniqValues = new string[] { "N", "Y" }, RealTimeValidation=true)]
        public virtual string Completed { get; set; }

        [Column("GP_FISH_DATE")]
        [CurentDateTimeDefaultValue()]
        public virtual DateTime? Date { get; set; }
        
        public virtual Worksheet Worksheet { get; private set; }

        public virtual Probe Probe { get; private set; }

        public virtual EntitiesCollection<WorksheetRow> WorksheetRows { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            Worksheet = null;
            Probe = null;
            WorksheetRows.Clear();
        }
    }
}
