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

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class EntityInterceptor<T> : MVCEngine.Model.Internal.Interceptor, IDisposable where T : Entity
    {
        #region Members
        private List<Discriminator> _discriminators;
        private Action<object, object> _setter;
        private Func<object, object> _getter;
        private Relation _relation;
        #endregion Members

        #region Constructor
        public EntityInterceptor()
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
                        Table parentTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                        if (parentTable.IsNotNull() && entity.Table.IsNotNull())
                        {
                            List<Relation> relations = EntitiesContext.GetContext(entity.Context.EntitiesContextType).Relations.Where(r => r.ParentTableName == parentTable.TableName
                                                                    && r.ChildTableName == entity.Table.TableName
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
                            if (!entity.GetTableUidForProperty(name).Equals(relation.ParentTable.Uid))
                            {
                                List<Entity> list = relation.ParentTable.Entities.Where(p => p.State != EntityState.Deleted && relation.ParentValue(p).
                                        Equals(relation.ChildValue(invocation.InvocationTarget)) &&
                                        _discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(p); }))).ToList();

                                if (list.Count() == 1)
                                {
                                    invocation.ReturnValue = list[0].CastToType<T>();
                                    if (_setter.IsNotNull())
                                    {
                                        _setter(entity, invocation.ReturnValue);
                                    }
                                    entity.SetTableUidForProperty(name, relation.ParentTable.Uid);
                                }
                                else if (list.Count() > 1)
                                {
                                    throw new ModelException();
                                }
                                else
                                {
                                    invocation.ReturnValue = default(T);
                                    if (_setter.IsNotNull())
                                    {
                                        _setter(entity, invocation.ReturnValue);
                                    }
                                    entity.SetTableUidForProperty(name, relation.ParentTable.Uid);
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
                    Where(p => p.IsTypeOf<Discriminator>()).Select(p => p.CastToType<Discriminator>()));
                _setter = LambdaTools.PropertySetter(entityType, info);
                _getter = LambdaTools.PropertyGetter(entityType, info);
            }
        }
        #endregion Initialize

        #region Properties
        [ValueFromAttribute("")]
        public string RelationName { get; set; }
        #endregion Properies

        #region Dispose & Destructor
        public void Dispose()
        {
            _relation = null;
            _getter = null;
            _setter = null;
            _discriminators.Clear();
        }

        ~EntityInterceptor()
        {
            Dispose();
        }
        #endregion Dispose & Destructor
    }
}
