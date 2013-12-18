using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using attribute = MVCEngine.Model.Attributes;

namespace MVCEngine.Model.Internal
{
    public abstract class Interceptor : IInterceptor
    {
        #region Initialize
        public abstract void Initialize(Type entityType, attribute.Interceptor interceptor);
        #endregion Initialize

        #region Intercept
        public abstract void Intercept(IInvocation invocation);
        #endregion Intercept
    }
}
