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
        new public void Add(T item)
        {
            throw new NotSupportedException();
        }

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
                    case EntityState.Added: base.RemoveItem(index);
                                            break;
                    case EntityState.Deleted: throw new InvalidOperationException();
                }
            }
            else
            {
                base.RemoveItem(index);
            }
        }
        #endregion Override

        #region Create Object
        private T CreateInstance()
        {            
            var options = new ProxyGenerationOptions(new InterceptorGenerationHook()) { Selector = new InterceptorSelector() };
            var proxy = _generator.Value.CreateClassProxy(typeof(T), options, InterceptorDispatcher.GetInstnace().GetInterceptorsObject(typeof(T)).ToArray());
            if (proxy.IsTypeOf<Entity>())
            {
                Entity entity = proxy.CastToType<Entity>();
                entity.Context = this.Context;
                entity.State = EntityState.Added;
            }
            return proxy as T;
        }
        #endregion Create Object

        #region  AcceptChanges
        public void AcceptChanges()
        {
            foreach(T obj in this)
            {
                obj.AcceptChanges();
            }
            for (int i = Count - 1; i >= 0; i--)
            {
                if (base[i].State == EntityState.Deleted)
                {
                    base[i].State = EntityState.Added;
                    RemoveAt(i);
                }
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
