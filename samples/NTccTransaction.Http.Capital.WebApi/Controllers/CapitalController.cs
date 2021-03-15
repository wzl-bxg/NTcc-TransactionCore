using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTccTransaction.Http.Capital.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CapitalController : ControllerBase
    {

        private readonly ILogger<CapitalController> _logger;
        private readonly ICapitalService _capitalService;

        public CapitalController(ILogger<CapitalController> logger, ICapitalService capitalService)
        {
            _logger = logger;
            _capitalService = capitalService;
        }

        [HttpPost("[action]")]
        public async Task<string> RecordAsync(TransactionContext transactionContext)
        {
            return await _capitalService.Record(transactionContext);
        }

        [HttpGet]
        public async Task<string> get()
        {
            return await Task.FromResult("test");
        }
    }
}
