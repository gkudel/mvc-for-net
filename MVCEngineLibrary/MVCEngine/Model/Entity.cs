using MVCEngine.Internal;
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
        private static Lazy<Dictionary<string, Dictionary<string,Func<object, object>>>> _propertiesGetter;
        #endregion Members

        #region Constructor
        static Entity()
        {
            _propertiesGetter = new Lazy<Dictionary<string, Dictionary<string, Func<object, object>>>>(() =>
           {
               return new Dictionary<string, Dictionary<string, Func<object, object>>>();
           },true);
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
            Table table = Context.Tables.FirstOrDefault(t => t.ClassName == GetType().Name);
            if (table.IsNull() && GetType().BaseType.IsNotNull())
            {
                table = Context.Tables.FirstOrDefault(t => t.ClassName == GetType().BaseType.Name);
            }
            if (table.IsNotNull())
            {
                Context.Relations.Where(r => r.ParentTable == table.TableName).ToList().ForEach((r) =>
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

        #region GetValue
        public T GetValue<T>(string column)
        {
            if (this.State == EntityState.Deleted) throw new InvalidOperationException();
            if (!_propertiesGetter.Value.ContainsKey(GetType().Name))
            {
                _propertiesGetter.Value.Add(GetType().Name, new Dictionary<string, Func<object, object>>());
            }
            if (!_propertiesGetter.Value[GetType().Name].ContainsKey(column))
            {
                _propertiesGetter.Value[GetType().Name].Add(column, LambdaTools.PropertyGetter(GetType(), column));
            }
            return (T)_propertiesGetter.Value[GetType().Name][column](this);
        }
        #endregion GetValue

        #region Dispose
        public void Dispose()
        {
            Context = null;
        }

        ~Entity()
        {
            Dispose();
        }
        #endregion Dispose
    }
}
