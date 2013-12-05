using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal;
using Castle.DynamicProxy;
using MVCEngine.Model.Interceptors.Interface;

namespace MVCEngine.Model
{
    public class ModelContext
    {
        #region Constructor
        public ModelContext()
        {
        }
        #endregion Constructor

        #region Freeze & UnFreeze
        public void Freeze<T>(T obj) where T : ISecurity
        {
            obj.CastToType<ISecurity>().IsFrozen = true;
        }

        public void UnFreeze<T>(T obj) where T : ISecurity
        {
            obj.CastToType<ISecurity>().IsFrozen = false;
        }
        #endregion Freeze & UnFreeze
    }
}
