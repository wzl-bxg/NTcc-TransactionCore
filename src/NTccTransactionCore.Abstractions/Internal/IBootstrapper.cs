using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTccTransactionCore.Abstractions
{
    public interface IBootstrapper
    {
        Task BootstrapAsync(CancellationToken stoppingToken);
    }
}
