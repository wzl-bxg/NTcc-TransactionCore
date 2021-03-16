using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransaction.SqlServer
{
    public class DbContextOptionsFactory
    {
        public static DbContextOptions<TDbContext> Create<TDbContext>(IServiceProvider serviceProvider)
           where TDbContext : DbContext
        {
            var dbContextOptions = new DbContextOptionsBuilder<TDbContext>()
                .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());

            var options = serviceProvider.GetRequiredService<IOptions<SqlServerOptions>>().Value;

            dbContextOptions.UseSqlServer(options.ConnectionString);

            return dbContextOptions.Options;
        }
    }
}
