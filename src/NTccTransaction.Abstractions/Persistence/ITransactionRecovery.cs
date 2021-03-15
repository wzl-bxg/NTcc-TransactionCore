using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransaction.Abstractions
{
    public interface ITransactionRecovery
    {
        Task StartRecoverAsync();
    }
}
