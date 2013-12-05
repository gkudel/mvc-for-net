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
    [attributes.CollectionInterceptor("Screenings", "GP_FISHSCREENING")]
    public class Worksheet : ModelObject
    {
        [attributes.Column("GP_RES_RECID")]
        public virtual long Id { get; set; }

        [attributes.Column("GP_RES_STRING")]        
        public virtual string Description { get; set; }

        [attributes.Column("GP_RES_NAME")]
        public virtual string Name { get; set; }

        public virtual List<Screening> Screenings { get; set; }
    }
}
