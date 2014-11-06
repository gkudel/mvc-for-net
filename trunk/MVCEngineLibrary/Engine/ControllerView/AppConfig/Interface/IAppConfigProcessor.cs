using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MVCEngine.ControllerView.AppConfig.Interface
{
    interface IAppConfigProcessor
    {
        void Process(Configuration config);
    }
}
