using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal.Validation;

namespace MVCEngine.Model.Attributes
{
    public enum OnDelete { Cascade, SetNull, Nothing };
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Column : System.Attribute
    {
        #region Members
        private string columnName;
        #endregion Members

        #region Constructor
        public Column(string columnName)
        {
            ArgumentValidator.GetInstnace().
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
        public virtual string RelationName { get; set; }
        public virtual bool IsForeignKey { get; set; }
        public virtual string ForeignTable { get; set; }
        public virtual string ForeignColumn { get; set; }
        public virtual OnDelete OnDelete { get; set; }
        #endregion Properties
    }
}
