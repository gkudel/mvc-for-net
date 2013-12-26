using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using MVCEngine;
using MVCEngine.Exceptions;
using MVCEngine.Internal.Descriptors;

namespace MVCEngine.Internal
{
    internal static class LambdaTools
    {
        #region Convert Function
        private static MethodInfo _miChangeType;
        #endregion Convert Function

        #region Constructor
        static LambdaTools()
        {
            _miChangeType = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });            
        }
        #endregion Constructor
        
        #region ObjectActivator
        internal static Func<object> ObjectActivator(string objectType, string genericType)
        {
            string typeString = objectType;
            if (!genericType.IsNullOrEmpty())
            {
                typeString += "`1[[" + genericType + "]]";
            }
            Type type = Type.GetType(typeString);
            if(type.IsNotNull())
            {
                return LambdaTools.ObjectActivator(type);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        internal static Func<object> ObjectActivator(Type objectType)
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
        #endregion ObjectActivator

        #region PropertySetter
        internal static Action<object, object> PropertySetter(Type objectType, string name)
        {
            PropertyInfo pinfo = objectType.GetProperty(name);
            if (pinfo.IsNotNull())
            {
                return PropertySetter(objectType, pinfo);
            }
            return null;
        }

        internal static Action<object, object> PropertySetter(Type objectType, PropertyInfo propertyInfo)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object));
            Expression convertObj = Expression.Convert(obj, objectType);
            ParameterExpression value = Expression.Parameter(typeof(object));
            DefaultExpression defaultvalue = Expression.Default(propertyInfo.PropertyType);
            return Expression.Lambda<Action<object, object>>(Expression.TryCatch(
                    Expression.Assign(Expression.MakeMemberAccess(convertObj, propertyInfo), Expression.Convert(value, propertyInfo.PropertyType)),
                    Expression.Catch(typeof(InvalidOperationException), Expression.Assign(Expression.MakeMemberAccess(convertObj, propertyInfo), defaultvalue)),
                    Expression.Catch(typeof(ArgumentNullException), Expression.Assign(Expression.MakeMemberAccess(convertObj, propertyInfo), defaultvalue))),
                obj, value).Compile();
        }
        #endregion PropertySetter

        #region PropertyGetter
        internal static Func<object, object> PropertyGetter(Type objectType, string name)
        {
            PropertyInfo pinfo = objectType.GetProperty(name);
            if (pinfo.IsNotNull())
            {
                return PropertyGetter(objectType, pinfo);
            }
            return null;
        }

        internal static Func<object, object> PropertyGetter(Type objectType, PropertyInfo propertyInfo)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object));
            Expression convertObj = Expression.Convert(obj, objectType);
            return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.MakeMemberAccess(convertObj, propertyInfo), typeof(object)), obj).Compile();
        }
        #endregion PropertyGetter

        #region MethodTriger
        internal static Func<object, object[], object> MethodTriger(Type objectType, MethodInfo info)
        {            
            ParameterExpression obj = Expression.Parameter(typeof(object));
            Expression convertObj = Expression.Convert(obj, objectType);
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            ParameterInfo[] paramsInfo = info.GetParameters();
            Expression[] argsExp = new Expression[paramsInfo.Length];
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                if (_miChangeType.IsNotNull() 
                    && (paramType.IsPrimitive || paramType == typeof(string)))
                {
                    argsExp[i] = Expression.TryCatch(Expression.Convert(Expression.Call(_miChangeType, Expression.ArrayIndex(param, index), Expression.Constant(paramType)),paramType),
                                 Expression.Catch(typeof(InvalidCastException), Expression.Default(paramType)),
                                 Expression.Catch(typeof(FormatException), Expression.Default(paramType)),
                                 Expression.Catch(typeof(OverflowException), Expression.Default(paramType)),
                                 Expression.Catch(typeof(ArgumentNullException), Expression.Default(paramType)));
                }
                else
                {
                    argsExp[i] = Expression.TryCatch(Expression.Convert(Expression.ArrayIndex(param, index), paramType),
                                                     Expression.Catch(typeof(ArgumentNullException), Expression.Default(paramType)),
                                                     Expression.Catch(typeof(InvalidOperationException), Expression.Default(paramType)));
                }
            }          
            if (!info.ReturnType.Equals(typeof(void)))
            {                
                return Expression.Lambda<Func<object, object[], object>>(Expression.Call(convertObj, info, argsExp), obj, param).Compile();
            }
            else
            {
                return Expression.Lambda<Func<object, object[], object>>(Expression.Block(Expression.Call(convertObj, info, argsExp), 
                                                                          Expression.Constant(null)), obj, param).Compile();
            }
        }
        #endregion MethodTriger

        #region Method Attributes
        public static Func<object, object[]> GetMethodAttributes(object param, Method method)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object));
            ParameterExpression converted = Expression.Variable(param.GetType());
            ParameterExpression ret = Expression.Variable(typeof(object[]));

            PropertyInfo[] propertyinfo = param.IfNullDefault<object, PropertyInfo[]>(() => { return param.GetType().GetProperties(); },
                                                                    new PropertyInfo[0]);
            var joinquery = from p in method.Parameters
                            join v in propertyinfo on p.ParameterName equals v.Name.ToUpper() into vp
                            from v in vp.DefaultIfEmpty()
                            select new { Parameter = p, Value = v };
            List<Expression> assignExpression = new List<Expression>();
            int i = 0;
            if (joinquery.Count() > 0)
            {
                joinquery.ToList().ForEach((v) =>
                {
                    if (v.Value.IsNotNull())
                    {
                        assignExpression.Add(Expression.Assign(Expression.ArrayAccess(ret, Expression.Constant(i)),
                            Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(obj, param.GetType()), v.Value), typeof(object))));
                    }
                    else
                    {
                        assignExpression.Add(Expression.Assign(Expression.ArrayAccess(ret, Expression.Constant(i)),
                            Expression.Convert(Expression.Constant(null), typeof(object))));
                    }
                    i++;
                });
            }
            else 
            {
                assignExpression.Add(Expression.Assign(Expression.ArrayAccess(ret, Expression.Constant(i)),
                    Expression.Convert(Expression.Constant(null), typeof(object)))); 
            }


            BlockExpression block =   Expression.Block(
                new[] { ret, converted },
                Expression.TryCatch(
                    Expression.Block(
                     Expression.Assign(converted, Expression.Convert(obj, param.GetType())),
                     Expression.Assign(ret, Expression.NewArrayBounds(typeof(object), Expression.Constant(method.Parameters.Count))),
                            Expression.Block(
                                assignExpression
                            ),
                     ret
                ), Expression.Catch(typeof(ArgumentNullException),
                     ret
                ),
                Expression.Catch(typeof(InvalidOperationException),
                     ret
                )
            ));
            return Expression.Lambda<Func<object, object[]>>(block, obj).Compile();
        }
        #endregion Method Attributes

        #region FieldGetter
        internal static Func<object, object> FieldGetter(Type objectType, FieldInfo finfo)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object));
            Expression convertObj = Expression.Convert(obj, objectType);
            return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.MakeMemberAccess(convertObj, finfo), typeof(object)), obj).Compile();
        }
        #endregion FieldGetter 
    }
}
