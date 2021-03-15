using System;
using System.Runtime.Serialization;

namespace NTccTransaction.Abstractions
{
    public class TryingException : TransactionException
    {
        public override string Message => $"NTccTransaction，Trying exception：{ base.Message}";

        public TryingException()
            : base()
        {

        }

        public TryingException(string message)
            : base(message)
        {

        }

        public TryingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public TryingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
