using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal.Validation;

namespace MVCEngine.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Column : System.Attribute
    {
        #region Members
        private string columnName;
        #endregion Members

        #region Constructor
        public Column(string columnName)
        {
            Validator.GetInstnace().
                       IsNotNull(columnName, "columnName");

            this.columnName = columnName;
        }
        #endregion Constructor

        #region Properties
        public string ColumnName
        {
            get { return columnName; }
        }

        public virtual bool IsPrimaryKey { get; set; }
        public virtual bool IsForeignKey { get; set; }
        public virtual string ForeignTable { get; set; }
        #endregion Properties
    }
}
