using System;
using System.Collections.Generic;

namespace NTccTransaction.Abstractions
{
    public interface ITransactionRepository
    {
        void Create(ITransaction transaction);

        void Update(ITransaction transaction);

        void Delete(ITransaction transaction);

        ITransaction FindByXid(TransactionXid xid);

        IEnumerable<ITransaction> FindAllUnmodifiedSince(DateTime dateTime);
    }
}
