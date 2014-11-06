using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.ControllerView.AppConfig;
using MVCEngine.Tools;

namespace MVCEngine.ControllerView.ControllerEngine
{
    static class ControllerConfiguration
    {
        internal static IEnumerator<Type> Process(System.Configuration.Configuration config)
        {
            ControllerSection controllersection = config.GetSection("RegisterControllers") as ControllerSection;
            if (controllersection.IsNotNull())
            {
                foreach (Controller controller in controllersection.Controllers)
                {
                    yield return Type.GetType(controller.Class);
                }
            }
        }
    }
}
