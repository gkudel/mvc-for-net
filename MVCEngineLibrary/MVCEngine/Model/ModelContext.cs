﻿using MVCEngine.Model.Interceptors;
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
            if (ctx.IsNotNull())
            {
                ctx = ctx.Copy().InitailizeRows(this); 
            }
            else
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
                    PropertyInfo pRows = typeof(Table).GetProperty("Rows");
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
                            PropertyInfo ctxInfo = f.FieldType.GetProperty("Context");                   
                            Type entityType = f.FieldType.GetGenericArguments().First<Type>();
                            InterceptorDispatcher.GetInstnace().Initialize(entityType);
                            Table table = new Table();                            
                            var query = from a in System.Attribute.GetCustomAttributes(entityType)
                                        where a is attribute.Table
                                        select a.CastToType<attribute.Table>();
                            attribute.Table tableAttribute = query.FirstOrDefault();
                            if (tableAttribute.IsNotNull())
                            {
                                if (!ctx.Tables.Exists((t) => { return t.TableName == tableAttribute.TableName; }))
                                {
                                    table.TableName = tableAttribute.TableName;
                                    table.RowsFieldName = f.Name;
                                    table.ClassName = entityType.Name;
                                    table.EntityType = entityType;
                                    table.RowsFieldGetter = LambdaTools.FieldGetter(typeof(T), f);
                                    table.ContextSetter = LambdaTools.PropertySetter(f.FieldType, ctxInfo);

                                    var columnquery = entityType.GetProperties().Where(p => p.CanWrite && System.Attribute.GetCustomAttributes(p).Count() > 0).
                                        SelectMany(p => System.Attribute.GetCustomAttributes(p).Where(a => a.IsTypeOf<attribute.Column>()).Select(a => a.CastToType<attribute.Column>()),
                                        (p, a) => new { Property = p, Attrubute = a });
                                    columnquery.ToList().ForEach((pa) =>
                                    {
                                        Column column = new Column()
                                        {
                                            Name = pa.Attrubute.ColumnName,
                                            Property = pa.Property.Name,
                                            ColumnType = pa.Property.PropertyType,
                                            PrimaryKey = pa.Attrubute.IsPrimaryKey,
                                        };
                                        if (pa.Attrubute.IsForeignKey && pa.Attrubute.ForeignTable.IsNullOrEmpty())
                                        {
                                            throw new ModelException("Type[" + entityType.FullName + "] coulmn[" + pa.Attrubute.ColumnName + "] is defined as IsForeignKey but ForeignTable is empty");
                                        }
                                        else if (pa.Attrubute.IsForeignKey)
                                        {
                                            ctx.Relations.Add(new Relation()
                                            {
                                                ChildKey = pa.Property.Name,
                                                ChildTable = table.TableName,
                                                ParentKey = pa.Attrubute.ForeignColumn,
                                                ParentTable = pa.Attrubute.ForeignTable
                                            });
                                        }
                                        table.Columns.Add(column);
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
                            ctx.Tables.Add(table);
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
