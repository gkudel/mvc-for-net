using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Model.Internal.Descriptions;
using MVCEngine.Internal.Validation;

namespace MVCEngine.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Relation : System.Attribute
    {
        #region Members
        private string relationName;
        #endregion Members

        #region Constructor
        public Relation(string relationName)
        {
            ArgumentValidator.GetInstnace().
                       IsNotNull(relationName, "relationName");

            this.relationName = relationName;
            this.OnDelete = OnDelete.Nothing;
        }
        #endregion Constructor

        #region Properties
        public string RelationName
        {
            get { return relationName; }
        }

        public virtual string ForeignEntity { get; set; }
        public virtual string ForeignProperty { get; set; }
        public virtual OnDelete OnDelete { get; set; }
        #endregion Properties
    }
}
