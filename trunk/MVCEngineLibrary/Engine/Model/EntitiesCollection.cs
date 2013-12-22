using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MVCEngine;
using Castle.DynamicProxy;
using MVCEngine.Model.Interceptors;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal;
using MVCEngine.Model.Internal.Descriptions;
using MVCEngine.Model.Interceptors.Exceptions;
using System.Collections;
using MVCEngine.Model.Interface;
using MVCEngine.Model.Attributes.Discriminators;

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
        internal bool ChildCollection { get; set; }
        internal string ChildColumn { get; set; }
        internal object ParentValue { get; set; }
        internal bool CopiedFromMain { get; set; }
        internal List<Discriminator> Discriminators { get; set; }
        #endregion Child Collection

        #region Table Property
        private Table _table;
        private Table Table
        {
            get
            {
                if(_table.IsNull())
                {
                    _table = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                }
                return _table;
            }
        }
        #endregion Table Property

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
            if (!ChildCollection)
            {
                if (item.GetType() == typeof(T)
                    && !item.GetType().Assembly.FullName.Contains("DynamicProxy"))
                {
                    throw new ModelException("Please use CreateInstance function CreateInstance or AddNew to create proper object entity");
                }
                if (Table.IsNotNull())
                {
                    if (item.Session.IsNullOrEmpty())
                    {
                        item.Validate((v) => { return v.RealTimeValidation; },
                                       (v) => { throw new ValidationException(v.ErrrorMessage); });
                    }
                    if (Table.Entities.FirstOrDefault(e => e.Equals(item)).IsNotNull())
                    {
                        throw new ValidationException("You can't add twice the same object[" + typeof(T).Name + "] to collection");
                    }
                    Table.MarkedAsModified();
                }
                base.InsertItem(index, item);
                foreach (Entity e in Table.Triggers.Keys)
                {
                    Table.Triggers[e].ForEach((t) => { t(e); });
                }
            }
            else
            {
                if (!CopiedFromMain)
                {
                    Table table = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                    if (table.IsNotNull())
                    {
                        IList list = table.Entities as IList;
                        if (list.IsNotNull())
                        {
                            list.Add(item);
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
            if (!ChildCollection)
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
                    base.RemoveItem(index);
                }
                if (obj.IsNotNull())
                {
                    Context.Tables.ForEach((t) =>
                    {
                        if (t.Triggers.ContainsKey(obj))
                        {
                            t.Triggers[obj].Clear();
                            t.Triggers.Remove(obj);
                        }
                    });
                }
                if (Table.IsNotNull())
                {
                    Table.MarkedAsModified();
                    foreach (Entity e in Table.Triggers.Keys)
                    {
                        Table.Triggers[e].ForEach((t) => { t(e); });
                    }
                }
            }
            else
            {
                if (!CopiedFromMain)
                {
                    if (Table.IsNotNull())
                    {
                        IList list = Table.Entities as IList;
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
            return CreateInstance(typeof(T), true) as T;
        }

        public object CreateInstance(Type type, bool defaultValue)
        {            
            var options = new ProxyGenerationOptions(new EntityInterceptorGenerationHook()) { Selector = new EntityInterceptorSelector() };
            var proxy = _generator.Value.CreateClassProxy(type, options, InterceptorDispatcher.GetInstnace().GetInterceptorsObject(type).ToArray());            
            Entity entity = proxy.CastToType<Entity>();
            if (entity.IsNotNull())
            {                               
                entity.Context = Context;
                entity.State = EntityState.Added;
                if (defaultValue)
                {
                    entity.Default();
                }
                if (ChildCollection)
                {
                    entity[ChildColumn] = ParentValue;
                    if (Discriminators.IsNotNull())
                    {
                        Discriminators.ForEach((d) => d.Default(entity));
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
            Context = null;
        }

        ~EntitiesCollection()
        {
            Dispose();
        }
        #endregion Dispose

        #region ITypedList
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
