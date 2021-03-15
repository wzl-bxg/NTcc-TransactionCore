using System;
using System.Runtime.Serialization;

namespace NTccTransaction.Abstractions
{
    public class ConfirmingException: TransactionException
    {
        public override string Message => $"NTccTransaction，Confirming exception：{ base.Message}";

        public ConfirmingException()
            : base()
        {

        }

        public ConfirmingException(string message)
            : base(message)
        {

        }

        public ConfirmingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ConfirmingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
