using Microsoft.Extensions.DependencyInjection;
using NTccTransactionCore.Abstractions;
using System;
using System.Threading.Tasks;

namespace NTccTransactionCore
{
    [Serializable]
    public class Participant : IParticipant
    {
        public TransactionXid Xid { get; set; }

        public InvocationContext ConfirmInvocationContext { get; set; }

        public InvocationContext CancelInvocationContext { get; set; }

       
        public async Task CommitAsync(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                ITransactionMethodInvoker tccTransactionMethodInvoker = scope.ServiceProvider.GetRequiredService<ITransactionMethodInvoker>();
                await tccTransactionMethodInvoker.InvokeAsync(new TransactionContext(Xid, TransactionStatus.CONFIRMING), ConfirmInvocationContext);
            }
        }

        public async Task RollbackAsync(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                ITransactionMethodInvoker tccTransactionMethodInvoker = scope.ServiceProvider.GetRequiredService<ITransactionMethodInvoker>();
                await tccTransactionMethodInvoker.InvokeAsync(new TransactionContext(Xid, TransactionStatus.CANCELLING), CancelInvocationContext);
            }
        }
    }
}
