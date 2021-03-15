using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransaction.Abstractions
{
    public interface IParticipant
    {
        TransactionXid Xid { get; set; }

        InvocationContext ConfirmInvocationContext { get; set; }

        InvocationContext CancelInvocationContext { get; set; }


        Task CommitAsync(IServiceScopeFactory serviceScopeFactory);
        Task RollbackAsync(IServiceScopeFactory serviceScopeFactory);
    }
}
