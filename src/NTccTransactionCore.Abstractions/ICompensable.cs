using System;

namespace NTccTransactionCore.Abstractions
{
   public interface ICompensable
    {
        string ConfirmMethod { get; }

        string CancelMethod { get; }

        Propagation Propagation { get; }

        Type[] DelayCancelExceptionTypes { get; }

    }
}
