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
        private string interceptorClass;
        #endregion Members

        #region Constructor
        public Interceptor(string interceptorClass)
            : this(interceptorClass, new string[0])
        { }

        public Interceptor(string interceptorClass, params string[] methodsName)
        {
            Validator.GetInstnace().
                IsNotEmpty(interceptorClass, "interceptorClass").
                IsNotNull(methodsName, "methodsName");

            this.interceptorClass = interceptorClass;
            this.methodsName = methodsName;
        }
        #endregion Constructor

        #region Properties
        public string InterceptorClass
        {
            get { return interceptorClass; }
        }

        public string[] MethodsName
        {
            get { return methodsName; }
        }

        public virtual string RegEx { get; set; }
        public virtual string GenericType { get; set; }
        #endregion Properties

    }
}
