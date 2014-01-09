using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MVCEngine;
using Castle.DynamicProxy;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal;
using MVCEngine.Model.Internal.Descriptions;
using System.Collections;
using MVCEngine.Model.Interface;
using MVCEngine.Model.Attributes.Discriminators;
using MVCEngine.Model.Interceptors;
using System.Diagnostics;

namespace MVCEngine.Model
{
    public class EntitiesCollection<T> : BindingList<T>, IDisposable, ITypedList, IEntityCollection where T : Entity
    {
        #region Members
        private static readonly Lazy<ProxyGenerator> _generator;
        [NonSerialized()]
        private PropertyDescriptorCollection _properties;
        #endregion Members

        #region Context
        public Context Context { get; set; }
        #endregion Context

        #region Child Collection
        internal ReletedEntity Releted { get; set; }
        internal object ParentValue { get; set; }
        internal bool CopyFromMainCollection { get; set; }
        #endregion Child Collection

        #region Entity Class Property
        private EntityClass _entityClass;
        private EntityClass EntityCtx
        {
            get
            {
                if (_entityClass.IsNull())
                {
                    _entityClass = Context.Entites.FirstOrDefault(t => t.Name == typeof(T).Name);
                }
                return _entityClass;
            }
        }
        #endregion Entity Class Property

        #region Constructors
        static EntitiesCollection()
        {
            _generator = new Lazy<ProxyGenerator>(() =>
            {
                return new ProxyGenerator(new PersistentProxyBuilder());
            });
        }

        public EntitiesCollection()
            : base()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(
                                                typeof(T),
                                                new Attribute[] { new BrowsableAttribute(true) });
            _properties = pdc.Sort();
        }

        internal EntitiesCollection(IList<T> list)
            : base(list)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(
                                                typeof(T),
                                                new Attribute[] { new BrowsableAttribute(true) });
            _properties = pdc.Sort();
        }
        #endregion Constructors

        #region New Method Implmentation
        new public bool AllowEdit 
        {
            get
            {
                return base.AllowEdit;
            }
            set
            {
                foreach (T obj in this)
                {
                    obj.IsFrozen = !value;
                }
                base.AllowEdit = value;
            }
        }
        #endregion New Method Implmentation

        #region Override
        protected override void InsertItem(int index, T item)
        {
            if (Releted.IsNull())
            {
                if (item.GetType() == typeof(T)
                    && !item.GetType().Assembly.FullName.Contains("DynamicProxy"))
                {
                    throw new ModelException("Please use CreateInstance function CreateInstance or AddNew to create proper object entity");
                }
                if (EntityCtx.IsNotNull())
                {
                    if (item.Session.IsNullOrEmpty())
                    {
                        item.Validate((v) => { return v.RealTimeValidation; },
                                       (v) => { throw new ValidationException(v.ErrrorMessage); });
                    }
                    if (EntityCtx.Entities.FirstOrDefault(e => e.Equals(item)).IsNotNull())
                    {
                        throw new ValidationException("You can't add twice the same object[" + typeof(T).Name + "] to collection");
                    }
                    EntityCtx.MarkedAsModified();
                }
                base.InsertItem(index, item);
                if (EntityCtx.Synchronizing)
                {
                    foreach (string entityName in EntityCtx.SynchronizedCollection.Keys)
                    {
                        EntityClass releted = Context.Entites.FirstOrDefault(e => e.Name == entityName);
                        Debug.Assert(releted.IsNotNull(), "Internal error");
                        if (releted.IsNotNull())
                        {
                            foreach (Entity e in releted.Entities)
                            {
                                foreach (string propertyName in EntityCtx.SynchronizedCollection[entityName])
                                {
                                    object o = e[propertyName];
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (!CopyFromMainCollection)
                {
                    if (EntityCtx.IsNotNull())
                    {
                        IList list = EntityCtx.Entities as IList;
                        if (list.IsNotNull())
                        {
                            EntityCtx.Synchronizing = false;
                            list.Add(item);
                            EntityCtx.Synchronizing = true;
                        }
                    }
                }
                base.InsertItem(index, item);
            }
        }

        protected override object AddNewCore()
        {            
            if (AllowNew)
            {
                T obj = CreateInstance();
                base.Add(obj);
                return obj;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected override void RemoveItem(int index)
        {
            if (Releted.IsNull())
            {
                Entity obj = null;
                if (AllowRemove && index < Count)
                {
                    obj = base[index];
                    if (obj.IsFrozen) throw new SecurityException("Security Exception. You try to modified object for which you don't have an accesss");
                    switch (obj.State)
                    {
                        case EntityState.Modified:
                        case EntityState.Unchanged: obj.State = EntityState.Deleted;
                            break;
                        case EntityState.Added: obj.State = EntityState.Deleted;
                            base.RemoveItem(index);
                            break;
                        case EntityState.Deleted: base.RemoveItem(index);
                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
                if (EntityCtx.IsNotNull())
                {
                    EntityCtx.MarkedAsModified();
                    foreach (string entityName in EntityCtx.SynchronizedCollection.Keys)
                    {
                        EntityClass releted = Context.Entites.FirstOrDefault(e => e.Name == entityName);
                        Debug.Assert(releted.IsNotNull(), "Internal error");
                        if (releted.IsNotNull())
                        {
                            foreach (Entity e in releted.Entities)
                            {
                                foreach (string propertyName in EntityCtx.SynchronizedCollection[entityName])
                                {
                                    object o = e[propertyName];
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (!CopyFromMainCollection)
                {
                    if (EntityCtx.IsNotNull())
                    {
                        IList list = EntityCtx.Entities as IList;
                        if (list.IsNotNull())
                        {
                            if (index < Count)
                            {
                                Entity e = base[index];
                                if (list.Contains(e))
                                {
                                    list.Remove(e);
                                }
                            }
                        }
                    }
                }
                base.RemoveItem(index);
            }
        }
        #endregion Override

        #region Create Object
        public T CreateInstance()
        {
            return CreateInstance(typeof(T), true, null) as T;
        }       

        public object CreateInstance(Type type, bool defaultValue, object[] constructorArguments)
        {
            var options = new ProxyGenerationOptions(new EntityInterceptorGenerationHook()) { Selector = new EntityInterceptorSelector() };
            object proxy = null;
            if (constructorArguments.IsNotNull())
            {
                proxy = _generator.Value.CreateClassProxy(type, options, constructorArguments, EntitiesContext.GetInterceptors(type));
            }
            else
            {
                proxy = _generator.Value.CreateClassProxy(type, options, EntitiesContext.GetInterceptors(type));
            }            
            if (proxy.IsNotNull())
            {
                Entity entity = proxy.CastToType<Entity>();
                if (entity.IsNotNull())
                {
                    entity.Context = Context;

                    if (Context.EntityCreated.IsNotNull())
                    {
                        Context.EntityCreated(entity);
                    }
                    if (defaultValue)
                    {
                        entity.Default();
                    }
                    if (Releted.IsNotNull())
                    {
                        entity[Releted.Relation.Child.Key] = ParentValue;
                        if (Releted.Discriminators.IsNotNull())
                        {
                            Releted.Discriminators.ForEach((d) => d.Default(entity));
                        }
                    }
                }
            }
            return proxy;
        }
        #endregion Create Object

        #region  AcceptChanges
        public void AcceptChanges()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                this[i].AcceptChanges();
            }
        }
        #endregion AcceptChanges

        #region Dispose
        public void Dispose()
        {
            Releted = null;
            Context = null;
        }

        ~EntitiesCollection()
        {
            Dispose();
        }
        #endregion Dispose

        #region ITypedList
        public void AddPropertyDescriptor(string name, Type propertyType)
        {
            if (_properties.Find(name, true).IsNull())
            {
                _properties.Add(new EntityPropertyDescriptor(name, EntityCtx.EntityType, propertyType));
            }
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return _properties;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return "EntitiesCollection["+typeof(T).Name+"]";
        }
        #endregion ITypedList
    }
}
