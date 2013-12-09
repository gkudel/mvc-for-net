using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;
using attribute = MVCEngine.Model.Attributes;
using Castle.Core.Interceptor;
using MVCEngine.Internal;
using System.Text.RegularExpressions;
using MVCEngine.Model.Exceptions;
using MVCEngine.Attributes;
using System.Reflection;

namespace MVCEngine.Model.Internal
{
    internal class InterceptorDispatcher
    {
        #region Mebers
        private static Lazy<InterceptorDispatcher> _instance;
        private List<ModelClass> _modelClass;
        #endregion Members

        #region Constructors
        static InterceptorDispatcher()
        {
            _instance = new Lazy<InterceptorDispatcher>(() =>
            {
                return new InterceptorDispatcher();
            }, true);
        }

        private InterceptorDispatcher()
        {
            _modelClass = new List<ModelClass>();
        }
        #endregion Constructors

        #region GetInstnace
        public static InterceptorDispatcher GetInstnace()
        {
            return _instance.Value;
        }
        #endregion GetInstance

        #region Methods
        public bool ShouldBeIntercept(Type type, System.Reflection.MethodInfo methodInfo)
        {
            return _modelClass.FirstOrDefault(m => m.FullName == type.FullName).Interceptors.SelectMany(i => i.Methods.Where(m => m == methodInfo.Name), (i, m) => m).Count() > 0 ||
                   _modelClass.Where(m => m.FullName == type.FullName).SelectMany(m => m.Interceptors.Where(i =>  !i.RegEx.IsNullOrEmpty() && Regex.IsMatch(methodInfo.Name, i.RegEx, RegexOptions.IgnoreCase)), (m, i) => i).Count() > 0;
        }

        public List<Interceptor> GetInterceptors(Type type, System.Reflection.MethodInfo methodInfo)
        {
            List<Interceptor> interceptors = _modelClass.FirstOrDefault(m => m.FullName == type.FullName).Interceptors.SelectMany(i => i.Methods.Where(m => m == methodInfo.Name), (i, m) => i).ToList();
            interceptors.AddRange(_modelClass.Where(m => m.FullName == type.FullName).SelectMany(m => m.Interceptors.Where(i => !i.RegEx.IsNullOrEmpty() && Regex.IsMatch(methodInfo.Name, i.RegEx, RegexOptions.IgnoreCase)), (m, i) => i).ToList());
            return interceptors;
        }

        public List<IInterceptor> GetInterceptorsObject(Type type)
        {
            return _modelClass.FirstOrDefault(m => m.FullName == type.FullName).InterceptorObjects;
        }

        internal void Initialize(Type type)
        {
            ModelClass modelclass = _modelClass.FirstOrDefault(m => m.FullName == type.FullName).IfNullDefault(() =>
            {
                ModelClass model = new ModelClass()
                {
                    FullName = type.FullName
                };
                return model;
            });

            _modelClass.AddIfNotContains(modelclass);

            var query = from a in System.Attribute.GetCustomAttributes(type)
                        where a.IsTypeOf<attribute.Interceptor>()
                        select a.CastToType<attribute.Interceptor>();
            query.ToList().ForEach((i) =>
            {
                Initialize(type, i);
            });           
        }

        public void Initialize(Type type, attribute.Interceptor i)
        {
            ModelClass model = _modelClass.FirstOrDefault(m => m.FullName == type.FullName);
            if (model.IsNotNull())
            {
                TryCatchStatment.Try().Invoke(() =>
                {
                    IInterceptor interceptor = LambdaTools.ObjectActivator(i.InterceptorClass, i.GenericType)().CastToType<IInterceptor>();
                    if (interceptor.IsNotNull())
                    {
                        model.InterceptorObjects.Add(interceptor);

                        Interceptor inter = new Interceptor()
                        {
                            InterceptorFullName = interceptor.GetType().FullName,
                            Methods = new List<string>(i.MethodsName),
                            RegEx = i.RegEx
                        };
                        model.Interceptors.Add(inter);

                        interceptor.GetType().GetProperties().AsEnumerable().Where(p => p.CanWrite).
                        SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<ValueFromAttribute>()),
                        (p, a) => new { Property = p, Attribute = a.CastToType<ValueFromAttribute>() }).
                        ToList().ForEach((pa) =>
                        {
                            PropertyInfo info = i.GetType().GetProperty(pa.Attribute.PropertyName.IfNullOrEmptyDefault(pa.Property.Name));
                            if (info.IsNotNull())
                            {
                                pa.Property.SetValue(interceptor, info.GetValue(i, null), null);
                            }
                        });
                    }
                    else
                    {
                        this.ThrowException<InterceptorDispatcherException>("Class[" + i.InterceptorClass + "] should implement IInterceptor interface");
                    }
                }).Catch((Message, Source, StackTrace, Exception) =>
                {
                    this.ThrowException<InterceptorDispatcherException>(Message);
                });
            }
        }
        #endregion Methods
    }
}
