using Microsoft.Extensions.Logging;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransaction.Http.Order.WebApi
{
    public class OrderService : IOrderService, INTccTransactionService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly ICapitalProxy _capitalProxy;
        private readonly IOrderService1 _orderService1;

        public OrderService(ILogger<OrderService> logger, ICapitalProxy capitalProxy, IOrderService1 orderService1)
        {
            _logger = logger;
            _capitalProxy = capitalProxy;
            _orderService1 = orderService1;
        }

        [Compensable(CancelMethod = "CancelOrder", ConfirmMethod = "ConfirmOrder")]
        public async Task<string> PostOrder(string input, TransactionContext transactionContext = null)
        {
            _logger.LogInformation("OrderService.PostOrder:" + input);

              var result = await _capitalProxy.Record(null);
            //var result = await _orderService1.PostOrder("vvv");

            if (input.StartsWith("Exception"))
            {
                throw new Exception("Exception Test");
            }
            return await Task.FromResult("");
            //  return await Task.FromResult(result);
        }

        public async Task ConfirmOrder(string input, TransactionContext transactionContext = null)
        {
            _logger.LogInformation("OrderService.ConfirmOrder:" + input);
            await Task.CompletedTask;
        }

        public async Task CancelOrder(string input, TransactionContext transactionContext = null)
        {
            _logger.LogInformation("OrderService.CancelOrder:" + input);
            await Task.CompletedTask;
        }
    }
}
