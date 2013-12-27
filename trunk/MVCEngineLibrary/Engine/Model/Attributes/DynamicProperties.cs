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
        private string _codeProperty;
        private string[] _valueProperties;
        #endregion Members

        #region Constructors
        public DynamicProperties(string codeProperty, params string[] valueProperties)
        {
            this._codeProperty = codeProperty;
            this._valueProperties = valueProperties;
        }
        #endregion Constructors

        #region Properties
        public string CodeProperty { get { return _codeProperty; } }
        public string[] ValueProperties { get { return _valueProperties; } }
        #endregion Properties
    }
}
