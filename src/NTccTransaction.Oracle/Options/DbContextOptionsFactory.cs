using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NTccTransaction.Oracle;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransaction.Oracle
{
    public class DbContextOptionsFactory
    {
        public static DbContextOptions<TDbContext> Create<TDbContext>(IServiceProvider serviceProvider)
           where TDbContext : DbContext
        {
            var dbContextOptions = new DbContextOptionsBuilder<TDbContext>()
                .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());

            var options = serviceProvider.GetRequiredService<IOptions<OracleOptions>>().Value;

            dbContextOptions.UseOracle(options.ConnectionString, (builder) =>
            {
                if (!string.IsNullOrEmpty(options.Version))
                {
                    builder.UseOracleSQLCompatibility(options.Version);
                }
            });

            return dbContextOptions.Options;
        }
    }
}
