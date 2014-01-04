using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interface
{
    public interface IEntityCollection : IList 
    {
        object CreateInstance(Type type, bool defaultValue, object[] constructorArguments);
        Context Context { get; set; }
        bool AllowEdit { get; set; }
    }
}
