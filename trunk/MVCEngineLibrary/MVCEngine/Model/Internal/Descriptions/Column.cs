using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class Column
    {
        #region Properties
        public string Name { get; internal set; }
        internal string Property { get; set; }
        public Type ColumnType { get; internal set; }
        public bool PrimaryKey { get; internal set; }
        #endregion Properties
    }
}
