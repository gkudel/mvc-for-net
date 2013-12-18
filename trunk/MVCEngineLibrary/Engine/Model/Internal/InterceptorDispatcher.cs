using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;
using attribute = MVCEngine.Model.Attributes;
using Castle.DynamicProxy;
using MVCEngine.Internal;
using System.Text.RegularExpressions;
using MVCEngine.Model.Exceptions;
using MVCEngine.Attributes;
using System.Reflection;
using MVCEngine.Model.Internal.Descriptions;
using desription = MVCEngine.Model.Internal.Descriptions;

namespace MVCEngine.Model.Internal
{
    internal class InterceptorDispatcher : IDisposable
    {
        #region Mebers
        private static Lazy<InterceptorDispatcher> _instance;
        private List<EntityClass> _modelClass;
        private Dictionary<string, Dictionary<string, List<desription.Interceptor>>> _interceptorMatched;
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
            _modelClass = new List<EntityClass>();
            _interceptorMatched = new Dictionary<string, Dictionary<string, List<desription.Interceptor>>>();
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
            return GetInterceptors(type.FullName, methodInfo.Name).Count() > 0;
        }

        public List<desription.Interceptor> GetInterceptors(System.Reflection.MethodInfo methodInfo)
        {
            return GetInterceptors(methodInfo.ReflectedType.FullName, methodInfo.Name);
        }

        public List<IInterceptor> GetInterceptorsObject(Type type)
        {
            return _modelClass.FirstOrDefault(m => m.FullName == type.FullName).InterceptorObjects;
        }

        internal void Initialize(Type type)
        {
            EntityClass modelclass = _modelClass.FirstOrDefault(m => m.FullName == type.FullName).IfNullDefault(() =>
            {
                EntityClass model = new EntityClass()
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
            EntityClass model = _modelClass.FirstOrDefault(m => m.FullName == type.FullName);
            if (model.IsNotNull())
            {
                Interceptor interceptor = LambdaTools.ObjectActivator(i.InterceptorClass, i.GenericType)().CastToType<Interceptor>();
                if (interceptor.IsNotNull())
                {
                    interceptor.Initialize(type, i);
                    model.InterceptorObjects.Add(interceptor);

                    desription.Interceptor inter = new desription.Interceptor()
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
                    throw new InterceptorDispatcherException("Class[" + i.InterceptorClass + "] should implement Interceptor abstract class");
                }
            }
        }

        private List<desription.Interceptor> GetInterceptors(string typeName, string methodName)
        {
            if (!_interceptorMatched.ContainsKey(typeName))
            {
                _interceptorMatched.Add(typeName, new Dictionary<string, List<desription.Interceptor>>());
            }
            if(!_interceptorMatched[typeName].ContainsKey(methodName))
            {
                List<desription.Interceptor> interceptors = new List<desription.Interceptor>();
                EntityClass entity = _modelClass.FirstOrDefault(m => m.FullName == typeName);
                if (entity.IsNotNull())
                {
                    interceptors = entity.Interceptors.SelectMany(i => i.Methods.Where(m => m == methodName), (i, m) => i).ToList();
                    interceptors.AddRange(_modelClass.Where(m => m.FullName == typeName).SelectMany(m => m.Interceptors.Where(i => !i.RegEx.IsNullOrEmpty() 
                                  && Regex.IsMatch(methodName, i.RegEx, RegexOptions.IgnoreCase)), (m, i) => i).ToList());
                }
                _interceptorMatched[typeName].Add(methodName, interceptors.ToList());
            }
            return _interceptorMatched[typeName][methodName];
        }
        #endregion Methods

        #region Dispose & Destructor
        public void Dispose()
        {
            _interceptorMatched.Clear();
            _modelClass.ForEach((e) => 
            {
                e.InterceptorObjects.Clear();
            });
            _modelClass.Clear();            
        }

        ~InterceptorDispatcher()
        {
            Dispose();
        }
        #endregion Dispose & Destructor
    }
}
