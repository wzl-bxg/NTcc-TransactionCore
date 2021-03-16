using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransaction.Aop
{
    public class CompensableTransactionInterceptor : INTccInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<CompensableTransactionInterceptor> _logger;
        private HashSet<Type> _delayCancelExceptionTypes;

        public CompensableTransactionInterceptor(IServiceProvider serviceProvider, ITransactionManager transactionManager
                                                , ILogger<CompensableTransactionInterceptor> logger, NTccTransactionOptions nTccTransactionOptions)
        {
            _serviceProvider = serviceProvider;
            _transactionManager = transactionManager;
            _logger = logger;
            _delayCancelExceptionTypes = nTccTransactionOptions.DelayCancelExceptionTypes;
        }
        public async Task InterceptAsync(IMethodInvocation invocation)
        {
            if (!CompensableHelper.IsCompensableMethod(invocation.Method))
            {
                await invocation.ProceedAsync();
                return;
            }

            CompensableMethodContext compensableMethodContext = new CompensableMethodContext(invocation, _serviceProvider);
            var isTransactionActive = _transactionManager.IsTransactionActive();

            if (!TransactionUtils.IsLegalTransactionContext(isTransactionActive, compensableMethodContext))
            {
                throw new TransactionException($"no available compensable transaction, the method {compensableMethodContext.MethodInfo.Name} is illegal");
            }

            switch (compensableMethodContext.GetMethodRole(isTransactionActive))
            {
                case MethodRole.ROOT:
                    await RootMethodProceed(compensableMethodContext);
                    break;
                case MethodRole.PROVIDER:
                    await ProviderMethodProceed(compensableMethodContext);
                    break;
                default:
                    await invocation.ProceedAsync();
                    break;
            }

        }

        private async Task RootMethodProceed(CompensableMethodContext compensableMethodContext)
        {
            var allDelayCancelExceptionTypes = new HashSet<Type>();

            allDelayCancelExceptionTypes.AddRange(this._delayCancelExceptionTypes);
            allDelayCancelExceptionTypes.AddRange(compensableMethodContext.Compensable.DelayCancelExceptionTypes);


            using (var transaction = _transactionManager.Begin(compensableMethodContext.GetUniqueIdentity()))
            {
                try
                {
                    await compensableMethodContext.MethodInvocation.ProceedAsync();
                }
                catch (Exception ex)
                {
                    if (!_transactionManager.IsDelayCancelException(ex, allDelayCancelExceptionTypes))
                    {
                        _logger.LogError(string.Format("compensable transaction trying failed. transaction content:{0} ", JsonConvert.SerializeObject(transaction)));
                        await _transactionManager.RollbackAsync();
                    }
                    throw ex;
                }

                await _transactionManager.CommitAsync();
            }
        }

        private async Task ProviderMethodProceed(CompensableMethodContext compensableMethodContext)
        {

            //It designed to reuse API by calling difference method base on incoming transaction status from remote calling
            switch (compensableMethodContext.TransactionContext.Status)
            {
                case TransactionStatus.TRYING:
                    _transactionManager.PropagationNewBegin(compensableMethodContext.TransactionContext);

                    await compensableMethodContext.MethodInvocation.ProceedAsync();
                    break;
                case TransactionStatus.CONFIRMING:
                    try
                    {
                        using (_transactionManager.PropagationExistBegin(compensableMethodContext.TransactionContext))
                        {
                            await _transactionManager.CommitAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "the transaction has been committed");
                        //the transaction has been commit, ignore it.
                    }
                    break;
                case TransactionStatus.CANCELLING:
                    try
                    {
                        using (_transactionManager.PropagationExistBegin(compensableMethodContext.TransactionContext))
                        {
                            await _transactionManager.RollbackAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "the transaction has been cancelled");
                        //the transaction has been rollback, ignore it.
                    }
                    break;
            }
        }
    }
}
