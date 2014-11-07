using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.ControllerView.AppConfig.Interface;
using MVCEngine.Tools;
using attribute = MVCEngine.ControllerView.Attributes;
using MVCEngine.ControllerView.Descriptors;

namespace MVCEngine.ControllerView.ViewEngine
{
    class ViewDispatcher : IAppConfigProcessor, IDisposable
    {
        #region Member
        List<string> _registeredView;
        List<View> _viewes;
        #endregion Member

        #region Constructor
        internal ViewDispatcher()
        {
            _registeredView = new List<string>();
            _viewes = new List<View>();
        }
        #endregion Constructor

        #region IAppConfigProcessor Members
        public void Process(System.Configuration.Configuration config)
        {
            IEnumerator<Type> registerd = ViewConfiguration.Process(config);
            while (registerd.MoveNext())
            {
                RegisterView(registerd.Current);
            }            
        }
        #endregion

        #region Register View
        private void RegisterView(Type type)
        {
            if (!_registeredView.Contains(type.FullName))
            {
                type.GetMethods().Where(m => !m.IsConstructor && !m.IsGenericMethod && m.IsPublic).
                        SelectMany(m => System.Attribute.GetCustomAttributes(m).Where(a => a.IsTypeOf<attribute.ActionCallBack>()),
                        (m, a) => new { Method = m, Attribute = a.CastToType<attribute.ActionCallBack>() }).ToList().ForEach((m) =>
                {
                    attribute.ActionCallBack actioncallback = m.Attribute;
                    View view = _viewes.FirstOrDefault(c => c.Name == actioncallback.ControllerName).
                    IfNullDefault<View>(() =>
                    {
                        return new View()
                        {
                            Name = actioncallback.ControllerName
                        };
                    });
                    ActionCallBack callback = view.CallBack.FirstOrDefault(call => call.ActionName == actioncallback.ActionName).
                    IfNullDefault<ActionCallBack>(() =>
                    {
                        return new ActionCallBack()
                        {
                            ActionName = actioncallback.ActionName
                        };
                    });
                    view.CallBack.AddIfNotContains(callback);
                    _viewes.AddIfNotContains(view);

                    Listener listener = callback.Listeners.FirstOrDefault(l => l.ThisObject == null && l.FullTypeName == type.FullName).
                    IfNullDefault(() =>
                    {
                        return new Listener()
                        {
                            FullTypeName = type.FullName
                        };
                    });
                    callback.Listeners.Add(listener);

                    Method callbackmethod = null;
                    if (m.Attribute.IsTypeOf<attribute.ActionMethodCallBack>())
                    {
                        callbackmethod = listener.ActionCallBack = new Method() { MethodTriger = LambdaTools.MethodTriger(type, m.Method) };
                    }
                    else if (m.Attribute.IsTypeOf<attribute.ActionMethodErrorBack>())
                    {
                        callbackmethod = listener.ActionErrorBack = new Method() { MethodTriger = LambdaTools.MethodTriger(type, m.Method) };
                    }
                    if (callbackmethod.IsNotNull())
                    {
                        m.Method.GetParameters().ToList().ForEach((p) =>
                        {
                            callbackmethod.Parameters.Add(new Parameter()
                            {
                                ParameterName = p.Name.ToUpper(),
                                ParameterType = p.ParameterType
                            });
                        });
                    }
                });
                _registeredView.Add(type.FullName);
            }
        }
        #endregion Register View

        #region Register Listener
        internal void RegisterListener(object view)
        {
            RegisterView(view.GetType());
            UpdateListner(view);
        }
        
        private void UpdateListner(object listener)
        {
            var listnerquery = _viewes.SelectMany(v => v.CallBack.
                        SelectMany(a => a.Listeners.Where(l => l.FullTypeName == listener.GetType().FullName && l.ThisObject.IsNull()).Take(1),
                                    (a, l) => new { CallBack = a, Listener = l }),
                        (v, al) => new { View = v, ActionListner = al });
            listnerquery.ToList().ForEach((element) =>
            {
                View v = element.View;
                ActionCallBack a = element.ActionListner.CallBack;
                Listener l = element.ActionListner.Listener;
                a.Listeners.Add(new Listener()
                {
                    ThisObject = listener,
                    ActionCallBack = l.ActionCallBack,
                    ActionErrorBack = l.ActionErrorBack
                });
            });
        }
        #endregion Register Listener

        #region UnRegister View
        public void UnRegisterListener(object listener)
        {
            var listnerquery = _viewes.SelectMany(v => v.CallBack.
                        SelectMany(a => a.Listeners.Where(l => listener.Equals(l.ThisObject)),
                                  (a, l) => new { ActionMethod = a, Listener = l }),
                        (v, al) => new { View = v, ActionListner = al });
            listnerquery.ToList().ForEach((element) => 
            {
                ActionCallBack a = element.ActionListner.ActionMethod;
                Listener l = element.ActionListner.Listener;

                l.ThisObject = null;
                a.Listeners.Remove(l);
            });
        }
        #endregion UnRegister View

        #region Proceed
        internal void Proceed(object arguments, string controllerName, string method, object value)
        {
            View view = _viewes.FirstOrDefault(c => c.Name == controllerName);
            if (view.IsNotNull())
            {
                ActionCallBack action = view.CallBack.FirstOrDefault(m => m.ActionName == method);
                if (action.IsNotNull())
                {
                    if (action.Listeners.IsNotNull())
                    {
                        foreach (Listener l in action.Listeners.Where(l => l.ThisObject.IsNotNull()))
                        {
                            if (value.IsNotNull())
                            {
                                ErrorView errorView = value.CastToType<ErrorView>();
                                if (errorView.IsNotNull())
                                {
                                    if (l.ActionErrorBack.IsNotNull())
                                    {
                                        MethodInvoker.Invoke(l.ThisObject, l.ActionErrorBack, errorView.Params);
                                    }
                                }
                                else if (l.ActionCallBack.IsNotNull())
                                {
                                    MethodInvoker.Invoke(l.ThisObject, l.ActionCallBack, value);
                                }
                            }
                            else if (l.ActionCallBack.IsNotNull())
                            {
                                MethodInvoker.Invoke(l.ThisObject, l.ActionCallBack, value);
                            }
                        }
                    }
                }
            }
        }
        #endregion Proceed

        #region IDisposable Members
        internal void Clear()
        {
            _viewes.ForEach((v) =>
            {
                v.CallBack.ForEach((cb) => 
                {
                    cb.Listeners.ForEach((l) => 
                    {
                        l.ThisObject = null;
                    });
                });
            });
            _viewes.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
        #endregion
    }
}
