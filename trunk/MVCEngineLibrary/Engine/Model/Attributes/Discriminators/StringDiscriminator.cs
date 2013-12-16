using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes.Discriminators
{
    public class StringDiscriminator : Discriminator
    {
        #region Constructor
        public StringDiscriminator(string propertyName)
            : base(propertyName)
        {
        }
        #endregion Constructor

        #region Properties
        public virtual string Value { get; set; }
        #endregion Properties

        #region Discriminate
        public override bool Discriminate(Entity e)
        {
            bool ret = true;
            if (!Value.IsNullOrEmpty())
            {
                return e[propertyName].Equals(Value);
            }
            return ret;
        }
        #endregion Discriminate
    }
}
