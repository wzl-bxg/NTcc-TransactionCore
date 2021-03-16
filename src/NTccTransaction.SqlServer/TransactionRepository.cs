using Microsoft.EntityFrameworkCore;
using NTccTransaction;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NTccTransaction.SqlServer
{
    public class TransactionRepository : CachableTransactionRepository
    {
        private readonly TransactionDbContext _dbContext;
        private readonly DbSet<NTccTransaction> _dbSet;
        private readonly ISerializer _serializer;

        public TransactionRepository(TransactionDbContext dbContext, ISerializer serializer)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<NTccTransaction>();

            _serializer = serializer;
        }

        protected override int DoCreate(ITransaction transaction)
        {
            Check.NotNull(transaction, nameof(transaction));
            var currentVersion = transaction.Version;
            var entity = NewEntity(transaction);
            _dbSet.Add(entity);
            return _dbContext.SaveChanges();
        }

        protected override int DoUpdate(ITransaction transaction)
        {
            Check.NotNull(transaction, nameof(transaction));
            var transactionId = transaction.Xid.ToString();
            var entity = _dbSet.FirstOrDefault(x => x.TransactionId == transactionId);
            UpdateEntity(transaction, entity);
            return _dbContext.SaveChanges();
        }

        protected override int DoDelete(ITransaction transaction)
        {
            Check.NotNull(transaction, nameof(transaction));

            var id = transaction.Xid.ToString();
            var entity = _dbSet.FirstOrDefault(x => x.TransactionId == id);
            if (entity != null)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
                return _dbContext.SaveChanges();
            }
            return 0;
        }

        protected override ITransaction DoFindOne(TransactionXid xid)
        {
            var id = xid.ToString();

            // be careful the cache data.
            var entity = _dbSet.FirstOrDefault(x => x.TransactionId == id);

            return ToTransaction(entity);
        }

        private NTccTransaction NewEntity(ITransaction transaction)
        {
            Check.NotNull(transaction, nameof(transaction));
            return new NTccTransaction
            {
                TransactionId = transaction.Xid.ToString(),
                GlobalTransactionId = transaction.Xid.GlobalTransactionId,
                BranchQualifier = transaction.Xid.BranchQualifier,
                CreateUtcTime = transaction.CreateUtcTime,
                LastUpdateUtcTime = transaction.LastUpdateUtcTime,
                RetriedCount = transaction.RetriedCount,
                TransactionType = transaction.TransactionType,
                Status = transaction.Status,
                Version = transaction.Version,

                Content = _serializer.Serialize(transaction),
            };
        }

        private void UpdateEntity(ITransaction transaction, NTccTransaction entity)
        {
            Check.NotNull(transaction, nameof(transaction));
            Check.NotNull(entity, nameof(entity));

            var currentVersion = transaction.Version;
            if (entity.Version != currentVersion)
            {
                throw new ConcurrencyTransactionException();
            }

            transaction.AddVersion();
            entity.CreateUtcTime = transaction.CreateUtcTime;
            entity.LastUpdateUtcTime = transaction.LastUpdateUtcTime;
            entity.RetriedCount = transaction.RetriedCount;
            entity.TransactionType = transaction.TransactionType;
            entity.Status = transaction.Status;
            entity.Version = transaction.Version;
            entity.Content = _serializer.Serialize(transaction);
        }

        protected override IEnumerable<ITransaction> DoFindAllUnmodifiedSince(DateTime dateTime)
        {
            var transactions = _dbSet.Where(a => a.LastUpdateUtcTime < dateTime).Take(100).ToList();
            foreach (var transaction in transactions)
            {
                yield return ToTransaction(transaction);
            }
        }

        private ITransaction ToTransaction(NTccTransaction transaction)
        {

            if (transaction == null)
            {
                return null;
            }

            return _serializer.Deserialize<ITransaction>(transaction.Content);
        }
    }
}
