using NTccTransaction;
using NTccTransaction.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NTccTransactionConfigExtension
    {
        internal static IServiceCollection ServiceCollection;

        /// <summary>
        /// Adds and configures the consistence services for the consistency.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="NTccTransactionOptions" />.</param>
        /// <returns>An <see cref="NTccTransactionBuilder" /> for application services.</returns>
        public static NTccTransactionBuilder AddNTccTransaction(this IServiceCollection services, Action<NTccTransactionOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            ServiceCollection = services;

            services.AddSingleton<ITransactionManager, TransactionManager>();
            services.AddSingleton<ITransactionContextEditor, DefaultTransactionContextEditor>();

            services.AddTransient<IAmbientTransaction, AmbientTransaction>();
            services.AddTransient<IParticipant, Participant>();

            services.AddSingleton<ISerializer, StringSerializer>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, TransactionRecoveryProcessingServer>());
            services.TryAddSingleton<ITransactionRecoveryProcessingServer, TransactionRecoveryProcessingServer>();

            services.TryAddSingleton<ITransactionRecovery, TransactionRecovery>();

            services.TryAddSingleton<MethodMatcherCache>();
            services.TryAddSingleton<ITransactionMethodInvoker, TransactionMethodInvoker>();
            services.TryAddSingleton<ITransactionServiceSelector, TransactionServiceSelector>();


            //Options and extension service
            var options = new NTccTransactionOptions();
            setupAction(options);

            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }

            services.AddSingleton(options);

            //Startup and Hosted 
            services.AddSingleton<IBootstrapper, Bootstrapper>();
            services.AddHostedService<Bootstrapper>();

            return new NTccTransactionBuilder(services);
        }
    }
}
