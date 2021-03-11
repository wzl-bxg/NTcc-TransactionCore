﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTccTransactionCore.Abstractions
{
    public interface IParticipant
    {
        TransactionXid Xid { get; set; }

        InvocationContext ConfirmInvocationContext { get; set; }

        InvocationContext CancelInvocationContext { get; set; }


        Task CommitAsync();
        Task RollbackAsync();
    }
}
