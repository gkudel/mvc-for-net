using Castle.Core.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model.Interceptors.Exceptions;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class SecurityInterceptor : IInterceptor
    {
        #region Constructor
        public SecurityInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InvocationTarget.IsTypeOf<Entity>())
            {
                if (!invocation.InvocationTarget.CastToType<Entity>().IsFrozen)
                {
                    invocation.Proceed();
                }
                else
                {
                    this.ThrowException<SecurityException>("Security Exception. You try to modified object for which you don't have an accesss");
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
        #endregion Inetercept
    }
}
