namespace NTccTransaction.Abstractions
{
    /// <summary>
    /// Transaction accessor
    /// </summary>
    public interface ITransactionAccessor
    {
        ITransaction Transaction { get; }

        /// <summary>
        /// Register transaction
        /// </summary>
        /// <param name="transaction">The transaction need to be register</param>
        void RegisterTransaction(ITransaction transaction);

        /// <summary>
        /// Unregister transaction
        /// </summary>
        /// <returns>The transaction that has been unregister</returns>
        ITransaction UnRegisterTransaction();
    }
}
