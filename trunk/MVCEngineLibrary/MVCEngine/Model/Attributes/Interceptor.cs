using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal.Validation;
using MVCEngine.Model.Interceptors;

namespace MVCEngine.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = true)]
    public class Interceptor : System.Attribute
    {
        #region Members
        private string[] methodsName;
        private string interceptorName;
        #endregion Members

        #region Constructor
        public Interceptor(string interceptorName)
            : this(interceptorName, new string[0])
        { }

        public Interceptor(string interceptorName, params string[] methodsName)
        {
            Validator.GetInstnace().
                IsNotEmpty(interceptorName, "interceptorName").
                IsNotNull(methodsName, "methodsName");

            this.interceptorName = interceptorName;
            this.methodsName = methodsName;
            switch (interceptorName)
            {
                case DefaultInterceptors.SecurityInterceptor:
                case DefaultInterceptors.ModificationInterceptor:
                case DefaultInterceptors.CollectionInterceptor:
                    Namespace = "MVCEngine.Model.Interceptors";
                    break;
            }
        }
        #endregion Constructor

        #region Properties
        public string InterceptorName
        {
            get { return interceptorName; }
        }

        public string[] MethodsName
        {
            get { return methodsName; }
        }

        public virtual string Namespace { get; set; }
        public virtual string Assembly { get; set; }
        public virtual string RegEx { get; set; }
        #endregion Properties

    }
}
