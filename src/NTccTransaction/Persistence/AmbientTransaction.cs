using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NTccTransaction
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
            transaction.Disoped += UnRegisterSelfAfterDisoped;
        }

        public ITransaction UnRegisterTransaction()
        {
            if (!_transactionStack.Value.IsNullOrEmpty())
            {
                var transaction= _transactionStack.Value.Pop();
                transaction.Disoped -= UnRegisterSelfAfterDisoped;
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

        private void UnRegisterSelfAfterDisoped(object sender, EventArgs args)
        {
            this.UnRegisterTransaction();
        }
    }
}
