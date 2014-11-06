using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal.Tools.Validation;

namespace MVCEngine.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | AttributeTargets.Property)]
    public sealed class PrimaryKey : System.Attribute
    {
        #region Constructor
        public PrimaryKey()
        {
        }
        #endregion Constructor
    }
}
