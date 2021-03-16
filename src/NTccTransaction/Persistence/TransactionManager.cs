using Microsoft.Extensions.DependencyInjection;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NTccTransaction
{
    public class TransactionManager : ITransactionManager
    {
        private readonly IAmbientTransaction _ambientTransaction;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ITransaction Current => _ambientTransaction.Transaction;

        public TransactionManager(
            IAmbientTransaction ambientTransaction,
            IServiceScopeFactory serviceScopeFactory)
        {
            _ambientTransaction = ambientTransaction;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ITransaction Begin()
        {
            var transaction = new Transaction(TransactionType.ROOT);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                transactionRepository.Create(transaction);

                RegisterTransaction(transaction);
                return transaction;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        public ITransaction Begin(object uniqueIdentity)
        {
            var transaction = new Transaction(TransactionType.ROOT);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                transactionRepository.Create(transaction);

                RegisterTransaction(transaction);

                return transaction;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionContext"></param>
        /// <returns></returns>
        public ITransaction PropagationNewBegin(TransactionContext transactionContext)
        {
            var transaction = new Transaction(transactionContext);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                transactionRepository.Create(transaction);

                RegisterTransaction(transaction);
                return transaction;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionContext"></param>
        /// <returns></returns>
        public ITransaction PropagationExistBegin(TransactionContext transactionContext)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                var transaction = transactionRepository.FindByXid(transactionContext.Xid);
                if (transaction == null)
                {
                    throw new NoExistedTransactionException();
                }

                transaction.ChangeStatus(transactionContext.Status);
                RegisterTransaction(transaction);
                return transaction;
            }
        }

        /// <summary>
        /// Commit
        /// </summary>
        public async Task CommitAsync()
        {
            var transaction = this.Current;

            // When confirm successfully, delete the NTccTransaction from database
            // The confirm action will be call from the top of stack, so delete the transaction data will not impact the excution under it
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                transaction.ChangeStatus(TransactionStatus.CONFIRMING);
                transactionRepository.Update(transaction);

                try
                {
                    await transaction.CommitAsync(_serviceScopeFactory);
                    transactionRepository.Delete(transaction);
                }
                catch (Exception commitException)
                {
                    throw new ConfirmingException("Confirm failed", commitException);
                }
            }
        }

        /// <summary>
        /// Rollback
        /// </summary>
        public async Task RollbackAsync()
        {
            var transaction = this.Current;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

                transaction.ChangeStatus(TransactionStatus.CANCELLING);
                transactionRepository.Update(transaction);

                try
                {
                    await transaction.RollbackAsync(_serviceScopeFactory);
                    transactionRepository.Delete(transaction);
                }
                catch (Exception rollbackException)
                {
                    throw new CancellingException("Rollback failed", rollbackException);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="participant"></param>
        public void AddParticipant(IParticipant participant)
        {
            var transaction = this.Current;
            transaction.AddParticipant(participant);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                transactionRepository.Update(transaction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsTransactionActive()
        {
            return Current != null;
        }


        private void RegisterTransaction(ITransaction transaction)
        {
            _ambientTransaction.RegisterTransaction(transaction);
        }

        public bool IsDelayCancelException(Exception tryingException, HashSet<Type> allDelayCancelExceptionTypes)
        {
            if (null == allDelayCancelExceptionTypes || !allDelayCancelExceptionTypes.Any())
            {
                return false;
            }

            var tryingExceptionType = tryingException.GetType();

            return allDelayCancelExceptionTypes.Any(t => t.IsAssignableFrom(tryingExceptionType));
        }
    }
}
