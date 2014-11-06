using MVCEngine.Tools.Exceptions;
using MVCEngine.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using descriptor = MVCEngine.ControllerView.Descriptors;
using MVCEngine;
using System.Configuration;
using MVCEngine.ControllerView.ViewEngine;
using System.Linq.Expressions;
using MVCEngine.Internal.Tools.Validation;
using System.Threading;
using Castle.DynamicProxy;
using MVCEngine.ControllerView.Interceptors;
using MVCEngine.Tools;
using MVCEngine.ControllerView.ControllerEngine;
using MVCEngine.ControllerView.AppConfig;
using MVCEngine.ControllerView.AppConfig.Interface;

namespace MVCEngine.ControllerView
{
    public sealed class Dispatcher: IDisposable 
    {
        #region Members
        private static readonly object _threadlock = new object();
        private static Lazy<Dispatcher> _instance = new Lazy<Dispatcher>(() => { return new Dispatcher(); });
        private ControllerDispatcher _controllerDispatcher;
        #endregion Members

        #region Constructor
        private Dispatcher() 
        {
            _controllerDispatcher = new ControllerDispatcher();
        }
        #endregion Constructor

        #region Properties
        internal static object ThreadLockObject { get { return _threadlock; } }
        #endregion Properties

        #region Instance Factory
        public static Dispatcher GetInstance()
        {
            return _instance.Value;
        }
        #endregion Instance Factory

        #region Invoke Action Method
        public void InvokeActionMethod(string controllerName, string actionMethodName, object param)
        {
            _controllerDispatcher.InvokeActionMethod(controllerName, actionMethodName, param);
        }
        #endregion Invoke Action Method

        #region Register View
        public void RegisterView(object view)
        {
            //lock (_threadlock)
            //{
            //    RegisterView(view.GetType());
            //    RegisterListener(view);
            //}
        }

        private void RegisterView(Type type)
        {
            /*if (!_views.Contains(type.FullName))
            {
                var idPropertiesList = type.GetProperties().Where(p => p.CanRead).SelectMany(p => System.Attribute.GetCustomAttributes(p).
                    Where(a => a.IsTypeOf<Id>()), (p, a) => new { Property = p, Attribute = a.CastToType<Id>(), Getter = LambdaTools.PropertyGetter(type, p) }).ToList();

                type.GetMethods().Where(m => !m.IsConstructor && !m.IsGenericMethod && m.IsPublic).
                        SelectMany(m => System.Attribute.GetCustomAttributes(m).Where(a => a.IsTypeOf<ActionCallBack>()),
                        (m, a) => new { Method = m, Attribute = a.CastToType<ActionCallBack>() }).ToList().ForEach((m) =>
                {
                    ActionCallBack actioncallback = m.Attribute;
                    descriptor.Controller controller = _controllers.FirstOrDefault(c => c.Name == actioncallback.ControllerName).
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
                    _controllers.AddIfNotContains(controller);

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
                _views.Add(type.FullName);
            }*/
        }
        #endregion Register View

        #region Register Listener
        public void RegisterListener(object listener)
        {
            /*lock (_threadlock)
            {
                var listnerquery = _controllers.SelectMany(c => c.ActionMethods.
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
            }*/
        }
        #endregion Register Listener

        #region UnRegister View
        public void UnRegisterListener(object listener)
        {
            /*var listnerquery = _controllers.SelectMany(c => c.ActionMethods.
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
            });*/
        }
        #endregion UnRegister View

        #region Proceed
        internal void Proceed(object arguments, string controllerName, string method, object value)
        {
            /*descriptor.Controller controller = _controllers.FirstOrDefault(c => c.Name == controllerName);
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
            }*/
        }
        #endregion Proceed
        
        #region Is Controller ActionMethod
        internal bool IsControllerActionMethod(Type type, string name)
        {
            return _controllerDispatcher.IsControllerActionMethod(type, name);
        }
        #endregion Is Controller ActionMethod

        #region App Config
        public void InitConfiguration()
        {
            AppConfiguration.Process(new IAppConfigProcessor[] { _controllerDispatcher });
        }
        #endregion 

        #region Dispose & Desctructor
        public void Dispose()
        {
            _controllerDispatcher.Clear();
            _controllerDispatcher = null;
            /* _views.Clear();*/
            _instance = null;
        }

        ~Dispatcher()
        {
            Dispose();
        }
        #endregion Dispose & Desctructor
    }
}

