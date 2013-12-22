using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model.Interceptors.Exceptions;
using MVCEngine.Model.Internal;
using attribute = MVCEngine.Model.Attributes;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class SecurityInterceptor : Interceptor
    {
        #region Constructor
        public SecurityInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public override void Intercept(IInvocation invocation)
        {
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (!entity.Disposing && entity.IsNotNull())
            {
                if (!entity.IsFrozen)
                {
                    invocation.Proceed();
                }
                else
                {
                    throw new SecurityException("Security Exception. You try to modified object for which you don't have an accesss");
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
        #endregion Inetercept

        #region Initialize
        public override void Initialize(Type entityType, attribute.Interceptor interceptor)
        { }
        #endregion Initialize
    }
}
