using System;
using System.Runtime.Serialization;

namespace NTccTransaction.Abstractions
{
    public class CancellingException : TransactionException
    {
        public override string Message => $"NTccTransaction，Cancelling exception：{ base.Message}";

        public CancellingException()
            : base()
        {

        }

        public CancellingException(string message)
            : base(message)
        {

        }

        public CancellingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public CancellingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
