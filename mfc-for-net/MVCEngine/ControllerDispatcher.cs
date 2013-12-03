using MVCEngine.Attributes;
using MVCEngine.Exceptions;
using MVCEngine.Internal;
using MVCEngine.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using descriptor = MVCEngine.Internal.Descriptor;
using MVCEngine;
using System.Configuration;
using MVCEngine.View;
using appconfig = MVCEngine.AppConfig;
using System.Linq.Expressions;

namespace MVCEngine
{
    public sealed class ControllerDispatcher: IDisposable 
    {
        #region Members
        private Lazy<List<descriptor.Controller>> _controllers;
        private Lazy<List<string>> _views;
        private MethodInfo _miChangeType;
        private static ControllerDispatcher _instance = null;
        #endregion Members

        #region Constructor
        private ControllerDispatcher() 
        {
            _controllers = new Lazy<List<descriptor.Controller>>(() => { return new List<descriptor.Controller>(); }, true);
            _views = new Lazy<List<string>>(() => { return new List<string>(); }, true);
            _miChangeType = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });
        }
        #endregion Constructor

        #region Instance Factory
        public static ControllerDispatcher GetInstance()
        {
            _instance = _instance.IfNullDefault<ControllerDispatcher>(() => { return new ControllerDispatcher(); });
            return _instance;
        }
        #endregion Instance Factory

        #region Invoke Action Method
        public object InvokeActionMethod(string controllerName, string actionMethod, object param = null, object controllerProperties = null)
        {
            Validator.GetInstnace().
                IsNotEmpty(controllerName, "controllerName").
                IsNotEmpty(actionMethod, "actionMethod");

            object ret = null;
            RedirectView redirect = null;
            var actionquery = _controllers.Value.Where(c => c.Name == controllerName).
                SelectMany(c => c.ActionMethods.Where(a => a.ActionName == actionMethod),
                (c, a) => new { Controller = c, ActionMethod = a });
            var ca = actionquery.FirstOrDefault();
            if (ca.IsNotNull())
            {
                descriptor.Controller controller = ca.Controller;
                descriptor.ActionMethod action = ca.ActionMethod;
                object objecttoinvoke = null;
                Func<object> controllerActivator = controller.ControllerActivator.IfNullDefault(action.ControllerActivator);
                if (controllerActivator.IsNotNull())
                {
                    TryCatchStatment.Try().Invoke(() =>
                    {
                        objecttoinvoke = controllerActivator();
                        if (controller.ControllerActivator.IsNotNull())
                        {
                            controller.DefaultValues.Keys.ToList().ForEach((k) =>
                            {   
                                controller.PropertiesSetters[k](objecttoinvoke, controller.DefaultValues[k]);
                            });
                        }
                        if (controllerProperties.IsNotNull() && controllerProperties.IsAnonymousType())
                        {
                            controllerProperties.GetType().GetProperties().ToList().ForEach((p) =>
                            {
                                if (!controller.PropertiesSetters.ContainsKey(p.Name))
                                {
                                    Action<object, object> a = GetPropertySetter(objecttoinvoke.GetType(), p.Name);
                                    if (a.IsNotNull())
                                    {
                                        controller.PropertiesSetters.Add(p.Name, a);
                                    }
                                }
                                if (controller.PropertiesSetters.ContainsKey(p.Name))
                                {
                                    controller.PropertiesSetters[p.Name](objecttoinvoke, p.GetValue(controllerProperties, null));
                                }
                            });
                        }
                    }).Catch((Message, Source, StackTrace, Exception) =>
                    {
                        this.ThrowException<ActionMethodInvocationException>(Message);
                    });
                }

                ret = param;
                if (objecttoinvoke.IsNotNull())
                {
                    TryCatchStatment.Try().Invoke(() =>
                    {
                        ret = InvokeMethod(action.Action, objecttoinvoke, param);
                    }).Catch((Message, Source, StackTrace, Exception) =>
                    {
                        this.ThrowException<ActionMethodInvocationException>(Message);
                    }).Finally(() =>
                    {
                        if (objecttoinvoke.IsTypeOf<IDisposable>())
                        {
                            objecttoinvoke.CastToType<IDisposable>().Dispose();
                        }
                    });
                }
                List<descriptor.Listener> listeners = action.Listernes;
                if(ret.IsTypeOf<ForwardView>())
                {
                    ForwardView forward = ret.CastToType<ForwardView>();
                    var redirectquery = controller.ActionMethods.Where(a => a.ActionName == forward.ActionMethod).
                        Select(a => a.Listernes);

                    if (!forward.ControllerName.IsNullOrEmpty() &&
                        !forward.ControllerName.IsEquals(controllerName))
                    {
                        redirectquery = _controllers.Value.Where(c => c.Name == forward.ControllerName).
                            SelectMany(c => c.ActionMethods.Where(a => a.ActionName == forward.ActionMethod),
                            (c, a) => a.Listernes);
                    }

                    listeners = redirectquery.FirstOrDefault();
                    ret = forward.Params;
                }
                if (ret.IsTypeOf<RedirectView>())
                {
                    redirect = ret.CastToType<RedirectView>();
                    ret = redirect.Params;
                }
                if (listeners.IsNotNull())
                {
                    foreach (descriptor.Listener l in listeners.Where(l => l.ThisObject.IsNotNull()))
                    {
                        if (l.IdProperty.IsNotNull())
                        {
                            object viewid = l.IdProperty.GetValue(l.ThisObject, null);
                            if (viewid.IsNull()) continue;
                            PropertyInfo pinfo = ret.GetType().GetProperty(l.IdParameterName);
                            object retid = null;
                            if (pinfo.IsNull())
                            {
                                pinfo = param.GetType().GetProperty(l.IdParameterName);
                                if (pinfo.IsNull()) continue;
                                retid = pinfo.GetValue(param, null);
                            }
                            else
                            {
                                retid = pinfo.GetValue(ret, null);
                            }
                            if (!viewid.GetType().IsInstanceOfType(retid))
                            {
                                TryCatchStatment.Try().Invoke(() =>
                                {
                                    retid = Convert.ChangeType(retid, viewid.GetType());
                                }).Catch<InvalidCastException, FormatException, OverflowException, ArgumentNullException>(() =>
                                {
                                    retid = null;
                                });
                            }
                            if (viewid.IsNotEquals(retid)) continue;
                        }

                        if (ret.IsTypeOf<ErrorView>())
                        {
                            if (l.ActionErrorBack.IsNotNull())
                            {
                                TryCatchStatment.Try().Invoke(() =>
                                {
                                    InvokeMethod(l.ActionErrorBack, l.ThisObject, ret.CastToType<ErrorView>().Params);
                                }).Catch((Message, Source, StackTrace, Exception) =>
                                {
                                    this.ThrowException<ActionMethodInvocationException>(Message);
                                });
                            }
                        }
                        else if (l.ActionCallBack.IsNotNull())
                        {
                            TryCatchStatment.Try().Invoke(() =>
                            {
                                InvokeMethod(l.ActionCallBack, l.ThisObject, ret);
                            }).Catch((Message, Source, StackTrace, Exception) =>
                            {
                                this.ThrowException<ActionMethodInvocationException>(Message);
                            });
                        }
                    }
                }
            }
            else
            {
                this.ThrowException<ActionMethodInvocationException>("There is no Controller[" + controllerName + "] or Action Method[" + actionMethod + "] register");
            }
            
            if (redirect.IsNotNull())
            {
                return InvokeActionMethod(redirect.ControllerName.IfNullOrEmptyDefault(controllerName), redirect.ActionMethod, redirect.RedirectParams, redirect.ControllerProperties.IfNullDefault(controllerProperties));
            }
            else
            {
                return ret;
            }
        }

        private object InvokeMethod(descriptor.Method method, object objecttoinvoke, object param)
        {
            object ret = null;
            List<object> parameters = new List<object>();
            if (param.IsNotNull() && param.IsAnonymousType())
            {
                PropertyInfo[] propertyinfo = param.IfNullDefault<object, PropertyInfo[]>(() => { return param.GetType().GetProperties(); }, 
                                                                                                    new PropertyInfo[0]);
                var joinquery = from p in method.Parameters
                                join v in propertyinfo on p.ParameterName equals v.Name.ToUpper() into vp
                                from v in vp.DefaultIfEmpty()
                                select new { Parameter = p, Value = v };

                joinquery.ToList().ForEach((v) =>
                {
                    if (v.Value.IsNotNull())
                    {
                        parameters.Add(v.Value.GetValue(param, null));
                    }
                    else
                    {
                        parameters.Add(null);
                    }
                });
            }
            else if (param.IsNotNull() && param.IsTypeOf<object[]>())
            {
                parameters.AddAndAppendByDefault(param.CastToType<object[]>(), method.Parameters.Count, null);
            }
            else if (param.IsNotNull())
            {
                parameters.AddAndAppendByDefault(new object[] { param }, method.Parameters.Count, null);
            }
            else
            {
                parameters.AddAndAppendByDefault(new object[0], method.Parameters.Count, null);
            }
            ret = method.MethodTriger(objecttoinvoke, (parameters.Count > 0 ? parameters.ToArray() : null));
            return ret;
        }
        #endregion Invoke Action Method

        #region Register Controller
        public void RegisterController(Type type)
        {
            RegisterController(type, null);
        }

        private void RegisterController(Type type, Func<object> controlActivator)
        {
            var query = from a in System.Attribute.GetCustomAttributes(type)
                        where a is Controller
                        select a.CastToType< Controller>();
            Controller controlerAttribute = query.FirstOrDefault();
            if (controlerAttribute.IsNotNull())
            {
                descriptor.Controller controller = _controllers.Value.FirstOrDefault(c => c.Name == controlerAttribute.ControllerName);
                if (controller.IsNull() || controller.ControllerActivator.IsNull())
                {
                    type.GetMethods().AsEnumerable().Where(m => !m.IsConstructor && !m.IsGenericMethod && m.IsPublic).
                        SelectMany(m => System.Attribute.GetCustomAttributes(m).Where(a => a.IsTypeOf<ActionMethod>()),
                        (m, a) => new { Method = m, Attribute = a.CastToType<ActionMethod>() }).
                        ToList().ForEach((ma) =>
                    {
                        ActionMethod action = ma.Attribute;
                        controller = controller.IfNullDefault<descriptor.Controller>(() =>
                        {
                            descriptor.Controller c = new descriptor.Controller()
                            {
                                Name = controlerAttribute.ControllerName
                            };
                            type.GetProperties().AsEnumerable().Where(p => p.CanWrite).
                            SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<ValueFromControllerAttribute>()),
                            (p, a) => new { Property = p, Attribute = a.CastToType<ValueFromControllerAttribute>() }).
                            ToList().ForEach((pa) =>
                            {
                                PropertyInfo info = controlerAttribute.GetType().GetProperty(pa.Attribute.PropertyName.IfNullOrEmptyDefault(pa.Property.Name));
                                if (info.IsNotNull())
                                {
                                    c.DefaultValues.Add(info.Name, info.GetValue(controlerAttribute, null));
                                    c.PropertiesSetters.Add(info.Name, GetPropertySetter(type, pa.Property));
                                }
                            });
                            return c;
                        });

                        controller.ControllerActivator = controlActivator.IfNullDefault(() => { return GetControllerActivator(type); });
                        AddActionMethod(controller, action.ActionName, GetMethodTriger(type, ma.Method), ma.Method);
                    });

                    if (controller.IsNotNull())
                    {
                        _controllers.Value.AddIfNotContains(controller);
                    }
                }
                else
                {
                    this.ThrowException<ControllerRegistrationException>(controlerAttribute.ControllerName + " already register ");
                }
            }
            else
            {
                this.ThrowException<ControllerRegistrationException>("Type[" + type.FullName + "] it cann't be recognise as valid Controller. Controller Attribute on class level is required.");
            }
        }

        public void RegisterActionMethod(Type controllerType, string controllerMethod, string controllerName, string actionMethod)
        {
            MethodInfo mInfo = controllerType.GetMethod(controllerMethod);
            if (mInfo.IsNotNull() && mInfo.IsPublic)
            {
                descriptor.Controller controller = _controllers.Value.FirstOrDefault(c => c.Name == controllerName);
                controller = controller.IfNullDefault<descriptor.Controller>(() =>
                {
                    return new descriptor.Controller()
                    {
                        Name = controllerName
                    };                    
                });
                descriptor.ActionMethod method = AddActionMethod(controller, actionMethod, GetMethodTriger(controllerType, mInfo), mInfo);
                method.ControllerActivator = GetControllerActivator(controllerType);
                _controllers.Value.AddIfNotContains(controller);
            }
            else
            {
                this.ThrowException<ControllerRegistrationException>("Type[" + controllerType.FullName + "] doesn't have public implementation of Method[" + controllerMethod + "]");
            }
        }

        private descriptor.ActionMethod AddActionMethod(descriptor.Controller controller, string ActionName, Func<object, object[], object> methodTriger, MethodInfo mInfo)
        {
            descriptor.ActionMethod method = controller.ActionMethods.FirstOrDefault(am => am.ActionName == ActionName);
            if (method.IsNull() || method.Action.IsNull())
            {
                method = method.IfNullDefault<descriptor.ActionMethod>(() =>
                {
                    return new descriptor.ActionMethod()
                    {
                        ActionName = ActionName
                    };
                });
                method.Action = new descriptor.Method() { MethodTriger = methodTriger };
                mInfo.GetParameters().ToList().ForEach((p) =>
                {
                    method.Action.Parameters.Add(new descriptor.Parameter()
                    {
                        ParameterName = p.Name.ToUpper(),
                        ParameterType = p.ParameterType
                    });
                });
                controller.ActionMethods.Add(method);
            }
            else
            {
                this.ThrowException<ControllerRegistrationException>("Action[" +ActionName + "] is declared at least twice");
            }
            return method;
        }
        #endregion Register Controller

        #region Register View
        public void RegisterView(object view)
        {
            RegisterView(view.GetType());
            RegisterListener(view);
        }

        private void RegisterView(Type type)
        {
            if (!_views.Value.Contains(type.FullName))
            {
                var propertyquery = type.GetProperties().
                    SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<ViewId>()),
                    (p, a) => new { Property = p, Attribute = a.CastToType<ViewId>() });
                var propertyid = propertyquery.FirstOrDefault();
                if (propertyquery.Count() > 1)
                {
                    this.ThrowException<ViewRegistrationException>("At least two properties is marked as Id");
                }

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

                    descriptor.Listener listener = new descriptor.Listener()
                    {
                        FullTypeName = type.FullName
                    };
                    if (propertyid.IsNotNull())
                    {
                        listener.IdProperty = propertyid.Property;
                        listener.IdParameterName = propertyid.Attribute.ParameterName.IfNullOrEmptyDefault(propertyid.Property.Name);
                    }
                    actionmethod.Listernes.Add(listener);

                    descriptor.Method callbackmethod = null;
                    if (m.Attribute.IsTypeOf<ActionMethodCallBack>())
                    {
                        callbackmethod = listener.ActionCallBack = new descriptor.Method() { MethodTriger = GetMethodTriger(type, m.Method) };
                    }
                    else if (m.Attribute.IsTypeOf<ActionMethodErrorBack>())
                    {
                        callbackmethod = listener.ActionErrorBack = new descriptor.Method() { MethodTriger = GetMethodTriger(type, m.Method) };
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
                    IdProperty = l.IdProperty,
                    IdParameterName = l.IdParameterName,
                    ActionCallBack = l.ActionCallBack,
                    ActionErrorBack = l.ActionErrorBack
                });
            });
        }
        #endregion Register Listener

        #region UnRegister View
        public void UnRegisterView(object view)
        {
            var listnerquery = _controllers.Value.SelectMany(c => c.ActionMethods.
                        SelectMany(a => a.Listernes.Where(l => view.Equals(l.ThisObject)),
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

        #region Dispose & Desctructor
        public void Dispose()
        {
            _controllers.Value.ForEach((c) =>
            {
                c.ActionMethods.ForEach((a) =>
                {
                    a.Listernes.ForEach((l) =>
                    {
                        l.ThisObject = null;
                    });
                });
            });
        }

        ~ControllerDispatcher()
        {
            Dispose();
        }
        #endregion Dispose & Desctructor

        #region Lambda Expressions
        private Func<object> GetControllerActivator(Type controllerType)
        {
            Func<object> ret = null;
            ConstructorInfo ctor = controllerType.GetConstructors().FirstOrDefault(c => c.GetParameters().Count() == 0);
            if (ctor != null)
            {
                ret = (Func<object>)Expression.Lambda(typeof(Func<object>), Expression.New(ctor, null), null).Compile();
            }
            else
            {
                this.ThrowException<ControllerRegistrationException>("Type[" + controllerType.FullName + "] should have no arguments constructor");
            }
            return ret;
        }

        private Action<object, object> GetPropertySetter(Type objectType, string name)
        {
            PropertyInfo pinfo = objectType.GetProperty(name);
            if (pinfo.IsNotNull())
            {
                return GetPropertySetter(objectType, pinfo);
            }
            return null;
       }

        private Action<object, object> GetPropertySetter(Type objectType, PropertyInfo propertyInfo)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object));
            Expression convertObj = Expression.Convert(obj, objectType);
            ParameterExpression value = Expression.Parameter(typeof(object));
            DefaultExpression defaultvalue = Expression.Default(propertyInfo.PropertyType);
            return Expression.Lambda<Action<object, object>>(Expression.TryCatch(
                    Expression.Assign(Expression.MakeMemberAccess(convertObj, propertyInfo), Expression.Convert(value, propertyInfo.PropertyType)),
                    Expression.Catch(typeof(InvalidCastException), Expression.Assign(Expression.MakeMemberAccess(convertObj, propertyInfo), defaultvalue)),
                    Expression.Catch(typeof(FormatException), Expression.Assign(Expression.MakeMemberAccess(convertObj, propertyInfo), defaultvalue)),
                    Expression.Catch(typeof(OverflowException), Expression.Assign(Expression.MakeMemberAccess(convertObj, propertyInfo), defaultvalue)),
                    Expression.Catch(typeof(ArgumentNullException), Expression.Assign(Expression.MakeMemberAccess(convertObj, propertyInfo), defaultvalue))),
                obj, value).Compile();
        }

        private Func<object, object[], object> GetMethodTriger(Type objectType, MethodInfo info)
        {
            
            ParameterExpression obj = Expression.Parameter(typeof(object));
            Expression convertObj = Expression.Convert(obj, objectType);
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            ParameterInfo[] paramsInfo = info.GetParameters();
            Expression[] argsExp = new Expression[paramsInfo.Length];
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                if (_miChangeType.IsNotNull())
                {
                    argsExp[i] = Expression.TryCatch(Expression.Convert(Expression.Call(_miChangeType, Expression.ArrayIndex(param, index), Expression.Constant(paramType)),paramType),
                                 Expression.Catch(typeof(Exception), Expression.Default(paramType)));
                }
                else
                {
                    argsExp[i] = Expression.TryCatch(Expression.Convert(Expression.ArrayIndex(param, index), paramType),
                                                     Expression.Catch(typeof(Exception), Expression.Default(paramType)));
                }
            }          
            if (!info.ReturnType.Equals(typeof(void)))
            {                
                return Expression.Lambda<Func<object, object[], object>>(Expression.Call(convertObj, info, argsExp), obj, param).Compile();
            }
            else
            {
                return Expression.Lambda<Func<object, object[], object>>(Expression.Block(Expression.Call(convertObj, info, argsExp), 
                                                                          Expression.Constant(null)), obj, param).Compile();
            }
        }
        #endregion Lambda Expressions

        #region App Config
        public void AppeConfigInitialization()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            appconfig.ControllerSection controllersection = config.GetSection("RegisterControllers") as appconfig.ControllerSection;
            if (controllersection.IsNotNull())
            {
                foreach (appconfig.Controller controller in controllersection.Controllers)
                {
                    object obj = null;                    
                    TryCatchStatment.Try().Invoke(() =>
                    {
                        Type type = Type.GetType(controller.Class + controller.Assembly.IfNotNullOrEmptyDefault("," + controller.Assembly));
                        Func<object> objectActivator = GetControllerActivator(type);
                        obj = objectActivator();
                        RegisterController(obj.GetType(), objectActivator);
                    }).Catch((Message, Source, StackTrace, Exception) =>
                    {
                        this.ThrowException<ControllerRegistrationException>(Message);
                    }).Finally(() =>
                    {
                        if (obj.IsNotNull() && obj.IsTypeOf<IDisposable>())
                        {
                            obj.CastToType<IDisposable>().Dispose();
                        }
                    });
                }
            }

            appconfig.ViewSection viewsection = config.GetSection("RegisterViews") as appconfig.ViewSection;
            if (viewsection.IsNotNull())
            {
                foreach (appconfig.View view in viewsection.Views)
                {
                    object obj = null;
                    TryCatchStatment.Try().Invoke(() =>
                    {
                        Type type = Type.GetType(view.Class + view.Assembly.IfNotNullOrEmptyDefault("," + view.Assembly));
                        Func<object> objectActivator = GetControllerActivator(type);
                        obj = objectActivator();
                        if (obj.IsNotNull())
                        {
                            RegisterView(obj.GetType());
                        }

                    }).Catch((Message, Source, StackTrace, Exception) =>
                    {
                        this.ThrowException<ControllerRegistrationException>(Message);
                    }).Finally(() =>
                    {
                        if (obj.IsNotNull() && obj.IsTypeOf<IDisposable>())
                        {
                            obj.CastToType<IDisposable>().Dispose();
                        }
                    });
                }
            }
        }
        #endregion App Config
    }
}

