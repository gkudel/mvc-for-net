using CtgWorksheet.Model.Attributes;
using MVCEngine.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MVCEngine.Tools;
using MVCEngine.Model.Internal.Descriptions;

namespace CtgWorksheet.Model
{
    public class EntityRow : Entity
    {
        #region Members
        private Table _table;
        private Dictionary<string, KeyValuePair<string, EntityProperty>> _map;
        #endregion Members

        #region Constructor
        public EntityRow()
        {
            _map = new Dictionary<string, KeyValuePair<string, EntityProperty>>();
        }

        public EntityRow(DataRow row)
            : this()
        {
            Row = row;
        }
        #endregion Consturctor

        #region Properties
        public DataRow Row { get; set; }

        public Table Table
        {
            get
            {
                if (_table.IsNull())
                {
                    Attribute attribute = EntityCtx.Attributes.FirstOrDefault(a => a.IsTypeOf<Table>());
                    if (attribute.IsNotNull())
                    {
                        _table = attribute.CastToType<Table>();

                    }
                }
                return _table;
            }
        }        
        #endregion Properties

        #region DataTable Column
        public KeyValuePair<string,EntityProperty> GetDataTableColumn(string name)
        { 
            if (!_map.ContainsKey(name))
            {
                _map.Add(name, new KeyValuePair<string,EntityProperty>(string.Empty, null));
                EntityProperty property = EntityCtx.Properties.FirstOrDefault(p => p.Name == name);
                if (property.IsNotNull())
                {
                    Column column = property.Attibutes.FirstOrDefault(a => a.IsTypeOf<Column>()) as Column;
                    if (column.IsNotNull())
                    {
                        _map[name] = new KeyValuePair<string,EntityProperty>(column.ColumnName, property);
                    }
                }
            }
            return _map[name];
        }
        #endregion DataTable Column

        #region Object State
        public override EntityState State
        {
            get
            {
                switch (Row.RowState)
                {
                    case DataRowState.Added: return EntityState.Added;
                    case DataRowState.Deleted :
                    case DataRowState.Detached: return EntityState.Deleted;
                    case DataRowState.Modified : return EntityState.Modified;
                    case DataRowState.Unchanged: return EntityState.Unchanged;
                    default: return EntityState.Unchanged;
                }
            }
            set
            {
                base.State = value;
                switch (value)
                {
                    case EntityState.Deleted :
                        Row.Delete();
                        break;
                    case EntityState.Unchanged:
                        Row.AcceptChanges();
                        break;
                }                
            }
        }
        #endregion Object State

        #region Dispose
        public override void Dispose()
        {
            base.Dispose();
            Row = null;
            _table = null;
        }

        ~EntityRow()
        {
            Dispose();
        }
        #endregion Dispose
    }
}
