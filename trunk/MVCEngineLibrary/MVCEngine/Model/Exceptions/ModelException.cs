using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MVCEngine.Model.Exceptions
{
    [Serializable]
    public class ModelException : Exception
    {
        #region Constructors
        public ModelException()
        { }

        public ModelException(string message)
            : base(message)
        { }

        public ModelException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ModelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
