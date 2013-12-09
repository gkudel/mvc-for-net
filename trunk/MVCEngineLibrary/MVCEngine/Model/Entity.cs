using MVCEngine.Internal;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MVCEngine.Model
{
    public enum EntityState { Added, Modified, Deleted, Unchanged };

    public abstract class Entity : INotifyPropertyChanged,  IDisposable
    {
        #region Members
        private bool _isFrozen = false;
        private EntityState _entityState = EntityState.Added;
        private Lazy<bool> _isvalid;
        private Table _table;       
        #endregion Members

        #region Constructor
        public Entity()
        {
            _isvalid = new Lazy<bool>(() =>
            {
                return Validate();
            }, true);
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
                EnumerateByChildren((e) => { e.IsFrozen = value; });
            }
        }
        #endregion IsFroze

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
                _entityState = value;
                if (_entityState == EntityState.Deleted) EnumerateByChildren((e) => { e.State = EntityState.Deleted; });
                if (_entityState == EntityState.Added)
                {
                    if(Table.IsNotNull())
                    {
                        Table.Columns.Where(c => c.DefaultValue.IsNotNull()).ToList().ForEach((c) =>
                        {
                            if (this[c.Name].IsNull())
                            {
                                this[c.Name] = c.DefaultValue.Value(this, c);
                            }
                        });
                    }
                }
            }
        }

        public void AcceptChanges()
        {
            if (State == EntityState.Modified) State = EntityState.Unchanged;
            if (State == EntityState.Added) State = EntityState.Unchanged;            
        }
        #endregion Object State

        #region Enumerate by Children
        public void EnumerateByChildren(Action<Entity> action)
        {
            if (Table.IsNotNull())
            {
                Context.Relations.Where(r => r.ParentTable == Table.TableName).ToList().ForEach((r) =>
                {
                    Table childTable = Context.Tables.FirstOrDefault(t => t.TableName == r.ChildTable);
                    if (childTable.IsNotNull())
                    {
                        childTable.Entities.Where(row => row.State != EntityState.Deleted &&
                        r.ChildValue(row).Equals(r.ParentValue(this))).ToList().ForEach((row) =>
                        {
                            action(row);
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

        #region Table
        public Table Table
        {
            get
            {
                if (_table.IsNull())
                {
                    _table = Context.Tables.FirstOrDefault(t => t.ClassName == GetType().Name);
                    if (_table.IsNull() && GetType().BaseType.IsNotNull())
                    {
                        _table = Context.Tables.FirstOrDefault(t => t.ClassName == GetType().BaseType.Name);
                    } 
                }
                return _table;
            }
        }
        #endregion Table

        #region GetValue
        public object this[string column]
        {
            get 
            {
                if (Table.IsNotNull())
                {
                    Column c = Table.Columns.FirstOrDefault(col => col.Property == column || col.Name == column);
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
                if (Table.IsNotNull())
                {
                    Column c = Table.Columns.FirstOrDefault(col => col.Property == column || col.Name == column);
                    if (c.IsNotNull())
                    {
                        c.Setter(this, value);
                    }
                    else
                    {
                        throw new ModelException("Type[" + GetType().Name + "] doesn't have column[" + column + "]");
                    }
                }
            }
        }
        #endregion GetValue

        #region Validate
        public bool Validate()
        {
            bool ret = true;
            if (Table.IsNotNull())
            {
                Table.Validators.ForEach((v) =>
                {
                    ret &= v.Validate(this);
                });

                Table.Columns.SelectMany(c => c.Validators, (c, v) => new { Column = c, Validator = v }).
                    ToList().ForEach((cv) => 
                {
                    ret &= cv.Validator.Validate(this[cv.Column.Name]);
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
                EnumerateByChildren((e) => { ret &= e.IsValidWithChildren; });
                return ret;
            }
        }
        #endregion Validate

        #region Dispose
        public void Dispose()
        {
            _table = null;
            Context = null;
        }

        ~Entity()
        {
            Dispose();
        }
        #endregion Dispose
    }
}
