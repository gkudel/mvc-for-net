﻿using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model;
using attributes = MVCEngine.Model.Attributes;
using MVCEngine.Model.Interceptors;
using MVCEngine.Model.Attributes.Validation;

namespace CtgWorksheet.Model
{
    [Table("GP_FISHSCREENING")]
    [attributes.Interceptor(DefaultInterceptors.SecurityInterceptor, "", RegEx = "^(?=(?:(?!set_Worksheet).)*$).*?set_*")]
    [attributes.Interceptor(DefaultInterceptors.ModificationInterceptor, "", RegEx = "^(?=(?:(?!set_Worksheet).)*$).*?set_*|get_*")]
    [attributes.EntityInterceptor("Worksheet", "CtgWorksheet.Model.Worksheet, mfc-for-net")]
    [attributes.Validation.PrimaryKeyValidator("Id", RealTimeValidation= false)]
    public class Screening : Entity
    {
        [Column("GP_FISH_RECID", IsPrimaryKey=true)]
        public virtual long Id { get; set; }

        [Column("GP_FISH_WORKSHEETID", IsForeignKey = true, ForeignTable = "GP_RESWORKSHEET")]
        public virtual long WorksheetId { get; set; }
        
        [Column("GP_FISH_VALUEA")]
        [StringLengthValidator(Length=10, RealTimeValidation=false)]
        public virtual string ValueA { get; set; }

        [Column("GP_FISH_VALUEB")]
        [RangeValidator(Min=10, Max=20, RealTimeValidation=false)]
        public virtual string ValueB { get; set; }

        [Column("GP_FISH_VALUERESULT")]
        public virtual string ValueResult { get; set; }

        public virtual Worksheet Worksheet { get; set; }
    }
}
