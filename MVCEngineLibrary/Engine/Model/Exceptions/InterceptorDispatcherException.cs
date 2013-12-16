using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MVCEngine.Model.Exceptions
{
    [Serializable]
    public class InterceptorDispatcherException : Exception
    {
        #region Constructors
        public InterceptorDispatcherException()
        { }

        public InterceptorDispatcherException(string message)
            : base(message)
        { }

        public InterceptorDispatcherException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected InterceptorDispatcherException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
