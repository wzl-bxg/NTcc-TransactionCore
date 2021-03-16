using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTccTransaction.Abstractions
{
    public interface ITransaction : IDisposable
    {
        event EventHandler Disoped;

        TransactionXid Xid { get; }
        TransactionStatus Status { get; set; }
        TransactionType TransactionType { get; }
        int RetriedCount { get; set; }
        DateTime CreateUtcTime { get; }
        DateTime LastUpdateUtcTime { get; set; }
        int Version { get; set; }

        Task CommitAsync(IServiceScopeFactory serviceScopeFactory);

        Task RollbackAsync(IServiceScopeFactory serviceScopeFactory);

        void AddRetriedCount();

        void AddVersion();

        void ChangeStatus(TransactionStatus status);



        #region Participant

        void AddParticipant(IParticipant participant);

        IReadOnlyList<IParticipant> FindAllParticipantAsReadOnly();

        #endregion
    }
}
