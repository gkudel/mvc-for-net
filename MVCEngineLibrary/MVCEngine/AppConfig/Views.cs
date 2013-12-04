using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MVCEngine.AppConfig
{
    public class Views : ConfigurationElementCollection
    {
        public Views()
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
            return new View();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((View)element).Name;
        }

        public View this[int index]
        {
            get
            {
                return (View)BaseGet(index);
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

        new public View this[string Name]
        {
            get
            {
                return (View)BaseGet(Name);
            }
        }

        public int IndexOf(View view)
        {
            return BaseIndexOf(view);
        }

        public void Add(View view)
        {
            BaseAdd(view);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(View view)
        {
            if (BaseIndexOf(view) >= 0)
                BaseRemove(view.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}
