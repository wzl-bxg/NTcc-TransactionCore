using NTccTransactionCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransactionCore.Http.Capital.WebApi
{
    public class CapitalService : ICapitalService, INTccTransactionService
    {
        [Compensable(CancelMethod = "CancelRecord", ConfirmMethod = "ConfirmRecord")]
        public async Task<string> Record(TransactionContext transactionContext)
        {
            return await Task.FromResult("Capital Return");
        }

        public void ConfirmRecord(TransactionContext transactionContext = null)
        {

        }

        public void CancelRecord(TransactionContext transactionContext = null)
        {
        }
    }
}
