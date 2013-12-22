using MVCEngine.Model;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Default;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using attributes = MVCEngine.Model.Attributes;

namespace CtgWorksheet.Model
{
    [Table("GP_PROBE")]
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation = true, ErrrorMessage = "Integrity Constraint")]
    [attributes.CollectionInterceptor("Screenings", "CtgWorksheet.Model.Screening, mfc-for-net", RelationName = "Probe_Screening", AutoRefresh=true)]
    public class Probe : Entity
    {        
        [Column("GP_PROB_RECID", IsPrimaryKey = true)]
        [PrimaryKeyDefaultValue()]
        public virtual long Id { get; set; }

        [Column("GP_PROB_CODE")]
        [StringDefaultValue(StringValue = "10")]
        public virtual string Code { get; set; }

        [Column("GP_PROB_NAME")]
        [StringDefaultValue(StringValue = "10")]
        public virtual string Name { get; set; }

        public virtual EntitiesCollection<Screening> Screenings { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            Screenings.Clear();            
        }
    }
}
