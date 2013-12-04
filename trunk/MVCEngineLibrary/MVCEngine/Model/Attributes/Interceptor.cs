using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal.Validation;

namespace MVCEngine.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class Interceptor : System.Attribute
    {
        #region Members
        private string[] columnsName;
        private string interceptorName;
        #endregion Members

        #region Constructor
        public Interceptor(string interceptorName)
            : this(interceptorName, new string[0])
        { }

        public Interceptor(string interceptorName, params string[] columnsName)
        {
            Validator.GetInstnace().
                IsNotNull(interceptorName, "interceptorName").
                IsNotNull(columnsName, "columnsName");

            this.interceptorName = interceptorName;
            this.columnsName = columnsName;
        }
        #endregion Constructor

        #region Properties
        public string InterceptorName
        {
            get { return interceptorName; }
        }

        public string[] ColumnsName
        {
            get { return columnsName; }
        }
        #endregion Properties

    }
}
