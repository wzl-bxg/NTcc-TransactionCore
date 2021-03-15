using NTccTransaction.Abstractions;
using System;
using System.Reflection;

namespace NTccTransaction
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CompensableAttribute : Attribute, ICompensable
    {

        public Propagation Propagation { get; set; }

        public string ConfirmMethod { get; set; }

        public string CancelMethod { get; set; }

        public Type[] DelayCancelExceptionTypes { get; set; }

        /// <summary>
        /// Used to prevent starting a compensable transaction for the method.
        /// Default: false.
        /// </summary>
        public bool IsDisabled { get; set; }

        public CompensableAttribute()
        {
            DelayCancelExceptionTypes = new Type[0];
        }
    }

}
