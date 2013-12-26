using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;

namespace MVCEngine.Model.Attributes.Validation
{
    public class NotNullValidator : ColumnValidator
    {
        #region Validate
        public override bool Validate(object value)
        {
            return value.IsNotNull();
        }
        #endregion Validate
    }
}
