using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MVCEngine.Session.Exceptions
{
    [Serializable]
    public class InvalidSessionIdException : Exception
    {
        #region Constructors
        public InvalidSessionIdException()
        { }

        public InvalidSessionIdException(string message)
            : base(message)
        { }

        public InvalidSessionIdException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected InvalidSessionIdException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
