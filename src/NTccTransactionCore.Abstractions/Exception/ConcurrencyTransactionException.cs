﻿using System;

namespace NTccTransactionCore.Abstractions
{
    public class ConcurrencyTransactionException : Exception
    {
        public ConcurrencyTransactionException()
        {

        }

        public ConcurrencyTransactionException(string message)
            : base(message)
        {

        }

        public ConcurrencyTransactionException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
