using Microsoft.Extensions.DependencyInjection;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransaction.Aop
{
    public class AopNTccTransactionOptionsExtension : INTccTransactionOptionsExtension
    {

        public void AddServices(IServiceCollection services)
        {
            services.AddTransient<CompensableTransactionInterceptor>();
            services.AddTransient<INTccInterceptor, CompensableTransactionInterceptor>();

            services.AddTransient<ResourceCoordinatorInterceptor>();
            services.AddTransient<INTccInterceptor, ResourceCoordinatorInterceptor>();

            services.AddTransient(typeof(TccAsyncDeterminationInterceptor<>));
        }
    }
}
