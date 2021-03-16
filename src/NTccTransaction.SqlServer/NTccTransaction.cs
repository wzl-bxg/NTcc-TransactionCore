using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NTccTransaction.SqlServer
{
    [Table("NTCC_TRANSACTION")]
    public class NTccTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("TRANSACTION_ID")]
        public string TransactionId { get; set; }

        [Column("GLOBAL_TRANSACTION_ID")]
        //public byte[] GlobalTransactionId { get; set; }
        public string GlobalTransactionId { get; set; }

        [Column("BRANCH_QUALIFIER")]
        //public byte[] BranchQualifier { get; set; }
        public string BranchQualifier { get; set; }

        [Column("STATUS")]
        public TransactionStatus Status { get; set; }

        [Column("TRANSACTION_TYPE")]
        public TransactionType TransactionType { get; set; }

        [Column("RETRIED_COUNT")]
        public int RetriedCount { get; set; }

        [Column("CREATE_UTC_TIME")]
        public DateTime CreateUtcTime { get; set; }

        [Column("LAST_UPDATE_UTC_TIME")]
        public DateTime LastUpdateUtcTime { get; set; }

        [Column("VERSION")]
        public int Version { get; set; }

        [Column("CONTENT")]
        public string Content { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var transactionEntity = obj as NTccTransaction;
            if (transactionEntity == null)
                return false;

            if (TransactionId == null ||
                transactionEntity.TransactionId == null)
                return false;

            if (TransactionId == transactionEntity.TransactionId)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            if (TransactionId == null)
            {
                return base.GetHashCode();
            }

            return TransactionId.GetHashCode() ^ 31 + 1000;
        }
    }
}
