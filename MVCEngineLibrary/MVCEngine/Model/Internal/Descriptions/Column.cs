using MVCEngine.Model.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class Column
    {
        #region Constructor
        internal Column()
        {
            Validators = new List<ColumnValidator>();
        }
        #endregion Constructor

        #region Properties
        internal string Property { get; set; }
        internal List<ColumnValidator> Validators { get; set; }
        public string Name { get; internal set; }
        public Type ColumnType { get; internal set; }
        public bool PrimaryKey { get; internal set; }
        internal Action<object, object> Setter { get; set; }
        internal Func<object, object> Getter { get; set; }
        #endregion Properties
    }
}
