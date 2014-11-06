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
using MVCEngine.Tools;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class ValidationInterceptor : Interceptor
    {
        #region Members
        private static ValidationInterceptor _instance;
        #endregion Members

        #region Constructor
        private ValidationInterceptor()
        {
        }
        #endregion Constructor

        #region GetInstance
        internal static ValidationInterceptor GetInstance()
        {
            if (_instance.IsNull()) _instance = new ValidationInterceptor();
            return _instance;
        }
        #endregion GetInstance

        #region Inetercept
        public const string Id = "ValidationInterceptor";

        public string GetId()
        {
            return ValidationInterceptor.Id;
        }

        public void Intercept(IInvocation invocation)
        {           
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (!entity.Disposing && entity.IsNotNull() && entity.Session.IsNullOrEmpty() && invocation.Arguments.Count() == 1)
            {
                string propertyName = invocation.Method.Name;
                if (propertyName.StartsWith("set_"))
                {
                    propertyName = propertyName.Substring(4, propertyName.Length - 4);
                }
                if (entity.EntityCtx.IsNotNull())
                {
                    bool validated = true;
                    var validateentityquery = entity.EntityCtx.Validators.Where(v => v.RealTimeValidation && (v.PropritesName.IsNull() || v.PropritesName.Contains(propertyName)));
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
                    var validatorcolumnquery = entity.EntityCtx.Properties.Where(p => p.Name == propertyName).
                        SelectMany(p => p.Validators.Where(v => v.RealTimeValidation), (p, v) => v);
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
        #endregion Inetercept
    }
}
