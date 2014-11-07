using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.ControllerView.AppConfig;
using MVCEngine.Tools;

namespace MVCEngine.ControllerView.ViewEngine
{
    static class ViewConfiguration
    {
        internal static IEnumerator<Type> Process(System.Configuration.Configuration config)
        {
            ViewSection viewsection = config.GetSection("RegisterViews") as ViewSection;
            if (viewsection.IsNotNull())
            {
                foreach (View view in viewsection.Views)
                {
                    yield return Type.GetType(view.Class);                    
                }
            }
        }
    }
}
