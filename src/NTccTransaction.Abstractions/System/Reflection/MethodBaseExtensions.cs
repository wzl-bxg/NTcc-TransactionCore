using NTccTransaction.Abstractions;
using System.Linq;

namespace System.Reflection
{
    public static class MethodBaseExtensions
    {
        public static Type[] GetParameterTypes(this MethodBase method)
        {
            Check.NotNull(method, nameof(method));

            return method.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        public static int GetParameterPosition<T>(this MethodBase methodInfo)
        {
            var parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (parameterTypes[i] == typeof(T))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
