using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NTccTransaction;
using NTccTransaction.Abstractions;
using System;

namespace NTccTransaction.SqlServer
{
    internal class SqlServerNTccTransactionOptionsExtension : INTccTransactionOptionsExtension
    {
        private readonly Action<SqlServerOptions> _configure;

        public SqlServerNTccTransactionOptionsExtension(Action<SqlServerOptions> configure)
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