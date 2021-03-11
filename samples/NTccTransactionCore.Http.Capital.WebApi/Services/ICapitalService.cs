using NTccTransactionCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransactionCore.Http.Capital.WebApi
{
    public interface ICapitalService
    {
        Task<string> Record(TransactionContext transactionContext);
    }
}
