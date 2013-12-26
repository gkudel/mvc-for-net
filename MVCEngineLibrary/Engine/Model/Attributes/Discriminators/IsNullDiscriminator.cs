using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;

namespace MVCEngine.Model.Attributes.Discriminators
{
    public class IsNullDiscriminator : Discriminator
    {
        #region Constructor
        public IsNullDiscriminator(string propertyName)
            : base(propertyName)
        {
        }
        #endregion Constructor

        #region Discriminate
        public override bool Discriminate(Entity e)
        {
            return e[propertyName].IsNull();
        }

        public override void Default(Entity e)
        {
            e[propertyName] = null;
        }
        #endregion Discriminate
    }
}
