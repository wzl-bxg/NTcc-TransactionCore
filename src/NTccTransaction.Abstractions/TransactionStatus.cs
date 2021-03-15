namespace NTccTransaction.Abstractions
{
    /// <summary>
    /// Transaction status
    /// </summary>
    public enum TransactionStatus: int
    {
        TRYING=0,

        CONFIRMING=1,

        CANCELLING = 2
    }
}
