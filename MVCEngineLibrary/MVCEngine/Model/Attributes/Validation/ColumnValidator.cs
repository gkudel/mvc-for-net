using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes.Validation
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public abstract class ColumnValidator : Validator
    {
        #region Validate
        public abstract bool Validate(object value);
        #endregion Validate

        #region Properties
        public virtual bool RealTimeValidation { get; set; }
        public virtual bool IfFaildThrowException { get; set; }
        #endregion Properties
    }
}
