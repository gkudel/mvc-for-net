using Castle.Core.Interceptor;
using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal;

namespace MVCEngine.Model.Interceptors
{
    internal class InterceptorSelector : IInterceptorSelector
    {
        #region SelectInterceptors
        public IInterceptor[] SelectInterceptors(Type type, System.Reflection.MethodInfo method, IInterceptor[] interceptors)
        {
            var query = from a in System.Attribute.GetCustomAttributes(type)
                        where a.IsTypeOf<Interceptor>() && a.CastToType<Interceptor>().ColumnsName.Contains(GetMethodName(method))
                        join i in interceptors on a.CastToType<Interceptor>().InterceptorName equals i.GetType().Name
                        select i;
            return query.ToList().ToArray();
        }
        #endregion SelectInterceptors

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
            return obj is InterceptorSelector;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Equals & GetHashCode
    }
}
