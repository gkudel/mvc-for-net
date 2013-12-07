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
                //Freeze(this); TODO Freeze child Objects
            }
        }
        #endregion IsFroze

        #region Object State
        public EntityState State { get; set; }
        public void AcceptChanges()
        {
            if (State == EntityState.Modified) State = EntityState.Unchanged;
            if (State == EntityState.Added) State = EntityState.Unchanged;
        }
        #endregion Object State

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
