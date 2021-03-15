using NTccTransaction;
using NTccTransaction.Aop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NTccTransactionOptionsExtensions
    {
        public static NTccTransactionOptions UseCastleInterceptor(this NTccTransactionOptions options)
        {

            options.RegisterExtension(new AopNTccTransactionOptionsExtension());

            return options;
        }
    }
}
