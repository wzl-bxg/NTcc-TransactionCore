using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace NTccTransaction.Abstractions
{
    /// <summary>
    /// Transaction manager
    /// Start root transaction
    /// Propagate branch transaction
    /// Manage transaction lifetime
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Get current transaction
        /// </summary>
        ITransaction Current { get; }

        /// <summary>
        /// Begin transaction
        /// </summary>
        /// <returns></returns>
        ITransaction Begin();

        /// <summary>
        /// Begin transaction
        /// </summary>
        /// <param name="uniqueIdentity"></param>
        /// <returns></returns>
        ITransaction Begin(object uniqueIdentity);

        /// <summary>
        /// Propagate branch transaction
        /// </summary>
        /// <param name="transactionContext"></param>
        /// <returns></returns>
        ITransaction PropagationNewBegin(TransactionContext transactionContext);

        /// <summary>
        /// Propagate existing branch transaction
        /// </summary>
        /// <param name="transactionContext"></param>
        /// <returns></returns>
        ITransaction PropagationExistBegin(TransactionContext transactionContext);

        /// <summary>
        /// Has already has transaction
        /// </summary>
        /// <returns></returns>
        bool IsTransactionActive();

        /// <summary>
        /// Commit transaction
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Rollback transaction
        /// </summary>
        Task RollbackAsync();

        /// <summary>
        /// Add participant
        /// </summary>
        /// <param name="participant"></param>
        void AddParticipant(IParticipant participant);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tryingException"></param>
        /// <param name="allDelayCancelExceptionTypes"></param>
        /// <returns></returns>
        bool IsDelayCancelException(Exception tryingException, HashSet<Type> allDelayCancelExceptionTypes);

    }
}
