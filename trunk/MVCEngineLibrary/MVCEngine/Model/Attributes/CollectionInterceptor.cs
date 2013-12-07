using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes
{
    public class CollectionInterceptor : Interceptor
    {
        #region Constructor
        public CollectionInterceptor(string propertyName, string genericType)
            : base(DefaultInterceptors.CollectionInterceptor, new string[] { "get_" + propertyName})
        {
            GenericType = genericType;
        }

        #endregion Constructor
        
        #region Properties
        public override string RegEx { get; set; }
        #endregion Properties
    }
}
