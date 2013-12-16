using MVCEngine.Internal.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes.Discriminators
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public abstract class Discriminator : System.Attribute
    {
        #region Members
        protected string propertyName;
        #endregion Members

        #region Constructor
        public Discriminator(string propertyName)
        {
            ArgumentValidator.GetInstnace().
                IsNotNull(propertyName, "propertyName");

            this.propertyName = propertyName;
        }
        #endregion Constructor

        #region Discriminate
        public abstract bool Discriminate(Entity e);
        #endregion Discriminate
    }
}
