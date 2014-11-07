using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MVCEngine.ControllerView.AppConfig
{    
    public class View : ConfigurationElement
    {        
        [ConfigurationProperty("class", IsRequired = true, IsKey=true)]
        public string Class
        {
            get
            {
                return this["class"] as string;
            }
            set
            {
                this["class"] = value;
            }
        }
    }
}
