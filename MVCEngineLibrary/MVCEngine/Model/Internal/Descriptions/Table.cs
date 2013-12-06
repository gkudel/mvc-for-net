using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class Table
    {
        #region Constructor
        internal Table()
        {
            Children = new List<Relation>();
            Parents = new List<Relation>();
            Columns = new List<Column>();
        }
        #endregion Constructor

        #region Properties
        public string TableName { get; internal set; }
        internal string FieldName { get; set; }
        internal Type EntityType { get; set; }
        internal List<Relation> Children { get; set; }
        internal List<Relation> Parents { get; set; }
        public List<Column> Columns { get; internal set; }
        #endregion Properties
    }
}
