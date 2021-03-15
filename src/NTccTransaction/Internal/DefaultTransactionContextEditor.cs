using NTccTransaction.Abstractions;
using System.Linq;
using System.Reflection;

namespace NTccTransaction
{
    public class DefaultTransactionContextEditor : ITransactionContextEditor
    {
        public TransactionContext GetContext(object target, MethodBase methodBase, object[] args)
        {
            int position = GetTransactionContextParamPosition(methodBase);
            if (position >= 0)
            {
                return (TransactionContext)args[position];
            }

            return null;
        }

        public void SetContext(TransactionContext transactionContext, object target, MethodBase methodBase, object[] args)
        {
            int position = GetTransactionContextParamPosition(methodBase);
            if (position >= 0)
            {
                args[position] = transactionContext;
            }
        }

        private int GetTransactionContextParamPosition(MethodBase methodBase)
        {
            var parameterTypes = methodBase.GetParameters().Select(p => p.ParameterType).ToArray();
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (parameterTypes[i] == typeof(TransactionContext))
                {
                    return i;
                }
            }

            return -1;
        }

    }
}
