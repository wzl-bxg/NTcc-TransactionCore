using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace NTccTransaction.Http.Order.WebApi
{
    public interface ICapitalApi : IHttpApi
    {
        [HttpPost("Capital/Record")]
        Task<string> Record([JsonContent] TransactionContext transactionContext = null);
    }
}
