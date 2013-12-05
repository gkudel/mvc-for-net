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
    internal class InterceptorSelector : IInterceptorSelector
    {
        #region SelectInterceptors
        public IInterceptor[] SelectInterceptors(Type type, System.Reflection.MethodInfo method, IInterceptor[] interceptors)
        {               
            var query = from a in InterceptorDispatcher.GetInstnace().GetInterceptors(type, method)
                        join i in interceptors on new
                        {
                            a.Name,
                            a.Namespace,
                            Assembly = GetAssemblyName(a.Assembly.IfNullOrEmptyDefault(Assembly.GetExecutingAssembly().FullName))
                        }
                        equals new
                        {
                            i.GetType().Name,
                            i.GetType().Namespace,
                            Assembly = GetAssemblyName(i.GetType().Assembly.FullName)
                        }
                        select i;
            return query.ToList().ToArray();
        }

        private string GetAssemblyName(string assembly)
        {
            return string.IsNullOrEmpty(assembly) ? string.Empty :
                assembly.Substring(0, assembly.IndexOf(','));
        }
        #endregion SelectInterceptors

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
