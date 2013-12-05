using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model.Interceptors.Interface;

namespace CtgWorksheet.Model
{
    [Table("GP_RESWORKSHEET")]
    [Interceptor("SecurityInterceptor", "set_Id", "set_Description", "set_Name")]
    [Interceptor("ModifyInterceptor", "", RegEx = "set_*")]
    public class Worksheet : ISecurity, IObjectState
    {
        [Column("GP_RES_RECID")]
        public virtual long Id { get; set; }

        [Column("GP_RES_STRING")]        
        public virtual string Description { get; set; }

        [Column("GP_RES_NAME")]
        public virtual string Name { get; set; }

        public virtual List<Screening> Screenings { get; set; }

        #region ISecurity Members
        public bool IsFrozen { get; set; }
        #endregion

        #region IObjectState Members
        public ObjectState State { get; set; }
        #endregion
    }
}
