using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes.Formatter
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public abstract class Formatter : System.Attribute
    {
        #region Constructor
        public Formatter()
        {
            PropertyName = string.Empty;
        }
        #endregion Constructor

        #region Format
        public abstract object Format(object value);
        #endregion Format

        #region Properties
        public string PropertyName { get; set; }
        #endregion Properties
    }
}
