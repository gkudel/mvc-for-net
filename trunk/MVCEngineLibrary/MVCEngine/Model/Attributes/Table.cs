using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal.Validation;

namespace MVCEngine.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class Table : System.Attribute
    {
        #region Members
        private string tableName;
        #endregion Members

        #region Constructor
        public Table(string tableName)
        {
            Validator.GetInstnace().
                       IsNotNull(tableName, "tableName");

            this.tableName = tableName;
        }
        #endregion Constructor

        #region Properties
        public string TableName
        {
            get { return tableName; }
        }
        #endregion Properties
    }
}
