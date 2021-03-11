using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NTccTransactionCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransactionCore
{
    public class TransactionRecovery : ITransactionRecovery
    {
        private readonly NTccTransactionOptions _options;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionRecovery> _logger;

        public TransactionRecovery(NTccTransactionOptions nTccTransactionOptions, ITransactionRepository transactionRepository, ILogger<TransactionRecovery> logger)
        {
            _options = nTccTransactionOptions;
            _transactionRepository = transactionRepository;
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
                    transaction.UpdateRetriedCount();

                    if (transaction.Status == TransactionStatus.CONFIRMING)
                    {

                        transaction.ChangeStatus(TransactionStatus.CONFIRMING);
                        _transactionRepository.Update(transaction);
                        await transaction.CommitAsync();
                        _transactionRepository.Delete(transaction);

                    }
                    else if (transaction.Status == TransactionStatus.CANCELLING || transaction.TransactionType == TransactionType.ROOT)
                    {

                        transaction.ChangeStatus(TransactionStatus.CANCELLING);
                        _transactionRepository.Update(transaction);
                        await transaction.RollbackAsync();
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
