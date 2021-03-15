using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransaction.Abstractions
{
    public interface IProcessingServer : IDisposable
    {
        void Pulse();

        void Start();
    }
}
