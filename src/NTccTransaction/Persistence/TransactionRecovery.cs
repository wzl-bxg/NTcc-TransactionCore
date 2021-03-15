using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransaction
{
    public class TransactionRecovery : ITransactionRecovery
    {
        private readonly NTccTransactionOptions _options;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionRecovery> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TransactionRecovery(NTccTransactionOptions nTccTransactionOptions, ITransactionRepository transactionRepository
                                    , IServiceScopeFactory serviceScopeFactory, ILogger<TransactionRecovery> logger)
        {
            _options = nTccTransactionOptions;
            _transactionRepository = transactionRepository;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task StartRecoverAsync()
        {
            var date = DateTime.UtcNow.AddSeconds(-_options.RecoverDuration);
            var transactions = _transactionRepository.FindAllUnmodifiedSince(date);

            await RecoverErrorTransactionsAsync(transactions);
        }

        private async Task RecoverErrorTransactionsAsync(IEnumerable<ITransaction> transactions)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                foreach (var transaction in transactions)
                {
                    if (transaction.RetriedCount > _options.FailedRetryCount)
                    {
                        _logger.LogError(String.Format("recover failed with max retry count,will not try again. txid:{0}, status:{1},retried count:{2},transaction content:{3}", transaction.Xid, (int)transaction.Status, transaction.RetriedCount, JsonConvert.SerializeObject(transaction)));
                        continue;
                    }
                    if (transaction.TransactionType.Equals(TransactionType.BRANCH)
                        && transaction.CreateUtcTime.AddSeconds(_options.RecoverDuration * _options.FailedRetryCount) > DateTime.UtcNow)
                    {
                        continue;
                    }

                    try
                    {
                        transaction.AddRetriedCount();

                        if (transaction.Status == TransactionStatus.CONFIRMING)
                        {
                            transaction.ChangeStatus(TransactionStatus.CONFIRMING);
                            _transactionRepository.Update(transaction);

                            await transaction.CommitAsync(_serviceScopeFactory);
                            _transactionRepository.Delete(transaction);

                        }
                        else if (transaction.Status == TransactionStatus.CANCELLING || transaction.TransactionType == TransactionType.ROOT)
                        {
                            transaction.ChangeStatus(TransactionStatus.CANCELLING);
                            _transactionRepository.Update(transaction);

                            await transaction.RollbackAsync(_serviceScopeFactory);
                            _transactionRepository.Delete(transaction);

                        }
                    }
                    catch (Exception ex)
                    {
                        if (typeof(ConcurrencyTransactionException).IsInstanceOfType(ex)
                            || ex.GetType().IsAssignableFrom(typeof(ConcurrencyTransactionException))
                            || ex.GetType().Name.Contains("Concurrency"))
                        {
                            _logger.LogWarning(ex, String.Format("optimisticLockException happened while recover. txid:{0}, status:{1},retried count:{2},transaction content:{3}", transaction.Xid, (int)transaction.Status, transaction.RetriedCount, JsonConvert.SerializeObject(transaction)));
                        }
                        else
                        {
                            _logger.LogError(ex, String.Format("recover failed, txid:{0}, status:{1},retried count:{2},transaction content:{3}", transaction.Xid, (int)transaction.Status, transaction.RetriedCount, JsonConvert.SerializeObject(transaction)));
                        }
                    }
                }
            }
        }
    }
}
