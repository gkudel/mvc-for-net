using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal;
using Castle.DynamicProxy;
using System.Reflection;
using MVCEngine.Model.Internal;
using MVCEngine.Session;
using MVCEngine.Model.Internal.Descriptions;
using attribute = MVCEngine.Model.Attributes;
using MVCEngine.Model.Exceptions;
using MVCEngine.Internal.Validation;

namespace MVCEngine.Model
{
    public abstract class ModelContext : IDisposable
    {
        #region Members
        private static Lazy<List<Context>> _contexts;        
        #endregion Members

        #region Constructor
        static ModelContext()
        {
            _contexts = new Lazy<List<Context>>(() =>
            {
                return new List<Context>();
            });
        }

        public ModelContext()
        {
            string name = this.GetType().Name;
            if (MVCEngine.Session.Session.IsUserSessionExists(name))
            {
                try
                {
                    string sessionId = MVCEngine.Session.Session.GetUserSessionId(name);
                    Task task = MVCEngine.Session.Session.GetSessionData<Task>(sessionId, "InitializeTask");
                    if (task.IsNotNull())
                    {
                        task.Wait();
                    }
                }
                catch (Session.Exceptions.InvalidSessionIdException)
                { }
                catch (AggregateException)
                { }
            }
            Context ctx = _contexts.Value.FirstOrDefault(c => c.Name == name);
            if (ctx.IsNotNull())
            {
                Context = ctx.Copy().InitailizeRows(this); 
            }
            else
            {
                throw new ModelException("Context[" + name + "] has to be initialized before create.");
            }
        }
        #endregion Constructor

        #region GetChildEntities
        public List<C> GetChildEntities<P, C>(P parent, Relation relation) where P : Entity
                                                                           where C: Entity
        {
            List<C> ret = new List<C>();
            if (Context.IsNotNull())
            {
                Table parentTable = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(P).Name);
                if (parentTable.IsNotNull())
                {
                    Table childTable = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(C).Name);
                    if (childTable.IsNotNull())
                    {
                        if (relation.ParentTable == parentTable.TableName && relation.ChildTable == childTable.TableName)
                        {
                            ret = childTable.Entities.Cast<C>().Where(c => c.State != EntityState.Deleted && relation.ParentValue(parent).
                                        Equals(relation.ChildValue(c))).ToList();                                
                        }
                        else
                        {
                            throw new ModelException("Parent table["+relation.ParentTable+"] and child table["+relation.ChildTable+"] " +
                                                     " are not match to objects type of " + typeof(P).Name + " and "  + typeof(C).Name);
                        }
                    }
                    else
                    {
                        throw new ModelException("There is not table mapped to class " + typeof(C).Name);
                    }
                }
                else
                {
                    throw new ModelException("There is not table mapped to class " + typeof(P).Name);
                }
            }
            else
            {
                throw new ModelException("Context is null");
            }
            return ret;
        }
        #endregion GetChildEntities

        #region GetRelatedEntity
        public C GetRelatedEntity<P, C>(P parent, Relation relation)
            where P : Entity
            where C : Entity
        {
            C entity = default(C);
            if (Context.IsNotNull())
            {
                Table parentTable = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(P).Name);
                if (parentTable.IsNotNull())
                {
                    Table childTable = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(C).Name);
                    if (childTable.IsNotNull())
                    {
                        if (relation.ParentTable == parentTable.TableName && relation.ChildTable == childTable.TableName)
                        {
                            List<C> list = childTable.Entities.Cast<C>().Where(c => c.State != EntityState.Deleted && relation.ParentValue(parent).
                                        Equals(relation.ChildValue(c))).ToList();
                            if (list.Count() == 1)
                            {
                                entity = list.First();
                            }
                            else if (list.Count() > 1)
                            {
                                throw new ModelException("Integrity constraint exception");
                            }
                            else
                            {
                                entity = default(C);
                            }
                        }
                        else
                        {
                            throw new ModelException("Parent table[" + relation.ParentTable + "] and child table[" + relation.ChildTable + "] " +
                                                     " are not match to objects type of " + typeof(P).Name + " and " + typeof(C).Name);
                        }
                    }
                    else
                    {
                        throw new ModelException("There is not table mapped to class " + typeof(C).Name);
                    }
                }
                else
                {
                    throw new ModelException("There is not table mapped to class " + typeof(P).Name);
                }
            }
            else
            {
                throw new ModelException("Context is null");
            }
            return entity;
        }
        #endregion GetChildEntity

        #region Context
        internal Context Context { get; set; }
        #endregion Context
        
        #region Context Initializtion
        public static void ModelContextInitialization<T>() where T : ModelContext
        {            
            if (_contexts.Value.FirstOrDefault(c => c.Name == (typeof(T).Name)).IsNull())
            {
                string sessionid = MVCEngine.Session.Session.CreateUserSession(typeof(T).Name);
                Task task = new Task(() =>
                {
                    Context ctx = new Context()
                    {
                        Name = typeof(T).Name,
                        Tables = new List<Table>()
                    };
                    _contexts.Value.Add(ctx);

                    typeof(T).GetFields().Where(f => f.FieldType.Name == "ModelBindingList`1" && f.IsPublic).
                    ToList().ForEach((f) =>
                    {
                        List<string> realTimeValidator = new List<string>();
                        if (f.FieldType.IsGenericType)
                        {
                            PropertyInfo ctxInfo = f.FieldType.GetProperty("Context");                   
                            Type entityType = f.FieldType.GetGenericArguments().First<Type>();                            
                            Table table = new Table();                            
                            var query = from a in System.Attribute.GetCustomAttributes(entityType)
                                        where a.IsTypeOf<attribute.Table>()
                                        select a.CastToType<attribute.Table>();
                            attribute.Table tableAttribute = query.FirstOrDefault();
                            if (tableAttribute.IsNotNull())
                            {
                                if (!ctx.Tables.Exists((t) => { return t.TableName == tableAttribute.TableName; }))
                                {
                                    table.TableName = tableAttribute.TableName;
                                    table.EntityFieldName = f.Name;
                                    table.ClassName = entityType.Name;
                                    table.EntityType = entityType;
                                    table.EntityFieldGetter = LambdaTools.FieldGetter(typeof(T), f);
                                    table.ContextSetter = LambdaTools.PropertySetter(f.FieldType, ctxInfo);

                                    var columnquery = entityType.GetProperties().Where(p => p.CanWrite && p.CanRead && System.Attribute.GetCustomAttributes(p).Count() > 0).
                                        SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<attribute.Column>()).Select(a => a.CastToType<attribute.Column>()),
                                        (p, a) => new { Property = p, Attrubute = a });
                                    columnquery.ToList().ForEach((pa) =>
                                    {
                                        if(table.Columns.Exists(new Predicate<Column>((c) => { return c.Name == pa.Attrubute.ColumnName; })))
                                        {
                                            throw new ModelException("Column[" + pa.Attrubute.ColumnName + "] is declared twice");
                                        }

                                        Column column = new Column()
                                        {
                                            Name = pa.Attrubute.ColumnName,
                                            Property = pa.Property.Name,
                                            ColumnType = pa.Property.PropertyType,
                                            PrimaryKey = pa.Attrubute.IsPrimaryKey,
                                            Setter = LambdaTools.PropertySetter(entityType, pa.Property),
                                            Getter = LambdaTools.PropertyGetter(entityType, pa.Property)
                                        };                                        
                                        if (pa.Attrubute.IsPrimaryKey && table.Columns.Exists(new Predicate<Column>((c) => { return c.PrimaryKey; })))
                                        {
                                            throw new ModelException("Column[" + pa.Attrubute.ColumnName + "] is defined as PrimaryKey but there is other column marked as Primary Key");
                                        }
                                        if (pa.Attrubute.IsPrimaryKey)
                                        {
                                            table.PrimaryKey = LambdaTools.PropertyGetter(entityType, pa.Property);
                                            table.PrimaryKeyColumn = column;
                                        }
                                        if (pa.Attrubute.IsForeignKey && pa.Attrubute.ForeignTable.IsNullOrEmpty())
                                        {
                                            throw new ModelException("Type[" + entityType.FullName + "] coulmn[" + pa.Attrubute.ColumnName + "] is defined as IsForeignKey but ForeignTable is empty");
                                        }
                                        else if (pa.Attrubute.IsForeignKey)
                                        {                                            
                                            ctx.Relations.Add(new Relation()
                                            {
                                                Name = pa.Attrubute.RelationName, 
                                                ChildKey = pa.Property.Name,
                                                ChildType = pa.Property.PropertyType, 
                                                ChildTable = table.TableName,
                                                ParentKey = pa.Attrubute.ForeignColumn,
                                                ParentTable = pa.Attrubute.ForeignTable,
                                                OnDelete = pa.Attrubute.OnDelete
                                            });
                                        }
                                        table.Columns.Add(column);
                                    });
     
                                    var defaultquery = entityType.GetProperties().Where(p => p.CanWrite && p.CanRead && System.Attribute.GetCustomAttributes(p).Count() > 0).
                                             SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<attribute.Default.DefaultValue>() || a.IsTypeOf<attribute.Validation.ColumnValidator>()).
                                             Select(a => a), (p, a) => new { Property = p, Attrubute = a }).
                                             SelectMany(pa => table.Columns.Where(c => c.Property == pa.Property.Name), (pa, c) => new { Property = pa.Property, Attribute = pa.Attrubute, Column = c });
                                    defaultquery.ToList().ForEach((pac) =>
                                    {
                                        if(pac.Attribute.IsTypeOf<attribute.Default.DefaultValue>())
                                        {
                                            pac.Column.DefaultValue = pac.Attribute.CastToType<attribute.Default.DefaultValue>();
                                        }
                                        else
                                        {
                                            attribute.Validation.ColumnValidator validator = pac.Attribute.CastToType<attribute.Validation.ColumnValidator>();
                                            pac.Column.Validators.Add(validator);
                                            if (validator.RealTimeValidation) realTimeValidator.Add("set_" + pac.Property.Name);
                                        }
                                    });
                                }
                                else
                                {
                                    throw new ModelException("Table[" + tableAttribute.TableName + "] is defined twice.");
                                }
                            }
                            else
                            {
                                throw new ModelException("Type[" + entityType.FullName + "] it cann't be recognise as valid entity.");
                            }
                            var validatorentityquery = from a in System.Attribute.GetCustomAttributes(entityType)
                                                 where a.IsTypeOf<attribute.Validation.EntityValidator>()
                                                 select a.CastToType<attribute.Validation.EntityValidator>();
                            validatorentityquery.ToList().ForEach((v) =>
                            {
                                table.Validators.Add(v);
                                if (v.RealTimeValidation)
                                {
                                    if (v.ColumnsName.IsNotNull())
                                    {
                                        realTimeValidator.AddRange(v.ColumnsName.ToList().Select(c => "set_" + c));
                                    }
                                    else if (v.IsTypeOf<attribute.Validation.PrimaryKeyValidator>())
                                    {
                                        Column primary = table.Columns.FirstOrDefault(c => c.PrimaryKey);
                                        if (primary.IsNotNull())
                                        {
                                            realTimeValidator.Add("set_" + primary.Property);
                                        }
                                    }
                                }
                            });

                            ctx.Tables.Add(table);
                            InterceptorDispatcher.GetInstnace().Initialize(entityType);
                            if (realTimeValidator.Count() > 0)
                            {
                                InterceptorDispatcher.GetInstnace().Initialize(entityType, new attribute.Interceptor(DefaultInterceptors.ValidationInterceptor, realTimeValidator.ToArray()));
                            }
                        }
                    });
                    

                    var relationquery = ctx.Relations.SelectMany(r => ctx.Tables.Where(t => t.TableName == r.ParentTable), (r, t) => new { Relation = r, Parent = t }).
                        SelectMany(pr => ctx.Tables.Where(t => t.TableName == pr.Relation.ChildTable), (pr, c) => new { Parent = pr.Parent, Relation = pr.Relation, Child = c });
                    relationquery.ToList().ForEach((prc) =>
                    {
                        if(prc.Relation.ParentKey.IsNullOrEmpty())
                        {
                            Column parentKey = prc.Parent.Columns.FirstOrDefault(c => c.PrimaryKey);
                            if(parentKey.IsNotNull())
                            {
                                prc.Relation.ParentKey = parentKey.Property;
                                prc.Relation.ParentType = parentKey.ColumnType;
                            }
                            else
                            {
                                throw new ModelException("Table["+prc.Parent.TableName+"] doesn't have Primary Key");
                            }
                            prc.Relation.ParentValue = LambdaTools.PropertyGetter(prc.Parent.EntityType, prc.Relation.ParentKey);
                            prc.Relation.ChildValue = LambdaTools.PropertyGetter(prc.Child.EntityType, prc.Relation.ChildKey);
                        }
                    });
                });

                task.ContinueWith((antecedent) =>
                {
                    MVCEngine.Session.Session.ReleaseSession(sessionid);
                });

                task.ContinueWith((antecedent) =>
                {      
                    //ToDo log exception into log file
                }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);

                MVCEngine.Session.Session.SetSessionData(sessionid, "InitializeTask", task);

                task.Start();
            }
        }
        #endregion Context Initializtion

        #region Freeze & UnFreeze
        public static void Freeze(Entity obj)
        {
            obj.IsFrozen = true;
        }

        public static void UnFreeze(Entity obj)
        {
            obj.IsFrozen = false;
        }
        #endregion Freeze & UnFreeze

        #region AcceptChanges
        public void AcceptChanges()
        {
            if (Context.IsNotNull())
            {
                Context.Tables.ForEach((t) =>
                {
                    t.Entities.ToList().ForEach((r) =>
                    {
                        r.AcceptChanges();
                    });
                });
            }
        }
        #endregion AcceptChanges

        #region Dispose & Destructor
        public void Dispose()
        {
            if (Context.IsNotNull())
            {
                Context.Dispose();
                Context = null;
            }
        }

        ~ModelContext()
        {
            Dispose();
        }
        #endregion Dispose & Destructor
    }
}
