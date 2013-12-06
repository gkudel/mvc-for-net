using attributes = MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model;
using MVCEngine.Model.Interceptors;

namespace CtgWorksheet.Model
{
    [attributes.Table("GP_RESWORKSHEET")]
    [attributes.Interceptor(DefaultInterceptors.SecurityInterceptor, "set_Id", "set_Description", "set_Name")]
    [attributes.Interceptor(DefaultInterceptors.ModificationInterceptor, "", RegEx = "^(?=(?:(?!set_Screenings|!get_Screenings).)*$).*?set_*|get_*")]
    [attributes.Interceptor(DefaultInterceptors.CollectionInterceptor, "get_Screenings", GenericType = "CtgWorksheet.Model.Screening, mfc-for-net")]
    public class Worksheet : Entity
    {
        [attributes.Column("GP_RES_RECID", IsPrimaryKey=true)]
        public virtual long Id { get; set; }

        [attributes.Column("GP_RES_STRING")]        
        public virtual string Description { get; set; }

        [attributes.Column("GP_RES_NAME")]
        public virtual string Name { get; set; }

        [attributes.Table("GP_FISHSCREENING")]
        public virtual List<Screening> Screenings { get; set; }
    }
}
