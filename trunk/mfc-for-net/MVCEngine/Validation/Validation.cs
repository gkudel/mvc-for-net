using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Validation
{
    public sealed class Validator
    {
        #region Constructor
        public Validator()
        {
        }
        #endregion Constructor

        #region Throw Exception
        public void ThrowExeception(Exception e)
        {
            throw e;
        }
        #endregion Throw Exception

        #region Begin Validation
        public static Validator GetInstnace()
        {
            return null;
        }
        #endregion Begin Validation 
    }
}
