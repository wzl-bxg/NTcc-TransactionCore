using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTccTransaction
{
    public class Bootstrapper : BackgroundService, IBootstrapper
    {
        private readonly ILogger<Bootstrapper> _logger;

        private IEnumerable<IProcessingServer> _processors { get; }

        public Bootstrapper(
            ILogger<Bootstrapper> logger,
            IEnumerable<IProcessingServer> processors)
        {
            _logger = logger;
            _processors = processors;
        }

        public async Task BootstrapAsync(CancellationToken stoppingToken)
        {
            await BootstrapCoreAsync();
        }

        protected virtual Task BootstrapCoreAsync()
        {
            foreach (var item in _processors)
            {
                try
                {
                    item.Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BootstrapAsync(stoppingToken);
        }
    }
}
