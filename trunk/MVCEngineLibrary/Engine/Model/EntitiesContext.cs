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
    public abstract class EntitiesContext : IDisposable
    {
        #region Members
        private static Lazy<List<Context>> _contexts;        
        #endregion Members

        #region Constructor
        static EntitiesContext()
        {
            _contexts = new Lazy<List<Context>>(() =>
            {
                return new List<Context>();
            });
        }

        public EntitiesContext()
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
                Context.EntitiesContextType = GetType();
            }
            else
            {
                throw new ModelException("Context[" + name + "] has to be initialized before create.");
            }
        }
        #endregion Constructor

        #region Properties
        public Action ContextModifed 
        {
            get { return Context.IsNotNull() ? Context.ContextModifed : null; }
            set { if (Context.IsNotNull()) Context.ContextModifed = value; }
        }
        public Action ChangesAccepted { get; set; }
        #endregion Properties

        #region Context
        internal Context Context { get; set; }
        #endregion Context
        
        #region Context Initializtion
        public static void EntitiesContextInitialization<T>() where T : EntitiesContext
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

                    typeof(T).GetFields().Where(f => f.FieldType.Name == "EntitiesCollection`1" && f.IsPublic).
                    ToList().ForEach((f) =>
                    {
                        List<string> realTimeValidator = new List<string>();
                        if (f.FieldType.IsGenericType)
                        {
                            PropertyInfo ctxInfo = f.FieldType.GetProperty("Context");                   
                            Type entityType = f.FieldType.GetGenericArguments().First<Type>();
                            if (typeof(Entity).IsAssignableFrom(entityType))
                            {
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

                                        var columnquery = entityType.GetProperties().Where(p => p.CanRead && System.Attribute.GetCustomAttributes(p).Count() > 0).
                                            SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<attribute.Column>()).Select(a => a.CastToType<attribute.Column>()),
                                            (p, a) => new { Property = p, Attrubute = a });
                                        columnquery.ToList().ForEach((pa) =>
                                        {
                                            if (table.Columns.Exists(new Predicate<Column>((c) => { return c.Name == pa.Attrubute.ColumnName; })))
                                            {
                                                throw new ModelException("Column[" + pa.Attrubute.ColumnName + "] is declared twice");
                                            }

                                            Column column = new Column()
                                            {
                                                Name = pa.Attrubute.ColumnName,
                                                Property = pa.Property.Name,
                                                ColumnType = pa.Property.PropertyType,
                                                PrimaryKey = pa.Attrubute.IsPrimaryKey,
                                                Setter = pa.Property.CanWrite ? LambdaTools.PropertySetter(entityType, pa.Property) : null,
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
                                                    ChildTableName = table.TableName,
                                                    ParentTableName = pa.Attrubute.ForeignTable,
                                                    ParentKey = pa.Attrubute.ForeignColumn,
                                                    OnDelete = pa.Attrubute.OnDelete
                                                });
                                            }
                                            table.Columns.Add(column);
                                        });

                                        var defaultvalidationquery = entityType.GetProperties().Where(p => p.CanWrite && p.CanRead && System.Attribute.GetCustomAttributes(p).Count() > 0).
                                                 SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<attribute.Default.DefaultValue>() || a.IsTypeOf<attribute.Validation.ColumnValidator>()).
                                                 Select(a => a), (p, a) => new { Property = p, Attrubute = a }).
                                                 SelectMany(pa => table.Columns.Where(c => c.Property == pa.Property.Name), (pa, c) => new { Property = pa.Property, Attribute = pa.Attrubute, Column = c });
                                        defaultvalidationquery.ToList().ForEach((pac) =>
                                        {
                                            if (pac.Attribute.IsTypeOf<attribute.Default.DefaultValue>())
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
                                        var dynamicpropertyquery = entityType.GetProperties().Where(p => p.CanWrite && p.CanRead && System.Attribute.GetCustomAttributes(p).Count() > 0).
                                                 SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<attribute.DynamicProperties>()).
                                                 Select(a => a), (p, a) => new { Property = p, Attrubute = a.CastToType<attribute.DynamicProperties>() });
                                        var dynamicpa = dynamicpropertyquery.FirstOrDefault();
                                        if (dynamicpa.IsNotNull())
                                        {
                                            if (dynamicpa.Property.PropertyType.Name == "EntitiesCollection`1")
                                            {
                                                table.DynamicProperties = new DynamicProperties()
                                                {
                                                    Entities = LambdaTools.PropertyGetter(entityType, dynamicpa.Property),
                                                    PropertyCode = dynamicpa.Attrubute.PropertyCode,
                                                    EntityType = dynamicpa.Property.PropertyType.GetGenericArguments().First<Type>()
                                                };
                                                table.DynamicProperties.PropertiesValue = table.DynamicProperties.EntityType.GetProperties().Where(p => dynamicpa.Attrubute.PropertiesValue.Contains(p.Name)
                                                    && System.Attribute.GetCustomAttributes(p).FirstOrDefault(a => a.IsTypeOf<attribute.Column>()).IsNotNull() && p.CanRead).
                                                    Select(p => new KeyValuePair<Type, string>(p.PropertyType, p.Name)).ToArray();
                                            }
                                        }
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
                            else
                            {
                                throw new ModelException("Type[" + entityType.FullName + "] it cann't be recognise as valid entity.");
                            }
                        }
                    });
                    

                    var relationquery = ctx.Relations.SelectMany(r => ctx.Tables.Where(t => t.TableName == r.ParentTableName), (r, t) => new { Relation = r, Parent = t }).
                        SelectMany(pr => ctx.Tables.Where(t => t.TableName == pr.Relation.ChildTableName), (pr, c) => new { Parent = pr.Parent, Relation = pr.Relation, Child = c });
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
                    t.Entities.ToList().ForEach((e) =>
                    {
                        e.AcceptChanges();
                    });
                });
                Context.IsModified = false;
                if (ChangesAccepted.IsNotNull()) ChangesAccepted();
            }
        }
        #endregion AcceptChanges

        #region Context By Type
        internal static Context GetContext(Type type)
        {
            Context ctx = _contexts.Value.FirstOrDefault(c => c.Name == type.Name);
            if (ctx.IsNotNull())
            {
                return ctx;
            }
            else
            {
                throw new ModelException("Context[" + type.Name + "] has to be initialized before create.");
            }
        }
        #endregion Context By Type

        #region Dispose & Destructor
        public void Dispose()
        {
            if (Context.IsNotNull())
            {
                Context.Dispose();
                Context = null;
            }
            ChangesAccepted = null;
        }

        ~EntitiesContext()
        {
            Dispose();
        }
        #endregion Dispose & Destructor
    }
}
