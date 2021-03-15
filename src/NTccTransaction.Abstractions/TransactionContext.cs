using System;

namespace NTccTransaction.Abstractions
{
    [Serializable]
   public class TransactionContext
    {
        public TransactionXid Xid { get; set; }

        public TransactionStatus Status { get; set; }

        public TransactionContext()
        {

        }

        public TransactionContext(TransactionXid xid, TransactionStatus status)
        {
            Xid = xid?? throw new ArgumentNullException(nameof(xid));
            Status = status;
        }
    }
}
