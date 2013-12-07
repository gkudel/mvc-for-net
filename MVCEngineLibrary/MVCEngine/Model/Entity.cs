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
        #endregion Members

        #region IsFrozen
        public bool IsFrozen 
        {
            get 
            { 
                return _isFrozen; 
            }
            set
            {
                _isFrozen = value;
                EnumerateByChildren((e) => { e.IsFrozen = value; });
            }
        }
        #endregion IsFroze

        #region Object State
        public EntityState State { get; set; }
        public void AcceptChanges()
        {
            if (State == EntityState.Modified) State = EntityState.Unchanged;
            if (State == EntityState.Added) State = EntityState.Unchanged;
            if (State == EntityState.Deleted) EnumerateByChildren((e) => { e.State = EntityState.Deleted; });
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
                        childTable.Rows.Where(row => row.State != EntityState.Deleted &&
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
