using Castle.Core.Interceptor;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class ValidationInterceptor : IInterceptor
    {
        #region Constructor
        public ValidationInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (entity.IsNotNull() && invocation.Arguments.Count() == 1)
            {
                string propertyName = invocation.Method.Name.StartsWith("set_") ? invocation.Method.Name.Substring(4, invocation.Method.Name.Length - 4 ) :
                                      invocation.Method.Name;
                if(!propertyName.IsNullOrEmpty())
                {                    
                    Table table = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().Name);
                    if (table.IsNull() && entity.GetType().BaseType.IsNotNull())
                    {
                        table = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().BaseType.Name);
                    }
                    if (table.IsNotNull())
                    {
                        bool validated = true;
                        var validateentityquery = table.Validators.Where(v => v.RealTimeValidation && (v.ColumnsName.IsNull() || v.ColumnsName.Contains(propertyName)));
                        validateentityquery.ToList().ForEach((v) =>
                        {
                            if (!v.Validate(entity, propertyName, invocation.Arguments[0]))
                            {
                                validated = false;
                                if (v.IfFaildThrowException)
                                {
                                    throw new ValidationException(v.ErrrorMessage);
                                }
                            }
                        });
                        var validatorcolumnquery = table.Columns.Where(c => c.Property == propertyName).
                            SelectMany(c => c.Validators.Where(v => v.RealTimeValidation), (c, v) => v);
                        if (validatorcolumnquery.ToList().Count() > 0)
                        {
                            validatorcolumnquery.ToList().ForEach((v) =>
                            {
                                if (!v.Validate(invocation.Arguments[0]))
                                {
                                    validated = false;
                                    if (v.IfFaildThrowException)
                                    {
                                        throw new ValidationException(v.ErrrorMessage);
                                    }
                                }
                            });
                        }
                        if (validated) invocation.Proceed();
                    }
                    else
                    {
                        invocation.Proceed();
                    }
                }
                else
                {
                    invocation.Proceed();
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
        #endregion Inetercept
    }
}
