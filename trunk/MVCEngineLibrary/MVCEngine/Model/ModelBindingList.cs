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
        private object _lockThread = new object();
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
                lock (_lockThread)
                {
                    foreach (T obj in this)
                    {
                        obj.IsFrozen = !value;
                    }
                    base.AllowEdit = value;
                }
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
                table.Validators.Where(v => v.RealTimeValidation).ToList().
                    ForEach((v) =>
                {
                    if (!v.Validate(item))
                    {
                        throw new ValidationException(v.ErrrorMessage);
                    }                    
                });
                table.Columns.SelectMany(c => c.Validators.Where(v => v.RealTimeValidation),
                    (c, v) => new { Column = c, Validator = v }).ToList().
                    ForEach((cv) =>
                {
                    if(!cv.Validator.Validate(item[cv.Column.Name]))
                    {
                        throw new ValidationException(cv.Validator.ErrrorMessage);
                    }
                });
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
            lock (_lockThread)
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
        }

        protected override void RemoveItem(int index)
        {
            lock (_lockThread)
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
                Table table = Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                if (table.IsNotNull())
                {
                    table.MarkedAsModified();
                }
            }
        }
        #endregion Override

        #region Create Object
        public T CreateInstance()
        {            
            var options = new ProxyGenerationOptions(new InterceptorGenerationHook()) { Selector = new InterceptorSelector() };
            var proxy = _generator.Value.CreateClassProxy(typeof(T), options, InterceptorDispatcher.GetInstnace().GetInterceptorsObject(typeof(T)).ToArray());
            if (proxy.IsTypeOf<Entity>())
            {
                Entity entity = proxy.CastToType<Entity>();
                entity.Context = this.Context;
                entity.State = EntityState.Added;
                if (entity.Table.IsNotNull())
                {
                    entity.Table.Columns.Where(c => c.DefaultValue.IsNotNull()).ToList().ForEach((c) =>
                    {
                        if (entity[c.Name].IsNull() || entity[c.Name].Equals(c.ColumnType.GetDefaultValue()))
                        {
                            entity[c.Name] = c.DefaultValue.Value(entity, c);
                        }
                    });
                }
            }
            return proxy as T;
        }
        #endregion Create Object

        #region  AcceptChanges
        public void AcceptChanges()
        {
            lock (_lockThread)
            {
                foreach (T obj in this)
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
