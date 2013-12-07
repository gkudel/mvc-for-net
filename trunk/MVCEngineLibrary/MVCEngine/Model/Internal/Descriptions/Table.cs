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
            Columns = new List<Column>();
            Uid = Guid.NewGuid().ToString();
        }
        #endregion Constructor

        #region Properties
        internal string ClassName { get; set; }
        internal string RowsFieldName { get; set; }
        internal Func<object, object> RowsFieldGetter{ get; set; }
        internal Action<object, object> ContextSetter { get; set; }
        internal IEnumerable<Entity> Rows { get; set; }
        internal string Uid { get; set; }
        internal Func<object, object> PrimaryKey { get; set; }
        internal Type EntityType { get; set; }
        public List<Column> Columns { get; internal set; }
        public string TableName { get; internal set; }
        #endregion Properties
    }
}
