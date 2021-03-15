using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTccTransaction
{
    public class CompensableHelper
    {
        public static bool IsCompensableMethod(MethodInfo methodInfo)
        {
            CompensableAttribute compensableAttribute;
            Check.NotNull(methodInfo, nameof(methodInfo));

            //Method declaration
            var attrs = methodInfo.GetCustomAttributes(true).OfType<CompensableAttribute>().ToArray();
            if (attrs.Any())
            {
                compensableAttribute = attrs.First();
                return !compensableAttribute.IsDisabled;
            }

            //compensableAttribute = null;
            return false;
        }
    }
}
