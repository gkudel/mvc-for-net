using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MVCEngine.AppConfig
{
    public class ControllerSection : ConfigurationSection
    {
        [ConfigurationProperty("Controllers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(Controllers),
            AddItemName = "Controller")]
        public Controllers Controllers 
        {
            get
            {
                Controllers controllers =
                    (Controllers)base["Controllers"];
                return controllers;
            }
        }
    }
}
