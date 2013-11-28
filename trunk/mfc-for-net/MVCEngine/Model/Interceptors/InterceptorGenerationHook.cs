using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal;
using MVCEngine.Model.Attributes;

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
            var query = from a in System.Attribute.GetCustomAttributes(type)
                        where a.IsTypeOf<Interceptor>() && a.CastToType<Interceptor>().ColumnsName.Contains(GetMethodName(methodInfo))
                        select a;
            return query.Count() > 0;
        }
        #endregion IProxyGenerationHook

        #region Get Method Name
        private string GetMethodName(System.Reflection.MethodInfo method)
        {
            if (method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal))
            {
                return method.Name.Substring(4);
            }
            return method.Name;
        }
        #endregion Get Method Name

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
