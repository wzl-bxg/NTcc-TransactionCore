using Newtonsoft.Json;
using NTccTransactionCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace NTccTransactionCore
{
    [Serializable]
    public class Transaction : ITransaction, IDisposable
    {
        [field: NonSerialized]
        public event EventHandler Disoped;

        [field: NonSerialized]
        public event EventHandler PrevCommit;

        [field: NonSerialized]
        public event EventHandler PrevRollback;

        public TransactionXid Xid { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType TransactionType { get; set; }
        public int RetriedCount { get; set; }
        public DateTime CreateUtcTime { get; set; }
        public DateTime LastUpdateUtcTime { get; set; }
        public long Version { get; set; }

        private Transaction()
        {
            CreateUtcTime = DateTime.Now.ToUniversalTime();
            LastUpdateUtcTime = DateTime.Now.ToUniversalTime();
            Version = 1L;
        }

        public Transaction(TransactionType transactionType)
            : this()
        {
            Xid = new TransactionXid();
            Status = TransactionStatus.TRYING;
            TransactionType = transactionType;
        }

        public Transaction(string uniqueIdentity, TransactionType transactionType)
            : this()
        {
            Xid = new TransactionXid(uniqueIdentity);
            Status = TransactionStatus.TRYING;
            TransactionType = transactionType;
        }

        public Transaction(TransactionContext transactionContext)
            : this()
        {
            this.Xid = transactionContext.Xid;
            this.Status = TransactionStatus.TRYING;
            this.TransactionType = TransactionType.BRANCH;
        }

        public async Task CommitAsync()
        {
            ChangeStatus(TransactionStatus.CONFIRMING);
            PrevCommit?.Invoke(this, new EventArgs());

            foreach (var participant in FindAllParticipantAsReadOnly())
            {
                await participant.CommitAsync();
            }
        }

        public async Task RollbackAsync()
        {
            ChangeStatus(TransactionStatus.CANCELLING);
            PrevRollback?.Invoke(this, new EventArgs());

            foreach (var participant in FindAllParticipantAsReadOnly())
            {
                await participant.RollbackAsync();
            }
        }

        public void UpdateRetriedCount()
        {
            this.RetriedCount++;
            this.LastUpdateUtcTime = DateTime.Now.ToUniversalTime();
        }

        public void UpdateVersion()
        {
            this.Version++;
            this.LastUpdateUtcTime = DateTime.Now.ToUniversalTime();
        }

        public void ChangeStatus(TransactionStatus status)
        {
            this.Status = status;
            this.LastUpdateUtcTime = DateTime.Now.ToUniversalTime();
        }


        public List<IParticipant> Participants { get; private set; } = new List<IParticipant>();

        public void AddParticipant(IParticipant participant)
        {
            Check.NotNull(participant, nameof(participant));
            Participants.Add(participant);
        }

        public IReadOnlyList<IParticipant> FindAllParticipantAsReadOnly()
        {
            return Participants.ToImmutableList();
        }

        public void Dispose()
        {
            Disoped?.Invoke(this, new EventArgs());
        }
    }
}
