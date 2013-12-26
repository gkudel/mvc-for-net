using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes.Validation
{
    [System.AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple=true)]
    public abstract class EntityValidator : Validator
    {
        #region Members
        private string[] propertiesName;
        #endregion Members

        #region Constructor
        public EntityValidator(params string[] propertiesName)
        {
            this.propertiesName = propertiesName;
        }
        #endregion Constructor

        #region Validate
        public abstract bool Validate(Entity entity, string column, object value);
        public abstract bool Validate(Entity entity);
        #endregion Validate

        #region Properties
        public string[] PropritesName { get { return propertiesName; } }
        #endregion Properties

    }
}
