using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Validation
{
    public static class ValidationExtensions
    {
        #region IsNotNull
        public static Validator IsNotNull(this Validator validation, object theObject, string paramName)
        {
            if (theObject == null)
            {
                (validation ?? new Validator()).ThrowExeception(new ArgumentNullException(paramName));
            }

            return validation;
        }

        public static Validator IsNotNull(this Validator validation, object theObject, string paramName, Func<object, bool> func)
        {
            if (!func(theObject))
            {
                (validation ?? new Validator()).ThrowExeception(new ArgumentNullException(paramName));
            }
            return validation;
        }
        #endregion IsNotNull

        #region IsNotNull
        public static Validator IsNotEmpty(this Validator validation, string theValue, string paramName)
        {
            if (string.IsNullOrEmpty(theValue))
            {
                (validation ?? new Validator()).ThrowExeception(new ArgumentNullException(paramName));
            }

            return validation;
        }

        public static Validator IsNotEmpty(this Validator validation, string theValue, string paramName, Func<string, bool> func)
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
