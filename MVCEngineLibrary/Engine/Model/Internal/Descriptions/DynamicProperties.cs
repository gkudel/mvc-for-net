﻿using MVCEngine.Model.Attributes.Discriminators;
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
            Discriminators = new List<Discriminator>();
        }
        #endregion Constructor

        #region Properties
        internal Func<object, object> Entities { get; set; }
        internal List<Discriminator> Discriminators { get; set; }
        internal KeyValuePair<Type, string>[] PropertiesValue { get; set; }
        internal Type EntityType { get; set; }
        #endregion Properties
    }
}
