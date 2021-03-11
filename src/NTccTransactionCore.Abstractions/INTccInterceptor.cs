using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransactionCore.Abstractions
{
   public interface INTccInterceptor
    {
        Task InterceptAsync(IMethodInvocation invocation);
    }
}
