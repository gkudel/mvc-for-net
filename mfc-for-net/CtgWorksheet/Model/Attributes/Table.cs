using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;
using MVCEngine.Model.Internal.Descriptions;

namespace CtgWorksheet.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class Table : System.Attribute, Interceptor
    {
        #region Members
        private string _tableName;
        #endregion Members

        #region Constructors
        public Table(string tableName)
        {
            _tableName = tableName;
        }
        #endregion Constructors

        #region Properties
        public string TableName
        {
            get
            {
                return _tableName;
            }
        }
        #endregion Properties

        #region Interceptor
        public const string Id = "TableInterceptor";

        public string GetId()
        {
            return Table.Id;
        }

        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            EntityRow entity = invocation.InvocationTarget.CastToType<EntityRow>();
            if (entity.IsNotNull() && entity.Row.IsNotNull())
            {
                string propertyName = invocation.Method.Name;
                if (propertyName.StartsWith("get_") || propertyName.StartsWith("set_"))
                {
                    propertyName = propertyName.Substring(4, propertyName.Length - 4);
                }
                if (invocation.Method.Name.StartsWith("get_"))
                {
                    invocation.ReturnValue = entity[propertyName];
                }
                else if (invocation.Method.Name.StartsWith("set_") && invocation.Arguments.Count() > 0)
                {
                    entity[propertyName] = invocation.Arguments[0];
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        #endregion Interceptor
    }
}
