using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MVCEngine.Model.Exceptions
{
    [Serializable]
    public class ModelListException : Exception
    {
        #region Constructors
        public ModelListException()
        { }

        public ModelListException(string message)
            : base(message)
        { }

        public ModelListException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ModelListException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion Constructors
    }
}
