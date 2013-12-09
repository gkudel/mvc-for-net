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
    public class EntityInterceptor<T> : Interceptor where T : Entity
    {
        #region Members
        private T _entity;
        private string _uid;
        private List<Discriminator> _discriminators;
        #endregion Members

        #region Constructor
        public EntityInterceptor()
        {
            _uid = string.Empty;
            _entity = default(T);
            _discriminators = new List<Discriminator>();
        }
        #endregion Constructor

        #region Inetercept
        public override void Intercept(IInvocation invocation)
        {                                  
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (entity.IsNotNull())
            {
                Table parentTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                if (parentTable.IsNotNull() && entity.Table.IsNotNull())
                {
                    if (parentTable.Uid != _uid)
                    {
                        List<Relation> relations = entity.Context.Relations.Where(r => r.ParentTable == parentTable.TableName
                                                                                  && r.ChildTable == entity.Table.TableName
                                                                                  && (RelationName.IsNullOrEmpty() || RelationName.Equals(r.Name))).ToList();
                        if (relations.Count() == 1)
                        {
                            Relation relation = relations.First();
                            if (relation.IsNotNull())
                            {
                                List<T> list = parentTable.Entities.Cast<T>().Where(p => p.State != EntityState.Deleted && relation.ParentValue(p).
                                        Equals(relation.ChildValue(invocation.InvocationTarget)) &&
                                        _discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(p); }))).ToList();
                                if (list.Count() == 1)
                                {
                                    _entity = list.First();
                                }
                                else if (list.Count() > 1)
                                {
                                    throw new ModelException("Integrity constraint exception");
                                }
                                else
                                {
                                    _entity = default(T);
                                }
                            }
                        }
                        else if (relations.Count() > 1)
                        {
                            throw new ModelException();
                        }
                        _uid = parentTable.Uid;
                    }
                }
            }
            invocation.ReturnValue = _entity;
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
