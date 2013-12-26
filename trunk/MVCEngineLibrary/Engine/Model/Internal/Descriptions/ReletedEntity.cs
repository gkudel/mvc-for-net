using MVCEngine.Model.Attributes.Discriminators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Model.Internal.Descriptions
{
    internal enum Releted { None, Entity, List };
    internal class ReletedEntity
    {
        #region Constructor
        internal ReletedEntity()
        {
            Discriminators = new List<Discriminator>();
        }
        #endregion Constructor

        #region Properties
        internal Releted Related { get; set; }
        internal bool Synchronized { get; set; }
        internal string RelatedEntityName { get; set; }
        internal EntityClass RelatedEntity { get; set; }
        internal string RelationName { get; set; }
        internal EntitiesRelation Relation { get; set; }
        internal List<Discriminator> Discriminators { get; set; }
        #endregion Properties
    }
}
