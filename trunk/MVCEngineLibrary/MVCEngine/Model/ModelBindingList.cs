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

namespace MVCEngine.Model
{
    public class ModelBindingList<T> : BindingList<T> where T : ModelObject
    {
        #region Members
        private static readonly Lazy<ProxyGenerator> _generator;
        #endregion Members

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
                ModelObject obj = base[index];
                switch (obj.State)
                {
                    case ObjectState.Modified: 
                    case ObjectState.Unchanged: obj.State = ObjectState.Deleted;
                                                break;
                    case ObjectState.Added: base.RemoveItem(index);
                                            break;
                    case ObjectState.Deleted: throw new InvalidOperationException();
                }
            }
            else
            {
                base.RemoveItem(index);
            }
        }
        #endregion Override

        #region Create Object
        private static T CreateInstance()
        {            
            var options = new ProxyGenerationOptions(new InterceptorGenerationHook()) { Selector = new InterceptorSelector() };
            var proxy = _generator.Value.CreateClassProxy(typeof(T), options, InterceptorDispatcher.GetInstnace().GetInterceptorsObject(typeof(T)).ToArray());
            if (proxy.IsTypeOf<ModelObject>())
            {
                proxy.CastToType<ModelObject>().State = ObjectState.Added;
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
                if (base[i].State == ObjectState.Deleted)
                {
                    base[i].State = ObjectState.Added;
                    RemoveAt(i);
                }
            }
        }
        #endregion AcceptChanges
    }
}
