using Architecture.Shell.Correlation;
using Architecture.Shell.Correlation.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
#if !NETSTANDARD2_0
using Architecture.Shell.Correlation.Grpc;
#endif

namespace Architecture.Shell.Test.Correlation;

public class CorrelationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCorrelation_WithoutOptions_ShouldRegisterRequiredServices()
    {
        // Given
        var services = new ServiceCollection();

        // When
        services.AddCorrelation();

        // Then
        var serviceProvider = services.BuildServiceProvider();
        
        serviceProvider.GetService<ICorrelationContextAccessor>().ShouldNotBeNull();
        serviceProvider.GetService<ICorrelationContextFactory>().ShouldNotBeNull();
        serviceProvider.GetService<ICorrelationService>().ShouldNotBeNull();
        serviceProvider.GetService<ICorrelationPropagator>().ShouldNotBeNull();
        serviceProvider.GetService<IOptions<CorrelationIdOptions>>().ShouldNotBeNull();
        
        // Verify protocol handlers are registered
        var protocolHandlers = serviceProvider.GetServices<ICorrelationProtocolHandler>().ToList();
        protocolHandlers.ShouldNotBeEmpty();
        protocolHandlers.ShouldContain(h => h.GetType() == typeof(HttpCorrelationProtocolHandler));
#if !NETSTANDARD2_0
        protocolHandlers.ShouldContain(h => h.GetType() == typeof(GrpcCorrelationProtocolHandler));
#endif
    }

    [Fact]
    public void AddCorrelation_WithOptions_ShouldRegisterServicesAndApplyOptions()
    {
        // Given
        var services = new ServiceCollection();
        const string customHeader = "X-Custom-Header";

        // When
        services.AddCorrelation(options =>
        {
            options.RequestHeader = customHeader;
            options.EnforceHeader = true;
        });

        // Then
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<CorrelationIdOptions>>().Value;
        
        options.RequestHeader.ShouldBe(customHeader);
        options.EnforceHeader.ShouldBeTrue();
    }

    [Fact]
    public void AddCorrelation_CalledMultipleTimes_ShouldNotRegisterDuplicateServices()
    {
        // Given
        var services = new ServiceCollection();

        // When
        services.AddCorrelation();
        services.AddCorrelation();

        // Then
        var serviceProvider = services.BuildServiceProvider();
        var correlationServices = serviceProvider.GetServices<ICorrelationService>();
        correlationServices.Count().ShouldBe(1);
    }

    [Fact]
    public void AddCorrelation_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Given
        ServiceCollection services = null;

        // When & Assert
        Should.Throw<ArgumentNullException>(() => services.AddCorrelation())
            .ParamName.ShouldBe("services");
    }

    [Fact]
    public void AddCorrelation_WithNullConfigureOptions_ShouldThrowArgumentNullException()
    {
        // Given
        var services = new ServiceCollection();

        // When & Assert
        Should.Throw<ArgumentNullException>(() => services.AddCorrelation(null))
            .ParamName.ShouldBe("configureOptions");
    }

    [Fact]
    public void AddCorrelation_ShouldRegisterCorrectServiceLifetimes()
    {
        // Given
        var services = new ServiceCollection();

        // When
        services.AddCorrelation();

        // Then
        var accessorDescriptor = services.Single(x => x.ServiceType == typeof(ICorrelationContextAccessor));
        accessorDescriptor.Lifetime.ShouldBe(ServiceLifetime.Singleton);

        var factoryDescriptor = services.Single(x => x.ServiceType == typeof(ICorrelationContextFactory));
        factoryDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);

        var serviceDescriptor = services.Single(x => x.ServiceType == typeof(ICorrelationService));
        serviceDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
        
        var propagatorDescriptor = services.Single(x => x.ServiceType == typeof(ICorrelationPropagator));
        propagatorDescriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddHttpCorrelation_ShouldRegisterHttpCorrelationHandler()
    {
        // Given
        var services = new ServiceCollection();
        services.AddCorrelation();

        // When
        services.AddHttpCorrelation();

        // Then
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<HttpCorrelationHandler>().ShouldNotBeNull();
    }

#if !NETSTANDARD2_0
    [Fact]
    public void AddGrpcCorrelation_ShouldRegisterGrpcInterceptors()
    {
        // Given
        var services = new ServiceCollection();
        services.AddCorrelation();

        // When
        services.AddGrpcCorrelation();

        // Then
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<GrpcCorrelationClientInterceptor>().ShouldNotBeNull();
        serviceProvider.GetService<GrpcCorrelationServerInterceptor>().ShouldNotBeNull();
    }
#endif

    [Fact]
    public void AddCorrelationPropagation_WithHttpClientBuilder_ShouldAddMessageHandler()
    {
        // Given
        var services = new ServiceCollection();
        services.AddCorrelation();
        services.AddHttpCorrelation();
        
        // When
        var builder = services.AddHttpClient("TestClient");
        var result = builder.AddCorrelationPropagation();

        // Then
        result.ShouldBe(builder);
        
        // Verify that the message handler is configured (this is challenging to test directly,
        // but we can at least verify the builder chain works)
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("TestClient");
        httpClient.ShouldNotBeNull();
    }

    [Fact]
    public void AddHttpCorrelation_CalledMultipleTimes_ShouldNotRegisterDuplicateHandlers()
    {
        // Given
        var services = new ServiceCollection();
        services.AddCorrelation();

        // When
        services.AddHttpCorrelation();
        services.AddHttpCorrelation();

        // Then
        var serviceProvider = services.BuildServiceProvider();
        var handlers = serviceProvider.GetServices<HttpCorrelationHandler>();
        handlers.Count().ShouldBe(1);
    }

#if !NETSTANDARD2_0
    [Fact]
    public void AddGrpcCorrelation_CalledMultipleTimes_ShouldNotRegisterDuplicateInterceptors()
    {
        // Given
        var services = new ServiceCollection();
        services.AddCorrelation();

        // When
        services.AddGrpcCorrelation();
        services.AddGrpcCorrelation();

        // Then
        var serviceProvider = services.BuildServiceProvider();
        var clientInterceptors = serviceProvider.GetServices<GrpcCorrelationClientInterceptor>();
        var serverInterceptors = serviceProvider.GetServices<GrpcCorrelationServerInterceptor>();
        
        clientInterceptors.Count().ShouldBe(1);
        serverInterceptors.Count().ShouldBe(1);
    }
#endif

    [Fact]
    public void AddCorrelation_ShouldRegisterProtocolHandlersAsEnumerable()
    {
        // Given
        var services = new ServiceCollection();

        // When
        services.AddCorrelation();

        // Then
        var handlerDescriptors = services.Where(x => x.ServiceType == typeof(ICorrelationProtocolHandler)).ToList();
        handlerDescriptors.Count.ShouldBeGreaterThanOrEqualTo(1); // At least HTTP handler
        
        // All handlers should be transient
        handlerDescriptors.ShouldAllBe(d => d.Lifetime == ServiceLifetime.Transient);
    }
}