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
    [attributes.Interceptor(DefaultInterceptors.SecurityInterceptor, "", RegEx = "^(?=(?:(?!set_Screenings).)*$).*?set_*")]
    [attributes.Interceptor(DefaultInterceptors.ModificationInterceptor, "", RegEx = "^(?=(?:(?!set_Screenings).)*$).*?set_*|get_*")]
    [attributes.CollectionInterceptor("Screenings", "CtgWorksheet.Model.Screening, mfc-for-net")]
    [attributes.Validation.PrimaryKeyValidator("Id", RealTimeValidation=true)]
    public class Worksheet : Entity
    {
        [attributes.Column("GP_RES_RECID", IsPrimaryKey=true)]
        [attributes.Default.PrimaryKeyDefaultValue()]
        public virtual long Id { get; set; }

        [attributes.Column("GP_RES_STRING")]     
        [attributes.Validation.StringLengthValidator(Length=15, RealTimeValidation=true)]
        public virtual string Description { get; set; }

        [attributes.Column("GP_RES_NAME")]
        public virtual string Name { get; set; }

        public virtual List<Screening> Screenings { get; set; }
    }
}
