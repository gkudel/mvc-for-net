using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MVCEngine;
using Castle.DynamicProxy;
using MVCEngine.Model.Interceptors;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal;
using MVCEngine.Model.Interceptors.Interface;

namespace MVCEngine.Model
{
    public class ModelList<T> : BindingList<T> where T : class
    {
        #region Members
        private static readonly Lazy<ProxyGenerator> _generator;
        #endregion Members

        #region Constructors
        static ModelList()
        {
            _generator = new Lazy<ProxyGenerator>(() =>
            {
                return new ProxyGenerator(new PersistentProxyBuilder());
            });
        }
        #endregion Constructors

        #region New Method Implmentation
        new public void Add(T item)
        {
            this.ThrowException<ModelListException>("Method AddNew should be used instead of Add");
        }
        #endregion New Method Implmentation

        #region Override
        protected override object AddNewCore()
        {
            return CreateInstance();
        }
        #endregion Override


        #region Create Object
        private static T CreateInstance()
        {            
            var options = new ProxyGenerationOptions(new InterceptorGenerationHook()) { Selector = new InterceptorSelector() };
            var proxy = _generator.Value.CreateClassProxy(typeof(T), options, InterceptorDispatcher.GetInstnace().GetInterceptorsObject(typeof(T)).ToArray());
            if (proxy.IsTypeOf<IObjectState>())
            {
                proxy.CastToType<IObjectState>().State = ObjectState.Added;
            }
            return proxy as T;
        }
        #endregion Create Object
    }
}
