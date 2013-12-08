using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Internal.Validation
{
    internal static class ArgumentValidatorExtensions
    {
        #region IsNotNull
        internal static ArgumentValidator IsNotNull(this ArgumentValidator validation, object theObject, string paramName)
        {
            if (theObject == null)
            {
                (validation ?? new ArgumentValidator()).ThrowExeception(new ArgumentNullException(paramName));
            }

            return validation;
        }

        internal static ArgumentValidator IsNotNull(this ArgumentValidator validation, object theObject, string paramName, Func<object, bool> func)
        {
            if (!func(theObject))
            {
                (validation ?? new ArgumentValidator()).ThrowExeception(new ArgumentNullException(paramName));
            }
            return validation;
        }
        #endregion IsNotNull

        #region IsNotNull
        internal static ArgumentValidator IsNotEmpty(this ArgumentValidator validation, string theValue, string paramName)
        {
            if (string.IsNullOrEmpty(theValue))
            {
                (validation ?? new ArgumentValidator()).ThrowExeception(new ArgumentNullException(paramName));
            }

            return validation;
        }

        internal static ArgumentValidator IsNotEmpty(this ArgumentValidator validation, string theValue, string paramName, Func<string, bool> func)
        {
            if (!func(theValue))
            {
                (validation ?? new ArgumentValidator()).ThrowExeception(new ArgumentNullException(paramName));
            }
            return validation;
        }
        #endregion IsNotNull
    }
}
