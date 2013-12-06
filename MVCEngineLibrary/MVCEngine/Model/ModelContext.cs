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
                TryCatchStatment.Try().Invoke(() =>
                {
                    string sessionId = MVCEngine.Session.Session.GetUserSessionId(name);
                    Task task = MVCEngine.Session.Session.GetSessionData<Task>(sessionId, "InitializeTask");
                    if (task.IsNotNull())
                    {
                        task.Wait();
                    }
                }).Catch<MVCEngine.Session.Exceptions.InvalidSessionIdException>((e) =>
                {}).Throw();                
            }
            Context ctx = _contexts.Value.FirstOrDefault(c => c.Name == name);
            if (ctx.IsNull())
            {
                throw new ModelException("Context[" + name + "] has to be initialized before create.");
            }
        }
        #endregion Constructor

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
                        if (f.FieldType.IsGenericType)
                        {
                            Type entityType = f.FieldType.GetGenericArguments().First<Type>();
                            InterceptorDispatcher.GetInstnace().Initialize(entityType);
                            Table table = new Table();                            
                            var query = from a in System.Attribute.GetCustomAttributes(entityType)
                                        where a is attribute.Table
                                        select a.CastToType<attribute.Table>();
                            attribute.Table tableAttribute = query.FirstOrDefault();
                            if (tableAttribute.IsNotNull())
                            {
                                table.TableName = tableAttribute.TableName;
                                table.FieldName = f.Name; 
                                table.EntityType = entityType;
                                var columnquery = entityType.GetProperties().Where(p => p.CanWrite && System.Attribute.GetCustomAttributes(p).Count() > 0).
                                    SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<attribute.Column>()).Select(a => a.CastToType<attribute.Column>()),
                                    (p, a) => new { Property = p, Attrubute = a });
                                columnquery.ToList().ForEach((pa) =>
                                {
                                    Column column = new Column() 
                                    {
                                        Name = pa.Attrubute.ColumnName,
                                        ColumnType = pa.Property.PropertyType,
                                        PrimaryKey = pa.Attrubute.IsPrimaryKey,
                                        ForeignKey = pa.Attrubute.IsForeignKey,
                                        ForeignTable = pa.Attrubute.ForeignTable
                                    };
                                    if (column.ForeignKey && column.ForeignTable.IsNullOrEmpty())
                                    {
                                        throw new ModelException("Type[" + entityType.FullName + "] coulmn[" + pa.Attrubute.ColumnName + "] is defined as IsForeignKey but ForeignTable is empty");
                                    }
                                    table.Columns.Add(column);
                                });
                                var foreignquery = entityType.GetProperties().Where(p => p.CanWrite && System.Attribute.GetCustomAttributes(p).Count() > 0).
                                                                    SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<attribute.Table>()).Select(a => a.CastToType<attribute.Table>()),
                                                                    (p, a) => new { Property = p, Attrubute = a });
                                foreignquery.ToList().ForEach((pa) =>
                                {
                                    if (table.Columns.Exists(c => c.ForeignTable == pa.Attrubute.TableName && c.ForeignKey))
                                    {
                                        table.Parents.Add(new Relation()
                                        {
                                            PropertyName = pa.Property.Name,
                                            TableName = pa.Attrubute.TableName
                                        });
                                    }
                                    else
                                    {
                                        table.Children.Add(new Relation()
                                        {
                                            PropertyName = pa.Property.Name,
                                            TableName = pa.Attrubute.TableName
                                        });

                                    }
                                });
                            }
                            else
                            {
                                throw new ModelException("Type[" + entityType.FullName + "] it cann't be recognise as valid entity.");
                            }
                            ctx.Tables.Add(table);
                        }
                    });
                });

                task.ContinueWith((antecedent) =>
                {
                    MVCEngine.Session.Session.ReleaseSession(sessionid);
                });

                MVCEngine.Session.Session.SetSessionData(sessionid, "InitializeTask", task);

                task.Start();
            }
        }
        #endregion Context Initializtion

        #region Freeze & UnFreeze
        public void Freeze<T>(T obj) where T : Entity
        {
            obj.IsFrozen = true;
        }

        public void UnFreeze<T>(T obj) where T : Entity
        {
            obj.IsFrozen = false;
        }
        #endregion Freeze & UnFreeze

        #region Dispose & Destructor
        public void Dispose()
        {
        }

        ~ModelContext()
        {
            Dispose();
        }
        #endregion Dispose & Destructor
    }
}
