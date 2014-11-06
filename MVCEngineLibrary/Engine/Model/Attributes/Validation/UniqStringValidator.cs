using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Tools;

namespace MVCEngine.Model.Attributes.Validation
{
    public class UniqStringValidator : PropertyValidator
    {
        #region Validate
        public override bool Validate(object value)
        {
            if (value.IsNotNull())
            {
                string str = value.CastToType<string>();
                if(!string.IsNullOrEmpty(str))
                {
                    return UniqValues.Contains(str);
                }
                return false;
            }
            return false;
        }
        #endregion Validate

        #region Properties
        public string[] UniqValues { get; set; }
        #endregion Properties
    }
}
