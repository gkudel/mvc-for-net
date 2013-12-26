using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MVCEngine.Model.Exceptions
{
    [Serializable]
    public class SecurityException : Exception
    {
        #region Constructors
        public SecurityException()
        { }

        public SecurityException(string message)
            : base(message)
        { }

        public SecurityException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected SecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
