using Microsoft.EntityFrameworkCore;
using NTccTransaction.Oracle;
using NTccTransaction;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NTccTransactionOptionsExtensions
    {

        public static NTccTransactionOptions UseOracle(this NTccTransactionOptions options, Action<OracleOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new OracleNTccTransactionOptionsExtension(x =>
            {
                configure(x);
            }));

            return options;
        }

    }
}