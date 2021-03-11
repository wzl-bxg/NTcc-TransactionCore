using NTccTransactionCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NTccTransactionCore
{
    /// <summary>
    /// Handle the nested transactions and guarantee the thread safe
    /// </summary>
    public class AmbientTransaction : IAmbientTransaction
    {
        public ITransaction Transaction => GetCurrentTransaction();
        private readonly AsyncLocal<Stack<ITransaction>> _transactionStack = new AsyncLocal<Stack<ITransaction>>();

        public void RegisterTransaction(ITransaction transaction)
        {
            Check.NotNull(transaction,nameof(transaction));

            if (_transactionStack.Value.IsNull())
            {
                _transactionStack.Value = new Stack<ITransaction>();
            }

            _transactionStack.Value.Push(transaction);
            transaction.Disoped += UnregisterSelfAfterDisoped;
        }

        public ITransaction UnregisterTransaction()
        {
            if (!_transactionStack.Value.IsNullOrEmpty())
            {
                var transaction= _transactionStack.Value.Pop();
                transaction.Disoped -= UnregisterSelfAfterDisoped;
                return transaction;
            }
            return null;
        }

        private ITransaction GetCurrentTransaction()
        {
            if (!_transactionStack.Value.IsNullOrEmpty())
            {
                return _transactionStack.Value.Peek();
            }
            return null;
        }

        private void UnregisterSelfAfterDisoped(object sender, EventArgs args)
        {
            var pop = this.UnregisterTransaction();
            var self = (ITransaction)sender;

            Debug.Assert(self == pop, "Error：Not the current transaction");
        }
    }
}
