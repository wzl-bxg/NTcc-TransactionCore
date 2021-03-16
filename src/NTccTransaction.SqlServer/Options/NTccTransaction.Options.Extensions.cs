using Microsoft.EntityFrameworkCore;
using NTccTransaction.SqlServer;
using NTccTransaction;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NTccTransactionOptionsExtensions
    {

        public static NTccTransactionOptions UseSqlServer(this NTccTransactionOptions options, Action<SqlServerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new SqlServerNTccTransactionOptionsExtension(x =>
            {
                configure(x);
            }));

            return options;
        }

    }
}