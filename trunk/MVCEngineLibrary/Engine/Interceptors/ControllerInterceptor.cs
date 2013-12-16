using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;

namespace MVCEngine.Interceptors
{
    [Serializable]
    internal class ControllerInterceptor : IInterceptor
    {
        #region Members
        private string _controlerName;
        #endregion Members

        #region Constructor
        internal ControllerInterceptor(string controlerName)
        {
            this._controlerName = controlerName;
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            ControllerDispatcher.GetInstance().Proceed(invocation.Arguments,  _controlerName, invocation.Method.Name, invocation.ReturnValue);
        }
        #endregion Inetercept
    }
}
