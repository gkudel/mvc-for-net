using MVCEngine.Model.Attributes.Default;
using MVCEngine.Model.Attributes.Discriminators;
using MVCEngine.Model.Attributes.Validation;
using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MVCEngine;
using System.Diagnostics;

namespace MVCEngine.Model.Internal.Descriptions
{    
    public class EntityProperty
    {
        #region Members
        private PropertyInfo _pinfo;
        #endregion Members

        #region Constructor
        internal EntityProperty()
        {
            Validators = new List<ColumnValidator>();
            Interceptors = new Dictionary<string, List<string>>();
        }
        #endregion Constructor

        #region Properties
        public string Name { get; internal set; }
        public Type PropertyType { get; internal set; }        
        public bool PrimaryKey { get; internal set; }        
        internal List<ColumnValidator> Validators { get; set; }
        internal DefaultValue DefaultValue { get; set; }
        internal Action<object, object> Setter { get; set; }
        internal Func<object, object> Getter { get; set; }
        internal ReletedEntity ReletedEntity { get; set; }
        private Dictionary<string, List<string>> Interceptors { get; set; }
        internal PropertyInfo PropertyInfo
        {
            set
            {
                _pinfo = value;
                if (_pinfo.GetGetMethod().IsNotNull())
                {
                    Interceptors.Add(_pinfo.GetGetMethod().Name, new List<string>());
                    Interceptors[_pinfo.GetGetMethod().Name].Add(ModificationInterceptor.Id);
                }
                if (_pinfo.GetSetMethod().IsNotNull())
                {
                    Interceptors.Add(_pinfo.GetSetMethod().Name, new List<string>());
                    Interceptors[_pinfo.GetSetMethod().Name].Add(SecurityInterceptor.Id);
                    Interceptors[_pinfo.GetSetMethod().Name].Add(ValidationInterceptor.Id);
                    Interceptors[_pinfo.GetSetMethod().Name].Add(ModificationInterceptor.Id);
                }
            }
        }

        internal void AddGetInterceptor(string id)
        {
            Debug.Assert(_pinfo.IsNotNull(), "PropertyInfo is null");
            if (_pinfo.IsNotNull() && _pinfo.GetGetMethod().IsNotNull())
            {
                if (!Interceptors.ContainsKey(_pinfo.GetGetMethod().Name))
                {
                    Interceptors.Add(_pinfo.GetGetMethod().Name, new List<string>());
                }
                Debug.Assert(!Interceptors[_pinfo.GetGetMethod().Name].Contains(id), "Id[" + id + "] duplicated");
                Interceptors[_pinfo.GetGetMethod().Name].Add(id);
            }
        }

        internal void AddSetInterceptor(string id)
        {
            Debug.Assert(_pinfo.IsNotNull(), "PropertyInfo is null");
            if (_pinfo.IsNotNull() && _pinfo.GetSetMethod().IsNotNull())
            {
                if (!Interceptors.ContainsKey(_pinfo.GetSetMethod().Name))
                {
                    Interceptors.Add(_pinfo.GetSetMethod().Name, new List<string>());
                }
                Debug.Assert(Interceptors[_pinfo.GetSetMethod().Name].Contains(id), "Id[" + id + "] duplicated");
                Interceptors[_pinfo.GetSetMethod().Name].Add(id);
            }
        }

        internal string[] GetInterceptorsId(string methodName)
        {
            if (Interceptors.ContainsKey(methodName))
            {
                return Interceptors[methodName].ToArray();
            }
            return new string[0];
        }
        #endregion Properties
    }
}
