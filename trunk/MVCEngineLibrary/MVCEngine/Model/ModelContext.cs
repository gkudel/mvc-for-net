using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal;
using Castle.DynamicProxy;

namespace MVCEngine.Model
{
    public class ModelContext
    {
        #region Members
        private static readonly ProxyGenerator _generator = new ProxyGenerator(new PersistentProxyBuilder());
        #endregion Members

        #region Constructor
        public ModelContext()
        {
        }
        #endregion Constructor

        #region Create Object
        public static T CreateInstance<T>() where T : class
        {
            return CreateInstance<T>(null);
        }

        public static T CreateInstance<T>(object[] constuctorparams) where T : class
        {
            var options = new ProxyGenerationOptions(new InterceptorGenerationHook()) { Selector = new InterceptorSelector() };
            var proxy = constuctorparams.IsNull() ? _generator.CreateClassProxy(typeof(T), options, new SecurityInterceptor())
                : _generator.CreateClassProxy(typeof(T), options, constuctorparams, new SecurityInterceptor());
            return proxy as T;
        }
        #endregion Create Object
    }
}
