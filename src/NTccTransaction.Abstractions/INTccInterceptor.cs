using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransaction.Abstractions
{
   public interface INTccInterceptor
    {
        Task InterceptAsync(IMethodInvocation invocation);
    }
}
