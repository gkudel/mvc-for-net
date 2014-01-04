using MVCEngine.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtgWorksheet.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Column : Intercept
    {
        #region Members
        private string _columnName;
        #endregion Members

        #region Constructors
        public Column(string columnName)
            : base(Table.Id, new Method[] { Method.Get, Method.Set } )
        {
            _columnName = columnName;
        }
        #endregion Constructors

        #region Properties
        public string ColumnName
        {
            get
            {
                return _columnName;
            }
        }
        #endregion Properties
    }
}
