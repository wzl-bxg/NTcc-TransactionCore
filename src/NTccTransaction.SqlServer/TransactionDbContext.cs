using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NTccTransaction.SqlServer
{
    public class TransactionDbContext : DbContext
    {
        private ILoggerFactory _loggerFactory { get; set; }

        public TransactionDbContext(DbContextOptions<TransactionDbContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NTccTransaction>().Property(x => x.Version).IsConcurrencyToken();

            base.OnModelCreating(modelBuilder);
        }
    }
}
