using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTestGui.CtgWorksheet.Model
{
    [Table("GP_FISHSCREENING")]
    public class Screening
    {
        [Column("GP_FISH_RECID")]
        public long Id { get; set; }

        [Column("GP_FISH_WORKSHEET")]
        public long WorksheetId { get; set; }

        public virtual Worksheet Worksheet { get; set; }
    }
}
