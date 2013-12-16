using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MVCEngine.Exceptions
{
    [Serializable]
    public class ObjectActivatorException : Exception
    {
        #region Constructors
        public ObjectActivatorException()
        { }

        public ObjectActivatorException(string message)
            : base(message)
        { }

        public ObjectActivatorException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ObjectActivatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
