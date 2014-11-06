using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MVCEngine.Tools.Exceptions
{
    [Serializable]
    public class ControllerRegistrationException : Exception
    {
        #region Constructors
        public ControllerRegistrationException()
        { }

        public ControllerRegistrationException(string message)
            : base(message)
        { }

        public ControllerRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ControllerRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
