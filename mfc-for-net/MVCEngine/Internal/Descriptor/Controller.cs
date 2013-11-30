using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Internal.Descriptor
{
    internal class Controller
    {
        #region Constructor
        internal Controller()
        {
            ActionMethods = new List<ActionMethod>();
            PropertiesDefaultValues = new List<KeyValuePair<PropertyInfo, object>>();
        }
        #endregion Constructor

        #region Properties
        internal Type ControllerType { get; set; }
        internal List<KeyValuePair<PropertyInfo, object>> PropertiesDefaultValues { get; set; }
        internal string Name { get; set; }
        internal List<ActionMethod> ActionMethods;
        #endregion Properties
    }
}
