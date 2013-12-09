using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine
{
    public static class Extensions
    {
        #region IfNullDefault
        internal static T IfNullDefault<T>(this T thisObject, T defaultValue)
        {
            return thisObject == null ? defaultValue : thisObject;
        }

        internal static T IfNullDefault<T>(this T thisObject, Func<T> fun)
        {
            return thisObject == null ? fun() : thisObject;
        }

        internal static T2 IfNullDefault<T1, T2>(this T1 thisObject, Func<T2> fun, T2 defaultValue)
        {
            return thisObject != null ? fun() : defaultValue;
        }
        
        internal static T2 IfNullDefault<T1, T2>(this T1 thisObject, Func<T1, T2> fun, T2 defaultValue)
        {
            return thisObject != null ? fun(thisObject) : defaultValue;
        }
        #endregion IfNullDefault

        #region IfNullOrEmptyDefault
        internal static string IfNullOrEmptyDefault(this string thisObject, string defaultValue)
        {
            return string.IsNullOrEmpty(thisObject) ? defaultValue : thisObject;
        }

        internal static string IfNotNullOrEmptyDefault(this string thisObject, string defaultValue)
        {
            return !string.IsNullOrEmpty(thisObject) ? defaultValue : thisObject;
        }
        #endregion IfNullOrEmptyDefault

        #region IsNull
        internal static bool IsNull(this object thisObject)
        {
            return thisObject == null;
        }
        #endregion IsNull

        #region IsNotNull
        public static bool IsNotNull(this object thisObject)
        {
            return thisObject != null;
        }
        #endregion IsNull

        #region IsNull
        internal static bool IsNullOrEmpty(this string thisObject)
        {
            return string.IsNullOrEmpty(thisObject);
        }
        #endregion IsNull

        #region IsTypeOf
        public static bool IsTypeOf<T>(this object thisObject) where T : class
        {
            if (thisObject != null)
            {
                return typeof(T).IsInstanceOfType(thisObject)
                    || typeof(T).IsAssignableFrom(thisObject.GetType()); 
            }
            return false;
        }
        #endregion IsTypeOf

        #region IsAnonymousType
        internal static bool IsAnonymousType(this object thisObject)
        {
            Type type = thisObject.GetType();
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic; 
        }
        #endregion IsAnonymousType

        #region CastToTypeOf
        public static T CastToType<T>(this object thisObject) where T : class
        {
            if (thisObject != null && IsTypeOf<T>(thisObject))
            {
                return (T)thisObject;
            }
            return default(T);
        }
        #endregion IsTypeOf

        #region GetOrNull
        internal static T2 GetOrNull<T1, T2>(this Dictionary<T1, T2> dict, T1 key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return default(T2);
        }
        #endregion GetOrNull

        #region AddOrReplace
        internal static void AddOrReplace<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] = value;
            }
        }
        #endregion AddOrReplace

        #region ThrowException
        internal static void ThrowException<T>(this object o, string excMessage) where T : Exception
        {
            if (!string.IsNullOrEmpty(excMessage)) throw (T)Activator.CreateInstance(typeof(T), new object[] { excMessage });
        }
        #endregion ThrowException

        #region AddIfNotContains
        internal static bool AddIfNotContains<T>(this List<T> list, T obj)
        {
            if (!list.Contains(obj))
            {
                list.Add(obj);
                return true;
            }
            return false;
        }
        #endregion AddIfNotContains

        #region IsEquals
        internal static bool IsEquals(this object thisObject, object obj)
        {
            if((thisObject.IsNull() && obj.IsNull()) ||
                (thisObject.IsNotNull() && obj.IsNotNull() && thisObject.Equals(obj)))
            {
                return true;
            }
            return false;
        }
        #endregion IsEquals

        #region IsNotEquals
        internal static bool IsNotEquals(this object thisObject, object obj)
        {
            return !IsEquals(thisObject, obj);
        }
        #endregion IsNotEquals

        #region AppendByDefault
        internal static void AddAndAppendByDefault<T>(this ICollection<T> thisObject, ICollection<T> collection, int length, T defaultvalue)
        {
            for (int i = thisObject.Count, j = 0; i < length; i++, j++)
            {
                if (j < collection.Count)
                {
                    thisObject.Add(collection.ElementAt(j));
                }
                else
                {
                    thisObject.Add(defaultvalue);
                }
            }
        }
        #endregion AppendByDefault

        #region GetDefaultValue
        internal static object GetDefaultValue(this Type type)
        {
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            if (type.ContainsGenericParameters)
                throw new ArgumentException(
                    "[" + MethodInfo.GetCurrentMethod() + "] Error:\n\nThe supplied value type [" + type +
                    "] contains generic parameters, so the default value cannot be retrieved");

            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        "[" + MethodInfo.GetCurrentMethod() + "] Error:\n\nThe Activator.CreateInstance method could not " +
                        "create a default instance of the supplied value type [" + type +
                        "] (Inner Exception message: \"" + e.Message + "\")", e);
                }
            }

            throw new ArgumentException("[" + MethodInfo.GetCurrentMethod() + "] Error:\n\nThe supplied value type [" + type +
                "] is not a publicly-visible type, so the default value cannot be retrieved");
        }
        #endregion GetDefaultValue
    }
}
