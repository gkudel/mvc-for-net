using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MVCEngine.Exceptions
{
    [Serializable]
    public class ViewRegisterException : Exception
    {
        #region Constructors
        public ViewRegisterException()
        { }

        public ViewRegisterException(string message)
            : base(message)
        { }

        public ViewRegisterException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ViewRegisterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
