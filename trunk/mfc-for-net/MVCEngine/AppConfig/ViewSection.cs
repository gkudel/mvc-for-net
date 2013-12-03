using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MVCEngine.AppConfig
{
    public class ViewSection : ConfigurationSection
    {
        [ConfigurationProperty("Views", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(Views), AddItemName = "View")]
        public Views Views 
        {
            get
            {
                Views views = (Views)base["Views"];
                return views;
            }
        }
    }
}
