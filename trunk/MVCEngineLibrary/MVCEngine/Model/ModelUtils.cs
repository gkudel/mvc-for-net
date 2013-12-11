using MVCEngine.Internal.Validation;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model
{
    public class ModelUtils : IDisposable
    {
        #region Members
        private ModelUtils _instance;
        private ModelContext _context;
        private string _session;
        private bool _commited;
        #endregion Members

        #region Constructor
        private ModelUtils(ModelContext context)
        {
            this._context = context;
            this._session = Guid.NewGuid().ToString();
            this._commited = false;
        }
        #endregion Constructor

        #region Methods
        public static ModelUtils OpenSession(ModelContext context)
        {
            ArgumentValidator.GetInstnace().
                    IsNotNull(context, "context");

            return new ModelUtils(context);
        }

        public ModelUtils FillContext(Func<IEnumerator<string>> tables, Func<string, IEnumerator<object>> rows,
                                Func<object, string, bool> valuePresent, Func<object, string, object> values, Action<string, Entity> added = null)
        {
            ArgumentValidator.GetInstnace().
                    IsNotNull(tables, "tables").
                    IsNotNull(rows, "rows").
                    IsNotNull(values, "values").
                    IsNotNull(values, "values");

            IEnumerator<string> enumeratorTables = tables();
            while (enumeratorTables.MoveNext())
            {
                string tableName = enumeratorTables.Current;
                Table table = _context.Context.Tables.FirstOrDefault(t => t.TableName == tableName);
                if (table.IsNotNull())
                {
                    IList list = table.Entities as IList;
                    if (list.IsNotNull())
                    {
                        IEnumerator<object> enumerationRows = rows(tableName);
                        while (enumerationRows.MoveNext())
                        {
                            object row = enumerationRows.Current;
                            Entity entity = ModelBindingList<Entity>.CreateInstance(table.EntityType, _context.Context, false) as Entity;
                            if (entity.IsNotNull())
                            {                                
                                entity.Session = _session;
                                list.Add(entity);
                                if (added != null) added(tableName, entity);

                                table.Columns.ForEach((c) =>
                                {
                                    if (valuePresent(row, table.PrimaryKeyColumn.Name))
                                    {
                                        entity[c.Name] = values(row, c.Name);
                                    }
                                });
                            }
                        }
                    }
                }
            }
            return this;
        }

        public ModelUtils Merge(Func<IEnumerator<string>> tables, Func<string, IEnumerator<object>> rows, 
                                Func<object, string, bool> valuePresent, Func<object, string, object> values, Action<string, Entity> added = null)
        {
            ArgumentValidator.GetInstnace().
                    IsNotNull(tables, "tables").
                    IsNotNull(rows, "rows").
                    IsNotNull(valuePresent, "valuePresent").
                    IsNotNull(values, "values");

            IEnumerator<string> enumeratorTables = tables();
            while (enumeratorTables.MoveNext())
            {
                string tableName = enumeratorTables.Current;
                Table table = _context.Context.Tables.FirstOrDefault(t => t.TableName == tableName);
                if (table.IsNotNull())
                {
                    IList list = table.Entities as IList;
                    if (list.IsNotNull())
                    {
                        IEnumerator<object> enumerationRows = rows(tableName);
                        while (enumerationRows.MoveNext())
                        {
                            object row = enumerationRows.Current;
                            Entity entity = null;
                            if (table.PrimaryKeyColumn.IsNotNull())
                            {
                                if (valuePresent(row, table.PrimaryKeyColumn.Name))
                                {
                                    entity = table.Entities.FirstOrDefault(e => table.PrimaryKey(e).Equals(values(row, table.PrimaryKeyColumn.Name)));
                                }
                            }
                            if(entity.IsNull())
                            {
                                entity = ModelBindingList<Entity>.CreateInstance(table.EntityType, _context.Context, false) as Entity;
                                if (entity.IsNotNull())
                                {
                                    entity.Session = _session;
                                    list.Add(entity);
                                    if (added != null) added(tableName, entity);
                                }
                            }
                            table.Columns.ForEach((c) =>
                            {
                                if (valuePresent(row, table.PrimaryKeyColumn.Name))
                                {
                                    entity[c.Name] = values(row, c.Name);
                                }
                            });
                        }
                    }
                }
            }            
            return this;
        }

        public ModelUtils AcceptChanges()
        {
            var list = _context.Context.Tables.SelectMany(t => t.Entities.Where(e => e.Session == _session),
                        (t, e) => new { Table = t, Entity = e }).ToList();
            foreach (var te in list)
            {
                te.Entity.AcceptChanges();
            }
            return this;
        }

        public void Commit()
        {
            bool validated = true;
            var list = _context.Context.Tables.SelectMany(t => t.Entities.Where(e => e.Session == _session),
                (t, e) => new { Table = t, Entity = e }).ToList();
            foreach(var te in list)
            {
                te.Entity.Validate((v) => { return v.RealTimeValidation; }, 
                                   (v) => 
                                   {
                                       validated = false;
                                       RollBack();
                                   });
                if (!validated) break;
            }
            if (validated)
            {
                list.ForEach((te) => { te.Entity.Session = string.Empty; });
                _commited = true;
            }
        }
        
        private void RollBack()
        {
            var list = _context.Context.Tables.SelectMany(t => t.Entities.Where(e => e.Session == _session), 
                (t, e) => new { Table = t, Entity = e }).ToList();
            list.ForEach((te) =>
            {
                IList iList = te.Table.Entities as IList;
                if (iList.IsNotNull())
                {
                    te.Entity.State = EntityState.Added;
                    iList.Remove(te.Entity);
                }
            });
        }
        #endregion Methods

        #region Dispose & Destructor
        public void Dispose()
        {
            if (!_commited)
            {
                RollBack();
            }
            _context = null;
        }

        ~ModelUtils()
        {
            Dispose();
        }
        #endregion Dispose & Destructor
    }
}
