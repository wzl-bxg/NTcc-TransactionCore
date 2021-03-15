

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NTccTransaction.Abstractions
{

    /// <summary>
    /// A descriptor of user definition method.
    /// </summary>
    public class CacheExecutorDescriptor
    {
        public TypeInfo ServiceTypeInfo { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public TypeInfo ImplTypeInfo { get; set; }

        public ICompensable Attribute { get; set; }

        public IList<ParameterDescriptor> Parameters { get; set; }
    }

    public class ParameterDescriptor
    {
        public string Name { get; set; }

        public Type ParameterType { get; set; }

    }

}