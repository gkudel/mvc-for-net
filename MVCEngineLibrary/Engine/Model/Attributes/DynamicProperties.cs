using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class DynamicProperties : System.Attribute
    {
        #region Members
        private string _propertyCode;
        private string[] _propertiesValue;
        #endregion Members

        #region Constructors
        public DynamicProperties(string propertyCode, params string[] propertiesValue)
        {
            this._propertyCode = propertyCode;
            this._propertiesValue = propertiesValue;
        }
        #endregion Constructors

        #region Properties
        public string PropertyCode { get { return _propertyCode; } }
        public string[] PropertiesValue { get { return _propertiesValue; } }
        #endregion Properties
    }
}
