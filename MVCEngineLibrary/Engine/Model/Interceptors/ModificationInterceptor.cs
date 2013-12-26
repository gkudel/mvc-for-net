using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using MVCEngine;
using MVCEngine.Model.Internal;
using attribute = MVCEngine.Model.Attributes;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class ModificationInterceptor : Interceptor
    {
        #region Members
        private static ModificationInterceptor _instance;
        #endregion Members

        #region Constructor
        private ModificationInterceptor()
        {
        }
        #endregion Constructor

        #region GetInstance
        internal static ModificationInterceptor GetInstance()
        {
            if(_instance.IsNull()) _instance = new ModificationInterceptor();
            return _instance;
        }
        #endregion GetInstance

        #region Inetercept
        public static string Id
        {
            get
            {
                return "ModificationInterceptor";
            }
        }

        public override string GetId()
        {
            return ModificationInterceptor.Id;
        }

        public override void Intercept(IInvocation invocation)
        {
            Entity obj = invocation.InvocationTarget.CastToType<Entity>();
            if(!obj.Disposing && obj.IsNotNull())
            {
                if(obj.State == EntityState.Deleted)
                {
                    throw new InvalidOperationException();
                }
            }

            invocation.Proceed();     
            
            if(!obj.Disposing && obj.IsNotNull() && invocation.Method.Name.StartsWith("set_"))
            {                                           
                if (obj.State == EntityState.Unchanged)
                {
                    obj.State = EntityState.Modified;
                }
                obj.Modified();
                obj.FirePropertyChanged(invocation.Method.Name.Substring(4, invocation.Method.Name.Length - 4));
            }
        }
        #endregion Inetercept
    }
}
