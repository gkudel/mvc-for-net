using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.ControllerView;
using System.Configuration;
using MVCEngine.ControllerView.AppConfig.Interface;

namespace MVCEngine.ControllerView.AppConfig
{
    static class AppConfiguration
    {
        #region Process
        internal static void Process(IAppConfigProcessor[] processors)
        {
            Task.Factory.StartNew(() =>
            {
                lock (Dispatcher.ThreadLockObject)
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    foreach (IAppConfigProcessor processor in processors)
                    {
                        processor.Process(config);
                    }
                }
            });
        }
        #endregion Process
    }
}
