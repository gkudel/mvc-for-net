using MVCEngine.Model.Attributes.Discriminators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    internal class DynamicProperties
    {
        #region Constructor
        internal DynamicProperties()
        {
            ValuesProperties = new Dictionary<Type, string>();
        }
        #endregion Constructor

        #region Properties
        internal EntityProperty Property { get; set; }
        internal string CodeProperty { get; set; }
        internal Dictionary<Type, string> ValuesProperties { get; set; }
        #endregion Properties
    }
}
