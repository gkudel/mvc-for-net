using MVCEngine.Internal.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Attributes
{
    public sealed class CollectionInterceptor : Interceptor
    {
        #region Members
        private string id = string.Empty;
        private string foreignKey = string.Empty;
        private string tableName = string.Empty;
        #endregion Members

        #region Constructor
        public CollectionInterceptor(string propertyName, string tableName, string id, string foreignKey)
            : base("CollectionInterceptor", new string[] { "get_" + propertyName })
        {
            Validator.GetInstnace().
                IsNotEmpty(tableName, "tableName").
                IsNotEmpty(id, "id").                
                IsNotEmpty(foreignKey, "foreignKey");

            this.tableName = tableName;
            this.id = id;            
            this.foreignKey = foreignKey;
        }
        #endregion Constructor

        #region Properties
        public string TableName { get { return tableName; } }
        public string Id { get { return id; } }
        public string ForeignKey { get { return foreignKey; } }
        #endregion Properties
    }
}
