using Castle.Core.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class SecurityInterceptor : IInterceptor
    {
        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
        #endregion Inetercept
    }
}
