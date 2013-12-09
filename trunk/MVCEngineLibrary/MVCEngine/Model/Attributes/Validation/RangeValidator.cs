﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes.Validation
{
    public class RangeValidator : ColumnValidator
    {
        #region Validate
        public override bool Validate(object value)
        {
            if (value.IsNotNull())
            {
                long v;
                if(long.TryParse(value.ToString(), out v))
                {
                    return v > Min && v < Max;
                }
            }
            return true;
        }
        #endregion Validate

        #region Properties
        public long Min { get; set; }
        public long Max { get; set; }
        #endregion Properties
    }
}