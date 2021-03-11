using Castle.DynamicProxy;
using NTccTransactionCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransactionCore.Aop
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
