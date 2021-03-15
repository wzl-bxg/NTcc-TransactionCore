using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClientCore;

namespace NTccTransaction.Http.Order.WebApi
{
    public interface ICapitalProxy
    {
        Task<string> Record(TransactionContext transactionContext);

    }

    public class CapitalProxy : ICapitalProxy, INTccTransactionService
    {
        private readonly ICapitalApi _capitalApi;
        public CapitalProxy(ICapitalApi capitalApi)
        {
            _capitalApi = capitalApi;
        }

        [Compensable(CancelMethod = "Record", ConfirmMethod = "Record", Propagation = Propagation.SUPPORTS)]
        public async Task<string> Record(TransactionContext transactionContext = null)
        {
            return await _capitalApi.Record(transactionContext);

        }
    }
}
