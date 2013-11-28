using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Internal
{
    public static class MVCExtensions
    {
        #region IfNullDefault
        public static T IfNullDefault<T>(this T thisObject, T defaultValue)
        {
            return thisObject == null ? defaultValue : thisObject;
        }

        public static T IfNullDefault<T>(this T thisObject, Func<T> fun)
        {
            return thisObject == null ? fun() : thisObject;
        }

        public delegate T Func<T>();
        public static T2 IfNullDefault<T1, T2>(this T1 thisObject, Func<T2> fun, T2 defaultValue)
        {
            return thisObject != null ? fun() : defaultValue;
        }
        #endregion IfNullDefault

        #region IfNulOrEmptyDefault
        public static string IfNulOrEmptyDefault(this string thisObject, string defaultValue)
        {
            return string.IsNullOrEmpty(thisObject) ? defaultValue : thisObject;
        }
        #endregion IfNulOrEmptyDefault

        #region IsNull
        public static bool IsNull(this object thisObject)
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
        public static bool IsNullOrEmpty(this string thisObject)
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
        public static T2 GetOrNull<T1, T2>(this Dictionary<T1, T2> dict, T1 key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return default(T2);
        }
        #endregion GetOrNull

        #region AddOrReplace
        public static void AddOrReplace<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 value)
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
        public static void ThrowException<T>(this object o, string excMessage) where T : Exception
        {
            if (!string.IsNullOrEmpty(excMessage)) throw (T)Activator.CreateInstance(typeof(T), new object[] { excMessage });
        }
        #endregion ThrowException

        #region AddIfNotContains
        public static bool AddIfNotContains<T>(this List<T> list, T obj)
        {
            if (!list.Contains(obj))
            {
                list.Add(obj);
                return true;
            }
            return false;
        }
        #endregion AddIfNotContains
    }
}
