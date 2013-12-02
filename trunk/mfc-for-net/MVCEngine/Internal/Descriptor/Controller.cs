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
            PropertiesDefaultValues = new List<KeyValuePair<Action<object, object>, object>>();
        }
        #endregion Constructor

        #region Properties
        internal Func<object> ControllerActivator { get; set; }
        internal List<KeyValuePair<Action<object, object>, object>> PropertiesDefaultValues { get; set; }
        internal string Name { get; set; }
        internal List<ActionMethod> ActionMethods;
        #endregion Properties
    }
}
