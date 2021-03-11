using Microsoft.Extensions.Logging;
using NTccTransactionCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransactionCore.Http.Order.WebApi
{
    public class OrderService : IOrderService, INTccTransactionService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly ICapitalProxy _capitalProxy;

        public OrderService(ILogger<OrderService> logger, ICapitalProxy capitalProxy)
        {
            _logger = logger;
            _capitalProxy = capitalProxy;
        }

        [Compensable(CancelMethod = "CancelOrder", ConfirmMethod = "ConfirmOrder")]
        public async Task<string> PostOrder(string input, TransactionContext transactionContext = null)
        {
            var result = await _capitalProxy.Record(null);

            if (input.StartsWith("Exception"))
            {
                throw new Exception("Exception Test");
            }
            return await Task.FromResult(result);
        }

        public async Task ConfirmOrder(string input, TransactionContext transactionContext = null)
        {
            await Task.CompletedTask;
        }

        public async Task CancelOrder(string input, TransactionContext transactionContext = null)
        {
            await Task.CompletedTask;
        }
    }
}
