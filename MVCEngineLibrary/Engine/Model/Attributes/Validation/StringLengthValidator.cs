using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;

namespace MVCEngine.Model.Attributes.Validation
{
    public class StringLengthValidator : PropertyValidator
    {
        #region Validate
        public override bool Validate(object value)
        {
            if (value.IsNotNull())
            {
                if (value.IsTypeOf<string>())
                {
                    return value.ToString().Length <= Length;
                }
                else 
                {
                    return true;
                }
            }
            return true;
        }
        #endregion Validate

        #region Properties
        public long Length { get; set; }
        #endregion Properties
    }
}
