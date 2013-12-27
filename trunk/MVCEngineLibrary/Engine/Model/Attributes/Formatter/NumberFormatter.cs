using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes.Formatter
{
    public class NumberFormatter : Formatter
    {
        #region Format
        public override object Format(object value)
        {
            decimal? number = null;
            if (value.IsNotNull())
            {
                decimal d = decimal.Zero;
                if(decimal.TryParse(value.ToString(), out d))
                {
                    number = d;
                }
            }
            return number;
        }
        #endregion Format
    }
}
