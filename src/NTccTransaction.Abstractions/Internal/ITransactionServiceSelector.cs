using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransaction.Abstractions
{
    /// <summary>
    /// Defines an interface for selecting an consumer service method to invoke for the current message.
    /// </summary>
    public interface ITransactionServiceSelector
    {
        /// <summary>
        /// Selects a set of <see cref="CacheExecutorDescriptor" /> candidates for the current message associated with
        /// </summary>
        /// <returns>A set of <see cref="CacheExecutorDescriptor" /> candidates or <c>null</c>.</returns>
        IReadOnlyList<CacheExecutorDescriptor> SelectCandidates();

    }
}
