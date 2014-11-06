using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Tools.Exceptions
{
    [Serializable]
    public class ActionMethodInvocationException : Exception
    {
        #region Constructors
        public ActionMethodInvocationException()
        { }

        public ActionMethodInvocationException(string message)
            : base(message)
        { }

        public ActionMethodInvocationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ActionMethodInvocationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
