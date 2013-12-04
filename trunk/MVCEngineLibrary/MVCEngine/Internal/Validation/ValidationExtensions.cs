using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Internal.Validation
{
    internal static class ValidationExtensions
    {
        #region IsNotNull
        internal static Validator IsNotNull(this Validator validation, object theObject, string paramName)
        {
            if (theObject == null)
            {
                (validation ?? new Validator()).ThrowExeception(new ArgumentNullException(paramName));
            }

            return validation;
        }

        internal static Validator IsNotNull(this Validator validation, object theObject, string paramName, Func<object, bool> func)
        {
            if (!func(theObject))
            {
                (validation ?? new Validator()).ThrowExeception(new ArgumentNullException(paramName));
            }
            return validation;
        }
        #endregion IsNotNull

        #region IsNotNull
        internal static Validator IsNotEmpty(this Validator validation, string theValue, string paramName)
        {
            if (string.IsNullOrEmpty(theValue))
            {
                (validation ?? new Validator()).ThrowExeception(new ArgumentNullException(paramName));
            }

            return validation;
        }

        internal static Validator IsNotEmpty(this Validator validation, string theValue, string paramName, Func<string, bool> func)
        {
            if (!func(theValue))
            {
                (validation ?? new Validator()).ThrowExeception(new ArgumentNullException(paramName));
            }
            return validation;
        }
        #endregion IsNotNull
    }
}
