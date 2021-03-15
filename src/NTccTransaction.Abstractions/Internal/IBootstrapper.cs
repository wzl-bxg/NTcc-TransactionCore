using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTccTransaction.Abstractions
{
    public interface IBootstrapper
    {
        Task BootstrapAsync(CancellationToken stoppingToken);
    }
}
