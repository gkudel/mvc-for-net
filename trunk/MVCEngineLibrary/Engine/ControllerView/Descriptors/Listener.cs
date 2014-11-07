using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using MVCEngine.Tools;

namespace MVCEngine.ControllerView.Descriptors
{
    class Listener
    {
        #region Member
        private object _thisObject;
        #endregion Member

        #region Properties
        internal object ThisObject 
        {
            get { return _thisObject; }
            set { _thisObject = value; FullTypeName = _thisObject.IsNotNull() ? _thisObject.GetType().FullName : string.Empty; }
        }
        internal string FullTypeName { get; set; }
        internal Method ActionCallBack { get; set; }
        internal Method ActionErrorBack { get; set; }
        #endregion Properties
    }
}
