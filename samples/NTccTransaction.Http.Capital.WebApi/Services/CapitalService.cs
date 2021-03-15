using Microsoft.Extensions.Logging;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransaction.Http.Capital.WebApi
{
    public class CapitalService : ICapitalService, INTccTransactionService
    {
        private readonly ILogger<CapitalService> _logger;

        public CapitalService(ILogger<CapitalService> logger)
        {
            _logger = logger;
        }

        [Compensable(CancelMethod = "CancelRecord", ConfirmMethod = "ConfirmRecord")]
        public async Task<string> Record(TransactionContext transactionContext)
        {
            _logger.LogInformation("CapitalService.Record");

            return await Task.FromResult("Capital Return");
        }

        public void ConfirmRecord(TransactionContext transactionContext = null)
        {
            _logger.LogInformation("CapitalService.ConfirmRecord");
        }

        public void CancelRecord(TransactionContext transactionContext = null)
        {
            _logger.LogInformation("CapitalService.CancelRecord");
        }
    }
}
