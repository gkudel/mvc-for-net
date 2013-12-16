using Castle.Core.Interceptor;
using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model.Internal;
using System.Reflection;

namespace MVCEngine.Model.Interceptors
{
    internal class ModelInterceptorSelector : IInterceptorSelector
    {
        #region SelectInterceptors
        public IInterceptor[] SelectInterceptors(Type type, System.Reflection.MethodInfo method, IInterceptor[] interceptors)
        {               
            var query = from a in InterceptorDispatcher.GetInstnace().GetInterceptors(type, method)
                        join i in interceptors on new
                        {
                            Name = a.InterceptorFullName
                        }
                        equals new
                        {
                            Name = i.GetType().FullName
                        }
                        select i;
            return query.ToList().ToArray();
        }
        #endregion SelectInterceptors

        #region Equals & GetHashCode
        public override bool Equals(object obj)
        {
            return obj is ModelInterceptorSelector;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Equals & GetHashCode
    }
}
