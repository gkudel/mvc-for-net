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

namespace MVCEngine
{
    public class ControllerDispatcher: IDisposable 
    {
        #region Members
        private List<descriptor.Controller> _controllers;
        #endregion Members

        #region Constructor
        public ControllerDispatcher()
        { }
        #endregion Constructor

        #region Properties
        private List<descriptor.Controller> Controllers
        {
            get
            {
                if (_controllers == null) _controllers = new List<descriptor.Controller>();
                return _controllers;
            }
        }
        #endregion Properties

        #region Invoke Action Method
        public void InvokeActionMethod(string controllerName, string actionMethod, object param)
        {
            Validator.GetInstnace().
                IsNotEmpty(controllerName, "controllerName").
                IsNotEmpty(actionMethod, "actionMethod");
            descriptor.Controller controller = Controllers.FirstOrDefault(c => c.Name == controllerName);
            if(controller.IsNotNull())
            {
                descriptor.ActionMethod action = controller.ActionMethods.FirstOrDefault(a => a.ActionName == actionMethod);
                if (action.IsNotNull())
                {
                    object objecttoinvoke = null;
                    TryCatchStatment.Try().Invoke(() =>
                    {
                        if (controller.ConstructorParams.IsNotNull())
                        {
                            objecttoinvoke = Activator.CreateInstance(controller.ControllerType, controller.ConstructorParams());
                        }
                        else
                        {
                            objecttoinvoke = Activator.CreateInstance(controller.ControllerType);
                        }
                    }).Catch((Message, Source, StackTrace, Exception) =>
                    {
                        this.ThrowException<ActionMethodInvocationException>(Message);
                    });

                    if (objecttoinvoke.IsNotNull())
                    {
                        try
                        {
                            object ret = InvokeMethod(action.Action, objecttoinvoke, param);
                            if (action.Listernes.Count() > 0)
                            {
                                action.Listernes.ForEach((l) =>
                                {
                                    if (ret.IsTypeOf<ErrorView>())
                                    {
                                        if (l.ActionErrorBack.IsNotNull())
                                        {
                                            InvokeMethod(l.ActionErrorBack, l.ThisObject, ret.CastToType<ErrorView>().Params);
                                        }
                                    }
                                    else if (l.ActionCallBack.IsNotNull())
                                    {
                                        InvokeMethod(l.ActionCallBack, l.ThisObject, ret);
                                    }
                                });
                            }
                        }
                        finally
                        {
                            if (objecttoinvoke.IsTypeOf<IDisposable>())
                            {
                                objecttoinvoke.CastToType<IDisposable>().Dispose();
                            }
                        }
                    }
                }
                else
                {
                    this.ThrowException<ActionMethodInvocationException>(controllerName + " doesn't handle " + actionMethod + " action method");
                }
            }
            else
            {
                this.ThrowException<ActionMethodInvocationException>("There is no controller for " + controllerName);
            }            
        }

        private object InvokeMethod(descriptor.Method method, object objecttoinvoke, object param)
        {
            object ret = null;
            List<object> parameters = new List<object>();
            if (param.IsNull() || (param.GetType().Namespace.IsNull() && param.GetType().Name.Contains("Anonymous")))
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
                        if (v.Parameter.ParameterType.IsInstanceOfType(v.Value.PropertyType))
                        {
                            parameters.Add(v.Value.GetValue(param, null));
                        }
                        else
                        {
                            TryCatchStatment.Try().Invoke(() =>
                            {
                                parameters.Add(Convert.ChangeType(v.Value.GetValue(param, null), v.Parameter.ParameterType));
                            }).Catch<InvalidCastException, FormatException, OverflowException, ArgumentNullException>(() =>
                            {
                                parameters.Add(null);
                            }).Throw();
                        }
                    }
                    else
                    {
                        parameters.Add(null);
                    }
                });
                ret = method.MethodInfo.Invoke(objecttoinvoke, parameters.Count > 0 ? parameters.ToArray() : null);
            }
            else
            {
                this.ThrowException<ActionMethodInvocationException>(param.GetType().FullName + " isn't anonymous type. Only Anonymous type can be passed as parameter or return as result of ActionMethod ");
            }
            return ret;
        }
        #endregion Invoke Action Method

        #region Register Controller
        public void RegisterController(Type type, Func<object[]> constructorParams)
        {
            var query = from a in System.Attribute.GetCustomAttributes(type)
                        where a is Controller
                        select a;
            Controller controlerAttribute = query.FirstOrDefault().CastToType<Controller>();
            if (controlerAttribute.IsNotNull())
            {
                descriptor.Controller controller = Controllers.FirstOrDefault(c =>c.Name == controlerAttribute.ControllerName);
                if (controller.IsNull() || controller.ControllerType.IsNull())
                {
                    
                    type.GetMethods().AsEnumerable().Where(m => !m.IsConstructor
                        && !m.IsGenericMethod
                        && m.IsPublic).ToList().ForEach((m) =>
                    {
                        var atrributequery = from a in System.Attribute.GetCustomAttributes(m)
                                             where a.IsTypeOf<ActionMethod>()
                                             select a;
                        atrributequery.ToList().ForEach((a) => {
                            ActionMethod action = a.CastToType<ActionMethod>();
                            controller = controller.IfNullDefault<descriptor.Controller>(() => 
                                                    { 
                                                        return new descriptor.Controller() 
                                                        { 
                                                            Name = controlerAttribute.ControllerName
                                                        }; 
                                                    });

                            controller.ControllerType = type;
                            controller.ConstructorParams = constructorParams;

                            descriptor.ActionMethod method = controller.ActionMethods.FirstOrDefault(am => am.ActionName == action.ActionName);
                            if (method.IsNull() || method.Action.IsNull())
                            {
                                method = method.IfNullDefault<descriptor.ActionMethod>(() =>
                                                {
                                                    return new descriptor.ActionMethod() 
                                                    {
                                                        ActionName = action.ActionName
                                                    };
                                                });
                                method.Action = new descriptor.Method() { MethodInfo = m };
                                m.GetParameters().ToList().ForEach((p) =>
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
                                this.ThrowException<ActionMethodInvocationException>(action.ActionName + " is declared at least twice");
                            }
                        });
                        
                    });
                    if (controller.IsNotNull())
                    {
                        Controllers.AddIfNotContains(controller);
                    }
                }
                else
                {
                    this.ThrowException<ActionMethodInvocationException>(controlerAttribute.ControllerName + " already register ");
                }
            }
            else
            {
                this.ThrowException<ArgumentException>("For " + type.FullName + " isn't defined controler name.");
            }
        }
        #endregion Register Controller

        #region Register View
        public void RegisterView(object view)
        {
            view.GetType().GetMethods().Where(m => !m.IsConstructor
                    && !m.IsGenericMethod
                    && m.IsPublic).ToList().ForEach((m) =>
            {
                var atrributequery = from a in System.Attribute.GetCustomAttributes(m)
                                     where a.IsTypeOf<ActionCallBack>()
                                     select a;
                atrributequery.ToList().ForEach((a) =>
                {
                    ActionCallBack actioncallback = a.CastToType<ActionCallBack>();
                    descriptor.Controller controller = Controllers.FirstOrDefault(c => c.Name == actioncallback.ControllerName).
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
                    Controllers.AddIfNotContains(controller);

                    descriptor.Listener listener = new descriptor.Listener()
                    {
                        ThisObject = view
                    };
                    actionmethod.Listernes.Add(listener);

                    descriptor.Method callbackmethod = null;
                    if (a.IsTypeOf<ActionMethodCallBack>())
                    {
                        callbackmethod = listener.ActionCallBack = new descriptor.Method() { MethodInfo = m };
                    }
                    else if (a.IsTypeOf<ActionMethodErrorBack>())
                    {
                        callbackmethod = listener.ActionErrorBack = new descriptor.Method() { MethodInfo = m };
                    }
                    if (callbackmethod.IsNotNull())
                    {
                        m.GetParameters().ToList().ForEach((p) =>
                        {
                            callbackmethod.Parameters.Add(new descriptor.Parameter()
                            {
                                ParameterName = p.Name.ToUpper(),
                                ParameterType = p.ParameterType
                            });
                        });
                    }
                });
            });
        }
        #endregion Register View

        #region UnRegister View
        public void UnRegisterView(object view)
        {
            view.GetType().GetMethods().Where(m => !m.IsConstructor
                    && !m.IsGenericMethod
                    && m.IsPublic).ToList().ForEach((m) =>
            {
                var atrributequery = from a in System.Attribute.GetCustomAttributes(m)
                                     where a.IsTypeOf<ActionCallBack>()
                                     select a;
                atrributequery.ToList().ForEach((a) =>
                {
                    ActionCallBack actioncallback = a.CastToType<ActionCallBack>();
                    descriptor.Controller controller = Controllers.FirstOrDefault(c => c.Name == actioncallback.ControllerName);
                    if (controller.IsNotNull())
                    {
                        descriptor.ActionMethod actionmethod = controller.ActionMethods.FirstOrDefault(action => action.ActionName == actioncallback.ActionName);
                        if (actionmethod.IsNotNull())
                        {
                            actionmethod.Listernes.Where(l => l.ThisObject.Equals(view)).ToList().ForEach((l) =>
                            {
                                l.ThisObject = null;
                            });
                            actionmethod.Listernes.RemoveAll((l) => { return l.ThisObject.IsNull(); });
                        }
                    }
                });
            });
        }
        #endregion UnRegister View

        #region Dispose & Desctructor
        public void Dispose()
        {
            Controllers.ForEach((c) =>
            {
                c.ConstructorParams = null;
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
    }
}

