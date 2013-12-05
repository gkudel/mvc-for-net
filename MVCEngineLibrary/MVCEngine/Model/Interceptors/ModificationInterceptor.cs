using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;
using MVCEngine;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class ModificationInterceptor : IInterceptor
    {
        #region Constructor
        public ModificationInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            ModelObject obj = null;
            if (invocation.InvocationTarget.IsTypeOf<ModelObject>())
            {
                obj = invocation.InvocationTarget.CastToType<ModelObject>();
                if(obj.State == ObjectState.Deleted)
                {
                    throw new InvalidOperationException();
                }
            }

            invocation.Proceed();     
            
            if(obj.IsNotNull() && invocation.Method.Name.StartsWith("set_"))
            {                                           
                if (obj.State == ObjectState.Unchanged)
                {
                    obj.State = ObjectState.Modified;
                }                
                obj.FirePropertyChanged(invocation.Method.Name.Substring(4, invocation.Method.Name.Length - 4));
            }
        }
        #endregion Inetercept
    }
}
