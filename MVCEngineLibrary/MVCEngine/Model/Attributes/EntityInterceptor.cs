using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes
{
    public class EntityInterceptor : Interceptor
    {
        #region Constructor
        public EntityInterceptor(string propertyName, string genericType)
            : base(DefaultInterceptors.EntityInterceptor, new string[] { "get_" + propertyName })
        {
            GenericType = genericType;
        }

        #endregion Constructor
        
        #region Properties
        public override string RegEx 
        { 
            get { return string.Empty; }
            set { throw new NotSupportedException(); }
        }
        #endregion Properties
    }
}
