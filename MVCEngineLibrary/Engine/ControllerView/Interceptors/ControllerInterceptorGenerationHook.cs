using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using MVCEngine.ControllerView;

namespace MVCEngine.ControllerView.Interceptors
{
    class ControllerInterceptorGenerationHook : IProxyGenerationHook
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
            return Dispatcher.GetInstance().IsControllerActionMethod(type, methodInfo.Name);                            
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
