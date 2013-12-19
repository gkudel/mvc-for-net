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
        #region Member
        private bool _isModified;
        #endregion Member

        #region Constructor
        internal Context()
        {
            Tables = new List<Table>();
            Relations = new List<Relation>();
            IsModified = false;
        }
        #endregion Constructor

        #region Properties
        public string Name { get; internal set; }
        public List<Table> Tables { get; internal set; }
        internal List<Relation> Relations { get; set; }
        internal Action ContextModifed { get; set; }

        public bool IsModified
        {
            get
            {
                return _isModified;
            }
            internal set
            {
                _isModified = value;
                if (_isModified && ContextModifed.IsNotNull())
                {
                    ContextModifed();
                }
            }
        }
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
                    EntityType = t.EntityType, 
                    EntityFieldName = t.EntityFieldName,
                    EntityFieldGetter = t.EntityFieldGetter,
                    ContextSetter = t.ContextSetter,
                    PrimaryKey = t.PrimaryKey,
                    PrimaryKeyColumn = t.PrimaryKeyColumn
                };
                table.Validators.AddRange(t.Validators);
                t.Columns.ForEach((c) => 
                {
                    Column column = new Column() 
                    {
                        Name = c.Name,
                        Property = c.Property,
                        ColumnType = c.ColumnType,
                        PrimaryKey = c.PrimaryKey,
                        Getter = c.Getter, 
                        Setter = c.Setter,
                        DefaultValue = c.DefaultValue
                    };
                    column.Validators.AddRange(c.Validators);
                    table.Columns.Add(c);
                });
                ctx.Tables.Add(table);
            });
            Relations.ForEach((r) => 
            {
                Relation relation = new Relation() 
                {
                    Name = r.Name,
                    ParentTableName = r.ParentTableName,                    
                    ChildTableName = r.ChildTableName,
                    ParentKey = r.ParentKey,
                    ParentType = r.ParentType, 
                    ParentValue = r.ParentValue,
                    ChildKey = r.ChildKey, 
                    ChildType = r.ChildType, 
                    ChildValue = r.ChildValue,
                    OnDelete = r.OnDelete
                };
                ctx.Relations.Add(relation);
                relation.ParentTable = ctx.Tables.FirstOrDefault(t => t.TableName == r.ParentTableName);
                relation.ChildTable = ctx.Tables.FirstOrDefault(t => t.TableName == r.ChildTableName);
            });
            return ctx;
        }
        #endregion Copy

        #region Initialize 
        internal Context InitailizeRows(ModelContext mctx)
        {
            Tables.ForEach((t) =>
            {
                t.Entities = t.EntityFieldGetter(mctx).CastToType<IEnumerable<Entity>>();
                t.ContextSetter(t.EntityFieldGetter(mctx), this);
            });
            return this;
        }
        #endregion Initialize

        #region Dispose
        public void Dispose()
        {
            Tables.ForEach((t) =>
            {
                t.Entities = null;
            });
            ContextModifed = null;
        }

        ~Context()
        {
            Dispose();
        }
        #endregion Dispose
    }
}
