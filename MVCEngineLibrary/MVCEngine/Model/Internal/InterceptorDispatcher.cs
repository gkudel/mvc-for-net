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
            });
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
            Initialize(type);
            return _modelClass.FirstOrDefault(m => m.FullName == type.FullName).Interceptors.SelectMany(i => i.Methods.Where(m => m == methodInfo.Name), (i, m) => m).Count() > 0 ||
                   _modelClass.Where(m => m.FullName == type.FullName).SelectMany(m => m.Interceptors.Where(i =>  !i.RegEx.IsNullOrEmpty() && Regex.IsMatch(methodInfo.Name, i.RegEx, RegexOptions.IgnoreCase)), (m, i) => i).Count() > 0;
        }

        public List<Interceptor> GetInterceptors(Type type, System.Reflection.MethodInfo methodInfo)
        {
            Initialize(type);
            List<Interceptor> interceptors = _modelClass.FirstOrDefault(m => m.FullName == type.FullName).Interceptors.SelectMany(i => i.Methods.Where(m => m == methodInfo.Name), (i, m) => i).ToList();
            interceptors.AddRange(_modelClass.Where(m => m.FullName == type.FullName).SelectMany(m => m.Interceptors.Where(i => !i.RegEx.IsNullOrEmpty() && Regex.IsMatch(methodInfo.Name, i.RegEx, RegexOptions.IgnoreCase)), (m, i) => i).ToList());
            return interceptors;
        }

        public List<IInterceptor> GetInterceptorsObject(Type type)
        {
            Initialize(type);
            return _modelClass.FirstOrDefault(m => m.FullName == type.FullName).InterceptorObjects;
        }

        private void Initialize(Type type)
        {
            var query = from a in System.Attribute.GetCustomAttributes(type)
                        where a.IsTypeOf<attribute.Interceptor>()
                        select a.CastToType<attribute.Interceptor>();

            ModelClass modelclass = _modelClass.FirstOrDefault(m => m.FullName == type.FullName).IfNullDefault(() =>
            {
                ModelClass model = new ModelClass()
                {
                    FullName = type.FullName
                };
                query.ToList().ForEach((i) =>
                {
                    TryCatchStatment.Try().Invoke(() =>
                    {
                        model.Interceptors.Add(new Interceptor()
                        {
                            Name = i.InterceptorName,
                            Namespace = i.Namespace,
                            Assembly = i.Assembly, 
                            Methods = new List<string>(i.MethodsName),
                            RegEx = i.RegEx
                        });
                        model.InterceptorObjects.Add(CmnTools.GetObjectActivator(i.Namespace + "." + i.InterceptorName, i.Assembly)().CastToType<IInterceptor>());
                    }).Catch((Message, Source, StackTrace, Exception) =>
                    {
                        this.ThrowException<InterceptorDispatcherException>(Message);
                    });
                });
                return model;
            });
            _modelClass.AddIfNotContains(modelclass);

        }
        #endregion Methods

    }
}
