using NTccTransaction.Abstractions;

namespace NTccTransaction
{
    public class TransactionUtils
    {
        /// <summary>
        /// Validate transaction context
        /// </summary>
        /// <param name="isTransactionActive"></param>
        /// <param name="compensableMethodContext"></param>
        /// <returns></returns>
        public static bool IsLegalTransactionContext(bool isTransactionActive, CompensableMethodContext compensableMethodContext)
        {
            if (compensableMethodContext.Propagation==Propagation.MANDATORY && 
                !isTransactionActive && 
                compensableMethodContext.TransactionContext == null)
            {
                return false;
            }

            return true;
        }
    }
}
