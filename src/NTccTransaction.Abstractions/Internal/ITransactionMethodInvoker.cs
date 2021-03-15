using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTccTransaction.Abstractions
{

    /// <summary>
    /// Perform user definition method of tcc.
    /// </summary>
    public interface ITransactionMethodInvoker
    {
        /// <summary>
        /// Invoke tcc transaction method with TransactionContext and InvocationContext.
        /// </summary>
        /// <param name="transactionContext">The transaction context</param>
        /// <param name="invocationContext">The invocation context</param>
        /// <param name="cancellationToken">The object of <see cref="CancellationToken"/>.</param>
        Task InvokeAsync(TransactionContext transactionContext, InvocationContext invocationContext, CancellationToken cancellationToken = default);
    }
}
