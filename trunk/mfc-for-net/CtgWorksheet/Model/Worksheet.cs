using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTestGui.CtgWorksheet.Model
{
    [Table("GP_RESWORKSHEET")]
    [Interceptor("SecurityInterceptor", "Description", "Name")]
    public class Worksheet
    {
        [Column("GP_RES_RECID")]
        public long Id { get; set; }

        [Column("GP_RES_STRING")]        
        public virtual string Description { get; set; }

        [Column("GP_RES_NAME")]
        public virtual string Name { get; set; }

        public virtual List<Screening> Screenings { get; set; }
    }
}
