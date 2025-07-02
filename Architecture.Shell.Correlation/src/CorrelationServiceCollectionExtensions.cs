using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Architecture.Shell.Correlation.Http;
using Architecture.Shell.Correlation.Messaging;
#if !NETSTANDARD2_0
using Architecture.Shell.Correlation.Grpc;
#endif

namespace Architecture.Shell.Correlation;

public static class CorrelationServiceCollectionExtensions
{
    public static IServiceCollection AddCorrelation(this IServiceCollection services)
    {
        return services.AddCorrelation(_ => { });
    }

    public static IServiceCollection AddCorrelation(this IServiceCollection services,
        Action<CorrelationIdOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);

        services.TryAddSingleton<ICorrelationContextAccessor, CorrelationContextAccessor>();
        services.TryAddTransient<ICorrelationContextFactory, CorrelationContextFactory>();
        services.TryAddTransient<ICorrelationService, CorrelationService>();
        services.TryAddTransient<ICorrelationPropagator, CorrelationPropagator>();

        // Register protocol handlers
        services.TryAddEnumerable(ServiceDescriptor
            .Transient<ICorrelationProtocolHandler, HttpCorrelationProtocolHandler>());
#if !NETSTANDARD2_0
        services.TryAddEnumerable(ServiceDescriptor
            .Transient<ICorrelationProtocolHandler, GrpcCorrelationProtocolHandler>());
#endif

        return services;
    }

    /// <summary>
    /// Adds correlation support for HTTP clients
    /// </summary>
    public static IServiceCollection AddHttpCorrelation(this IServiceCollection services)
    {
        services.TryAddTransient<HttpCorrelationHandler>();
        return services;
    }

#if !NETSTANDARD2_0
    /// <summary>
    /// Adds correlation support for gRPC clients and servers
    /// </summary>
    public static IServiceCollection AddGrpcCorrelation(this IServiceCollection services)
    {
        services.TryAddTransient<GrpcCorrelationClientInterceptor>();
        services.TryAddTransient<GrpcCorrelationServerInterceptor>();
        return services;
    }
#endif

    /// <summary>
    /// Configures HttpClient to automatically propagate correlation IDs
    /// </summary>
    public static IHttpClientBuilder AddCorrelationPropagation(this IHttpClientBuilder builder)
    {
        return builder.AddHttpMessageHandler<HttpCorrelationHandler>();
    }

    /// <summary>
    /// Adds correlation support for messaging protocols (RabbitMQ, Kafka)
    /// </summary>
    public static IServiceCollection AddMessagingCorrelation(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor
            .Transient<ICorrelationProtocolHandler, RabbitMqCorrelationProtocolHandler>());
        services.TryAddEnumerable(ServiceDescriptor
            .Transient<ICorrelationProtocolHandler, KafkaCorrelationProtocolHandler>());

        // Register integration helpers
        services.TryAddTransient<RabbitMqCorrelationIntegration>();
        services.TryAddTransient<KafkaCorrelationIntegration>();

        return services;
    }
}