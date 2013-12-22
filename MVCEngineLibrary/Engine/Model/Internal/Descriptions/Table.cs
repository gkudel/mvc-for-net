﻿using MVCEngine.Model.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class Table
    {
        #region Constructor
        internal Table()
        {
            Columns = new List<Column>();
            Validators = new List<EntityValidator>();
            Triggers = new Dictionary<Entity, List<Func<object, object>>>();
            MarkedAsModified();
        }
        #endregion Constructor

        #region Properties
        internal string ClassName { get; set; }
        internal string EntityFieldName { get; set; }
        internal Func<object, object> EntityFieldGetter{ get; set; }
        internal Action<object, object> ContextSetter { get; set; }
        internal IEnumerable<Entity> Entities { get; set; }
        internal string Uid { get; private set; }
        internal Func<object, object> PrimaryKey { get; set; }
        internal Column PrimaryKeyColumn { get; set; }
        internal Type EntityType { get; set; }
        internal List<EntityValidator> Validators { get; set; }
        public List<Column> Columns { get; internal set; }
        public string TableName { get; internal set; }
        internal Dictionary<Entity, List<Func<object, object>>> Triggers { get; set; }
        internal DynamicProperties DynamicProperties { get; set; }
        #endregion Properties

        #region Marked as Modified
        public void MarkedAsModified()
        {
            Uid = Guid.NewGuid().ToString();
        }
        #endregion Marked as Modified
    }
}
