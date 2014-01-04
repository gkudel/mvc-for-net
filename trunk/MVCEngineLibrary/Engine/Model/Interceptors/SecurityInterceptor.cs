using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model.Interceptors.Exceptions;
using MVCEngine.Model.Internal;
using attribute = MVCEngine.Model.Attributes;
using MVCEngine;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class SecurityInterceptor : Interceptor
    {
        #region Members
        private static SecurityInterceptor _instance;
        #endregion Members

        #region Constructor
        private SecurityInterceptor()
        {
        }
        #endregion Constructor

        #region GetInstance
        internal static SecurityInterceptor GetInstance()
        {
            if (_instance.IsNull()) _instance = new SecurityInterceptor();
            return _instance;
        }
        #endregion GetInstance

        #region Inetercept
        public const string Id = "SecurityInterceptor";

        public string GetId()
        {
            return SecurityInterceptor.Id;
        }

        public void Intercept(IInvocation invocation)
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
    }
}
