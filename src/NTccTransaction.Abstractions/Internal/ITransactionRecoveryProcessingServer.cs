using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransaction.Abstractions
{
    public interface ITransactionRecoveryProcessingServer : IProcessingServer
    {
        bool IsHealthy();

        void ReStart(bool force = false);
    }
}
