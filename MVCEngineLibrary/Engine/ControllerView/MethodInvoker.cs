﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.ControllerView.Descriptors;
using MVCEngine.Tools;
using System.Reflection;

namespace MVCEngine.ControllerView
{
    static class MethodInvoker
    {
        #region Invoke
        internal static void Invoke(object thisObject, ActionMethod method, object param)
        {
            if (param.GetType().IsArray)
            {
                method.MethodInfo.Invoke(thisObject, param.CastToType<object[]>());
            }
            else
            {
                Invoke(thisObject, method.Method, param);
            }
        }

        internal static void Invoke(object thisObject, Method method, object param)
        {
            List<object> parameters = new List<object>();
            if (param.IsNotNull() && param.IsAnonymousType())
            {
                AnonymousType anonymous = method.Anonymous.FirstOrDefault(a => a.Name == param.GetType().FullName);

                if (anonymous.IsNull())
                {
                    PropertyInfo[] propertyinfo = param.IfNullDefault<object, PropertyInfo[]>(() => { return param.GetType().GetProperties(); },
                                        new PropertyInfo[0]);

                    anonymous = new AnonymousType()
                    {
                        Name = param.GetType().FullName,
                        MethodArguments = LambdaTools.GetMethodAttributes(param, method),
                    };
                    method.Anonymous.Add(anonymous);
                }
                if (method.Parameters.Count > 0)
                {
                    parameters.AddRange(anonymous.MethodArguments(param));
                }

                method.MethodTriger(thisObject, (parameters.Count > 0 ? parameters.ToArray() : null));
            }
        }
        #endregion Invoke
    }
}
