using System;
using System.Runtime.Serialization;

namespace NTccTransaction.Abstractions
{
    public class NoExistedTransactionException : TransactionException
    {
        public override string Message => $"NTccTransaction，transaction not existed exception：{ base.Message}";

        public NoExistedTransactionException()
            : base()
        {

        }

        public NoExistedTransactionException(string message)
            : base(message)
        {

        }

        public NoExistedTransactionException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public NoExistedTransactionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
