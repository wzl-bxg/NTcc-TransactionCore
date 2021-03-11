using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTccTransactionCore.Abstractions
{
    public interface ITransaction : IDisposable
    {
        event EventHandler Disoped;

        event EventHandler PrevCommit;

        event EventHandler PrevRollback;

        TransactionXid Xid { get; }
        TransactionStatus Status { get; set; }
        TransactionType TransactionType { get; }
        int RetriedCount { get; set; }
        DateTime CreateUtcTime { get; }
        DateTime LastUpdateUtcTime { get; set; }
        long Version { get; set; }

        Task CommitAsync();

        Task RollbackAsync();

        void UpdateRetriedCount();

        void UpdateVersion();

        void ChangeStatus(TransactionStatus status);



        #region Participant

        void AddParticipant(IParticipant participant);

        IReadOnlyList<IParticipant> FindAllParticipantAsReadOnly();

        #endregion
    }
}
