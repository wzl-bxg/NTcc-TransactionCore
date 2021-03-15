using System;
using System.Runtime.Serialization;

namespace NTccTransaction.Abstractions
{
    public class TransactionException: Exception
    {
        public override string Message => $"NTccTransaction：{ base.Message}";

        public TransactionException()
            :base()
        {

        }

        public TransactionException(string message)
            : base(message)
        {

        }

        public TransactionException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public TransactionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
