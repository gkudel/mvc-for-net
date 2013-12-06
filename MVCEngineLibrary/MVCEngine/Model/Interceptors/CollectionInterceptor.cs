using Castle.Core.Interceptor;
using MVCEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class CollectionInterceptor<T> : IInterceptor where T : ModelObject
    {
        #region Members
        private Lazy<List<T>> _list;
        #endregion Members

        #region Constructor
        public CollectionInterceptor()
        {
            _list = new Lazy<List<T>>(() =>
            {
                return new List<T>();
            });
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = _list.Value;
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
