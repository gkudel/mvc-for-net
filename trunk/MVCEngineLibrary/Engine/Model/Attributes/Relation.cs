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
        #region Constructor
        public Relation(string relationName, string parentEntity, string parentProperty, string childEntity, string childProperty)
        {
            ArgumentValidator.GetInstnace().
                       IsNotNull(relationName, "relationName");

            this.RelationName = relationName;
            this.ParentEntity = parentEntity;
            this.ParentProperty = parentProperty;
            this.ChildEntity = childEntity;
            this.ChildProperty = childProperty;
            this.OnDelete = OnAction.Nothing;
            this.OnAccept = OnAction.Nothing;
            this.OnFreeze = OnAction.Nothing;
        }
        #endregion Constructor

        #region Properties
        public string RelationName { get; private set; }
        internal string ParentEntity { get; private set; }
        internal string ParentProperty { get; private set; }
        internal string ChildEntity { get; private set; }
        internal string ChildProperty { get; private set; }
        public virtual OnAction OnDelete { get; set; }
        public virtual OnAction OnAccept { get; set; }
        public virtual OnAction OnFreeze { get; set; }
        #endregion Properties
    }
}
