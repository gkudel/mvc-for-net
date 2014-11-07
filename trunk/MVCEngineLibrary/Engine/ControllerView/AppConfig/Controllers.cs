using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MVCEngine.ControllerView.AppConfig
{
    public class Controllers : ConfigurationElementCollection
    {
        public Controllers()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Controller();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((Controller)element).Class;
        }

        public Controller this[int index]
        {
            get
            {
                return (Controller)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public Controller this[string Class]
        {
            get
            {
                return (Controller)BaseGet(Class);
            }
        }

        public int IndexOf(Controller controller)
        {
            return BaseIndexOf(controller);
        }

        public void Add(Controller controller)
        {
            BaseAdd(controller);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(Controller controller)
        {
            if (BaseIndexOf(controller) >= 0)
                BaseRemove(controller.Class);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string Class)
        {
            BaseRemove(Class);
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}
