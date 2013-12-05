using MVCEngine.Internal.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes
{
    public class CollectionInterceptor : Interceptor
    {
        #region Members
        private string tableName = string.Empty;
        #endregion Members

        #region Constructor
        public CollectionInterceptor(string propertyName, string tableName)
            : base("CollectionInterceptor", new string[] { "get_" + propertyName })
        {
            Validator.GetInstnace().
                IsNotEmpty(tableName, "tableName");

            this.tableName = tableName;
        }
        #endregion Constructor

        #region Properties
        public string TableName { get { return tableName; } }
        #endregion Properties
    }
}
