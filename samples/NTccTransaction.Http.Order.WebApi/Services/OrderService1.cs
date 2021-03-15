using Microsoft.Extensions.Logging;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransaction.Http.Order.WebApi
{
    public class OrderService1 : IOrderService1, INTccTransactionService
    {
        private readonly ILogger<OrderService1> _logger;
        private readonly ICapitalProxy _capitalProxy;

        public OrderService1(ILogger<OrderService1> logger, ICapitalProxy capitalProxy)
        {
            _logger = logger;
            _capitalProxy = capitalProxy;
        }

        [Compensable(CancelMethod = "CancelOrder", ConfirmMethod = "ConfirmOrder")]
        public async Task<string> PostOrder(string input, TransactionContext transactionContext = null)
        {
            _logger.LogInformation("OrderService1.PostOrder:" + input);
            return await Task.FromResult("1");
        }

        public async Task ConfirmOrder(string input, TransactionContext transactionContext = null)
        {
            _logger.LogInformation("OrderService1.ConfirmOrder:" + input);
            await Task.CompletedTask;
        }

        public async Task CancelOrder(string input, TransactionContext transactionContext = null)
        {
            _logger.LogInformation("OrderService1.CancelOrder:" + input);
            await Task.CompletedTask;
        }
    }
}
