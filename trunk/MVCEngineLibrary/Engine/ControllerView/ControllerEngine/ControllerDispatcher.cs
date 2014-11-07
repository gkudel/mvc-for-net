using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.ControllerView.ControllerEngine.Interface;
using MVCEngine.ControllerView.AppConfig.Interface;
using MVCEngine.Tools;
using Castle.DynamicProxy;
using MVCEngine.ControllerView.Descriptors;
using attribute = MVCEngine.ControllerView.Attributes;
using System.Reflection;
using MVCEngine.ControllerView.Interceptors;
using MVCEngine.Tools.Exceptions;
using MVCEngine.Internal.Tools.Validation;

namespace MVCEngine.ControllerView.ControllerEngine
{
    class ControllerDispatcher : IAppConfigProcessor, IDisposable
    {
        #region Members
        List<Controller> _controllers;
        List<IControllerMenagment> _managment;
        static readonly ProxyGenerator _generator;

        #endregion Members

        #region Constructor
        static ControllerDispatcher()
        {
            _generator = new ProxyGenerator(new PersistentProxyBuilder());
        }

        internal ControllerDispatcher()
        {
            _managment = new List<IControllerMenagment>();
            _controllers = new List<Controller>();
        }
        #endregion Constructor        
    
        #region IAppConfigProcessor Members
        public void Process(System.Configuration.Configuration config)
        {
            IEnumerator<Type> registerd = ControllerConfiguration.Process(config);
            while(registerd.MoveNext())
            {
                RegisterController(registerd.Current);
            }
        }
        #endregion

        #region Register Controller
        private void RegisterController(Type type)
        {
            var query = from a in System.Attribute.GetCustomAttributes(type)
                        where a is attribute.Controller
                        select a.CastToType<attribute.Controller>();
            attribute.Controller controlerAttribute = query.FirstOrDefault();
            if (controlerAttribute.IsNotNull())
            {
                Controller controller = _controllers.FirstOrDefault(c => c.Name == controlerAttribute.ControllerName);
                if (controller.IsNull() || controller.Object.IsNull())
                {
                    type.GetMethods().AsEnumerable().Where(m => !m.IsConstructor && !m.IsGenericMethod && m.IsPublic
                        && !m.IsStatic && m.IsVirtual).SelectMany(m => System.Attribute.GetCustomAttributes(m).Where(a => a.IsTypeOf<ActionMethod>()),
                        (m, a) => new { Method = m, Attribute = a.CastToType<ActionMethod>() }).
                        ToList().ForEach((ma) =>
                    {
                        ActionMethod action = ma.Attribute;
                        controller = controller.IfNullDefault<Controller>(() =>
                        {
                            Controller c = new Controller()
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
                        _controllers.AddIfNotContains(controller);
                        if (controller.Object.IsNull())
                        {
                            var options = new ProxyGenerationOptions(new ControllerInterceptorGenerationHook()) { Selector = new ControllerInterceptorSelector() };
                            controller.Object  = _generator.CreateClassProxy(type, options, new ControllerInterceptor(controlerAttribute.ControllerName));
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

        private ActionMethod AddActionMethod(Controller controller, Type controllerType, string ActionName, string methodName, MethodInfo methodInfo)
        {
            ActionMethod method = controller.ActionMethods.FirstOrDefault(am => am.ActionName == ActionName);
            if (method.IsNull())
            {
                method = method.IfNullDefault<ActionMethod>(() =>
                {
                    return new ActionMethod()
                    {
                        ActionName = ActionName,
                        MethodName = methodName,
                        MethodInfo = methodInfo,
                        Method = new Method() { MethodTriger = LambdaTools.MethodTriger(controllerType, methodInfo) }
                    };
                });
                methodInfo.GetParameters().ToList().ForEach((p) =>
                {
                    method.Method.Parameters.Add(new Parameter()
                    {
                        ParameterName = p.Name.ToUpper(),
                        ParameterType = p.ParameterType
                    });
                });
                controller.ActionMethods.Add(method);
            }
            else
            {
                throw new ControllerRegistrationException("Action[" + ActionName + "] is declared at least twice");
            }
            return method;
        }
        #endregion Register Controller

        #region Is Controller ActionMethod
        internal bool IsActionMethodForController(Type type, string name)
        {
            Controller controller = _controllers.FirstOrDefault(c => c.Type == type
                                                || (c.Type.BaseType.IsNotNull() && c.Type.BaseType == type));
            if (controller.IsNotNull())
            {
                return controller.ActionMethods.Exists(new Predicate<ControllerView.Descriptors.ActionMethod>((m) => { return m.MethodName == name; }));
            }
            return false;
        }
        #endregion Is Controller ActionMethod

        #region Invoke Action Method
        internal void InvokeActionMethod(string controllerName, string actionMethodName, object param)
        {
            ArgumentValidator.GetInstnace().
                IsNotEmpty(controllerName, "controllerName").
                IsNotEmpty(actionMethodName, "actionMethodName");

            var actionQuery = _controllers.Where(c => c.Name == controllerName).
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
                    MethodInvoker.Invoke(ca.Controller.Object, ca.ActionMethod.Method, param);
                }
            }
            else
            {
                throw new ActionMethodInvocationException("There is no Controller[" + controllerName + "] or Action Method[" + actionMethodName + "] register");
            }
        }
        #endregion Invoke Action Method

        #region IDisposable Members
        internal void Clear()
        {
            _controllers.ForEach((c) =>
            {
                c.Object = null;
                c.ActionMethods.ForEach((a) =>
                {
                    a.MethodInfo = null;
                });
            });
        }

        public void Dispose()
        {
            Clear();
        }
        #endregion
    }
}
