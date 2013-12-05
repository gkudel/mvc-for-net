using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Internal;

namespace MVCEngine.Model.Interceptors
{
    internal class InterceptorGenerationHook : IProxyGenerationHook
    {
        #region IProxyGenerationHook
        public void MethodsInspected()
        {
        }

        public void NonVirtualMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, System.Reflection.MethodInfo methodInfo)
        {
            return InterceptorDispatcher.GetInstnace().ShouldBeIntercept(type, methodInfo);
        }
        #endregion IProxyGenerationHook

        #region Equals & GetHashCode
        public override bool Equals(object obj)
        {
            return obj is InterceptorGenerationHook;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Equals & GetHashCode
    }
}
