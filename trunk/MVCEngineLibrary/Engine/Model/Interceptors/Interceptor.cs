using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Model.Interceptors
{
    public abstract class Interceptor : IInterceptor
    {
        public virtual void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }

        public abstract string GetId();        
    }
}
