using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;

namespace NTccTransaction
{
    public class NTccTransactionOptions
    {
        public IList<INTccTransactionOptionsExtension> Extensions { get; }

        /// <summary>
        /// Default succeeded message expiration time span, in seconds.
        /// </summary>
        public const int DefaultSucceedMessageExpirationAfter = 24 * 3600;

        /// <summary>
        /// Failed message retry waiting interval.
        /// </summary>
        public const int DefaultFailedMessageWaitingInterval = 60;

        /// <summary>
        /// Failed message retry count.
        /// </summary>
        public const int DefaultFailedRetryCount = 30;//30 seconds

        /// <summary>
        /// recover duration
        /// </summary>
        public const int DefaultRecoverDuration = 120;//120 seconds

        public NTccTransactionOptions()
        {
            SucceedMessageExpiredAfter = DefaultSucceedMessageExpirationAfter;
            FailedRetryInterval = DefaultFailedMessageWaitingInterval;
            FailedRetryCount = DefaultFailedRetryCount;
            RecoverDuration = DefaultRecoverDuration;
            Extensions = new List<INTccTransactionOptionsExtension>();
            DelayCancelExceptionTypes = new HashSet<Type>() { typeof(ConcurrencyTransactionException) };
        }

        /// <summary>
        /// Sent or received succeed message after time span of due, then the message will be deleted at due time.
        /// Default is 24*3600 seconds.
        /// </summary>
        public int SucceedMessageExpiredAfter { get; set; }

        /// <summary>
        /// Failed messages polling delay time.
        /// Default is 60 seconds.
        /// </summary>
        public int FailedRetryInterval { get; set; }


        /// <summary>
        /// recover duration 
        /// Default is 120 seconds.
        /// </summary>
        public int RecoverDuration { get; set; }

        /// <summary>
        /// The number of message retries, the retry will stop when the threshold is reached.
        /// Default is 50 times.
        /// </summary>
        public int FailedRetryCount { get; set; }

        /// <summary>
        /// Delay Cancel Exception Types collection
        /// </summary>
        public HashSet<Type> DelayCancelExceptionTypes { get; set; } = new HashSet<Type>();

        /// <summary>
        /// Registers an extension that will be executed when building services.
        /// </summary>
        /// <param name="extension"></param>
        public void RegisterExtension(INTccTransactionOptionsExtension extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            Extensions.Add(extension);
        }
    }
}
