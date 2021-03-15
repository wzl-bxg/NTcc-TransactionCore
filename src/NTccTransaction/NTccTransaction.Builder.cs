using Microsoft.Extensions.DependencyInjection;
using System;

namespace NTccTransaction
{
    /// <summary>
    /// Allows fine grained configuration of NTccTransaction services.
    /// </summary>
    public sealed class NTccTransactionBuilder
    {
        public NTccTransactionBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Gets the <see cref="IServiceCollection" /> where services are configured.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Adds a scoped service of the type specified in serviceType with an implementation
        /// </summary>
        private NTccTransactionBuilder AddScoped(Type serviceType, Type concreteType)
        {
            Services.AddScoped(serviceType, concreteType);
            return this;
        }

        /// <summary>
        /// Adds a singleton service of the type specified in serviceType with an implementation
        /// </summary>
        private NTccTransactionBuilder AddSingleton(Type serviceType, Type concreteType)
        {
            Services.AddSingleton(serviceType, concreteType);
            return this;
        }
    }
}