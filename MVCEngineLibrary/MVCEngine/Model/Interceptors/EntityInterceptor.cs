using Castle.Core.Interceptor;
using MVCEngine.Attributes;
using MVCEngine.Model.Exceptions;
using MVCEngine.Model.Internal.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCEngine.Model.Interceptors
{
    [Serializable]
    public class EntityInterceptor<T> : IInterceptor where T : Entity
    {
        #region Members
        private T _entity;
        private string _uid;
        #endregion Members

        #region Constructor
        public EntityInterceptor()
        {
            _uid = string.Empty;
            _entity = default(T);
        }
        #endregion Constructor

        #region Inetercept
        public void Intercept(IInvocation invocation)
        {                                  
            Entity entity = invocation.InvocationTarget.CastToType<Entity>();
            if (entity.IsNotNull())
            {
                Table childTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().Name);
                if (childTable.IsNull() && entity.GetType().BaseType.IsNotNull())
                {
                    childTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == entity.GetType().BaseType.Name);
                }
                Table parentTable = entity.Context.Tables.FirstOrDefault(t => t.ClassName == typeof(T).Name);
                if (parentTable.IsNotNull() && childTable.IsNotNull())
                {
                    if (parentTable.Uid != _uid)
                    {
                        Relation relation = entity.Context.Relations.FirstOrDefault(r => r.ParentTable == parentTable.TableName
                                                                                  && r.ChildTable == childTable.TableName);
                        if (relation.IsNotNull())
                        {
                            List<T> list = parentTable.Rows.Cast<T>().Where(p => p.State != EntityState.Deleted && relation.ParentValue(p).
                                    Equals(relation.ChildValue(invocation.InvocationTarget))).ToList();
                            if (list.Count() == 1)
                            {
                                _entity = list.First();
                            }
                            else if (list.Count() > 1)
                            {
                                throw new ModelException("Integrity constraint exception");
                            }
                            else
                            {
                                _entity = default(T);
                            }
                        }
                        _uid = childTable.Uid;
                    }
                }
            }
            invocation.ReturnValue = _entity;
        }
        #endregion Inetercept
    }
}
