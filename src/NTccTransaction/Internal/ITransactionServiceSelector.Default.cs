using Autofac;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NTccTransaction
{
    public class TransactionServiceSelector : ITransactionServiceSelector
    {
        private readonly ILifetimeScope _serviceProvider;

        public TransactionServiceSelector(ILifetimeScope serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IReadOnlyList<CacheExecutorDescriptor> SelectCandidates()
        {
            var executorDescriptorList = new List<CacheExecutorDescriptor>();

            executorDescriptorList.AddRange(FindMatchExecutorFromInterfaceTypes(_serviceProvider));

            return executorDescriptorList;
        }

        protected virtual IEnumerable<CacheExecutorDescriptor> FindMatchExecutorFromInterfaceTypes(
           ILifetimeScope provider)
        {
            var executorDescriptorList = new List<CacheExecutorDescriptor>();

            var types = provider.ComponentRegistry.Registrations
                         .Where(r => typeof(INTccTransactionService).IsAssignableFrom(r.Activator.LimitType))
                         .Select(r => r.Activator.LimitType).ToList();

            foreach (var type in types)
            {
                var serviceTypes = type.GetInterfaces().ToList();
                foreach (var serviceType in serviceTypes)
                {
                    if (serviceType == typeof(INTccTransactionService))
                    {
                        continue;
                    }
                    var serviceTypeInfo = serviceType.GetTypeInfo();
                    executorDescriptorList.AddRange(GetCompensableAttributesDescription(type.GetTypeInfo(), serviceTypeInfo));
                    break;
                }
            }

            return executorDescriptorList;
        }

        protected IEnumerable<CacheExecutorDescriptor> GetCompensableAttributesDescription(TypeInfo typeInfo, TypeInfo serviceTypeInfo = null)
        {
            IList<CacheExecutorDescriptor> listDescriptor = new List<CacheExecutorDescriptor>();

            foreach (var method in typeInfo.DeclaredMethods)
            {
                var compensableAttr = method.GetCustomAttributes<CompensableAttribute>(true);
                var compensableAttributes = compensableAttr as IList<CompensableAttribute> ?? compensableAttr.ToList();

                if (!compensableAttributes.Any())
                {
                    continue;
                }

                foreach (var attr in compensableAttributes)
                {

                    var parameters = method.GetParameters()
                        .Select(parameter => new ParameterDescriptor
                        {
                            Name = parameter.Name,
                            ParameterType = parameter.ParameterType
                        }).ToList();

                    listDescriptor.Add(InitDescriptor(attr, method, typeInfo, serviceTypeInfo, parameters));
                }
            }

            IList<CacheExecutorDescriptor> listCompensableDescriptor = new List<CacheExecutorDescriptor>();
            foreach (var descriptor in listDescriptor)
            {
                var cancelMethod = descriptor.Attribute.CancelMethod;
                var confirmMethod = descriptor.Attribute.ConfirmMethod;

                MethodInfo cancelMethodInfo = typeInfo.GetDeclaredMethod(cancelMethod);
                var cancelParameters = cancelMethodInfo.GetParameters()
                        .Select(parameter => new ParameterDescriptor
                        {
                            Name = parameter.Name,
                            ParameterType = parameter.ParameterType
                        }).ToList();

                listCompensableDescriptor.Add(InitDescriptor(null, cancelMethodInfo, typeInfo, serviceTypeInfo, cancelParameters));

                MethodInfo confirmMethodInfo = typeInfo.GetDeclaredMethod(confirmMethod);
                var confirmParameters = cancelMethodInfo.GetParameters()
                        .Select(parameter => new ParameterDescriptor
                        {
                            Name = parameter.Name,
                            ParameterType = parameter.ParameterType
                        }).ToList();

                listCompensableDescriptor.Add(InitDescriptor(null, confirmMethodInfo, typeInfo, serviceTypeInfo, confirmParameters));

            }



            return listDescriptor.Concat(listCompensableDescriptor);
        }


        private static CacheExecutorDescriptor InitDescriptor(
            CompensableAttribute attr,
            MethodInfo methodInfo,
            TypeInfo implType,
            TypeInfo serviceTypeInfo,
            IList<ParameterDescriptor> parameters)
        {
            var descriptor = new CacheExecutorDescriptor
            {
                Attribute = attr,
                MethodInfo = methodInfo,
                ImplTypeInfo = implType,
                ServiceTypeInfo = serviceTypeInfo,
                Parameters = parameters
            };

            return descriptor;
        }



        private class RegexExecuteDescriptor<T>
        {
            public string Name { get; set; }

            public T Descriptor { get; set; }
        }
    }
}
