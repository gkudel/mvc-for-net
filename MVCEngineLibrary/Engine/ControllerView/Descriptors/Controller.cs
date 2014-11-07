using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.ControllerView.Descriptors
{
    class Controller
    {
        #region Constructor
        internal Controller()
        {
            ActionMethods = new List<ActionMethod>();
        }
        #endregion Constructor

        #region Properties
        internal object Object { get; set; }
        internal string Name { get; set; }
        internal object Key { get; set; }
        internal Type Type { get; set; }
        internal List<ActionMethod> ActionMethods { get; set; }
        #endregion Properties
    }
}
