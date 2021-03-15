using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransaction.Http.Order.WebApi
{
    public interface IOrderService1
    {
        Task<string> PostOrder(string input, TransactionContext transactionContext = null);

        Task ConfirmOrder(string input, TransactionContext transactionContext = null);

        Task CancelOrder(string input, TransactionContext transactionContext = null);
    }
}
