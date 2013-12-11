using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;

namespace MVCEngine.Model.Internal.Descriptions
{
    internal class EntityClass
    {
        #region Constructor
        internal EntityClass()
        {
            Interceptors = new List<Interceptor>();
            InterceptorObjects = new List<IInterceptor>();
        }
        #endregion Constructor

        #region Prtoperties
        internal string FullName { get; set; }
        internal List<Interceptor> Interceptors { get; set; }
        internal List<IInterceptor> InterceptorObjects { get; set; }
        #endregion Prtoperties
    }
}
