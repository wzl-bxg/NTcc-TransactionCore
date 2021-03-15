using System.Reflection;

namespace NTccTransaction.Abstractions
{
    public interface ITransactionContextEditor
    {
        TransactionContext GetContext(object target, MethodBase methodBase, object[] args);

        void SetContext(TransactionContext transactionContext, object target, MethodBase methodBase, object[] args);
    }
}
