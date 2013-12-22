using Castle.DynamicProxy;
using MVCEngine.Attributes;
using MVCEngine.Model.Attributes.Discriminators;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using attribute = MVCEngine.Model.Attributes;
using MVCEngine.Internal;
using System.ComponentModel;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class CollectionInterceptor<T> : MVCEngine.Model.Internal.Interceptor, IDisposable where T : Entity
    {
        #region Members
        private List<Discriminator> _discriminators;        
        private Action<object, object> _setter;
        private Func<object, object> _getter;
        private Relation _relation;
        #endregion Members

        #region Constructor
        public CollectionInterceptor()
        {
            _discriminators = new List<Discriminator>();            
        }
        #endregion Constructor

        #region Inetercept
        public override void Intercept(IInvocation invocation)
        {
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (entity.IsNotNull())
            {
                if (!entity.Disposing)
                {
                    string name = invocation.Method.Name;
                    if (name.StartsWith("get_"))
                    {
                        name = name.Substring(4, name.Length - 4);
                    }
                    if (_relation.IsNull())
                    {
                        Table childTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                        if (entity.Table.IsNotNull() && childTable.IsNotNull())
                        {
                            List<Relation> relations = EntitiesContext.GetContext(entity.Context.EntitiesContextType).Relations.Where(r => r.ParentTableName == entity.Table.TableName
                                                                    && r.ChildTableName == childTable.TableName
                                                                    && (RelationName.IsNullOrEmpty() || RelationName.Equals(r.Name))).ToList();
                            if (relations.Count() == 1)
                            {
                                _relation = relations[0];
                            }
                            else
                            {
                                _relation = null;
                            }
                        }
                    }
                    if (_relation.IsNotNull())
                    {
                        Relation relation = entity.Context.Relations.FirstOrDefault(r => r.Ordinal == _relation.Ordinal);
                        if (relation.IsNotNull())
                        {
                            if (!entity.GetTableUidForProperty(name).Equals(relation.ChildTable.Uid))
                            {
                                if (AutoRefresh)
                                {
                                    if (!relation.ChildTable.Triggers.ContainsKey(entity))
                                    {
                                        relation.ChildTable.Triggers.Add(entity, new List<Func<object, object>>());
                                    }
                                    if (!relation.ChildTable.Triggers[entity].Contains(_getter))
                                    {
                                        relation.ChildTable.Triggers[entity].Add(_getter);
                                    }
                                }

                                invocation.Proceed();

                                List<T> list = relation.ChildTable.Entities.Where(c => c.State != EntityState.Deleted && relation.ParentValue(entity).
                                            Equals(relation.ChildValue(c)) &&
                                            _discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(c); }))).Cast<T>().ToList();

                                if (invocation.ReturnValue.IsNull())
                                {
                                    EntitiesCollection<T> collection = new EntitiesCollection<T>(list);
                                    collection.Context = entity.Context;
                                    collection.ChildCollection = true;
                                    collection.ChildColumn = relation.ChildKey;
                                    collection.ParentValue = relation.ParentValue(entity);
                                    collection.Discriminators = _discriminators;
                                    invocation.ReturnValue = collection;
                                    if (_setter.IsNotNull())
                                    {
                                        _setter(entity, invocation.ReturnValue);
                                    }
                                }
                                else
                                {
                                    EntitiesCollection<T> current = invocation.ReturnValue as EntitiesCollection<T>;
                                    current.CopiedFromMain = true;
                                    list.ForEach((e) => current.AddIfNotContains<T>(e));
                                    for (int i = current.Count - 1; i >= 0; i--)
                                    {
                                        T e = current[i];
                                        if (!list.Contains(e)) current.Remove(e);
                                    }
                                    current.CopiedFromMain = false;
                                }
                                entity.SetTableUidForProperty(name, relation.ChildTable.Uid);
                            }
                            else
                            {
                                invocation.Proceed();
                            }
                        }
                        else
                        {
                            throw new ModelException();
                        }
                    }
                    else
                    {
                        throw new ModelException();
                    }
                }
                else
                {
                    invocation.Proceed();
                }
            }
            else
            {
                throw new ModelException();
            }
        }
        #endregion Inetercept

        #region Initialize
        public override void Initialize(Type entityType, attribute.Interceptor interceptor)
        {
            string name = interceptor.MethodsName[0];
            if (name.StartsWith("get_"))
            {
                name = name.Substring(4, name.Length - 4);
            }
            PropertyInfo info = entityType.GetProperty(name);
            if (info.IsNotNull())
            {
                _discriminators.AddRange(Attribute.GetCustomAttributes(info).
                    Where(p => p.IsTypeOf<Discriminator>()).Select(p =>p.CastToType<Discriminator>()));
                _setter = LambdaTools.PropertySetter(entityType, info);
                _getter = LambdaTools.PropertyGetter(entityType, info);
            }
        }
        #endregion Initialize

        #region Properties
        [ValueFromAttribute("")]
        public string RelationName { get; set; }

        [ValueFromAttribute("")]
        public bool AutoRefresh { get; set; }
        #endregion Properies

        #region Dispose & Destructor
        public void Dispose()
        {
            _relation = null;
            _getter = null;
            _setter = null;
            _discriminators.Clear();
        }

        ~CollectionInterceptor()
        {
            Dispose();
        }
        #endregion Dispose & Destructor
    }
}
