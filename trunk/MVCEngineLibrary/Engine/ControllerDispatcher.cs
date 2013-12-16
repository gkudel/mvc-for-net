using MVCEngine.Attributes;
using MVCEngine.Exceptions;
using MVCEngine.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using descriptor = MVCEngine.Internal.Descriptors;
using MVCEngine;
using System.Configuration;
using MVCEngine.View;
using appconfig = MVCEngine.AppConfig;
using System.Linq.Expressions;
using MVCEngine.Internal.Validation;
using System.Threading;
using Castle.DynamicProxy;
using MVCEngine.Interceptors;

namespace MVCEngine
{
    public sealed class ControllerDispatcher: IDisposable 
    {
        #region Members
        private Lazy<List<descriptor.Controller>> _controllers;
        private Lazy<List<string>> _views;
        private static ControllerDispatcher _instance = null;
        private readonly object _threadlock = new object();
        private static readonly Lazy<ProxyGenerator> _generator;
        #endregion Members

        #region Constructor
        static ControllerDispatcher()
        {
            _generator = new Lazy<ProxyGenerator>(() =>
            {
                return new ProxyGenerator(new PersistentProxyBuilder());
            });
        }
        private ControllerDispatcher() 
        {
            _controllers = new Lazy<List<descriptor.Controller>>(() => { return new List<descriptor.Controller>(); }, true);
            _views = new Lazy<List<string>>(() => { return new List<string>(); }, true);            
        }
        #endregion Constructor

        #region Instance Factory
        public static ControllerDispatcher GetInstance()
        {
            _instance = _instance.IfNullDefault<ControllerDispatcher>(() => { return new ControllerDispatcher(); });
            return _instance;
        }
        #endregion Instance Factory

        #region Controller
        public T GetController<T>() where T : class
        {
            T controller = null;
            lock (_threadlock)
            {                
                descriptor.Controller c = _controllers.Value.FirstOrDefault(ctr => ctr.Type == typeof(T)
                                          || (ctr.Type.BaseType.IsNotNull() && ctr.Type.BaseType == typeof(T)));
                if (c.IsNotNull())
                {
                    if (c.Object.IsTypeOf<T>())
                    {
                        controller = c.Object.CastToType<T>();
                    }
                }
            }
            return controller;
        }

        public object GetController(string controllerName) 
        {
            object controller = null;
            lock (_threadlock)
            {
                descriptor.Controller c = _controllers.Value.FirstOrDefault(ctr => ctr.Name == controllerName);
                if (c.IsNotNull())
                {
                    controller = c.Object;
                }
            }
            return controller;
        }
        #endregion Controller

        #region Get Action Methods
        public List<descriptor.ActionMethod> GetActionMethods(string controllerName)
        {
            descriptor.Controller controller = _controllers.Value.FirstOrDefault(c => c.Name == controllerName);
            if (controller.IsNotNull())
            {
                return controller.ActionMethods;
            }
            return new List<descriptor.ActionMethod>();
        }

        public List<descriptor.ActionMethod> GetActionMethods(Type type)
        {
            descriptor.Controller controller = _controllers.Value.FirstOrDefault(c => c.Type == type
                                                || (c.Type.BaseType.IsNotNull() && c.Type.BaseType == type));
            if (controller.IsNotNull())
            {
                return controller.ActionMethods;
            }
            return new List<descriptor.ActionMethod>();
        }
        #endregion Get Action Methods

        #region Register Controller
        private void RegisterController(Type type)
        {
            var query = from a in System.Attribute.GetCustomAttributes(type)
                        where a is Controller
                        select a.CastToType< Controller>();
            Controller controlerAttribute = query.FirstOrDefault();
            if (controlerAttribute.IsNotNull())
            {
                descriptor.Controller controller = _controllers.Value.FirstOrDefault(c => c.Name == controlerAttribute.ControllerName);
                if (controller.IsNull() || controller.Object.IsNull())
                {
                    type.GetMethods().AsEnumerable().Where(m => !m.IsConstructor && !m.IsGenericMethod && m.IsPublic
                        && !m.IsStatic && m.IsVirtual).SelectMany(m => System.Attribute.GetCustomAttributes(m).Where(a => a.IsTypeOf<ActionMethod>()),
                        (m, a) => new { Method = m, Attribute = a.CastToType<ActionMethod>() }).
                        ToList().ForEach((ma) =>
                    {
                        ActionMethod action = ma.Attribute;
                        controller = controller.IfNullDefault<descriptor.Controller>(() =>
                        {
                            descriptor.Controller c = new descriptor.Controller()
                            {
                                Name = controlerAttribute.ControllerName,
                                Type = type
                            };
                            return c;
                        });

                        AddActionMethod(controller, action.ActionName, ma.Method.Name);
                    });
                    
                    if (controller.IsNotNull())
                    {
                        _controllers.Value.AddIfNotContains(controller);
                        if (controller.Object.IsNull())
                        {
                            var options = new ProxyGenerationOptions(new ControllerInterceptorGenerationHook()) { Selector = new ControllerInterceptorSelector() };
                            controller.Object  = _generator.Value.CreateClassProxy(type, options, new ControllerInterceptor(controlerAttribute.ControllerName));
                        }                        
                    }
                }
                else
                {
                    throw new ControllerRegistrationException(controlerAttribute.ControllerName + " already register ");
                }
            }
            else
            {
                throw new ControllerRegistrationException("Type[" + type.FullName + "] it cann't be recognise as valid Controller. Controller Attribute on class level is required.");
            }
        }

        /*public void RegisterActionMethod(Type controllerType, string controllerMethod, string controllerName, string actionMethod, bool isAsynchronousInvoke)
        {
            lock (_threadlock)
            {
                MethodInfo mInfo = controllerType.GetMethod(controllerMethod);
                if (mInfo.IsNotNull() && mInfo.IsPublic && !mInfo.IsStatic)
                {
                    descriptor.Controller controller = _controllers.Value.FirstOrDefault(c => c.Name == controllerName);
                    controller = controller.IfNullDefault<descriptor.Controller>(() =>
                    {
                        return new descriptor.Controller()
                        {
                            Name = controllerName
                        };
                    });
                    descriptor.ActionMethod method = AddActionMethod(controller, actionMethod, isAsynchronousInvoke, LambdaTools.MethodTriger(controllerType, mInfo), mInfo);
                    method.ControllerActivator = LambdaTools.ObjectActivator(controllerType);
                    _controllers.Value.AddIfNotContains(controller);
                }
                else
                {
                    throw new ControllerRegistrationException("Type[" + controllerType.FullName + "] doesn't have public non static implementation of Method[" + controllerMethod + "]");
                }
            }
        }*/

        private descriptor.ActionMethod AddActionMethod(descriptor.Controller controller, string ActionName, string methodName)
        {
            descriptor.ActionMethod method = controller.ActionMethods.FirstOrDefault(am => am.ActionName == ActionName);
            if (method.IsNull())
            {
                method = method.IfNullDefault<descriptor.ActionMethod>(() =>
                {
                    return new descriptor.ActionMethod()
                    {
                        ActionName = ActionName,
                        MethodName = methodName
                    };
                });
                controller.ActionMethods.Add(method);
            }
            else
            {
                throw new ControllerRegistrationException("Action[" +ActionName + "] is declared at least twice");
            }
            return method;
        }
        #endregion Register Controller

        #region Register View
        public void RegisterView(object view)
        {
            lock (_threadlock)
            {
                RegisterView(view.GetType());
                //RegisterListener(view);
            }
        }

        private void RegisterView(Type type)
        {
            /*if (!_views.Value.Contains(type.FullName))
            {               
                type.GetMethods().Where(m => !m.IsConstructor && !m.IsGenericMethod && m.IsPublic).
                        SelectMany(m => System.Attribute.GetCustomAttributes(m).Where(a => a.IsTypeOf<ActionCallBack>()),
                        (m, a) => new { Method = m, Attribute = a.CastToType<ActionCallBack>() }).ToList().ForEach((m) =>
                {
                    ActionCallBack actioncallback = m.Attribute;
                    descriptor.Controller controller = _controllers.Value.FirstOrDefault(c => c.Name == actioncallback.ControllerName).
                    IfNullDefault<descriptor.Controller>(() =>
                    {
                        return new descriptor.Controller()
                        {
                            Name = actioncallback.ControllerName
                        };
                    });
                    descriptor.ActionMethod actionmethod = controller.ActionMethods.FirstOrDefault(action => action.ActionName == actioncallback.ActionName).
                    IfNullDefault<descriptor.ActionMethod>(() =>
                    {
                        return new descriptor.ActionMethod()
                        {
                            ActionName = actioncallback.ActionName
                        };
                    });
                    controller.ActionMethods.AddIfNotContains(actionmethod);
                    _controllers.Value.AddIfNotContains(controller);

                    descriptor.Listener listener = actionmethod.Listernes.FirstOrDefault(l => l.ThisObject == null).
                    IfNullDefault(() =>
                    {
                        return new descriptor.Listener()
                        {
                            FullTypeName = type.FullName
                        };
                    });
                    actionmethod.Listernes.Add(listener);

                    descriptor.Method callbackmethod = null;
                    if (m.Attribute.IsTypeOf<ActionMethodCallBack>())
                    {
                        callbackmethod = listener.ActionCallBack = new descriptor.Method() { MethodTriger = LambdaTools.MethodTriger(type, m.Method) };
                    }
                    else if (m.Attribute.IsTypeOf<ActionMethodErrorBack>())
                    {
                        callbackmethod = listener.ActionErrorBack = new descriptor.Method() { MethodTriger = LambdaTools.MethodTriger(type, m.Method) };
                    }
                    if (callbackmethod.IsNotNull())
                    {
                        m.Method.GetParameters().ToList().ForEach((p) =>
                        {
                            callbackmethod.Parameters.Add(new descriptor.Parameter()
                            {
                                ParameterName = p.Name.ToUpper(),
                                ParameterType = p.ParameterType
                            });
                        });
                    }
                });
                _views.Value.Add(type.FullName);
            }*/
        }
        #endregion Register View

        #region Register Listener
        /*public void RegisterListener(object listener)
        {
            lock (_threadlock)
            {
                var listnerquery = _controllers.Value.SelectMany(c => c.ActionMethods.
                            SelectMany(a => a.Listernes.Where(l => l.FullTypeName == listener.GetType().FullName && l.ThisObject.IsNull()).Take(1),
                                        (a, l) => new { ActionMethod = a, Listener = l }),
                            (c, al) => new { Controller = c, ActionListner = al });
                listnerquery.ToList().ForEach((element) =>
                {
                    descriptor.Controller c = element.Controller;
                    descriptor.ActionMethod a = element.ActionListner.ActionMethod;
                    descriptor.Listener l = element.ActionListner.Listener;
                    a.Listernes.Add(new descriptor.Listener()
                    {
                        ThisObject = listener,
                        ActionCallBack = l.ActionCallBack,
                        ActionErrorBack = l.ActionErrorBack
                    });
                });
            }
        }*/
        #endregion Register Listener

        #region UnRegister View
        /*public void UnRegisterListener(object listener)
        {
            var listnerquery = _controllers.Value.SelectMany(c => c.ActionMethods.
                        SelectMany(a => a.Listernes.Where(l => listener.Equals(l.ThisObject)),
                                  (a, l) => new { ActionMethod = a, Listener = l }),
                        (c, al) => new { Controller = c, ActionListner = al });
            listnerquery.ToList().ForEach((element) => 
            {
                descriptor.Controller c = element.Controller;
                descriptor.ActionMethod a = element.ActionListner.ActionMethod;
                descriptor.Listener l = element.ActionListner.Listener;

                l.ThisObject = null;
                a.Listernes.Remove(l);
            });
        }*/
        #endregion UnRegister View

        #region Dispose & Desctructor
        public void Dispose()
        {
            if(_controllers.IsValueCreated)
            {
                _controllers.Value.ForEach((c) =>
                {
                    c.Object = null;
                    c.ActionMethods.ForEach((a) =>
                    {
                        if (a.ActionCallBack.IsNotNull())
                        {
                            Delegate[] del = a.ActionCallBack.GetInvocationList();
                            for (int i = del.Count() - 1; i >= 0; i--)
                            {
                                a.ActionCallBack -= (Action)del[i];
                            }
                        }
                    });
                });
            }
            if (_views.IsValueCreated)
            {
                _views.Value.Clear();
            }
            _instance = null;
        }

        ~ControllerDispatcher()
        {
            Dispose();
        }
        #endregion Dispose & Desctructor

        #region App Config
        public void AppeConfigInitialization()
        {
            Task.Factory.StartNew(() =>
            {
                lock (_threadlock)
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    appconfig.ControllerSection controllersection = config.GetSection("RegisterControllers") as appconfig.ControllerSection;
                    if (controllersection.IsNotNull())
                    {
                        foreach (appconfig.Controller controller in controllersection.Controllers)
                        {
                            Type type = Type.GetType(controller.Class);
                            RegisterController(type);
                        }
                    }

                    /*appconfig.ViewSection viewsection = config.GetSection("RegisterViews") as appconfig.ViewSection;
                    if (viewsection.IsNotNull())
                    {
                        foreach (appconfig.View view in viewsection.Views)
                        {
                            object obj = null;
                            try
                            {
                                Type type = Type.GetType(view.Class);
                                Func<object> objectActivator = LambdaTools.ObjectActivator(type);
                                obj = objectActivator();
                                if (obj.IsNotNull())
                                {
                                    RegisterView(obj.GetType());
                                }

                            }
                            finally
                            {
                                if (obj.IsNotNull() && obj.IsTypeOf<IDisposable>())
                                {
                                    obj.CastToType<IDisposable>().Dispose();
                                }
                            }
                        }
                    }*/
                }                
            });
        }
        #endregion App Config
    }
}

