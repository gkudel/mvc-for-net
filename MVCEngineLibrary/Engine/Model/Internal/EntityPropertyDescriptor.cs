using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MVCEngine;

namespace MVCEngine.Model.Internal
{
    internal class EntityPropertyDescriptor : PropertyDescriptor 
    {
        #region Members
        private Type _propertyType;
        private Type _componentType;
        #endregion Members

        #region Constructor
        internal EntityPropertyDescriptor(string name, Type componentType, Type propertyType)
            : base(name, null)
        {
            this._propertyType = propertyType;
            this._componentType = componentType;
        }
        #endregion Constructor

        #region PropertyDescriptor
        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get 
            {
                return _componentType;
            }
        }

        public override object GetValue(object component)
        {
            Entity entity = component as Entity;
            if (entity.IsNotNull())
            {
                return entity.GetValue(Name, _propertyType);
            }
            return null;
        }

        public override bool IsReadOnly
        {
            get 
            {
                return false;
            }
        }

        public override Type PropertyType
        {
            get 
            {
                return _propertyType;
            }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            Entity entity = component as Entity;
            if (entity.IsNotNull())
            {
                entity.SetValue(Name, value, _propertyType);
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        #endregion PropertyDescriptor
    }
}
