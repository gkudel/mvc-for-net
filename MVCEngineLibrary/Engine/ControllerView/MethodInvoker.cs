using System;
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
                    PropertyInfo pinfo = propertyinfo.FirstOrDefault(p => p.Name.ToUpper() == "ID");

                    anonymous = new AnonymousType()
                    {
                        Name = param.GetType().FullName,
                        MethodArguments = LambdaTools.GetMethodAttributes(param, method),
                    };
                    if (pinfo.IsNotNull()) anonymous.Id = LambdaTools.PropertyGetter(param.GetType(), pinfo);
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
