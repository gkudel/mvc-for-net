using MVCEngine.Model.Attributes.Validation;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Interface;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MVCEngine;
using MVCEngine.Internal.Validation;

namespace MVCEngine.Model
{
    public enum EntityState { Added, Modified, Deleted, Unchanged };

    public abstract class Entity : INotifyPropertyChanged,  IDisposable
    {
        #region Members
        private bool _isFrozen = false;
        private EntityState _entityState = EntityState.Added;
        private Lazy<bool> _isvalid;
        private EntityClass _Entity;
        private Dictionary<string, string> _uids;
        #endregion Members

        #region Constructor
        public Entity()
        {
            _isvalid = new Lazy<bool>(() =>
            {
                return Validate();
            }, true);
            _uids = new Dictionary<string, string>();
        }
        #endregion Constructor

        #region IsFrozen
        public bool IsFrozen 
        {
            get 
            { 
                return _isFrozen; 
            }
            internal set
            {
                _isFrozen = value;
                EnumerateByChildren((e, r) => { e.IsFrozen = value; });
            }
        }
        #endregion IsFroze

        #region Session
        internal string Session { get; set; }
        #endregion Session

        #region Object State
        public void Modified()
        {
            if (_isvalid.IsValueCreated)
            {
                _isvalid = new Lazy<bool>(() =>
                {
                    return Validate();
                }, true);
            }
        }

        public EntityState State
        {
            get 
            { 
                return _entityState; 
            }
            internal set
            {                
                if (value == EntityState.Deleted) EnumerateByChildren((e, r) => 
                {
                    if (r.OnDelete == OnDelete.Cascade)
                    {
                        IList iList = e.EntityCtx.Entities as IList;
                        if (iList.IsNotNull())
                        {
                            iList.Remove(e);
                        }
                    }
                    else if (r.OnDelete == OnDelete.SetNull)
                    {
                        e[r.ChildKey] = r.ChildType.GetDefaultValue();
                    }
                });
                _entityState = value;
                if (_entityState != EntityState.Unchanged) Context.IsModified = true;
                if (value == EntityState.Deleted)
                {
                    EntityCtx.MarkedAsModified();
                }
            }
        }

        public void AcceptChanges()
        {
            switch (State)
            {
                case EntityState.Modified:
                case EntityState.Added: State = EntityState.Unchanged;
                    break;
                case EntityState.Deleted: IList iList = EntityCtx.Entities as IList;
                    if (iList.IsNotNull())
                    {
                        iList.Remove(this);
                    }
                    break;
            }
            EntityCtx.MarkedAsModified();
        }
        #endregion Object State

        #region Enumerate by Children
        public void EnumerateByChildren(Action<Entity,EntitiesRelation> action)
        {
            if (EntityCtx.IsNotNull())
            {
                Context.Relations.Where(r => r.ParentEntityName == EntityCtx.Name).ToList().ForEach((r) =>
                {
                    EntityClass childEntity = Context.Entites.FirstOrDefault(t => t.Name == r.ChildEntityName);
                    if (childEntity.IsNotNull())
                    {
                        childEntity.Entities.Where(row => row.State != EntityState.Deleted &&
                        r.ChildValue(row).Equals(r.ParentValue(this))).ToList().ForEach((row) =>
                        {
                            action(row, r);
                        });
                    }
                });
            }
        }
        #endregion Enumerate by Children

        #region INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    
        public void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
        #endregion INotifyPropertyChanged

        #region Context
        internal Context Context { get; set; }
        #endregion Context

        #region Entity
        public EntityClass EntityCtx
        {
            get
            {
                if (_Entity.IsNull())
                {
                    _Entity = Context.Entites.FirstOrDefault(t => t.Name == GetType().Name);
                    if (_Entity.IsNull() && GetType().BaseType.IsNotNull())
                    {
                        _Entity = Context.Entites.FirstOrDefault(t => t.Name == GetType().BaseType.Name);
                    } 
                }
                return _Entity;
            }
        }
        #endregion Entity

        #region Get & Set Coulumn Value
        public object this[string property]
        {
            get 
            {
                if (EntityCtx.IsNotNull())
                {
                    EntityProperty c = EntityCtx.Properties.FirstOrDefault(prop => prop.Name == property);
                    if (c.IsNotNull())
                    {
                        return c.Getter(this);
                    }
                    return null;
                }
                return null;
            }
            set 
            {
                if (EntityCtx.IsNotNull())
                {
                    EntityProperty c = EntityCtx.Properties.FirstOrDefault(prop => prop.Name == property);
                    if (c.IsNotNull() && c.Setter.IsNotNull())
                    {
                        c.Setter(this, value);
                    }
                    else
                    {
                        throw new ModelException("Type[" + GetType().Name + "] doesn't have column[" + property + "]");
                    }
                }
            }
        }
        #endregion Get & Set Coulumn Value

        #region Get & Set Entity Uid for Property
        internal string GetUid(string propertyName)
        {
            if (_uids.ContainsKey(propertyName))
            {
                return _uids[propertyName];
            }
            return string.Empty;
        }

        internal void SetUid(string propertyName, string uid)
        {
            if (_uids.ContainsKey(propertyName))
            {
                _uids[propertyName] = uid;
            }
            else
            {
                _uids.Add(propertyName, uid);
            }
        }
        #endregion Get & Set Entity Uid for Property

        #region Validate
        public bool Validate()
        {
            return Validate((v) => { return true; }, (v) => { });
        }

        public bool Validate(Func<Validator, bool> predict, Action<Validator> validationFaild)
        {
            ArgumentValidator.GetInstnace().
                IsNotNull(predict, "predict").
                IsNotNull(validationFaild, "validationFaild");

            bool ret = true;
            if (EntityCtx.IsNotNull())
            {
                EntityCtx.Validators.ForEach((v) =>
                {
                    if (predict(v))
                    {
                        if (!v.Validate(this))
                        {
                            validationFaild(v);
                            ret = false;
                        }
                    }
                });

                EntityCtx.Properties.SelectMany(p => p.Validators, (p, v) => new { Property = p, Validator = v }).
                    ToList().ForEach((pv) => 
                {
                    if (predict(pv.Validator))
                    {
                        if(!pv.Validator.Validate(this[pv.Property.Name]))
                        {
                            validationFaild(pv.Validator);
                            ret = false;
                        }
                    }
                });
            }
            return ret;
        }

        public bool IsValid
        {
            get
            {
                return _isvalid.Value;
            }
        }

        public bool IsValidWithChildren
        {
            get
            {
                bool ret = IsValid;
                EnumerateByChildren((e, r) => { ret &= e.IsValidWithChildren; });
                return ret;
            }
        }
        #endregion Validate

        #region Default
        internal void Default()
        {
            EntityCtx.Properties.Where(p => p.DefaultValue.IsNotNull()).ToList().ForEach((p) =>
            {
                if (this[p.Name].IsNull() || this[p.Name].Equals(p.PropertyType.GetDefaultValue()))
                {
                    this[p.Name] = p.DefaultValue.Value(this, p);
                }
            });
        }
        #endregion Default

        #region Get & Set Child Value
        internal object GetValue(string name, Type propertyType)
        {
            /*if (Table.DynamicProperties.IsNotNull())
            {
                IEnumerable<Entity> entities = Table.DynamicProperties.Entities(this) as IEnumerable<Entity>;
                if (entities.IsNotNull())
                {
                    Entity entity = entities.FirstOrDefault(e => e[Table.DynamicProperties.PropertyCode].IsNotNull() && e[Table.DynamicProperties.PropertyCode].Equals(name));
                    if (entity.IsNotNull())
                    {
                        KeyValuePair<Type, string> kv = Table.DynamicProperties.PropertiesValue.FirstOrDefault(k => k.Key == propertyType);
                        if (kv.Value.IsNotNull())
                        {
                            return entity[kv.Value]; 
                        }
                    }
                }
            }*/
            return propertyType.GetDefaultValue();
        }

        internal void SetValue(string name, object value, Type propertyType)
        {
            /*if (Table.DynamicProperties.IsNotNull())
            {
                IEnumerable<Entity> entities = Table.DynamicProperties.Entities(this) as IEnumerable<Entity>;
                if (entities.IsNotNull())
                {
                    Entity entity = entities.FirstOrDefault(e => e[Table.DynamicProperties.PropertyCode].IsNotNull() && e[Table.DynamicProperties.PropertyCode].Equals(name));
                    if (entity.IsNotNull())
                    {
                        KeyValuePair<Type, string> kv = Table.DynamicProperties.PropertiesValue.FirstOrDefault(k => k.Key == propertyType);
                        if (kv.Value.IsNotNull())
                        {
                            entity[kv.Value] = value;
                        }
                    }
                    else
                    {
                        IEntityCollection collection = entities as IEntityCollection;
                        if (collection.IsNotNull())
                        {
                            entity = collection.CreateInstance(Table.DynamicProperties.EntityType, true) as Entity;
                            if (entity.IsNotNull())
                            {
                                KeyValuePair<Type, string> kv = Table.DynamicProperties.PropertiesValue.FirstOrDefault(k => k.Key == propertyType);
                                if (kv.Value.IsNotNull())
                                {
                                    entity[kv.Value] = value;
                                }
                                entity[Table.DynamicProperties.PropertyCode] = name;
                                collection.Add(entity);
                            }
                        }
                    }
                }
            }*/
        }
        #endregion Get & Set Child Value

        #region Dispose
        public bool Disposing { get; private set; }
        public virtual void Dispose()
        {
            Disposing = true;
            _Entity = null;
            Context = null;
        }

        ~Entity()
        {
            Dispose();
        }
        #endregion Dispose
    }
}
