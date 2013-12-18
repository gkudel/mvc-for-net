using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace MVCEngine.Interceptors
{
    internal class ControllerInterceptorSelector : IInterceptorSelector
    {
        #region SelectInterceptors
        public IInterceptor[] SelectInterceptors(Type type, System.Reflection.MethodInfo method, IInterceptor[] interceptors)
        {
            return interceptors;
        }
        #endregion SelectInterceptors

        #region Equals & GetHashCode
        public override bool Equals(object obj)
        {
            return obj is ControllerInterceptorSelector;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Equals & GetHashCode
    }
}
