using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using System.Reflection;
using MVCEngine.Model.Internal;
using MVCEngine.Model.Internal.Descriptions;
using attribute = MVCEngine.Model.Attributes;
using MVCEngine.Model.Exceptions;
using MVCEngine;
using MVCEngine.Internal.Validation;
using System.Diagnostics;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Discriminators;
using MVCEngine.Model.Attributes.Validation;
using MVCEngine.Model.Attributes.Default;
using MVCEngine.Model.Interceptors;
using MVCEngine.Internal;
using MVCEngine.Model.Attributes.Formatter;

namespace MVCEngine.Model
{
    public abstract class EntitiesContext : IDisposable
    {
        #region Members
        private static Lazy<List<Context>> _contexts;
        private static Lazy<Dictionary<string, EntityClass>> _entites;
        private static Lazy<Dictionary<string, Dictionary<string, Func<object, object>>>> _entitiesCollection;
        #endregion Members

        #region Constructor
        static EntitiesContext()
        {
            _contexts = new Lazy<List<Context>>(() =>
            {
                return new List<Context>();
            });
            _entites = new Lazy<Dictionary<string ,EntityClass>>(() =>
            {
                return new Dictionary<string, EntityClass>();
            });
            _entitiesCollection = new Lazy<Dictionary<string, Dictionary<string, Func<object, object>>>>(() =>
            {
                return new Dictionary<string, Dictionary<string, Func<object, object>>>();
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
            Debug.Assert(ctx.IsNotNull(), "Context[" + name + "] has to be initialized before create.");
            if (ctx.IsNotNull())
            {
                Context = ctx.Copy().InitailizeRows(this, _entitiesCollection.Value[ctx.Name]);
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
        public Context Context { get; set; }
        #endregion Context
        
        #region Context Initializtion
        public static void EntitiesContextInitialization<T>() where T : EntitiesContext
        {            
            if (_contexts.Value.FirstOrDefault(c => c.Name == (typeof(T).Name)).IsNull())
            {
                string sessionid = MVCEngine.Session.Session.CreateUserSession(typeof(T).Name);
                Task task = new Task(() =>
                {
                    Dictionary<MVCEngine.Model.Internal.Descriptions.DynamicProperties, string[]> dynamicList = 
                        new Dictionary<MVCEngine.Model.Internal.Descriptions.DynamicProperties, string[]>();
                    Context ctx = new Context()
                    {
                        Name = typeof(T).Name,
                        Entites = new List<EntityClass>()
                    };
                    _contexts.Value.Add(ctx);
                    _entitiesCollection.Value.Add(ctx.Name, new Dictionary<string, Func<object, object>>());

                    typeof(T).GetFields().Where(f => f.FieldType.Name == "EntitiesCollection`1" && f.IsPublic).
                    ToList().ForEach((f) =>
                    {
                        List<string> realTimeValidator = new List<string>();
                        PropertyInfo ctxInfo = f.FieldType.GetProperty("Context");                   
                        Type entityType = f.FieldType.GetGenericArguments().First<Type>();
                        Debug.Assert(typeof(Entity).IsAssignableFrom(entityType), "Entity[" + entityType.FullName + "] it cann't be recognise as valid entity.");
                        if (typeof(Entity).IsAssignableFrom(entityType))
                        {                            
                            if (!_entites.Value.ContainsKey(entityType.FullName))
                            {
                                EntityClass entityClass = new EntityClass();
                                Debug.Assert(!ctx.Entites.Exists((t) => { return t.Name == entityType.Name; }), "Entity[" + entityType.Name + "] is defined twice.");
                                if (!ctx.Entites.Exists((t) => { return t.Name == entityType.Name; }))
                                {
                                    entityClass.Name = entityType.Name;
                                    entityClass.EntityType = entityType;
                                    entityClass.Attributes.AddRange(Attribute.GetCustomAttributes(entityType));

                                    _entitiesCollection.Value[ctx.Name].Add(entityType.Name, LambdaTools.FieldGetter(typeof(T), f));

                                    var propertyquery = entityType.GetProperties().Where(p => p.CanRead && p.GetGetMethod().IsVirtual);
                                    propertyquery.ToList().ForEach((p) =>
                                    {
                                        EntityProperty property = new EntityProperty()
                                        {
                                            Name = p.Name,
                                            PropertyType = p.PropertyType,
                                            PropertyInfo = p,
                                            Setter = p.CanWrite ? LambdaTools.PropertySetter(entityType, p) : null,
                                            Getter = LambdaTools.PropertyGetter(entityType, p)
                                        };
                                        property.Attibutes.AddRange(Attribute.GetCustomAttributes(p));
                                        entityClass.Properties.Add(property);

                                        if (typeof(Entity).IsAssignableFrom(p.PropertyType))
                                        {
                                            property.ReletedEntity = new ReletedEntity()
                                            {
                                                Related = Releted.Entity,
                                                RelatedEntityName = p.PropertyType.Name
                                            };
                                        }
                                        else if (p.PropertyType.Name == "EntitiesCollection`1")
                                        {
                                            property.ReletedEntity = new ReletedEntity()
                                            {
                                                Related = Releted.List,
                                                RelatedEntityName = p.PropertyType.GetGenericArguments().First<Type>().Name                                                
                                            };                                            
                                        }
                                        Attribute.GetCustomAttributes(p).ToList().ForEach((a) =>
                                        {
                                            Relation relation = null;
                                            Discriminator discriminator = null;
                                            PropertyValidator validator = null;
                                            DefaultValue defaultValue = null;
                                            Synchronized synchronized = null;
                                            Formatter formatter = null;
                                            NotIntercept notIntercept = null;
                                            Intercept intercept = null;
                                            attribute.DynamicProperties dynamicProperties = null;
                                            if (a.IsTypeOf<PrimaryKey>())
                                            {
                                                Debug.Assert(entityClass.Properties.FirstOrDefault(primary => primary.PrimaryKey).IsNull(), "Entity[" + entityType.Name + "] at least two primary key property defined");
                                                property.PrimaryKey = true;
                                                entityClass.PrimaryKeyProperty = property;
                                                entityClass.PrimaryKey = property.Getter;
                                            }
                                            else if ((relation = a.CastToType<Relation>()).IsNotNull())
                                            {
                                                EntitiesRelation r = ctx.Relations.FirstOrDefault(re => re.Name == relation.RelationName);
                                                Debug.Assert(r.IsNull(), "Relation[" + relation.RelationName + "] is declared at least twice");
                                                if (property.ReletedEntity.IsNotNull())
                                                {
                                                    EntitiesRelation entityrelation = new EntitiesRelation()
                                                    {
                                                        Name = relation.RelationName,
                                                        Parent = new EntityRelated()
                                                        {
                                                            EntityName = relation.ParentEntity,
                                                            Key = relation.ParentProperty
                                                        },
                                                        Child = new EntityRelated()
                                                        {
                                                            EntityName = relation.ChildEntity,
                                                            Key = relation.ChildProperty
                                                        },
                                                        OnDelete = relation.OnDelete
                                                    };
                                                    ctx.Relations.Add(entityrelation);
                                                    property.ReletedEntity.Relation = entityrelation;
                                                }
                                            }
                                            else if ((synchronized = a.CastToType<Synchronized>()).IsNotNull())
                                            {
                                                if (property.ReletedEntity.IsNotNull())
                                                {
                                                    property.ReletedEntity.Synchronized = true;
                                                }
                                            }
                                            else if ((discriminator = a.CastToType<Discriminator>()).IsNotNull())
                                            {
                                                if (property.ReletedEntity.IsNotNull())
                                                {
                                                    property.ReletedEntity.Discriminators.Add(discriminator);
                                                }
                                            }
                                            else if ((validator = a.CastToType<PropertyValidator>()).IsNotNull())
                                            {
                                                property.Validators.Add(validator);
                                            }
                                            else if ((defaultValue = a.CastToType<DefaultValue>()).IsNotNull())
                                            {
                                                property.DefaultValue = defaultValue;
                                            }
                                            else if ((dynamicProperties = a.CastToType<attribute.DynamicProperties>()).IsNotNull())
                                            {
                                                if (property.ReletedEntity.IsNotNull() && property.ReletedEntity.Related == Releted.List)
                                                {
                                                    entityClass.DynamicProperties = new Internal.Descriptions.DynamicProperties() 
                                                    {
                                                        CodeProperty = dynamicProperties.CodeProperty,
                                                        Property = property,                                                       
                                                    };
                                                    dynamicList.Add(entityClass.DynamicProperties, dynamicProperties.ValueProperties);
                                                }
                                            }
                                            else if ((formatter = a.CastToType<Formatter>()).IsNotNull())
                                            {
                                                property.AddFormatter(formatter);
                                            }
                                            else if ((notIntercept = a.CastToType<NotIntercept>()).IsNotNull())
                                            {
                                                if (notIntercept.InterceptorId.IsNullOrEmpty())
                                                {
                                                    property.RemoveGetInterceptor(string.Empty);
                                                    property.RemoveSetInterceptor(string.Empty);
                                                }
                                                else
                                                {
                                                    if (notIntercept.Method == Method.Get)
                                                    {
                                                        property.RemoveGetInterceptor(notIntercept.InterceptorId);
                                                    }
                                                    else
                                                    {
                                                        property.RemoveSetInterceptor(notIntercept.InterceptorId);
                                                    }
                                                }
                                            }
                                            else if ((intercept = a.CastToType<Intercept>()).IsNotNull() && intercept.Method.IsNotNull())
                                            {
                                                if (intercept.Method.Contains(Method.Get))
                                                {
                                                    property.AddGetInterceptor(intercept.InterceptorId);
                                                }
                                                
                                                if (intercept.Method.Contains(Method.Set))
                                                {
                                                    property.AddSetInterceptor(intercept.InterceptorId);
                                                }
                                            }
                                        });
                                    });
                                    Attribute.GetCustomAttributes(entityType).ToList().ForEach((a) =>
                                    {
                                        EntityValidator validator = null;
                                        if ((validator = a.CastToType<EntityValidator>()).IsNotNull())
                                        {
                                            entityClass.Validators.Add(validator);
                                        }
                                    });
                                    ctx.Entites.Add(entityClass);
                                }
                                _entites.Value.Add(entityType.FullName, entityClass);
                            }
                            else
                            {
                                ctx.Entites.Add(_entites.Value[entityType.FullName]);
                            }
                        }                                                           
                    });
                    ctx.Relations.ForEach((r) =>
                    {
                        EntityClass entity = ctx.Entites.FirstOrDefault(e => e.Name == r.Parent.EntityName);
                        Debug.Assert(entity.IsNotNull(), "Relation[" + r.Name + "] parent entity not found");
                        r.Parent.Entity = entity;
                        EntityProperty property = entity.Properties.FirstOrDefault(p => p.Name == r.Parent.Key);
                        Debug.Assert(property.IsNotNull(), "Entity[" + entity.Name + "] property["+r.Parent.Key+"] not defined");
                        r.Parent.Type = property.PropertyType;
                        r.Parent.Value = property.Getter;

                        EntityClass childEntity = ctx.Entites.FirstOrDefault(e => e.Name == r.Child.EntityName);
                        Debug.Assert(childEntity.IsNotNull(), "Relation[" + r.Name + "] child entity not found");
                        r.Child.Entity = childEntity;
                        EntityProperty childProperty = childEntity.Properties.FirstOrDefault(p => p.Name == r.Child.Key);
                        Debug.Assert(childProperty.IsNotNull(), "Entity[" + childEntity.Name + "] property[" + r.Child.Key + "] not defined");
                        r.Child.Type = childProperty.PropertyType;
                        r.Child.Value = childProperty.Getter;
                    });

                    var reletedquery = ctx.Entites.Where(e => e.Properties.Count(p => p.ReletedEntity.IsNotNull()) > 0).
                        SelectMany(e => e.Properties.Where(p => p.ReletedEntity.IsNotNull()), (e, p) => new { Entity = e, Property = p });

                    reletedquery.ToList().ForEach((ep) =>
                    {
                        EntityClass entityClass = ctx.Entites.FirstOrDefault(e => e.Name == ep.Property.ReletedEntity.RelatedEntityName);
                        Debug.Assert(entityClass.IsNotNull(), "Entity[" + ep.Property.ReletedEntity.RelatedEntityName + "] dosen't have collection");
                        ep.Property.ReletedEntity.RelatedEntity = entityClass;
                        if (ep.Property.ReletedEntity.Relation.IsNull())
                        {
                            if (!ep.Property.ReletedEntity.RelationName.IsNullOrEmpty())
                            {
                                EntitiesRelation relation = ctx.Relations.FirstOrDefault(r => r.Name == ep.Property.ReletedEntity.RelationName);
                                Debug.Assert(relation.IsNotNull(), "Relation[" + ep.Property.ReletedEntity.RelationName + "] not defined");
                                ep.Property.ReletedEntity.Relation = relation;
                            }
                            else
                            {
                                List<EntitiesRelation> relations = null;
                                if (ep.Property.ReletedEntity.Related == Releted.Entity)
                                {
                                    relations = ctx.Relations.Where(r => r.Parent.EntityName == ep.Property.ReletedEntity.RelatedEntityName && r.Child.EntityName == ep.Entity.Name).ToList();
                                }
                                else
                                {
                                    relations = ctx.Relations.Where(r => r.Parent.EntityName == ep.Entity.Name && r.Child.EntityName == ep.Property.ReletedEntity.RelatedEntityName).ToList();
                                }

                                Debug.Assert(relations.Count() < 2, "Relation[" + ep.Property.ReletedEntity.RelatedEntityName + "-" + ep.Entity.Name + "] more then one");
                                if (relations.Count() == 1)
                                {
                                    ep.Property.ReletedEntity.Relation = relations[0];
                                }
                            }
                        }
                        if (ep.Property.ReletedEntity.Synchronized)
                        {
                            ep.Property.ReletedEntity.RelatedEntity.Synchronized(ep.Entity.Name, ep.Property.Name);
                        }
                        if (ep.Property.ReletedEntity.Related == Releted.List)
                        {
                            ep.Property.AddGetInterceptor(CollectionInterceptorDispatcher.GetId(ep.Property.ReletedEntity.Relation.Child.Entity.EntityType));
                        }
                        else
                        {
                            ep.Property.AddGetInterceptor(EntityInterceptorDispatcher.GetId(ep.Property.ReletedEntity.RelatedEntityName));
                        }
                    });

                    foreach (MVCEngine.Model.Internal.Descriptions.DynamicProperties dynamicProperty in dynamicList.Keys)
                    {
                        foreach (string p in dynamicList[dynamicProperty])
                        {
                            EntityProperty property = dynamicProperty.Property.ReletedEntity.RelatedEntity.Properties.FirstOrDefault(pr => pr.Name == p);
                            if (property.IsNotNull())
                            {
                                dynamicProperty.ValuesProperties.Add(property.PropertyType, property.Name);
                            }
                        }
                    }
                });

                task.ContinueWith((antecedent) =>
                {
                    MVCEngine.Session.Session.ReleaseSession(sessionid);
                });

                task.ContinueWith((antecedent) =>
                {
                    if (_contexts.IsValueCreated) _contexts.Value.Clear();
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
                Context.Entites.ForEach((t) =>
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
            Debug.Assert(ctx.IsNotNull(), "Context[" + type.Name + "] has to be initialized before create.");
            return ctx;
        }
        #endregion Context By Type

        #region Interceptor
        internal static bool ShouldInterceptMethod(Type type, System.Reflection.MethodInfo methodInfo)
        {
            if (_entites.Value.ContainsKey(type.FullName))
            {
                return _entites.Value[type.FullName].Properties.Where(p => p.GetInterceptorsId(methodInfo.Name).Count() > 0).Count() > 0;           
            }
            else
            {
                return false;
            }
        }

        internal static IInterceptor[] GetInterceptors(Type type)
        {
            EntityClass entity = _entites.Value[type.FullName];
            if (entity.IsNotNull())
            {
                //TO DO Cache
                List<IInterceptor> interceptors = new List<IInterceptor>();
                interceptors.Add(SecurityInterceptor.GetInstance());
                interceptors.Add(ModificationInterceptor.GetInstance());
                if (entity.Validators.Count(v => v.RealTimeValidation) > 0 ||
                   entity.Properties.SelectMany(p => p.Validators.Where(v => v.RealTimeValidation), (p, v) => v).Count() > 0)
                {
                    interceptors.Add(ValidationInterceptor.GetInstance());
                }
                entity.Properties.Where(p => p.ReletedEntity.IsNotNull()).ToList().
                    ForEach((p) =>
                {
                    if (p.ReletedEntity.Related == Releted.List)
                    {
                        interceptors.Add(CollectionInterceptorDispatcher.GetInstance(p.ReletedEntity.RelatedEntity.EntityType));
                    }
                    else
                    {
                        interceptors.Add(EntityInterceptorDispatcher.GetInstance(p.ReletedEntity.RelatedEntity.EntityType));
                    }
                });
                entity.Attributes.Where(a => a.IsTypeOf<Interceptor>()).Select(a => a.CastToType<Interceptor>()).ToList().
                    ForEach((i) =>
                {
                    interceptors.Add(i);
                });
                return interceptors.ToArray();
            }
            else
            {
                return new IInterceptor[0];
            }
        }

        internal static IInterceptor[] SelectInterceptors(MethodInfo info, IInterceptor[] interceptors)
        {
            Type type = info.ReflectedType;
            if (_entites.Value.ContainsKey(type.FullName))
            {
                EntityClass entity = _entites.Value[type.FullName];
                if (entity.IsNotNull())
                {
                    EntityProperty property = entity.Properties.FirstOrDefault(p => p.GetInterceptorsId(info.Name).Count() > 0);
                    if (property.IsNotNull())
                    {
                        var query = from i in property.GetInterceptorsId(info.Name)
                                    join ii in interceptors.Where(i => i.IsTypeOf<Interceptor>()).Select(i => i.CastToType<Interceptor>())
                                    on i equals ii.GetId()
                                    select ii.CastToType<IInterceptor>();
                        return query.ToArray();
                    }
                    else
                    {
                        return new IInterceptor[0];
                    }
                }
                else
                {
                    return new IInterceptor[0];
                }
            }
            else
            {
                return new IInterceptor[0];
            }
        }
        #endregion Interceptor

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
