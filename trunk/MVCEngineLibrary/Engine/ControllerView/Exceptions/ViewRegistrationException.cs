using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MVCEngine.Tools.Exceptions
{
    [Serializable]
    public class ViewRegistrationException : Exception
    {
        #region Constructors
        public ViewRegistrationException()
        { }

        public ViewRegistrationException(string message)
            : base(message)
        { }

        public ViewRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ViewRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
