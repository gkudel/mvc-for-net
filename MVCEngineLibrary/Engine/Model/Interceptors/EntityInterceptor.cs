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

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class EntityInterceptor<T> : MVCEngine.Model.Internal.Interceptor where T : Entity
    {
        #region Members
        private List<Discriminator> _discriminators;
        private Lazy<Relation> _relation;
        #endregion Members

        #region Constructor
        public EntityInterceptor()
        {
            _discriminators = new List<Discriminator>();
            _relation = new Lazy<Relation>();

        }
        #endregion Constructor

        #region Inetercept
        public override void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("set_")) throw new InvalidOperationException();
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (entity.IsNotNull())
            {
                if (!_relation.IsValueCreated)
                {
                    Table parentTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                    if (parentTable.IsNotNull() && entity.Table.IsNotNull())
                    {
                        List<Relation> relations = entity.Context.Relations.Where(r => r.ParentTable.TableName == parentTable.TableName
                                                              && r.ChildTable.TableName == entity.Table.TableName
                                                              && (RelationName.IsNullOrEmpty() || RelationName.Equals(r.Name))).ToList();
                        if (relations.Count() == 1)
                        {
                            _relation = new Lazy<Relation>(() => { return relations[0]; });
                        }
                        else
                        {
                            _relation = new Lazy<Relation>(() => { return null; });
                        }
                    }
                }
                if (_relation.Value.IsNotNull())
                {
                    List<Entity> list = _relation.Value.ParentTable.Entities.Where(p => p.State != EntityState.Deleted && _relation.Value.ParentValue(p).
                            Equals(_relation.Value.ChildValue(invocation.InvocationTarget)) &&
                            _discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(p); }))).ToList();

                    if (list.Count() == 1)
                    {
                        invocation.ReturnValue = list[0].CastToType<T>();
                    }
                    else
                    {
                        invocation.ReturnValue = default(T);
                    }
                }
                else
                {
                    throw new ModelException();
                }
            }
            else
            {
                invocation.ReturnValue = default(T);
            }

        }
        #endregion Inetercept

        #region Initialize
        public override void Initialize(Type entityType, attribute.Interceptor interceptor)
        {
            if (interceptor.MethodsName.Length == 1)
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
                }
            }
        }
        #endregion Initialize

        #region Properties
        [ValueFromAttribute("")]
        public string RelationName { get; set; }
        #endregion Properies
    }
}
