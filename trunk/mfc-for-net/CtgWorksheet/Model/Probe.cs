using MVCEngine.Model;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Default;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using attributes = MVCEngine.Model.Attributes;
using MVCEngine.Tools;
using CtgWorksheet.Model.Attributes;
using System.Data;

namespace CtgWorksheet.Model
{
    [Table("GP_PROBE")]
    public class Probe : EntityRow
    {        
        public Probe()
            : base()
        {}

        public Probe(DataRow row)
            : base(row)
        { }

        [attributes.PrimaryKey()]
        [Column("GP_PR_RECID")]
        public virtual long Id { get; set; }

        [Column("GP_PR_CODE")]
        public virtual string Code { get; set; }

        [Column("GP_PR_NAME")]
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
