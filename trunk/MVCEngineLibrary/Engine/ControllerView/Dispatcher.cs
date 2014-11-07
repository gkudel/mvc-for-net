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
        private ViewDispatcher _viewerDispatcher;
        #endregion Members

        #region Constructor
        private Dispatcher() 
        {
            _controllerDispatcher = new ControllerDispatcher();
            _viewerDispatcher = new ViewDispatcher();
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
        public void RegisterListener(object view)
        {
            lock (_threadlock)
            {
                _viewerDispatcher.RegisterListener(view);
            }
        }
        #endregion Register View

        #region UnRegister View
        public void UnRegisterListener(object listener)
        {
            _viewerDispatcher.UnRegisterListener(listener);
        }
        #endregion UnRegister View

        #region Proceed
        internal void Proceed(object arguments, string controllerName, string method, object value)
        {
            _viewerDispatcher.Proceed(arguments, controllerName, method, value);
        }
        #endregion Proceed
        
        #region Is Controller ActionMethod
        internal bool IsActionMethodForController(Type type, string name)
        {
            return _controllerDispatcher.IsActionMethodForController(type, name);
        }
        #endregion Is Controller ActionMethod

        #region App Config
        public void InitConfiguration()
        {
            AppConfiguration.Process(new IAppConfigProcessor[] { _controllerDispatcher, _viewerDispatcher });
        }
        #endregion 

        #region Dispose & Desctructor
        public void Dispose()
        {
            _controllerDispatcher.Clear();
            _controllerDispatcher = null;
            _viewerDispatcher.Clear();
            _viewerDispatcher = null;
            _instance = null;
        }

        ~Dispatcher()
        {
            Dispose();
        }
        #endregion Dispose & Desctructor
    }
}

