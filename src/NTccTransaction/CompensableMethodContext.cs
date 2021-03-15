using Microsoft.Extensions.DependencyInjection;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace NTccTransaction
{
    public class CompensableMethodContext
    {
        public object[] Arguments { get; }
        public MethodBase MethodInfo { get; }

        public object Target { get; }

        public ICompensable Compensable { get; }
        public Propagation Propagation { get; }
        public TransactionContext TransactionContext { get; }

        public IMethodInvocation MethodInvocation { get; set; }

        public CompensableMethodContext(IMethodInvocation methodInvocation, IServiceProvider serviceProvider)
        {
            ICompensable compensable;
            ValidateMethodInfo(methodInvocation.Method, out compensable);

            this.Target = methodInvocation.TargetObject;
            this.Arguments = methodInvocation.Arguments;
            this.MethodInfo = methodInvocation.Method;
            this.Compensable = compensable;
            this.Propagation = this.Compensable.Propagation;
            this.MethodInvocation = methodInvocation;

            var transactionContextEditor = serviceProvider.GetRequiredService<ITransactionContextEditor>();

            this.TransactionContext = transactionContextEditor.GetContext(this.Target, this.MethodInfo, this.Arguments);
        }

        /// <summary>
        /// Get method role
        /// </summary>
        /// <param name="isTransactionActive">whether exist transaction</param>
        /// <returns></returns>
        public MethodRole GetMethodRole(bool isTransactionActive)
        {
            // Root transaction
            if ((Propagation == Propagation.REQUIRED && !isTransactionActive && TransactionContext == null) ||
               Propagation == Propagation.REQUIRES_NEW)
            {
                return MethodRole.ROOT;
            }
            // Propagation transaction
            else if ((Propagation == Propagation.REQUIRED || Propagation == Propagation.MANDATORY) &&
                    !isTransactionActive &&
                    TransactionContext != null)
            {
                return MethodRole.PROVIDER;
            }
            // Normal
            else
            {
                return MethodRole.NORMAL;
            }
        }

        /// <summary>
        /// Get the transaction
        /// </summary>
        /// <returns></returns>
        public object GetUniqueIdentity()
        {
            var parameters = this.MethodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var parameterType = parameter.ParameterType;
                if (typeof(IUniqueIdentity).IsAssignableFrom(parameterType))
                {
                    return Arguments;
                }
            }

            return null;
        }

        private static ConcurrentDictionary<MethodBase, ICompensable> _methodLegalDic = new ConcurrentDictionary<MethodBase, ICompensable>();

        private void ValidateMethodInfo(MethodBase methodBase, out ICompensable compensable)
        {
            compensable = _methodLegalDic.GetOrAdd(methodBase, m =>
               {
                   var targetType = m.DeclaringType;
                   var parameterTypes = m.GetParameterTypes();

                   if (m.GetParameterPosition<TransactionContext>() < 0)
                   {
                       throw new TransactionException($"Method: {m.Name}, has no parameter that the type is TransactionContext");
                   }

                   var c = methodBase.GetCustomAttributes(false).OfType<ICompensable>().FirstOrDefault();
                   if (c == null)
                   {
                       throw new TransactionException($"Method: {m.Name}, not marked with Compensable attribute");
                   }

                   if (targetType.GetMethod(c.ConfirmMethod, parameterTypes) == null)
                   {
                       throw new TransactionException($"Method: {m.Name}, can not found the corresponding Confirm method");
                   }

                   if (targetType.GetMethod(c.CancelMethod, parameterTypes) == null)
                   {
                       throw new TransactionException($"Method: {m.Name}, can not found the corresponding Cancel method");
                   }

                   return c;
               });
        }
    }
}
