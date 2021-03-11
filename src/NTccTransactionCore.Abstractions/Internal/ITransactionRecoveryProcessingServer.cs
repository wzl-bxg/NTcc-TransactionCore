using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransactionCore.Abstractions
{
    public interface ITransactionRecoveryProcessingServer : IProcessingServer
    {
        bool IsHealthy();

        void ReStart(bool force = false);
    }
}
