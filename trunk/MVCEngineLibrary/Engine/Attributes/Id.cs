using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Id : Attribute
    {
        #region Constructor
        public Id(params string[] controllersName)
        {
            ControllersName = controllersName;
        }
        #endregion Constructor

        #region Properties
        public string[] ControllersName { get; set; }
        #endregion Properties
    }
}
