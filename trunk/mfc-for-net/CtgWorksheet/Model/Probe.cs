using MVCEngine.Model;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Default;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using attributes = MVCEngine.Model.Attributes;
using MVCEngine;

namespace CtgWorksheet.Model
{
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation = true, ErrrorMessage = "Integrity Constraint")]
    public class Probe : Entity
    {        
        [PrimaryKeyDefaultValue()]
        [attributes.PrimaryKey()]
        public virtual long Id { get; set; }

        [StringDefaultValue(StringValue = "10")]
        public virtual string Code { get; set; }

        [StringDefaultValue(StringValue = "10")]
        public virtual string Name { get; set; }

        [attributes.RelationName("Probe_Screening")]
        [attributes.Synchronized()]
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
