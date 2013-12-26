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

namespace CtgWorksheet.Model
{
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation= true, ErrrorMessage="Integrity Constraint")]
    public class Screening : Entity
    {
        [PrimaryKeyDefaultValue()]
        [attributes.PrimaryKey()]
        public virtual long Id { get; set; }

        [attributes.Relation("Worksheet_Screening", ForeignEntity="Worksheet", ForeignProperty="Id", OnDelete=OnDelete.Cascade)]
        public virtual long WorksheetId { get; set; }

        [attributes.Relation("Probe_Screening", ForeignEntity = "Probe", ForeignProperty = "Id", OnDelete = OnDelete.Cascade)]
        public virtual long ProbetId { get; set; }

        [StringDefaultValue(StringValue="10")]
        public virtual string ValueA { get; set; }

        [RangeValidator(Min=10, Max=20, RealTimeValidation=true)]
        [StringDefaultValue(StringValue = "15")]
        public virtual string ValueB { get; set; }

        public virtual string ValueResult { get; set; }

        public virtual string Comment { get; set; }

        [StringDefaultValue(StringValue="N")]
        [UniqStringValidator(UniqValues = new string[] { "N", "Y" }, RealTimeValidation=true)]
        public virtual string Completed { get; set; }

        [CurentDateTimeDefaultValue()]
        public virtual DateTime? Date { get; set; }

        [attributes.RelationName("Worksheet_Screening")]
        public virtual Worksheet Worksheet { get; private set; }

        [attributes.RelationName("Probe_Screening")]
        public virtual Probe Probe { get; private set; }

        [attributes.RelationName("WorksheetRow_Screening")]
        public virtual EntitiesCollection<WorksheetRow> WorksheetRows { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            Worksheet = null;
            Probe = null;
            if (WorksheetRows.IsNotNull())
            {
                WorksheetRows.Clear();
            }
        }
    }
}
