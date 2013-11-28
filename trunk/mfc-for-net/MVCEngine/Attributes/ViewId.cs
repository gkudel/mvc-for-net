using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Internal;

namespace MVCEngine.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ViewId : System.Attribute 
    {
        #region Members
        private string parameterName;
        #endregion Members

        #region Constructor
        public ViewId(string parameterName)
        {
            this.parameterName = parameterName.IfNulOrEmptyDefault("Id");
        }
        #endregion Constructor

        #region Properties
        public string ParameterName
        {
            get { return parameterName; }
        }
        #endregion Properties
    }
}
