using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace MVCEngine.Interceptors
{
    internal class ControllerInterceptorGenerationHook : IProxyGenerationHook
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
            return ControllerDispatcher.GetInstance().GetActionMethods(type).Exists(
                new Predicate<Internal.Descriptors.ActionMethod>((m) => { return m.MethodName == methodInfo.Name; }));
        }

        public void NonProxyableMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
        }
        #endregion IProxyGenerationHook

        #region Equals & GetHashCode
        public override bool Equals(object obj)
        {
            return obj is ControllerInterceptorGenerationHook;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Equals & GetHashCode
    }
}
