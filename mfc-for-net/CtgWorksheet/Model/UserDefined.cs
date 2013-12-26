using MVCEngine.Model;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Default;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using attributes = MVCEngine.Model.Attributes;

namespace CtgWorksheet.Model
{
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation = true, ErrrorMessage = "Integrity Constraint")]
    public class UserDefined : Entity
    {
        [PrimaryKeyDefaultValue()]
        [attributes.PrimaryKey()]
        public virtual long Id { get; set; }

        public virtual string Code { get; set; }

        public virtual string Value { get; set; }

        [Relation("WorksheetRow_UserDefined", ForeignEntity="WorksheetRow", ForeignProperty="Id", OnDelete=OnDelete.Cascade)]
        public virtual long WorksheetRowId { get; set; }

        [attributes.RelationName("WorksheetRow_UserDefined")]
        public virtual WorksheetRow WorksheetRow { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            WorksheetRow = null;
        }
    }
}
