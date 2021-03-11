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

        private readonly IServiceProvider _serviceProvider;

        private readonly ITransactionMethodInvoker _tccTransactionMethodInvoker;

        public Participant(IServiceProvider serviceProvider, ITransactionMethodInvoker tccTransactionMethodInvoker)
        {
            _serviceProvider = serviceProvider;
            _tccTransactionMethodInvoker = tccTransactionMethodInvoker;
        }

        public async Task CommitAsync()
        {
            await _tccTransactionMethodInvoker.InvokeAsync(new TransactionContext(Xid, TransactionStatus.CONFIRMING), ConfirmInvocationContext);
        }

        public async Task RollbackAsync()
        {
            await _tccTransactionMethodInvoker.InvokeAsync(new TransactionContext(Xid, TransactionStatus.CANCELLING), CancelInvocationContext);
        }
    }
}
