using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MVCEngine.Model
{
    public enum ObjectState { Added, Modified, Deleted, Unchanged };

    public abstract class ModelObject : INotifyPropertyChanged
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
        public ObjectState State { get; set; }
        public void AcceptChanges()
        {
            if (State == ObjectState.Modified) State = ObjectState.Unchanged;
            if (State == ObjectState.Added) State = ObjectState.Unchanged;
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
    }
}
