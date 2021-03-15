using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NTccTransaction;
using NTccTransaction.Abstractions;
using NTccTransaction.Oracle;
using System;

namespace NTccTransaction.Oracle
{
    internal class OracleNTccTransactionOptionsExtension : INTccTransactionOptionsExtension
    {
        private readonly Action<OracleOptions> _configure;

        public OracleNTccTransactionOptionsExtension(Action<OracleOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            

            services.Configure(_configure);

            services.AddTransient<TransactionDbContext>();
            services.TryAddTransient(DbContextOptionsFactory.Create<TransactionDbContext>);
        }


    }
}