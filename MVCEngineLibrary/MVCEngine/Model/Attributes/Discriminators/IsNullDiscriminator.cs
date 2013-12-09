using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        #endregion Discriminate
    }
}
