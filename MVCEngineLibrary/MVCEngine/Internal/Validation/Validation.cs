using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Internal.Validation
{
    internal sealed class Validator
    {
        #region Constructor
        internal Validator()
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
        internal static Validator GetInstnace()
        {
            return null;
        }
        #endregion Begin Validation 
    }
}
