using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Architecture.Shell.Correlation;

public static class CorrelationServiceCollectionExtensions
{
    public static IServiceCollection AddCorrelation(this IServiceCollection services)
    {
        return services.AddCorrelation(_ => { });
    }

    public static IServiceCollection AddCorrelation(this IServiceCollection services, Action<CorrelationIdOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);
        
        services.TryAddSingleton<ICorrelationContextAccessor, CorrelationContextAccessor>();
        services.TryAddTransient<ICorrelationContextFactory, CorrelationContextFactory>();
        services.TryAddTransient<ICorrelationService, CorrelationService>();

        return services;
    }
}