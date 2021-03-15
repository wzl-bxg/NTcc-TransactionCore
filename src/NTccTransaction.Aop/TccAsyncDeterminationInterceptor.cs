using Castle.DynamicProxy;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransaction.Aop
{
    public class TccAsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor
      where TInterceptor : INTccInterceptor
    {
        public TccAsyncDeterminationInterceptor(TInterceptor tccInterceptor)
            : base(new CastleAsyncInterceptorAdapter<TInterceptor>(tccInterceptor))
        {

        }
    }
}
