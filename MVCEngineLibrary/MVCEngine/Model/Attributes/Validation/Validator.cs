using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes.Validation
{
    public abstract class Validator : System.Attribute
    {
        #region Properties
        public string ErrrorMessage { get; set; }
        #endregion Properties
    }
}
