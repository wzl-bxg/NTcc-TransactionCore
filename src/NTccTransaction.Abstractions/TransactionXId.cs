using System;
using System.Linq;
using System.Text;

namespace NTccTransaction.Abstractions
{
    [Serializable]
    public class TransactionXid
    {
        [field: NonSerialized]
        private static string CUSTOMIZED_TRANSACTION_ID = "UniqueIdentity";

        public int FormatId { get; set; } = 1;
        public string GlobalTransactionId { get; set; }
        public string BranchQualifier { get; set; }

        public TransactionXid()
        {
            GlobalTransactionId = Guid.NewGuid().ToString();
            BranchQualifier = Guid.NewGuid().ToString();
        }

        public TransactionXid(object uniqueIdentity)
        {
            if (uniqueIdentity == null)
            {

                GlobalTransactionId = Guid.NewGuid().ToString();
                BranchQualifier = Guid.NewGuid().ToString();
            }
            else
            {
                this.GlobalTransactionId = CUSTOMIZED_TRANSACTION_ID;
                this.BranchQualifier = uniqueIdentity.ToString();
            }
        }


        public TransactionXid(string globalTransactionId)
        {
            this.GlobalTransactionId = globalTransactionId;
            this.BranchQualifier = Guid.NewGuid().ToString();
        }


        public TransactionXid(string globalTransactionId, string branchQualifier)
        {
            this.GlobalTransactionId = globalTransactionId;
            this.BranchQualifier = branchQualifier;
        }

        public TransactionXid Clone()
        {
            string cloneGlobalTransactionId = null;
            string cloneBranchQualifier = null;

            if (!string.IsNullOrEmpty(this.GlobalTransactionId))
            {
                cloneGlobalTransactionId = this.GlobalTransactionId.Clone().ToString();
            }

            if (!string.IsNullOrEmpty(this.BranchQualifier))
            {
                cloneBranchQualifier = this.BranchQualifier.Clone().ToString();
            }

            return new TransactionXid(cloneGlobalTransactionId, cloneBranchQualifier);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            if (CUSTOMIZED_TRANSACTION_ID.SequenceEqual(this.GlobalTransactionId))
            {
                // format：UniqueIdentity:xxxx-xxxx-xxxx-xxxx
                stringBuilder.Append(this.GlobalTransactionId.ToString());
                stringBuilder.Append(":").Append(this.BranchQualifier);

            }
            else
            {
                // format：xxxx-xxxx-xxxx-xxxx:xxxx-xxxx-xxxx-xxxx
                stringBuilder.Append(new Guid(this.GlobalTransactionId));
                stringBuilder.Append(":").Append(new Guid(this.BranchQualifier));
            }

            return stringBuilder.ToString();
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + this.FormatId;
            result = prime * result + this.BranchQualifier.GetHashCode();
            result = prime * result + this.GlobalTransactionId.GetHashCode();
            return result;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            else if (obj == null)
            {
                return false;
            }
            else if (this.GetType() != obj.GetType())
            {
                return false;
            }
            TransactionXid other = (TransactionXid)obj;
            if (this.FormatId != other.FormatId)
            {
                return false;
            }
            else if (!this.BranchQualifier.SequenceEqual(other.BranchQualifier))
            {
                return false;
            }
            else if (!this.GlobalTransactionId.SequenceEqual(other.GlobalTransactionId))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(TransactionXid left, TransactionXid right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TransactionXid left, TransactionXid right)
        {
            return !(left == right);
        }
    }
}
