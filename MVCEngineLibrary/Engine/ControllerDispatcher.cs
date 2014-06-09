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
        private readonly object _threadlock = new object();
        private static ControllerDispatcher _instance = null;        
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

        public static object GetInstance(string controllerName)
        {
            return GetInstance().GetController(controllerName);
        }
        #endregion Instance Factory

        #region Invoke Action Method
        public void InvokeActionMethod(string controllerName, string actionMethodName, object param)
        {
            ArgumentValidator.GetInstnace().
                IsNotEmpty(controllerName, "controllerName").
                IsNotEmpty(actionMethodName, "actionMethodName");

            var actionQuery = _controllers.Value.Where(c => c.Name == controllerName).
                SelectMany(c => c.ActionMethods.Where(a => a.ActionName == actionMethodName),
                (c, a) => new { Controller = c, ActionMethod = a });
            var ca = actionQuery.FirstOrDefault();
            if (ca.IsNotNull() && ca.Controller.Object.IsNotNull())
            {
                if (param.GetType().IsArray)
                {
                    ca.ActionMethod.MethodInfo.Invoke(ca.Controller.Object, param.CastToType<object[]>());
                }
                else
                {
                    InvokeMethod(ca.Controller.Object, ca.ActionMethod.Method, param);
                }
            }
            else
            {
                throw new ActionMethodInvocationException("There is no Controller[" + controllerName + "] or Action Method[" + actionMethodName + "] register");
            }
        }
        #endregion Invoke Action Method

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
                                Type = type,
                            };
                            return c;
                        });

                        AddActionMethod(controller, type, action.ActionName, ma.Method.Name, ma.Method);
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

        private descriptor.ActionMethod AddActionMethod(descriptor.Controller controller, Type controllerType, string ActionName, string methodName, MethodInfo methodInfo)
        {
            descriptor.ActionMethod method = controller.ActionMethods.FirstOrDefault(am => am.ActionName == ActionName);
            if (method.IsNull())
            {
                method = method.IfNullDefault<descriptor.ActionMethod>(() =>
                {
                    return new descriptor.ActionMethod()
                    {
                        ActionName = ActionName,
                        MethodName = methodName,
                        MethodInfo = methodInfo,
                        Method = new descriptor.Method() { MethodTriger = LambdaTools.MethodTriger(controllerType, methodInfo) }
                    };
                });
                methodInfo.GetParameters().ToList().ForEach((p) =>
                {
                    method.Method.Parameters.Add(new descriptor.Parameter()
                    {
                        ParameterName = p.Name.ToUpper(),
                        ParameterType = p.ParameterType
                    });
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
                RegisterListener(view);
            }
        }

        private void RegisterView(Type type)
        {
            if (!_views.Value.Contains(type.FullName))
            {
                var idPropertiesList = type.GetProperties().Where(p => p.CanRead).SelectMany(p => System.Attribute.GetCustomAttributes(p).
                    Where(a => a.IsTypeOf<Id>()), (p, a) => new { Property = p, Attribute = a.CastToType<Id>(), Getter = LambdaTools.PropertyGetter(type, p) }).ToList();

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

                    descriptor.Listener listener = actionmethod.Listernes.FirstOrDefault(l => l.ThisObject == null && l.FullTypeName == type.FullName).
                    IfNullDefault(() =>
                    {
                        var viewid = idPropertiesList.FirstOrDefault(id => id.Attribute.ControllersName.IsNotNull() && id.Attribute.ControllersName.Contains(actioncallback.ControllerName));
                        if (viewid.IsNull())
                        {
                            viewid = idPropertiesList.FirstOrDefault(id => id.Attribute.ControllersName.Length == 0 || id.Attribute.ControllersName.IsNull());
                        }

                        return new descriptor.Listener()
                        {
                            FullTypeName = type.FullName,
                            Id = viewid.IsNotNull() ? viewid.Getter : null
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
            }
        }
        #endregion Register View

        #region Register Listener
        public void RegisterListener(object listener)
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
                        Id = l.Id,
                        ActionCallBack = l.ActionCallBack,
                        ActionErrorBack = l.ActionErrorBack
                    });
                });
            }
        }
        #endregion Register Listener

        #region UnRegister View
        public void UnRegisterListener(object listener)
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
        }
        #endregion UnRegister View

        #region Proceed
        internal void Proceed(object arguments, string controllerName, string method, object value)
        {
            descriptor.Controller controller = _controllers.Value.FirstOrDefault(c => c.Name == controllerName);
            if (controller.IsNotNull())
            {
                descriptor.ActionMethod action = controller.ActionMethods.FirstOrDefault(m => m.MethodName == method);
                if (action.IsNotNull())
                {
                    if (action.Listernes.IsNotNull())
                    {
                        foreach (descriptor.Listener l in action.Listernes.Where(l => l.ThisObject.IsNotNull()))
                        {
                            Func<object, bool> checkid = delegate(object id)
                            {
                                return id.IsNull() ||
                                    (l.Id.IsNotNull() && l.Id(l.ThisObject).Equals(id));
                            };
                            if (value.IsNotNull())
                            {
                                ErrorView errorView = value.CastToType<ErrorView>();
                                if (errorView.IsNotNull())
                                {
                                    if (l.ActionErrorBack.IsNotNull())
                                    {
                                        InvokeMethod(l.ThisObject, l.ActionErrorBack, errorView.Params, checkid);
                                    }
                                }
                                else if (l.ActionCallBack.IsNotNull())
                                {
                                    InvokeMethod(l.ThisObject, l.ActionCallBack, value, checkid);
                                }
                            }
                            else if (l.ActionCallBack.IsNotNull())
                            {
                                InvokeMethod(l.ThisObject, l.ActionCallBack, value, checkid);
                            }
                        }
                    }
                }
            }
        }

        private void InvokeMethod(object thisObject, descriptor.Method method, object param, Func<object, bool> checkId = null)
        {
            List<object> parameters = new List<object>();
            object id = null;
            if (param.IsNotNull() && param.IsAnonymousType())
            {
                descriptor.AnonymousType anonymous = method.Anonymous.FirstOrDefault(a => a.Name == param.GetType().FullName);

                if (anonymous.IsNull())
                {
                    PropertyInfo[] propertyinfo = param.IfNullDefault<object, PropertyInfo[]>(() => { return param.GetType().GetProperties(); },
                                        new PropertyInfo[0]);
                    PropertyInfo pinfo = propertyinfo.FirstOrDefault(p => p.Name.ToUpper() == "ID");

                    anonymous = new descriptor.AnonymousType()
                    {
                        Name = param.GetType().FullName,
                        MethodArguments = LambdaTools.GetMethodAttributes(param, method),
                    };
                    if (pinfo.IsNotNull()) anonymous.Id = LambdaTools.PropertyGetter(param.GetType(), pinfo);
                    method.Anonymous.Add(anonymous);
                }
                if (method.Parameters.Count > 0)
                {
                    parameters.AddRange(anonymous.MethodArguments(param));
                }
                if (anonymous.Id.IsNotNull()) id = anonymous.Id(param);


                if (checkId.IsNull())
                {
                    method.MethodTriger(thisObject, (parameters.Count > 0 ? parameters.ToArray() : null));
                }
                else if (checkId(id))
                {
                    method.MethodTriger(thisObject, (parameters.Count > 0 ? parameters.ToArray() : null));
                }
            }
        }
        #endregion Proceed

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
                        a.MethodInfo = null;
                        a.Listernes.ForEach((l) =>
                        {
                            l.ThisObject = null;
                        });
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

                    appconfig.ViewSection viewsection = config.GetSection("RegisterViews") as appconfig.ViewSection;
                    if (viewsection.IsNotNull())
                    {
                        foreach (appconfig.View view in viewsection.Views)
                        {
                            Type type = Type.GetType(view.Class);
                            RegisterView(type);
                        }
                    }
                }                
            });
        }
        #endregion App Config
    }
}

