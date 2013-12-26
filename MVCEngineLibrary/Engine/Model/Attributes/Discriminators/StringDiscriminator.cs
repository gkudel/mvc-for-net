using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;

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
                if (e[propertyName].IsNotNull())
                {
                    return e[propertyName].Equals(Value);
                }
                else
                {
                    ret = false;
                }
            }
            return ret;
        }

        public override void Default(Entity e)
        {
            if (!Value.IsNullOrEmpty())
            {
                e[propertyName] = Value;
            }
        }
        #endregion Discriminate
    }
}
