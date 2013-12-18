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
        #region Constructor
        public ModificationInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public override  void Intercept(IInvocation invocation)
        {
            Entity obj = null;
            if (invocation.InvocationTarget.IsTypeOf<Entity>())
            {
                obj = invocation.InvocationTarget.CastToType<Entity>();
                if(obj.State == EntityState.Deleted)
                {
                    throw new InvalidOperationException();
                }
            }

            invocation.Proceed();     
            
            if(obj.IsNotNull() && invocation.Method.Name.StartsWith("set_"))
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

        #region Initialize
        public override void Initialize(Type entityType, attribute.Interceptor interceptor)
        { }
        #endregion Initialize
    }
}
