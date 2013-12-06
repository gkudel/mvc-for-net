using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCEngine.Internal;
using Castle.DynamicProxy;
using System.Reflection;
using MVCEngine.Model.Internal;
using MVCEngine.Session;

namespace MVCEngine.Model
{
    public abstract class ModelContext
    {       
        #region Constructor
        public ModelContext()
        {
            string name = this.GetType().Name;
            if (MVCEngine.Session.Session.IsUserSessionExists(name))
            {
                TryCatchStatment.Try().Invoke(() =>
                {
                    string sessionId = MVCEngine.Session.Session.GetUserSessionId(name);
                    object lockobject = MVCEngine.Session.Session.GetSessionData(sessionId, "_lock");
                    if (lockobject.IsNotNull())
                    {
                        lock (lockobject) 
                        {
                        }
                    }
                }).Catch<MVCEngine.Session.Exceptions.InvalidSessionIdException>((e) =>
                {}).Throw();                
            }
        }
        #endregion Constructor

        #region Freeze & UnFreeze
        public void Freeze<T>(T obj) where T : ModelObject
        {
            obj.IsFrozen = true;
        }

        public void UnFreeze<T>(T obj) where T : ModelObject
        {
            obj.IsFrozen = false;
        }
        #endregion Freeze & UnFreeze

        #region App Config 
        public static void ModelContextInitialization<T>() where T : ModelContext
        {
            object lockobject = new object();
            string sessionid = MVCEngine.Session.Session.CreateUserSession(typeof(T).Name);
            MVCEngine.Session.Session.SetSessionData(sessionid, "_lock", lockobject);
            Task.Factory.StartNew(() =>
            {
                TryCatchStatment.Try().Invoke(() =>
                {                    
                    lock(lockobject)
                    {    
                        typeof(T).GetFields().Where(f => f.FieldType.Name == "ModelBindingList`1").
                            ToList().ForEach((f) =>
                        {
                            if (f.FieldType.IsGenericType)
                            {
                                Type entityType = f.FieldType.GetGenericArguments().First<Type>();
                                InterceptorDispatcher.GetInstnace().Initialize(entityType);
                            }
                        });
                    }
                }).Finally(() =>
                {                    
                    MVCEngine.Session.Session.ReleaseSession(sessionid);
                }).Throw();                
            });
        }
        #endregion App Config
    }
}
