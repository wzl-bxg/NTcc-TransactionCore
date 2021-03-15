using Microsoft.Extensions.DependencyInjection;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransaction.Aop
{
    public class ResourceCoordinatorInterceptor : INTccInterceptor
    {
        private IServiceProvider _serviceProvider;
        private readonly ITransactionManager _transactionManager;

        public ResourceCoordinatorInterceptor(IServiceProvider serviceProvider, ITransactionManager transactionManager)
        {
            _serviceProvider = serviceProvider;
            _transactionManager = transactionManager;
        }

        public async Task InterceptAsync(IMethodInvocation invocation)
        {
            if (!CompensableHelper.IsCompensableMethod(invocation.Method))
            {
                await invocation.ProceedAsync();
                return;
            }

            var transaction = _transactionManager.Current;
            if (transaction != null)
            {
                switch (transaction.Status)
                {
                    case TransactionStatus.TRYING:
                        EnlistParticipant(invocation);
                        break;
                    case TransactionStatus.CONFIRMING:
                        break;
                    case TransactionStatus.CANCELLING:
                        break;
                }
            }
            await invocation.ProceedAsync();
        }

        private void EnlistParticipant(IMethodInvocation invocation)
        {
            var methodInfo = invocation.Method;
            var target = invocation.TargetObject;
            var targetType = invocation.TargetObject.GetType();
            var arguments = invocation.Arguments;

            var compensable = methodInfo.GetCustomAttributes(false).OfType<ICompensable>().FirstOrDefault();

            if (compensable == null)
            {
                throw new TransactionException($"Method {methodInfo.Name} is not marked as Compensable");
            }

            var confirmMethodName = compensable.ConfirmMethod;
            var cancelMethodName = compensable.CancelMethod;

            var transaction = _transactionManager.Current;
            var xid = new TransactionXid(transaction.Xid.GlobalTransactionId); // random branch transaction ID

            // Set the value of the parameter [TransactionContext] in the [Try] method 
            var transactionContextEditor = _serviceProvider.GetRequiredService<ITransactionContextEditor>();

            if (transactionContextEditor.GetContext(target, methodInfo, arguments) == null)
            {
                transactionContextEditor.SetContext(new TransactionContext(xid, TransactionStatus.TRYING), target, methodInfo, arguments);
            }

            var parameterTypes = methodInfo.GetParameterTypes();
            var confirmInvocation = new InvocationContext(targetType, confirmMethodName, parameterTypes, arguments);
            var cancelInvocation = new InvocationContext(targetType, cancelMethodName, parameterTypes, arguments);

            var participant = _serviceProvider.GetRequiredService<IParticipant>();
            participant.Xid = xid;
            participant.ConfirmInvocationContext = confirmInvocation;
            participant.CancelInvocationContext = cancelInvocation;

            _transactionManager.AddParticipant(participant);
        }
    }
}
