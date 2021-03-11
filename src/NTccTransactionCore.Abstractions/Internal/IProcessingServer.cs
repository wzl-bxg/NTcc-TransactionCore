using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransactionCore.Abstractions
{
    public interface IProcessingServer : IDisposable
    {
        void Pulse();

        void Start();
    }
}
