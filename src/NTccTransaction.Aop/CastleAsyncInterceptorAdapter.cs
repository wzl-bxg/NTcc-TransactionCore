using Castle.DynamicProxy;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransaction.Aop
{
    public class CastleAsyncInterceptorAdapter<TInterceptor> : AsyncInterceptorBase
         where TInterceptor : INTccInterceptor
    {
        private readonly TInterceptor _interceptor;

        public CastleAsyncInterceptorAdapter(TInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            await _interceptor.InterceptAsync(
                new CastleMethodInvocationAdapter(invocation, proceedInfo, proceed)
            );
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var adapter = new CastleMethodInvocationAdapterWithReturnValue<TResult>(invocation, proceedInfo, proceed);

            await _interceptor.InterceptAsync(
                adapter
            );

            return (TResult)adapter.ReturnValue;
        }
    }
}
