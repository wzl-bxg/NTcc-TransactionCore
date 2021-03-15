using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransaction.Http.Capital.WebApi
{
    public interface ICapitalService
    {
        Task<string> Record(TransactionContext transactionContext);
    }
}
