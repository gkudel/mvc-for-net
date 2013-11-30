using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ValueFromControllerAttribute : System.Attribute
    {
        #region Members
        private string propertyName;
        #endregion Members

        #region Constructor
        public ValueFromControllerAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }
        #endregion Constructor

        #region Properties
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }
        #endregion Properties

    }
}
