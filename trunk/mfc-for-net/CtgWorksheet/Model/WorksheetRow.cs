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

namespace CtgWorksheet.Model
{
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation = true, ErrrorMessage = "Integrity Constraint")]
    public class WorksheetRow : Entity
    {
        [PrimaryKeyDefaultValue()]
        [attributes.PrimaryKey()]
        public virtual long Id { get; set; }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

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
