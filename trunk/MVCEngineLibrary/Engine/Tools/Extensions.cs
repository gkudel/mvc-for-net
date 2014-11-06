using MVCEngine.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.ControllerView;

namespace MVCEngine.Tools
{
    public static class Extensions
    {
        #region IfNullDefault
        public static T IfNullDefault<T>(this T thisObject, T defaultValue)
        {
            return thisObject == null ? defaultValue : thisObject;
        }

        public static T IfNullDefault<T>(this T thisObject, Func<T> factory)
        {
            return thisObject == null ? factory() : thisObject;
        }

        public static T2 IfNullDefault<T1, T2>(this T1 thisObject, Func<T2> factory, T2 defaultValue)
        {
            return thisObject != null ? factory() : defaultValue;
        }

        public static T2 IfNullDefault<T1, T2>(this T1 thisObject, Func<T1, T2> factory, T2 defaultValue)
        {
            return thisObject != null ? factory(thisObject) : defaultValue;
        }
        #endregion IfNullDefault

        #region IfNullOrEmptyDefault
        public static string IfNullOrEmptyDefault(this string thisObject, string defaultValue)
        {
            return string.IsNullOrEmpty(thisObject) ? defaultValue : thisObject;
        }

        public static string IfNotNullOrEmptyDefault(this string thisObject, string defaultValue)
        {
            return !string.IsNullOrEmpty(thisObject) ? defaultValue : thisObject;
        }
        #endregion IfNullOrEmptyDefault

        #region IsNull
        public static bool IsNull(this object thisObject)
        {
            return thisObject == null;
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

        #region IsAnonymousType
        public static bool IsAnonymousType(this object thisObject)
        {
            Type type = thisObject.GetType();
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic; 
        }
        #endregion IsAnonymousType

        #region GetOrNull
        public static T2 GetOrDefault<T1, T2>(this Dictionary<T1, T2> dict, T1 key)
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

        public static bool AddIfNotContains<T>(this BindingList<T> list, T obj)
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
        public static bool IsEquals(this object thisObject, object obj)
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
        public static bool IsNotEquals(this object thisObject, object obj)
        {
            return !IsEquals(thisObject, obj);
        }
        #endregion IsNotEquals

        #region AppendByDefault
        public static void AddAndAppendByDefault<T>(this ICollection<T> thisObject, ICollection<T> collection, int length, T defaultvalue)
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
        public static object GetDefaultValue(this Type type)
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

        #region Entity Collection Extensions
        public static T FirstOrDefulatEntity<T>(this IEnumerable<T> collection, Func<T, bool> predict) where T : Entity
        {
            return collection.FirstOrDefault(e => e.State != EntityState.Deleted && predict(e));
        }

        public static IEnumerable<T> WhereEntity<T>(this IEnumerable<T> collection, Func<T, bool> predict) where T : Entity
        {
            return collection.Where(e => e.State != EntityState.Deleted && predict(e));
        }

        public static int CountEntity<T>(this IEnumerable<T> collection, Func<T, bool> predict) where T : Entity
        {
            return collection.Count(e => e.State != EntityState.Deleted && predict(e));
        }

        public static int CountEntity<T>(this IEnumerable<T> collection) where T : Entity
        {
            return collection.Count(e => e.State != EntityState.Deleted);
        }
        #endregion Entity Collection Extensions

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

        #region IsNotNull
        public static bool IsNotNull(this object thisObject)
        {
            return thisObject != null;
        }
        #endregion IsNull

        #region Invoke Action Method
        public static void InvokeActionMethod(this object o, string controllerName, string actionMethodName, object param)
        {
            Dispatcher.GetInstance().InvokeActionMethod(controllerName, actionMethodName, param);
        }
        #endregion Invoke Action Method

    }
}
