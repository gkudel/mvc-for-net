using Castle.Core.Interceptor;
using MVCEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class CollectionInterceptor : IInterceptor
    {
        #region Constructor
        public CollectionInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();     
        }
        #endregion Inetercept

        #region Properties
        [ValueFromAttribute("")]
        public string TableName { get; set; }
        [ValueFromAttribute("")]
        public string Id { get; set; }
        [ValueFromAttribute("")]
        public string ForeignKey { get; set; }
        #endregion Properties
    }
}
