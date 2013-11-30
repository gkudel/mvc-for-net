using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Internal;

namespace MVCEngine.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public sealed class ViewId : System.Attribute 
    {
        #region Members
        private string parameterName;
        #endregion Members

        #region Constructor
        public ViewId(string parameterName)
        {
            this.parameterName = parameterName;
        }
        #endregion Constructor

        #region Properties
        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }
        #endregion Properties
    }
}
