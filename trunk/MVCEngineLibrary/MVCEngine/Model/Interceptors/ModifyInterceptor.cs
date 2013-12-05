using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;
using MVCEngine;
using MVCEngine.Model.Interceptors.Interface;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class ModifyInterceptor : IInterceptor
    {
        #region Constructor
        public ModifyInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InvocationTarget.IsTypeOf<IObjectState>())
            {
                IObjectState obj = invocation.InvocationTarget.CastToType<IObjectState>();
                if (obj.State == ObjectState.Unchanged)
                {
                    obj.State = ObjectState.Modified;
                }
                invocation.Proceed();
            }
            else
            {
                invocation.Proceed();
            }
        }
        #endregion Inetercept
    }
}
