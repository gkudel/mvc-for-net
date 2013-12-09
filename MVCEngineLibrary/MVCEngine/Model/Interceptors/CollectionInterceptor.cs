using Castle.Core.Interceptor;
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
    internal class CollectionInterceptor<T> : Interceptor where T : Entity
    {
        #region Members
        private List<T> _list;
        private string _uid;
        private List<Discriminator> _discriminators;
        #endregion Members

        #region Constructor
        public CollectionInterceptor()
        {
            _uid = string.Empty;
            _list = new List<T>();
            _discriminators = new List<Discriminator>();
        }
        #endregion Constructor

        #region Inetercept
        public override void Intercept(IInvocation invocation)
        {                                  
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (entity.IsNotNull())
            {
                Table childTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                if (entity.Table.IsNotNull() && childTable.IsNotNull())
                {
                    if (childTable.Uid != _uid)
                    {
                        List<Relation> relations = entity.Context.Relations.Where(r => r.ParentTable == entity.Table.TableName
                                                                                  && r.ChildTable == childTable.TableName
                                                                                  && (RelationName.IsNullOrEmpty() || RelationName.Equals(r.Name))).ToList();

                        if (relations.Count() == 1)
                        {
                            Relation relation = relations.First();
                            if (relation.IsNotNull())
                            {
                                _list = childTable.Entities.Cast<T>().Where(c => c.State != EntityState.Deleted && relation.ParentValue(invocation.InvocationTarget).
                                            Equals(relation.ChildValue(c)) &&
                                            _discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(c); }))).ToList();
                            }
                        }
                        else if(relations.Count() > 1)
                        {
                            throw new ModelException();
                        }
                        _uid = childTable.Uid;
                    }
                }
            }
            invocation.ReturnValue = _list;
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
