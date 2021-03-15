using NTccTransaction.Abstractions;
using System;

namespace NTccTransaction.Abstractions
{
    /// <summary>
    /// Invocation context：Used to handle Participant's Confirm and Cancel metadata
    /// </summary>
    [Serializable]
    public class InvocationContext
    {
        public Type TargetType { get; }

        public string MethodName { get; }

        public Type[] Parameters { get; }

        public object[] Arguments { get; }

        public InvocationContext(Type targetType, string methodName, Type[] parameters, object[] arguments)
        {
            Check.NotNull(targetType, nameof(targetType));
            Check.NotNull(methodName, nameof(methodName));

            TargetType = targetType;
            MethodName = methodName;
            Parameters = parameters ?? new Type[0];
            Arguments = arguments ?? new object[0];
        }
    }
}
