using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTccTransaction
{
    public class TransactionMethodInvoker : ITransactionMethodInvoker
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<int, ObjectMethodExecutor> _executors;
        private readonly MethodMatcherCache _selector;

        public TransactionMethodInvoker(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, MethodMatcherCache selector)
        {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<TransactionMethodInvoker>();
            _executors = new ConcurrentDictionary<int, ObjectMethodExecutor>();
            _selector = selector;
        }

        public async Task InvokeAsync(TransactionContext transactionContext, InvocationContext invocationContext, CancellationToken cancellationToken = default)
        {
            CacheExecutorDescriptor matchExcutor;
            _selector.TryGetTccTransactionExecutor(invocationContext.TargetType.FullName + "." + invocationContext.MethodName, out matchExcutor);

            cancellationToken.ThrowIfCancellationRequested();

            var methodInfo = matchExcutor.MethodInfo;
            var executor = _executors.GetOrAdd(methodInfo.MetadataToken, x => ObjectMethodExecutor.Create(methodInfo, matchExcutor.ImplTypeInfo));
            using (var scope = _serviceProvider.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var obj = GetInstance(provider, matchExcutor);

                var parameterDescriptors = matchExcutor.Parameters;

                for (var i = 0; i < parameterDescriptors.Count; i++)
                {
                    
                    if (parameterDescriptors[i].ParameterType == typeof(TransactionContext))
                    {
                        invocationContext.Arguments[i] = transactionContext;
                    }
                   else if (parameterDescriptors[i].ParameterType == typeof(CancellationToken))
                    {
                        invocationContext.Arguments[i] = cancellationToken;
                    }
                }
                var resultObj = await ExecuteWithParameterAsync(executor, obj, invocationContext.Arguments);
            }
        }

        protected virtual object GetInstance(IServiceProvider provider, CacheExecutorDescriptor descriptor)
        {
            var srvType = descriptor.ServiceTypeInfo?.AsType();
            var implType = descriptor.ImplTypeInfo.AsType();

            object obj = null;
            if (srvType != null)
            {
                obj = provider.GetServices(srvType).FirstOrDefault(o => o.GetType() == implType);
            }

            if (obj == null)
            {
                obj = ActivatorUtilities.GetServiceOrCreateInstance(provider, implType);
            }

            return obj;
        }

        private async Task<object> ExecuteWithParameterAsync(ObjectMethodExecutor executor, object @class, object[] parameter)
        {
            if (executor.IsMethodAsync)
            {
                return await executor.ExecuteAsync(@class, parameter);
            }

            return executor.Execute(@class, parameter);
        }
    }
}
