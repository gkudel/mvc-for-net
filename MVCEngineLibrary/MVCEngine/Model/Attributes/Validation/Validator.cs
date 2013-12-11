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
        public virtual bool RealTimeValidation { get; set; }
        public virtual bool IfFaildThrowException { get; set; }
        #endregion Properties
    }
}
