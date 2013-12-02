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
            PropertiesSetters = new Dictionary<string, Action<object, object>>();
            DefaultValues = new Dictionary<string, object>();
        }
        #endregion Constructor

        #region Properties
        internal Func<object> ControllerActivator { get; set; }
        internal Dictionary<string, Action<object, object>> PropertiesSetters { get; set; }
        internal Dictionary<string, object> DefaultValues { get; set; }
        internal string Name { get; set; }
        internal List<ActionMethod> ActionMethods;
        #endregion Properties
    }
}
