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
using CtgWorksheet.Model.Attributes;
using System.Data;

namespace CtgWorksheet.Model
{
    [Table("GP_WORKSHEET")]
    public class Worksheet : EntityRow
    {
        public Worksheet()
            : base()
        {}

        public Worksheet(DataRow row)
            : base(row)
        { }

        [attributes.PrimaryKey()]
        [Column("GP_WKS_RECID")]
        public virtual long Id { get; set; }

        [attributes.Validation.StringLengthValidator(Length=15, RealTimeValidation=true)]
        [Column("GP_WKS_DESCRIPTION")]
        public virtual string Description { get; set; }

        [Column("GP_WKS_NAME")]
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
