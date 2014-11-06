using MVCEngine.Internal.Tools.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes
{
    public enum Method { Get, Set };

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple=true)]
    public class NotIntercept : System.Attribute
    {
        #region Constructor
        public NotIntercept()
        { }

        public NotIntercept(string interceptorId, Method method)
        {
            ArgumentValidator.GetInstnace().
                IsNotEmpty(interceptorId, "interceptorId");

            InterceptorId = interceptorId;
            Method = method;
        }
        #endregion Constructor

        #region Properties
        internal string InterceptorId { get; private set; }
        internal Method Method { get; private set; }
        #endregion Properties
    }
}
