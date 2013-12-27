using attributes = MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model;
using MVCEngine.Model.Interceptors;
using System.ComponentModel;
using MVCEngine;

namespace CtgWorksheet.Model
{
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation = true, ErrrorMessage = "Integrity Constraint")]
    public class Worksheet : Entity
    {
        [attributes.Default.PrimaryKeyDefaultValue()]
        [attributes.PrimaryKey()]
        public virtual long Id { get; set; }

        [attributes.Validation.StringLengthValidator(Length=15, RealTimeValidation=true)]
        public virtual string Description { get; set; }

        public virtual string Name { get; set; }

        [attributes.RelationName("Worksheet_Screening")]
        public virtual EntitiesCollection<Screening> Screenings { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            if (Screenings.IsNotNull())
            {
                Screenings.Clear();
            }
        }
    }
}
