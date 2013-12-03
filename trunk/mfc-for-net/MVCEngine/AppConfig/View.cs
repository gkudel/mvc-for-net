using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MVCEngine.AppConfig
{    
    public class View : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
            set
            {
                this["name"] = value;
            }

        }
        
        [ConfigurationProperty("class", IsRequired = true)]
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

        [ConfigurationProperty("assembly", IsRequired = false)]
        public string Assembly
        {
            get
            {
                return this["assembly"] as string;
            }
            set
            {
                this["assembly"] = value;
            }
        }

    }
}
