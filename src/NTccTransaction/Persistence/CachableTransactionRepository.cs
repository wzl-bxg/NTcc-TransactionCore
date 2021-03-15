using NTccTransaction.Abstractions;
using NTccTransaction;
using System;
using System.Runtime.Caching;
using System.Collections.Generic;

namespace NTccTransaction
{
    public abstract class CachableTransactionRepository : ITransactionRepository
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        public virtual void Create(ITransaction transaction)
        {
            Check.NotNull(transaction, nameof(transaction));

            try
            {
                int result = DoCreate(transaction);
                if (result > 0)
                {
                    PutToCache(transaction);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new TransactionException($"Exception occurred while persisting the transaction, xid:{transaction.Xid}", ex);
            }
        }

        public void Update(ITransaction transaction)
        {
            int result = 0;

            try
            {
                result = DoUpdate(transaction);
                if (result > 0)
                {
                    PutToCache(transaction);
                    return;
                }
                else
                {
                    throw new ConcurrencyTransactionException();
                }
            }
           
            finally
            {
                if (result <= 0)
                {
                    RemoveFromCache(transaction);
                }
            }
        }

        public void Delete(ITransaction transaction)
        {
            int result = 0;
            try
            {
                result = DoDelete(transaction);
            }
            finally
            {
                RemoveFromCache(transaction);
            }
        }

        public ITransaction FindByXid(TransactionXid xid)
        {
            var transaction = FindFromCache(xid);

            if (transaction == null)
            {
                transaction = DoFindOne(xid);

                if (transaction != null)
                {
                    PutToCache(transaction);
                }
            }

            return transaction;
        }

        public IEnumerable<ITransaction> FindAllUnmodifiedSince(DateTime dateTime)
        {

            IEnumerable<ITransaction> transactions = DoFindAllUnmodifiedSince(dateTime);

            foreach (ITransaction transaction in transactions)
            {
                PutToCache(transaction);
            }

            return transactions;
        }

        protected abstract IEnumerable<ITransaction> DoFindAllUnmodifiedSince(DateTime dateTime);

        protected abstract int DoCreate(ITransaction transaction);

        protected abstract int DoUpdate(ITransaction transaction);

        protected abstract int DoDelete(ITransaction transaction);

        protected abstract ITransaction DoFindOne(TransactionXid xid);

        #region Cache
        private void PutToCache(ITransaction transaction)
        {
            Check.NotNull(transaction, nameof(transaction));

            var policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(30)
            };
            Cache.Set(transaction.Xid.ToString(), transaction, policy);
        }

        private ITransaction FindFromCache(TransactionXid transactionXid)
        {
            Check.NotNull(transactionXid, nameof(transactionXid));
            return (ITransaction)Cache.Get(transactionXid.ToString());
        }

        private void RemoveFromCache(ITransaction transaction)
        {
            Check.NotNull(transaction, nameof(transaction));
            Cache.Remove(transaction.Xid.ToString());
        }

        #endregion
    }
}
