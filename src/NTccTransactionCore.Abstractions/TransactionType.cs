using System.ComponentModel;

namespace NTccTransactionCore.Abstractions
{
    /// <summary>
    /// Transaction type
    /// </summary>
    public enum TransactionType:int
    {
        [Description("Root transaction")]
        ROOT=1,

        [Description("Branch transaction")]
        BRANCH = 2
    }
}
