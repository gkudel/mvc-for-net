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
    internal class CollectionInterceptor<T> : MVCEngine.Model.Internal.Interceptor where T : Entity
    {
        #region Members
        private List<Discriminator> _discriminators;        
        private Action<object, object> _setter;
        private Func<object, object> _getter;
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
                string name = invocation.Method.Name;
                if (name.StartsWith("get_"))
                {
                    name = name.Substring(4, name.Length - 4);
                }
                Relation relation = null;
                Table childTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                if (entity.Table.IsNotNull() && childTable.IsNotNull())
                {
                    List<Relation> relations = entity.Context.Relations.Where(r => r.ParentTable.TableName == entity.Table.TableName
                                                            && r.ChildTable.TableName == childTable.TableName
                                                            && (RelationName.IsNullOrEmpty() || RelationName.Equals(r.Name))).ToList();
                    if (relations.Count() == 1)
                    {
                        relation = relations[0];
                    }
                    else
                    {
                        relation = null;
                    }
                }
                if (relation.IsNotNull())
                {
                    if (!entity.GetTableUidForProperty(name).Equals(relation.ChildTable.Uid))
                    {
                        invocation.ReturnValue = relation.ChildTable.Entities.Where(c => c.State != EntityState.Deleted && relation.ParentValue(invocation.InvocationTarget).
                                    Equals(relation.ChildValue(c)) &&
                                    _discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(c); }))).Cast<T>().ToList();
                        if (_setter.IsNotNull())
                        {
                            _setter(entity, invocation.ReturnValue);
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
        #endregion Properies
    }
}
