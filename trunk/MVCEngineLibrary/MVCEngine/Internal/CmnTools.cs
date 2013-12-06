using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using MVCEngine;
using MVCEngine.Exceptions;

namespace MVCEngine.Internal
{
    internal static class CmnTools
    {
        #region GetObjectActivator
        public static Func<object> GetObjectActivator(string objectType, string genericType)
        {
            string typeString = objectType;
            if (!genericType.IsNullOrEmpty())
            {
                typeString += "`1[[" + genericType + "]]";
            }
            Type type = Type.GetType(typeString);
            if(type.IsNotNull())
            {
                return CmnTools.GetObjectActivator(type);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static Func<object> GetObjectActivator(Type objectType)
        {
            Func<object> ret = null;
            ConstructorInfo ctor = objectType.GetConstructors().FirstOrDefault(c => c.GetParameters().Count() == 0);
            if (ctor != null)
            {
                ret = (Func<object>)Expression.Lambda(typeof(Func<object>), Expression.New(ctor, null), null).Compile();
            }
            else
            {
                throw new ObjectActivatorException("Type[" + objectType.FullName + "] should have no arguments constructor");
            }
            return ret;
        }
        #endregion GetObjectActivator
    }
}
