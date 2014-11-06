using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Tools;

namespace MVCEngine.Model.Attributes.Validation
{
    public class NotNullValidator : PropertyValidator
    {
        #region Validate
        public override bool Validate(object value)
        {
            return value.IsNotNull();
        }
        #endregion Validate
    }
}
