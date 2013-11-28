using System;
using System.Collections.Generic;
using System.Linq;
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
        }
        #endregion Constructor

        #region Properties
        internal Type ControllerType { get; set; }
        internal Func<object[]> ConstructorParams { get; set; }
        internal string Name { get; set; }
        internal List<ActionMethod> ActionMethods;
        #endregion Properties
    }
}
