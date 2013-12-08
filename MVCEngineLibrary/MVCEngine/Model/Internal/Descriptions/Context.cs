﻿using MVCEngine.Model.Exceptions;
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
            Relations = new List<Relation>();
        }
        #endregion Constructor

        #region Properties
        public string Name { get; set; }
        public List<Table> Tables { get; set; }
        internal List<Relation> Relations { get; set; }
        #endregion Properties

        #region Copy
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
                    ClassName = t.ClassName, 
                    RowsFieldName = t.RowsFieldName,
                    RowsFieldGetter = t.RowsFieldGetter,
                    ContextSetter = t.ContextSetter
                };
                t.Columns.ForEach((c) => 
                {
                    Column column = new Column() 
                    {
                        Name = c.Name,
                        Property = c.Property,
                        ColumnType = c.ColumnType,
                        PrimaryKey = c.PrimaryKey
                    };
                    table.Columns.Add(c);
                });
                ctx.Tables.Add(table);
            });
            Relations.ForEach((r) => 
            {
                ctx.Relations.Add(new Relation() 
                {
                    Name = r.Name, 
                    ParentTable = r.ParentTable,
                    ChildTable = r.ChildTable,
                    ParentKey = r.ParentKey,
                    ParentValue = r.ParentValue,
                    ChildKey = r.ChildKey, 
                    ChildValue = r.ChildValue
                });
            });
            return ctx;
        }
        #endregion Copy

        #region Initialize 
        internal Context InitailizeRows(ModelContext mctx)
        {
            Tables.ForEach((t) =>
            {
                t.Rows = t.RowsFieldGetter(mctx).CastToType<IEnumerable<Entity>>();
                t.ContextSetter(t.RowsFieldGetter(mctx), this);
            });
            return this;
        }
        #endregion Initialize

        #region Dispose
        public void Dispose()
        {
            Tables.ForEach((t) =>
            {
                t.Rows = null;
            });
        }

        ~Context()
        {
            Dispose();
        }
        #endregion Dispose
    }
}
