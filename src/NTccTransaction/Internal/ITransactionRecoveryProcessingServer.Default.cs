using Microsoft.Extensions.Logging;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace NTccTransaction
{
    public class TransactionRecoveryProcessingServer : ITransactionRecoveryProcessingServer
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly MethodMatcherCache _selector;
        private readonly NTccTransactionOptions _options;
        private readonly ITransactionRecovery _transactionRecovery;

        private CancellationTokenSource _cts;
        private static bool _isHealthy = true;
        private bool _disposed;
        private Task _compositeTask;



        public TransactionRecoveryProcessingServer(ILogger<TransactionRecoveryProcessingServer> logger, IServiceProvider serviceProvider
            , NTccTransactionOptions nTccTransactionOptions, ITransactionRecovery transactionRecovery)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _selector = serviceProvider.GetService<MethodMatcherCache>();
            _options = nTccTransactionOptions;
            _transactionRecovery = transactionRecovery;

            _cts = new CancellationTokenSource();
        }

        public bool IsHealthy()
        {
            return _isHealthy;
        }

        public void Start()
        {
            _selector.GetTccCandidatesMethods(); //collect tcc candidates methods

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (true)
                    {
                        await _transactionRecovery.StartRecoverAsync();

                        _cts.Token.ThrowIfCancellationRequested();
                        _cts.Token.WaitHandle.WaitOne(_options.FailedRetryInterval * 1000);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    //ignore
                }
            }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void ReStart(bool force = false)
        {
            if (!IsHealthy() || force)
            {
                Pulse();

                _cts = new CancellationTokenSource();
                _isHealthy = true;

                Start();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                Pulse();

                _compositeTask?.Wait(TimeSpan.FromSeconds(2));
            }
            catch (AggregateException ex)
            {
                var innerEx = ex.InnerExceptions[0];
                if (!(innerEx is OperationCanceledException))
                {
                    _logger.LogError(innerEx.Message);
                }
            }
        }

        public void Pulse()
        {
            _cts?.Cancel();
        }
    }
}
