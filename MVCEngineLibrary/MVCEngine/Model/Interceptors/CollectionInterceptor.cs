using Castle.Core.Interceptor;
using MVCEngine.Attributes;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class CollectionInterceptor<T> : IInterceptor where T : Entity
    {
        #region Members
        private List<T> _list;
        private string _uid;
        #endregion Members

        #region Constructor
        public CollectionInterceptor()
        {
            _uid = string.Empty;
            _list = new List<T>();
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {                                  
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (entity.IsNotNull())
            {
                Table parentTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().Name);
                if (parentTable.IsNull() && entity.GetType().BaseType.IsNotNull())
                {
                    parentTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().BaseType.Name);
                }
                Table childTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                if (parentTable.IsNotNull() && childTable.IsNotNull())
                {
                    if (childTable.Uid != _uid)
                    {
                        Relation relation = entity.Context.Relations.FirstOrDefault(r => r.ParentTable == parentTable.TableName
                                                                                  && r.ChildTable == childTable.TableName);
                        if (relation.IsNotNull())
                        {
                            _list = childTable.Rows.Cast<T>().Where(c => c.State != EntityState.Deleted && relation.ParentValue(invocation.InvocationTarget).
                                        Equals(relation.ChildValue(c))).ToList();                                
                        }
                        _uid = childTable.Uid;
                    }
                }
            }
            invocation.ReturnValue = _list;
        }
        #endregion Inetercept
    }
}
