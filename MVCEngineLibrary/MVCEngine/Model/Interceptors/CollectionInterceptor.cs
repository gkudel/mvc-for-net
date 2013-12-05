using Castle.Core.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class CollectionInterceptor : IInterceptor
    {
        #region Constructor
        public CollectionInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();     
        }
        #endregion Inetercept

    }
}
