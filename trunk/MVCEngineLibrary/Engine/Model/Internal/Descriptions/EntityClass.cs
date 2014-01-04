using Castle.DynamicProxy;
using MVCEngine.Model.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class EntityClass
    {
        #region Constructor
        internal EntityClass()
        {
            Properties = new List<EntityProperty>();
            Validators = new List<EntityValidator>();
            SynchronizedCollection = new Dictionary<string, List<string>>();
            Attributes = new List<Attribute>();
            Synchronizing = true;
            MarkedAsModified();
        }
        #endregion Constructor

        #region Properties
        public string Name { get; internal set; }
        public List<EntityProperty> Properties { get; private set; }
        public List<Attribute> Attributes { get; private set; }
        public IEnumerable<Entity> Entities { get; internal set; }
        public Type EntityType { get; internal set; }
        internal string Uid { get; private set; }
        internal Func<object, object> PrimaryKey { get; set; }
        internal EntityProperty PrimaryKeyProperty { get; set; }        
        internal List<EntityValidator> Validators { get; private set; }
        internal DynamicProperties DynamicProperties { get; set; }
        internal Dictionary<string, List<string>> SynchronizedCollection { get; set; }
        internal bool Synchronizing { get; set; }
        #endregion Properties

        #region Synchronized
        internal void Synchronized(string entityName, string propertyName)
        {
            if (!SynchronizedCollection.ContainsKey(entityName))
            {
                SynchronizedCollection.Add(entityName, new List<string>());
            }
            SynchronizedCollection[entityName].Add(propertyName);
        }
        #endregion Synchronized

        #region Marked as Modified
        public void MarkedAsModified()
        {
            Uid = Guid.NewGuid().ToString();
        }
        #endregion Marked as Modified
    }
}
