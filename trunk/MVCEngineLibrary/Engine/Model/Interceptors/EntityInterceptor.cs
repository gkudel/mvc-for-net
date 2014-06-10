using Castle.DynamicProxy;
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
using MVCEngine;
using System.Diagnostics;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class EntityInterceptor<T> : Interceptor where T : Entity
    {
        #region Constructor
        public EntityInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public string GetId()
        {
            return "EntityInterceptor[" + typeof(T).Name + "]";
        }

        public void Intercept(IInvocation invocation)
        {
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            Debug.Assert(entity.IsNotNull(), "EntityInterceptor error");
            if (!entity.Disposing)
            {
                string propertyName = invocation.Method.Name;
                if (propertyName.StartsWith("get_"))
                {
                    propertyName = propertyName.Substring(4, propertyName.Length - 4);
                }
                EntityProperty property = entity.EntityCtx.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (property.IsNotNull() && property.ReletedEntity.IsNotNull()
                    && property.ReletedEntity.Related == Releted.Entity && property.ReletedEntity.Relation.IsNotNull())
                {
                    EntitiesRelation relation = property.ReletedEntity.Relation;
                    EntityRelated relatedEntity = relation.Parent;
                    EntityRelated currentEntity = relation.Child;
                    Debug.Assert(relatedEntity.IsNotNull() || currentEntity.IsNotNull(), "EntityInterceptor error");
                    if (!relatedEntity.Entity.Uid.Equals(entity.GetUid(propertyName)))
                    {
                        List<T> list = relatedEntity.Entity.Entities.WhereEntity(p => relatedEntity.Value(p).
                                    Equals(currentEntity.Value(entity)) &&
                                    property.ReletedEntity.Discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(p); }))).Cast<T>().ToList();

                        if (list.Count() == 1)
                        {
                            invocation.ReturnValue = list[0].CastToType<T>();
                            if (property.Setter.IsNotNull()) entity[propertyName] = invocation.ReturnValue;
                            entity.SetUid(propertyName, relatedEntity.Entity.Uid);
                        }
                        else if (list.Count() > 1)
                        {
                            throw new ModelException();
                        }
                        else
                        {
                            invocation.ReturnValue = default(T);
                            if (property.Setter.IsNotNull()) entity[propertyName] = invocation.ReturnValue;
                            entity.SetUid(propertyName, relatedEntity.Entity.Uid);
                        }
                    }
                    else
                    {
                        invocation.Proceed();
                    }
                }
                else
                {
                    Debug.Assert(false, "EntityInterceptor error");
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
        #endregion Inetercept
    }

    internal static class EntityInterceptorDispatcher
    {
        #region Members
        private static Dictionary<Type, IInterceptor> _instances;
        #endregion Members

        #region GetInstance
        internal static IInterceptor GetInstance(Type reletedType)
        {
            if (_instances.IsNull()) _instances = new Dictionary<Type, IInterceptor>();
            if (!_instances.ContainsKey(reletedType))
            {
                Type generic = typeof(EntityInterceptor<>).MakeGenericType(reletedType);
                _instances.Add(reletedType, Activator.CreateInstance(generic) as IInterceptor);
            }
            return _instances[reletedType] as IInterceptor;
        }
        #endregion GetInstance

        #region GetId
        internal static string GetId(Type reletedType)
        {
            return "EntityInterceptor[" + reletedType.Name + "]";
        }

        internal static string GetId(string reletedTypeName)
        {
            return "EntityInterceptor[" + reletedTypeName + "]";
        }
        #endregion GetId
    }
}
