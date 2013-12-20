using MVCEngine.Internal;
using MVCEngine.Internal.Validation;
using MVCEngine.Model.Attributes.Validation;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections;
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
                    if (r.OnDelete == Attributes.OnDelete.Cascade)
                    {
                        IList iList = e.Table.Entities as IList;
                        if (iList.IsNotNull())
                        {
                            iList.Remove(e);
                        }
                    }
                    else if (r.OnDelete == Attributes.OnDelete.SetNull)
                    {
                        e[r.ChildKey] = r.ChildType.GetDefaultValue();
                    }
                });
                _entityState = value;
                if (_entityState != EntityState.Unchanged) Context.IsModified = true;
                if (value == EntityState.Deleted)
                {
                    Table.MarkedAsModified();
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
                case EntityState.Deleted: IList iList = Table.Entities as IList;
                    if (iList.IsNotNull())
                    {
                        iList.Remove(this);
                    }
                    break;
            }
            Table.MarkedAsModified();
        }
        #endregion Object State

        #region Enumerate by Children
        public void EnumerateByChildren(Action<Entity,Relation> action)
        {
            if (Table.IsNotNull())
            {
                Context.Relations.Where(r => r.ParentTableName == Table.TableName).ToList().ForEach((r) =>
                {
                    Table childTable = Context.Tables.FirstOrDefault(t => t.TableName == r.ChildTableName);
                    if (childTable.IsNotNull())
                    {
                        childTable.Entities.Where(row => row.State != EntityState.Deleted &&
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

        #region Get & Set Coulumn Value
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
        #endregion Get & Set Coulumn Value

        #region Get & Set Table Uid for Property
        internal string GetTableUidForProperty(string propertyName)
        {
            if (_uids.ContainsKey(propertyName))
            {
                return _uids[propertyName];
            }
            return string.Empty;
        }

        internal void SetTableUidForProperty(string propertyName, string uid)
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
        #endregion Get & Set Table Uid for Property

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
            if (Table.IsNotNull())
            {
                Table.Validators.ForEach((v) =>
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

                Table.Columns.SelectMany(c => c.Validators, (c, v) => new { Column = c, Validator = v }).
                    ToList().ForEach((cv) => 
                {
                    if (predict(cv.Validator))
                    {
                        if(!cv.Validator.Validate(this[cv.Column.Name]))
                        {
                            validationFaild(cv.Validator);
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
            Table.Columns.Where(c => c.DefaultValue.IsNotNull()).ToList().ForEach((c) =>
            {
                if (this[c.Name].IsNull() || this[c.Name].Equals(c.ColumnType.GetDefaultValue()))
                {
                    this[c.Name] = c.DefaultValue.Value(this, c);
                }
            });
        }
        #endregion Default

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
