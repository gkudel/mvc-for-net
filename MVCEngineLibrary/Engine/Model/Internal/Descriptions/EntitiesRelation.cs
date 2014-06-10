using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public enum OnAction { Nothing, Cascade, SetNull }
    public class EntitiesRelation
    {
        #region Members
        private static int _ordninal = 0;
        #endregion Members

        #region Constructor
        internal EntitiesRelation()
        {
            Ordinal = _ordninal++;
        }
        #endregion Constructor

        #region Properties
        internal int Ordinal { get; set; }
        public string Name { get; internal set; }
        public EntityRelated Parent { get; internal set; }
        public EntityRelated Child { get; internal set; }
        internal OnAction OnDelete { get; set; }
        internal OnAction OnAccept { get; set; }
        internal OnAction OnFreeze { get; set; }
        #endregion Properties
    }
}
