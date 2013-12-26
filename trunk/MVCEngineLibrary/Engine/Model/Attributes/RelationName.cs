using MVCEngine.Internal.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class RelationName : System.Attribute
    {
        #region Members
        private string relationName;
        #endregion Members

        #region Constructor
        public RelationName(string relationName)
        {
            ArgumentValidator.GetInstnace().
                       IsNotNull(relationName, "relationName");

            this.relationName = relationName;
        }
        #endregion Constructor

        #region Properties
        public string Name
        {
            get { return relationName; }
        }
        #endregion Properties
    }
}
