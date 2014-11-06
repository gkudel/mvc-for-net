using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Internal.Tools.Validation
{
    internal sealed class ArgumentValidator
    {
        #region Constructor
        internal ArgumentValidator()
        {
        }
        #endregion Constructor

        #region Throw Exception
        internal void ThrowExeception(Exception e)
        {
            throw e;
        }
        #endregion Throw Exception

        #region Begin Validation
        internal static ArgumentValidator GetInstnace()
        {
            return null;
        }
        #endregion Begin Validation 
    }
}
