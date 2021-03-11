using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransactionCore.Abstractions
{
    public interface ITransactionRecovery
    {
        Task StartRecoverAsync();
    }
}
