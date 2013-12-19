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
    internal class CollectionInterceptor<T> : MVCEngine.Model.Internal.Interceptor where T : Entity
    {
        #region Members
        private List<Discriminator> _discriminators;
        private Lazy<Relation> _relation;
        #endregion Members

        #region Constructor
        public CollectionInterceptor()
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
                    Table childTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                    if (entity.Table.IsNotNull() && childTable.IsNotNull())
                    {
                        List<Relation> relations = entity.Context.Relations.Where(r => r.ParentTable.TableName == entity.Table.TableName
                                                              && r.ChildTable.TableName == childTable.TableName
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
                    invocation.ReturnValue = _relation.Value.ChildTable.Entities.Where(c => c.State != EntityState.Deleted && _relation.Value.ParentValue(invocation.InvocationTarget).
                                Equals(_relation.Value.ChildValue(c)) &&
                                _discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(c); }))).Cast<T>().ToList();
                }
                else
                {
                    throw new ModelException();
                }
            }
            else
            {
                invocation.ReturnValue = null;
            }
        }
        #endregion Inetercept

        #region Initialize
        public override void Initialize(Type entityType, attribute.Interceptor interceptor)
        {
            if(interceptor.MethodsName.Length == 1)
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
