using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace NTccTransaction
{
    [Serializable]
    public class Transaction : ITransaction, IDisposable
    {
        [field: NonSerialized]
        public event EventHandler Disoped;

        public TransactionXid Xid { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType TransactionType { get; set; }
        public int RetriedCount { get; set; }
        public DateTime CreateUtcTime { get; set; }
        public DateTime LastUpdateUtcTime { get; set; }
        public int Version { get; set; }

        private Transaction()
        {
            CreateUtcTime = DateTime.Now.ToUniversalTime();
            LastUpdateUtcTime = DateTime.Now.ToUniversalTime();
            Version = 1;
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

        public async Task CommitAsync(IServiceScopeFactory serviceScopeFactory)
        {
            foreach (var participant in FindAllParticipantAsReadOnly())
            {
                await participant.CommitAsync(serviceScopeFactory);
            }
        }

        public async Task RollbackAsync(IServiceScopeFactory serviceScopeFactory)
        {
            foreach (var participant in FindAllParticipantAsReadOnly())
            {
                await participant.RollbackAsync(serviceScopeFactory);
            }
        }

        public void AddRetriedCount()
        {
            this.RetriedCount++;
            this.LastUpdateUtcTime = DateTime.Now.ToUniversalTime();
        }

        public void AddVersion()
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
