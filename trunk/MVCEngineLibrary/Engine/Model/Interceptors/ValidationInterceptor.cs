using Castle.DynamicProxy;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal;
using desription = MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using attribute = MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Validation;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class ValidationInterceptor : Interceptor
    {
        #region Constructor
        public ValidationInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public override void Intercept(IInvocation invocation)
        {           
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (entity.IsNotNull()  && entity.Session.IsNullOrEmpty() && invocation.Arguments.Count() == 1)
            {
                string propertyName = invocation.Method.Name.StartsWith("set_") ? invocation.Method.Name.Substring(4, invocation.Method.Name.Length - 4 ) :
                                      invocation.Method.Name;
                if(!propertyName.IsNullOrEmpty())
                {       
                    if (entity.Table.IsNotNull())
                    {
                        bool validated = true;
                        var validateentityquery = entity.Table.Validators.Where(v => v.RealTimeValidation && (v.ColumnsName.IsNull() || v.ColumnsName.Contains(propertyName)));
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
                        var validatorcolumnquery = entity.Table.Columns.Where(c => c.Property == propertyName).
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

        #region Initialize
        public override void Initialize(Type entityType, attribute.Interceptor interceptor)
        { }
        #endregion Initialize
    }
}
