using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using MVCEngine;
using MVCEngine.Model.Internal;
using attribute = MVCEngine.Model.Attributes;
using MVCEngine.Model.Internal.Descriptions;
using MVCEngine.Model.Attributes.Formatter;
using System.Diagnostics;
using MVCEngine.Tools;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class ModificationInterceptor : Interceptor
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
        public const string Id = "ModificationInterceptor";

        public string GetId()
        {
            return ModificationInterceptor.Id;
        }

        public void Intercept(IInvocation invocation)
        {
            Entity obj = invocation.InvocationTarget.CastToType<Entity>();
            if (obj.IsNotNull() && !obj.Disposing)
            {
                if(obj.State == EntityState.Deleted)
                {
                    throw new InvalidOperationException();
                }

                string propertyName = invocation.Method.Name;
                if (propertyName.StartsWith("set_"))
                {
                    propertyName = propertyName.Substring(4, propertyName.Length - 4);
                }
                EntityProperty  property = obj.EntityCtx.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (property.IsNotNull() && invocation.Arguments.Length > 0)
                {
                    if (property.Formatters.ContainsKey(string.Empty))
                    {
                        foreach (Formatter f in property.Formatters[string.Empty])
                        {
                            invocation.Arguments[0] = f.Format(invocation.Arguments[0]);
                        }
                    }

                    if(!obj.IsFormatting)
                    {
                        obj.IsFormatting = true;
                        property.Formatters.Keys.Where(k => k != string.Empty).ToList().ForEach((k) =>
                        {
                            Debug.Assert(obj.EntityCtx.Properties.FirstOrDefault(p => p.Name == k).IsNotNull(), "Property[" + k + "] is not defined");
                            object value = invocation.Arguments[0];
                            foreach (Formatter f in property.Formatters[k])
                            {
                                value = f.Format(value);
                            }
                            obj[k] = value;
                        });
                        obj.IsFormatting = false;
                    }
                }
            }

            invocation.Proceed();

            if (obj.IsNotNull() && !obj.Disposing && invocation.Method.Name.StartsWith("set_"))
            {                                           
                if (obj.State == EntityState.Unchanged)
                {
                    obj.State = EntityState.Modified;
                }
                obj.FirePropertyChanged(invocation.Method.Name.Substring(4, invocation.Method.Name.Length - 4));
            }
        }
        #endregion Inetercept
    }
}
