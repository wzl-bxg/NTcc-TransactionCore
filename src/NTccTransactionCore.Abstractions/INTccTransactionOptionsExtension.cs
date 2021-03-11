using Microsoft.Extensions.DependencyInjection;

namespace NTccTransactionCore.Abstractions
{
    /// <summary>
    /// NTccTransaction options extension
    /// </summary>
    public interface INTccTransactionOptionsExtension
    {
        /// <summary>
        /// Registered child service.
        /// </summary>
        /// <param name="services">add service to the <see cref="IServiceCollection" /></param>
        void AddServices(IServiceCollection services);
    }
}