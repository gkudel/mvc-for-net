using MVCEngine.Model.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class Context : IDisposable 
    {
        #region Constructor
        internal Context()
        {
            Tables = new List<Table>();
        }
        #endregion Constructor

        #region Properties
        public string Name { get; set; }
        public List<Table> Tables { get; set; }
        #endregion Properties

        /*#region Copy
        internal Context Copy()
        {
            Context ctx = new Context() 
            {
                Name = this.Name
            };
            Tables.ForEach((t) =>
            {
                Table table = new Table()
                {
                    TableName = t.TableName,
                    FieldName = t.FieldName,
                    EntityType = t.EntityType
                };
                t.Columns.ForEach((c) => 
                {
                    Column column = new Column() 
                    {
                        Name = c.Name,
                        ColumnType = c.ColumnType,
                        ForeignKey = c.ForeignKey,
                        ForeignTable = c.ForeignTable,                        
                        PrimaryKey = c.PrimaryKey
                    };
                    table.Columns.Add(c);
                });
                t.Parents.ForEach((p) =>
                {
                    Relation parent = new Relation() 
                    {
                        PropertyName = p.PropertyName,
                        TableName = p.TableName
                    };
                    table.Parents.Add(parent);
                });
                t.Children.ForEach((c) =>
                {
                    Relation child = new Relation()
                    {
                        PropertyName = c.PropertyName,
                        TableName = c.TableName
                    };
                    table.Children.Add(child);
                });
                ctx.Tables.Add(table);
            });
            return ctx;
        }
        #endregion Copy*/

        #region Dispose
        public void Dispose()
        {
        }
        #endregion Dispose
    }
}
