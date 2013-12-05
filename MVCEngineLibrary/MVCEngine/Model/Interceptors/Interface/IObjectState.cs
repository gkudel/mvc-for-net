using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors.Interface
{
    public enum ObjectState { Added, Modified, Deleted, Unchanged };
    public interface IObjectState
    {        
        ObjectState State { get; set; }
    }
}
