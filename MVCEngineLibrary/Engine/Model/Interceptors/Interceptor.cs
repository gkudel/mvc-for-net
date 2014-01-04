using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCEngine.Model.Interceptors
{
    public interface Interceptor : IInterceptor
    {
        string GetId();        
    }
}
