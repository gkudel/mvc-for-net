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

namespace MVCEngine.Model
{
    public class ModelBindingList<T> : BindingList<T>, IDisposable where T : Entity
    {
        #region Members
        private static readonly Lazy<ProxyGenerator> _generator;
        #endregion Members

        #region Context
        public Context Context { get; set; }
        #endregion Context

        #region Constructors
        static ModelBindingList()
        {
            _generator = new Lazy<ProxyGenerator>(() =>
            {
                return new ProxyGenerator(new PersistentProxyBuilder());
            });
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
            if (item.GetType() == typeof(T) 
                && !item.GetType().Assembly.FullName.Contains("DynamicProxy"))
            {
                throw new ModelException("Please use CreateInstance function CreateInstance or AddNew to create proper object entity");
            }
            Table table = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
            if (table.IsNotNull())
            {
                if (item.Session.IsNullOrEmpty())
                {
                    item.Validate((v) => { return v.RealTimeValidation; },
                                   (v) => { throw new ValidationException(v.ErrrorMessage); });
                }
                if (table.Entities.FirstOrDefault(e => e.Equals(item)).IsNotNull())
                {
                    throw new ValidationException("You can't add twice the same object["+typeof(T).Name+"] to collection");
                }
                table.MarkedAsModified();
            }            
            base.InsertItem(index, item);
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
            if (AllowRemove && index < Count)
            {
                Entity obj = base[index];
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
            Table table = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
            if (table.IsNotNull())
            {
                table.MarkedAsModified();
            }
        }
        #endregion Override

        #region Create Object
        public T CreateInstance()
        {
            return CreateInstance(typeof(T), this.Context) as T;
        }

        public static object CreateInstance(Type type, Context ctx)
        {
            return CreateInstance(type, ctx, true);
        }

        internal static object CreateInstance(Type type, Context ctx, bool defaultValue)
        {            
            var options = new ProxyGenerationOptions(new ModelInterceptorGenerationHook()) { Selector = new ModelInterceptorSelector() };
            var proxy = _generator.Value.CreateClassProxy(type, options, InterceptorDispatcher.GetInstnace().GetInterceptorsObject(type).ToArray());
            if (proxy.IsTypeOf<Entity>())
            {
                Entity entity = proxy.CastToType<Entity>();
                entity.Context = ctx;
                entity.State = EntityState.Added;
                if (defaultValue)
                {
                    entity.Default();
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

        ~ModelBindingList()
        {
            Dispose();
        }
        #endregion Dispose
    }
}
