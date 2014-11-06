using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.ControllerView.ControllerEngine.Interface
{
    public interface IControllerMenagment
    {
        void Init(string key, object controller);
    }
}
