using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors.Interface
{
    public interface ISecurity
    {
        bool IsFrozen { get; set; }
    }
}
