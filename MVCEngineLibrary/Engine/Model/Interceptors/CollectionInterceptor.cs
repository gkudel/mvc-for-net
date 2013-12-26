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
using System.ComponentModel;
using MVCEngine;
using System.Diagnostics;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    internal class CollectionInterceptor<T> : Interceptor where T : Entity
    {
        #region Constructor
        public CollectionInterceptor()
        {
        }
        #endregion Constructor

        #region Inetercept
        public override string GetId()
        {
            return "CollectionInterceptor["+typeof(T).Name+"]";
        }

        public override void Intercept(IInvocation invocation)
        {
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            Debug.Assert(entity.IsNotNull(), "CollectionInterceptor error");
            if (!entity.Disposing)
            {
                string propertyName = invocation.Method.Name;
                if (propertyName.StartsWith("get_"))
                {
                    propertyName = propertyName.Substring(4, propertyName.Length - 4);
                }
                EntityProperty property = entity.EntityCtx.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (property.IsNotNull() && property.ReletedEntity.IsNotNull()
                    && property.ReletedEntity.Related == Releted.List && property.ReletedEntity.Relation.IsNotNull())
                {
                    EntitiesRelation relation = property.ReletedEntity.Relation;
                    if (!relation.ChildEntity.Uid.Equals(entity.GetUid(propertyName)))
                    {
                        invocation.Proceed();

                        List<T> list = relation.ChildEntity.Entities.Where(c => c.State != EntityState.Deleted && relation.ParentValue(entity).
                                    Equals(relation.ChildValue(c)) &&
                                    property.ReletedEntity.Discriminators.TrueForAll(new Predicate<Discriminator>((d) => { return d.Discriminate(c); }))).Cast<T>().ToList();

                        if (invocation.ReturnValue.IsNull())
                        {
                            EntitiesCollection<T> collection = new EntitiesCollection<T>(list);
                            collection.Context = entity.Context;
                            collection.Releted = property.ReletedEntity;
                            collection.ParentValue = relation.ParentValue(entity);
                            invocation.ReturnValue = collection;
                            if (property.Setter.IsNotNull()) entity[propertyName] = collection;
                        }
                        else
                        {
                            EntitiesCollection<T> current = invocation.ReturnValue as EntitiesCollection<T>;
                            current.CopyFromMainCollection = true;
                            list.ForEach((e) => current.AddIfNotContains<T>(e));
                            for (int i = current.Count - 1; i >= 0; i--)
                            {
                                T e = current[i];
                                if (!list.Contains(e)) current.Remove(e);
                            }
                            current.CopyFromMainCollection = false;
                        }
                        entity.SetUid(propertyName, relation.ChildEntity.Uid);
                    }
                    else
                    {
                        invocation.Proceed();
                    }
                }
                else
                {
                    Debug.Assert(false, "CollectionInterceptor error");
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
        #endregion Inetercept
    }

    internal static class CollectionInterceptorDispatcher
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
                Type generic = typeof(CollectionInterceptor<>).MakeGenericType(reletedType);
                _instances.Add(reletedType, Activator.CreateInstance(generic) as IInterceptor);
            }
            return _instances[reletedType] as IInterceptor;
        }
        #endregion GetInstance

        #region GetId
        internal static string GetId(Type reletedType)
        {
            return "CollectionInterceptor[" + reletedType.Name + "]";
        }
        #endregion GetId
    }
}
